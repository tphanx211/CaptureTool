using System.Threading.Tasks;

namespace CaptureTool.Services.Transcribe;

public interface ITranscriptionService
{
    Task<TranscriptionResult> TranscribeAsync(string audioFilePath);
    
    Task<string> GenerateSrtFileAsync(TranscriptionResult result, string outputPath);
}