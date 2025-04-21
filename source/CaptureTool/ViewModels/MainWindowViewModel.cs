using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CaptureTool.Services.Settings;
using CommunityToolkit.Mvvm.Input;

namespace CaptureTool.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public AppSettings Settings {get;}
    private readonly ISettingsService _settingsService;

    public MainWindowViewModel()
    {
        // for previewer
    }

    public MainWindowViewModel(ISettingsService settings)
    {
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
    private void StartCapture()
    {
        
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