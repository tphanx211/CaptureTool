using System.Collections.Generic;
using System.Threading.Tasks;

namespace CaptureTool.Services.Screen;

public interface IScreenDetectionService
{
    Task<List<MonitorInfo>> GetMonitorsAsync();
}