namespace CaptureTool.Services.Audio;

public record AudioInputDevice(int DeviceNumber, string DeviceName)// trying out .NET 9.0 new record
{
    public override string ToString() => DeviceName;
}