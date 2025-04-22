using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using CaptureTool.Services.Screen;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CaptureTool.ViewModels;

public partial class ScreenPickerViewModel : ObservableObject
{
    [ObservableProperty] private List<MonitorInfo> _monitors;
    [ObservableProperty] MonitorInfo? _selectedMonitor;
    [ObservableProperty] private bool _canceled = true;

    private readonly IScreenDetectionService _screenDetectionService;
    private readonly Window _window;

    public ScreenPickerViewModel(IScreenDetectionService screenDetectionService, Window window)
    {
        _screenDetectionService = screenDetectionService;
        Monitors = _monitors;
        _window = window;
        
        _ = LoadMonitorsAsync();
    }

    [RelayCommand]
    private async Task LoadMonitorsAsync()
    {
        var results = await _screenDetectionService.GetMonitorsAsync();
        Monitors = results;
    }
    
    [RelayCommand]
    private void SelectMonitor(MonitorInfo monitor)
    {
        SelectedMonitor = monitor;
    }

    [RelayCommand]
    private void StartCapture()
    {
        Canceled = false;
        _window.Close();
    }
}