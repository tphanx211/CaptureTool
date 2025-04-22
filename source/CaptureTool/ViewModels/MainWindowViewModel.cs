using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using CaptureTool.Services.Audio;
using CaptureTool.Services.Capture;
using CaptureTool.Services.Mux;
using CaptureTool.Services.Screen;
using CaptureTool.Services.Settings;
using CaptureTool.Services.Transcribe;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CaptureTool.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public AppSettings Settings {get;}
    private readonly ISettingsService _settingsService;
    private readonly ICaptureService _captureService;
    
    [ObservableProperty] private string _captureText = "Start Capture";
    [ObservableProperty] private bool _isRecording;

    public MainWindowViewModel()
    {
        // for previewer
    }

    public MainWindowViewModel(ISettingsService settings, ICaptureService  captureService)
    {
        _captureService = captureService;
        _settingsService = settings;
        Settings = _settingsService.Settings;
    }

    [RelayCommand]
    private void GoToUserLink()
    {
        if (!string.IsNullOrWhiteSpace(Settings.UserLink))
        {
            try
            {
                using var process = new System.Diagnostics.Process();
                process.StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = Settings.UserLink,
                    UseShellExecute = true
                };
                process.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    
    [RelayCommand]
    private void ToggleAudioTranscription()
    {
        Settings.EnableVoiceTranscription = !Settings.EnableVoiceTranscription;
    }

    [RelayCommand]
    private async Task ToggleCaptureAsync()
    {
        if (!IsRecording)
        {
            IsRecording = true;
            await _captureService.StartCaptureAsync();
            CaptureText = "Stop Capture";
        }
        else
        {
            IsRecording = false;
            await _captureService.StopCaptureAsync();
            CaptureText = "Start Capture";
        }
    }
    
    [RelayCommand]
    private void Exit()
    {
        
        _settingsService.Save();
        
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }
    
    
}