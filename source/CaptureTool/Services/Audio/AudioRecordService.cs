using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NAudio.Wave;

namespace CaptureTool.Services.Audio;

public class AudioRecordService : IAudioRecordService
{
    private WaveInEvent? _waveIn;
    private WaveFileWriter? _writer;
    private bool _disposed;

    public Task StartRecordingAsync(string outputFilePath, AudioInputDevice inputDevice)
    {
        _waveIn = new WaveInEvent
        {
            DeviceNumber = inputDevice.DeviceNumber,
            WaveFormat = new WaveFormat(44100, 1) 
        };

        _writer = new WaveFileWriter(outputFilePath, _waveIn.WaveFormat);

        _waveIn.DataAvailable += (s, e) =>
        {
            _writer?.Write(e.Buffer, 0, e.BytesRecorded);
            _writer?.Flush();
        };

        _waveIn.RecordingStopped += (s, e) =>
        {
            _writer?.Dispose();
            _writer = null;
            _waveIn?.Dispose();
            _waveIn = null;
        };

        _waveIn.StartRecording();

        return Task.CompletedTask;
    }

    public Task StopRecordingAsync()
    {
        _waveIn?.StopRecording();
        return Task.CompletedTask;
    }
    
    public List<AudioInputDevice> ListInputDevices()
    {
        var devices = new List<AudioInputDevice>();

        for (int i = 0; i < WaveInEvent.DeviceCount; i++)
        {
            var vaps = WaveInEvent.GetCapabilities(i);
            devices.Add(new AudioInputDevice(i, vaps.ProductName));
            Console.WriteLine($"{i}: {vaps.ProductName}");
        }
        
        return devices;
    }
    
    private void DisposeWave()
    {
        _writer?.Dispose();
        _writer = null;

        _waveIn?.Dispose();
        _waveIn = null;
    }

    public void Dispose()
    {
        if (_disposed) return;
        DisposeWave();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}