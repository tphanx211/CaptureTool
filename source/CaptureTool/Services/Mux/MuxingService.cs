using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace CaptureTool.Services.Mux;

public class MuxingService : IMuxingService
{
    public async Task<string> MuxAsync(string videoPath, string audioPath, string? subtitlePath, string outputPath)
    {
        var tempMuxed = Path.Combine(Path.GetDirectoryName(outputPath), "temp.mp4");
        
        // Step 1: Combine video + audio
        await RunFfmpegAsync($"-y -i \"{videoPath}\" -i \"{audioPath}\" -c:v copy -c:a aac -shortest \"{tempMuxed}\"");

        // Step 2: Add subtitles if needed
        if (!string.IsNullOrWhiteSpace(subtitlePath))
        {
            var escapedSrt = EscapeSubtitlePathForWindows(subtitlePath);
            var subtitleFilter = $"subtitles='{escapedSrt}'";
            var subtitleArgs = $"-y -i {tempMuxed} -vf {subtitleFilter} -c:a copy {outputPath}";
            await RunFfmpegAsync(subtitleArgs);
            if (File.Exists(tempMuxed))
                File.Delete(tempMuxed);
        }
        else
        {
            File.Move(tempMuxed, outputPath, overwrite: true);
        }

        return outputPath;
    }

    private async Task RunFfmpegAsync(string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };

        process.Start();

        var stderr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
            throw new Exception($"FFmpeg failed: {stderr}");
    }
    
    string EscapeSubtitlePathForWindows(string path)
    {
        // Escape colon first to avoid doubling backslashes later
        path = path.Replace(@"\", @"\\");
        return path.Replace(":", @"\:");
    }

}