using System.Threading.Tasks;

namespace CaptureTool.Services.Mux;

public interface IMuxingService
{
    Task<string> MuxAsync(string videoPath, string audioPath, string? subtitlePath, string outputPath);
}