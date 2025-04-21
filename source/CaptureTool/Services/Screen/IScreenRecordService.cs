using System.Threading.Tasks;

namespace CaptureTool.Services.Screen;

public interface IScreenRecordService
{
    Task StartRecordingAsync(string outputFilePath, 
        IScreenRecordConfig config);
    Task StopRecordingAsync();
}