using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(PipeTiles.App))]

namespace PipeTiles;

using JetBrains.Annotations;
using RxBim.Application.Autocad;
using RxBim.Shared;

/// <inheritdoc />
public class App : RxBimApplication
{
    /// <summary>
    /// Запуск
    /// </summary>
    [UsedImplicitly]
    public PluginResult Start()
    {
        return PluginResult.Succeeded;
    }

    /// <summary>
    /// Завершение работы
    /// </summary>
    [UsedImplicitly]
    public PluginResult Shutdown()
    {
        return PluginResult.Succeeded;
    }
}