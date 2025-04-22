using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CaptureTool.Services.Screen;

public class ScreenRecordService : IScreenRecordService
{
    private Process? _ffmpegProcess;

    public Task StartRecordingAsync(string outputFilePath)
    {
        /*
         -f gdigrab: use GDI screen capture on Windows TODO: test later with avfoundation for MacOS
         -i desktop: capture the primary desktop screen
         -video_size 1920x1080: specify video size, if on 4k screen this will just capture the top left
         -vf "scale=1920:1080": ffmpeg's scaling filter to scale to desired resolution
         -c:v libx264: encode using H.264
         -pix_fmt yuv420p: standard pixel format
         -preset ultrafast: fast encoding for development stage
         scaling algorithms (scale=1920:1080:flags=lanczos):
            fast_bilinear: fast, low quality/blurring. Use when speed>quality
            bilinear: fast, ok quality, default
            bicubic: slow, smooth/sharper quality, better than bilinear for scaling
            lanczos: slowest, sharpest quality, best quality esp for downscaling
        */
        //var args = $"-f gdigrab -framerate 30 -i desktop -vf \"scale=1920:1080:flags=lanczos\" -c:v libx264 -pix_fmt yuv420p -preset ultrafast -crf 23 \"{outputFilePath}\"";
        // TODO: This captures all screens... -i desktop works fine if there's only 1 monitor but in a multiple monitor setup need to figure
        // out how to get get the bounds/coordinates from all the different screens to specify in the ffmpeg call

        var args = $"-f gdigrab -framerate 30 -offset_x 0 -offset_y 0 -video_size 3840x2160 -i desktop -c:v libx264 -pix_fmt yuv420p \"{outputFilePath}\""; 
        // Hard coded to my monitor for now
        
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
            catch
            {
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