using System.Threading.Tasks;

namespace CaptureTool.Services.Capture;

public interface ICaptureService
{
    Task StartCaptureAsync();
    Task StopCaptureAsync();
}