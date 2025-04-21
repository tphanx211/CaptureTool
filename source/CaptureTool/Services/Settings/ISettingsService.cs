namespace CaptureTool.Services.Settings;

public interface ISettingsService
{
    AppSettings Settings { get; }
    void Save();
}