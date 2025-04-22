using Avalonia.Media.Imaging;

namespace CaptureTool.Services.Screen;

public class MonitorInfo
{
    public string DeviceName { get; set; } = string.Empty;
    public System.Drawing.Rectangle Bounds { get; set; }
    public Bitmap? Screenshot { get; set; }
}