using System.Threading.Tasks;

namespace CaptureTool.Services.Audio;

public interface IAudioRecordService
{
    Task StartRecordingAsync(string outputFilePath);
    Task StopRecordingAsync();
}