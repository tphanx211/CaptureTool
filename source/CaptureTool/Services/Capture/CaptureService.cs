using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CaptureTool.Services.Audio;
using CaptureTool.Services.Mux;
using CaptureTool.Services.Screen;
using CaptureTool.Services.Settings;
using CaptureTool.Services.Transcribe;

namespace CaptureTool.Services.Capture;

public class CaptureService : ICaptureService
{
    private readonly IAudioRecordService _audioService;
    private readonly IScreenRecordService _screenService;
    private readonly ITranscriptionService _transcriptionService;
    private readonly IMuxingService _muxingService;
    private readonly ISettingsService _settingsService;

    private string? _audioPath;
    private string? _videoPath;
    private string? _srtPath;
    private string? _outputPath;

    public CaptureService(
        IAudioRecordService audioService,
        IScreenRecordService screenService,
        ITranscriptionService transcriptionService,
        IMuxingService muxingService,
        ISettingsService settingsService)
    {
        _audioService = audioService;
        _screenService = screenService;
        _transcriptionService = transcriptionService;
        _muxingService = muxingService;
        _settingsService = settingsService;
    }

    public async Task StartCaptureAsync(MonitorInfo monitor)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var folder = Path.Combine(
            _settingsService.Settings.CaptureSaveLocation,
            timestamp);

        Directory.CreateDirectory(folder);

        _audioPath = Path.Combine(folder, "audio.mp3");
        _videoPath = Path.Combine(folder, "screen.mp4");
        
        if (_settingsService.Settings.EnableVoiceTranscription)
            _srtPath = Path.Combine(folder, "transcript.srt");
        
        _outputPath = Path.Combine(folder, "final_output.mp4");

        await _audioService.StartRecordingAsync(_audioPath, _audioService.ListInputDevices().First()); // Default device for now until device selection implemented
        await _screenService.StartRecordingAsync(_videoPath, monitor.Bounds);
    }

    public async Task StopCaptureAsync()
    {
        await _audioService.StopRecordingAsync();
        await _screenService.StopRecordingAsync();

        if (_audioPath is null || _videoPath is null || _outputPath is null)
            throw new InvalidOperationException("Capture paths not initialized.");

        var result = await _transcriptionService.TranscribeAsync(_audioPath);
        
        if (_settingsService.Settings.EnableVoiceTranscription)
            await _transcriptionService.GenerateSrtFileAsync(result, _srtPath);

        await _muxingService.MuxAsync(_videoPath, _audioPath, _srtPath, _outputPath);

        Console.WriteLine($"Capture complete! Output saved to: {_outputPath}");
    }
}