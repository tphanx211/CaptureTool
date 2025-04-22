using System.Collections.Generic;

namespace CaptureTool.Services.Transcribe;

public class TranscriptionResult
{
    public string FullText { get; set; } = string.Empty;

    public List<TranscriptionSegment> Segments { get; set; } = new();
}

public class TranscriptionSegment
{
    public int Id { get; set; }
    public double Start { get; set; }
    public double End { get; set; }
    public string Text { get; set; } = string.Empty;
}