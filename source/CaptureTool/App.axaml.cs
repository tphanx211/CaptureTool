using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using CaptureTool.Services.Audio;
using CaptureTool.Services.Capture;
using CaptureTool.Services.Mux;
using CaptureTool.Services.Screen;
using CaptureTool.Services.Settings;
using CaptureTool.Services.Transcribe;
using CaptureTool.ViewModels;
using CaptureTool.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CaptureTool;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        DisableAvaloniaDataAnnotationValidation();
            
        var collection = new ServiceCollection();
        collection.AddSingleton<ISettingsService, SettingsService>();
        collection.AddSingleton<ITranscriptionService, TranscriptionService>();
        collection.AddSingleton<IAudioRecordService, AudioRecordService>();
        collection.AddSingleton<IScreenRecordService, ScreenRecordService>();
        collection.AddSingleton<IMuxingService, MuxingService>();
        collection.AddSingleton<ICaptureService, CaptureService>();
        collection.AddSingleton<MainWindowViewModel>();
        
        collection.AddTransient<IScreenDetectionService, ScreenDetectionService>();
        collection.AddTransient<ScreenPickerViewModel>();
        
        var services = collection.BuildServiceProvider();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var settingsService = services.GetRequiredService<ISettingsService>();
                
            desktop.Exit += (_, _) =>
            {
                settingsService.Save();
            };
            
            desktop.MainWindow = new MainWindow
            {
                DataContext = services.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}