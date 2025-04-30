using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
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
using CaptureTool.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace CaptureTool.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public AppSettings Settings {get;}
    private readonly ISettingsService _settingsService;
    private readonly ICaptureService _captureService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IAudioRecordService _audioRecordService;
    
    [ObservableProperty] private string _captureText = "Start Capture";
    [ObservableProperty] private bool _isRecording;
    [ObservableProperty] private List<AudioInputDevice> _audioInputDevices = new();
    [ObservableProperty] private AudioInputDevice? _selectedAudioInputDevice;

    public MainWindowViewModel()
    {
        // for previewer
    }

    public MainWindowViewModel(ISettingsService settings, ICaptureService captureService, IServiceProvider serviceProvider)
    {
        _captureService = captureService;
        _settingsService = settings;
        _serviceProvider = serviceProvider;
        _audioRecordService = _serviceProvider.GetRequiredService<IAudioRecordService>();
        
        Settings = _settingsService.Settings;
        
        // Initialize audio devices
        LoadAudioDevices();
    }
    
    private void LoadAudioDevices()
    {
        AudioInputDevices = _audioRecordService.ListInputDevices();
        if (AudioInputDevices.Count > 0)
        {
            SelectedAudioInputDevice = AudioInputDevices[0];
        }
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
            var screenPickerWindow = new ScreenPickerWindow();

            var detectionService = _serviceProvider.GetRequiredService<IScreenDetectionService>();

            var pickerViewModel = new ScreenPickerViewModel(detectionService, screenPickerWindow);
            screenPickerWindow.DataContext = pickerViewModel;

            // Show the window and wait for it to close
            await screenPickerWindow.ShowDialog((Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);

            if (pickerViewModel.SelectedMonitor is null)
                return;
            
            IsRecording = true;
            CaptureText = "Stop Capture";
            await _captureService.StartCaptureAsync(pickerViewModel.SelectedMonitor, SelectedAudioInputDevice);
        }
        else
        {
            IsRecording = false;
            CaptureText = "Start Capture";
            await _captureService.StopCaptureAsync();
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