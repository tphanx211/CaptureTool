using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CaptureTool.Services.Audio;

public class AudioRecordService : IAudioRecordService
{
    private Process? _ffmpegProcess;

    public Task StartRecordingAsync(string outputFilePath)
    {
        // -f dshow: Use DirectShow from Windows to get audio input
        // -i audio="device": Input device // TODO: implement device selection in a dropdown menu and pass it in here
        // -c:a libmp3lame: Use LAME MP3 encoder
        // -q:a 2: Audio quality VBR (0 = best, 9 = worst) — 2 is good quality
        
        //var args = $"-y -f dshow -i audio=\"Microphone (1080P Pro Stream)\" -c:a libmp3lame -q:a 2 \"{outputFilePath}\"";
        var args = $"-y -loglevel warning -f dshow -i audio=\"Microphone (1080P Pro Stream)\" -c:a libmp3lame -q:a 2 \"{outputFilePath}\"";
        
        _ffmpegProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        
        _ffmpegProcess.OutputDataReceived += (s, e) => Console.WriteLine("[FFmpeg stdout] " + e.Data);
        _ffmpegProcess.ErrorDataReceived += (s, e) => Console.WriteLine("[FFmpeg stderr] " + e.Data);

        _ffmpegProcess.Start();
        _ffmpegProcess.BeginOutputReadLine();
        _ffmpegProcess.BeginErrorReadLine();
        
        return Task.CompletedTask;
    }

    public async Task StopRecordingAsync()
    {
        if (_ffmpegProcess is { HasExited: false })
        {
            try
            {
                await _ffmpegProcess.StandardInput.WriteLineAsync("q");
                await _ffmpegProcess.WaitForExitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to stop FFmpeg gracefully: " + ex.Message);
                _ffmpegProcess.Kill(true); 
            }
            finally
            {
                _ffmpegProcess.Dispose();
                _ffmpegProcess = null;
            }
        }
    }
}