using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CaptureTool.Services.Audio;

public interface IAudioRecordService : IDisposable
{
    Task StartRecordingAsync(string outputFilePath, AudioInputDevice inputDevice);
    Task StopRecordingAsync();
    List<AudioInputDevice> ListInputDevices();
}