using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CaptureTool.Services.Settings;

public partial class AppSettings : ObservableObject
{
    [ObservableProperty] private bool _launchAtStartup;
    [ObservableProperty] private bool _enableSessionLogging;
    [ObservableProperty] private bool _captureLabels;
    [ObservableProperty] private bool _captureKeyboardInput;
    [ObservableProperty] private bool _captureSpecialKeys;
    [ObservableProperty] private bool _enableVoiceTranscription;
    [ObservableProperty] private string _userLink = "https://www.linkedin.com/in/anthony-phan-b2263b123/";
    [ObservableProperty] private string _captureSaveLocation;
    [ObservableProperty] private string _apiKeyPath = @"C:\Users\TonyH\ApiKey.txt";

    public AppSettings()
    {
        if (OperatingSystem.IsWindows())
            _captureSaveLocation = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
                "Videos" ,"CaptureTool","Captures");
        else
            _captureSaveLocation = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "Movies", "CaptureTool", "Captures");
    }
}