using System.Threading.Tasks;
using CaptureTool.Services.Screen;

namespace CaptureTool.Services.Capture;

public interface ICaptureService
{
    Task StartCaptureAsync(MonitorInfo monitor);
    Task StopCaptureAsync();
}