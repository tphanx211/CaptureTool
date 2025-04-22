using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CaptureTool.Services.Settings;

namespace CaptureTool.Services.Transcribe;

public class TranscriptionService : ITranscriptionService
{
    private readonly ISettingsService _settingsService;
    private readonly HttpClient _httpClient;

    public TranscriptionService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        _httpClient = new HttpClient();
    }

    public async Task<TranscriptionResult> TranscribeAsync(string audioFilePath)
    {
        var apiKey = await File.ReadAllTextAsync(_settingsService.Settings.ApiKeyPath);

        using var form = new MultipartFormDataContent();
        using var fileStream = File.OpenRead(audioFilePath);
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");

        form.Add(fileContent, "file", Path.GetFileName(audioFilePath));
        form.Add(new StringContent("whisper-1"), "model");
        form.Add(new StringContent("verbose_json"), "response_format");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey.Trim());

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/audio/transcriptions", form);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var parsed = JsonSerializer.Deserialize<WhisperApiResponse>(json);

        return new TranscriptionResult
        {
            FullText = parsed?.Text ?? string.Empty,
            Segments = parsed?.Segments.Select(s => new TranscriptionSegment
            {
                Id = s.Id,
                Start = s.Start,
                End = s.End,
                Text = s.Text
            }).ToList() ?? new()
        };
    }
    
    public async Task<string> GenerateSrtFileAsync(TranscriptionResult result, string outputPath)
    {
        var lines = result.Segments.Select((s, i) =>
            $"{i + 1}\n{ToSrtTime(s.Start)} --> {ToSrtTime(s.End)}\n{s.Text.Trim()}\n"
        );

        var srtContent = string.Join("\n", lines);

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(outputPath, srtContent);
        return outputPath;
    }

    private static string ToSrtTime(double seconds)
    {
        var ts = TimeSpan.FromSeconds(seconds);
        return ts.ToString(@"hh\:mm\:ss\,fff");
    }

    private class WhisperApiResponse
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("segments")]
        public List<Segment> Segments { get; set; } = new();

        public class Segment
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("start")]
            public double Start { get; set; }

            [JsonPropertyName("end")]
            public double End { get; set; }

            [JsonPropertyName("text")]
            public string Text { get; set; } = string.Empty;
        }
    }
}