using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace CaptureTool.Services.Screen;

public class ScreenDetectionService : IScreenDetectionService
{
    public async Task<List<MonitorInfo>> GetMonitorsAsync()
    {
        var monitors = new List<MonitorInfo>();
        var tasks = new List<Task>();

        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
            (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData) =>
            {
                var mi = new MonitorInfoEx();
                mi.Size = Marshal.SizeOf<MonitorInfoEx>();

                if (GetMonitorInfo(hMonitor, ref mi))
                {
                    var bounds = Rectangle.FromLTRB(
                        mi.Monitor.Left,
                        mi.Monitor.Top,
                        mi.Monitor.Right,
                        mi.Monitor.Bottom);

                    var deviceName = mi.DeviceName;

                    var task = Task.Run(async () =>
                    {
                        var screenshot = await CapturePreviewToMemory(bounds);
                        lock (monitors)
                        {
                            monitors.Add(new MonitorInfo
                            {
                                DeviceName = deviceName,
                                Bounds = bounds,
                                Screenshot = screenshot
                            });
                        }
                    });

                    tasks.Add(task);
                }

                return true;
            },
            IntPtr.Zero);

        await Task.WhenAll(tasks);
        return monitors;
    }
    
    private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

    [DllImport("user32.dll")]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left, Top, Right, Bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MonitorInfoEx
    {
        public int Size;
        public Rect Monitor;
        public Rect WorkArea;
        public uint Flags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
    }
    
    public async Task<Avalonia.Media.Imaging.Bitmap?> CapturePreviewToMemory(Rectangle bounds)
    {
        var args = $"-f gdigrab -framerate 1 " +
                   $"-offset_x {bounds.Left} -offset_y {bounds.Top} " +
                   $"-video_size {bounds.Width}x{bounds.Height} -i desktop " +
                   "-vframes 1 -f mjpeg -";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        using var ms = new MemoryStream();
        await process.StandardOutput.BaseStream.CopyToAsync(ms);
        ms.Seek(0, SeekOrigin.Begin);

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
            return null;

        return new Avalonia.Media.Imaging.Bitmap(ms); // Avalonia.Media.Imaging.Bitmap
    }
}