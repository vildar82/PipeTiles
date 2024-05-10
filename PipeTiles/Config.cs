using Microsoft.Extensions.Configuration;
using RxBim.Di;
using RxBim.Tools;
using RxBim.Tools.Autocad;

namespace PipeTiles;

public class Config : ICommandConfiguration
{
    public void Configure(IContainer container)
    {
        /*container.AddSingleton(() =>
        {
            var config = container.GetService<IConfiguration>();
            var section = config.GetSection(nameof(PluginSettings));
            var settings = section.Get<PluginSettings>();
            return settings;
        });*/

        container.AddSingletonFromConfig<PluginSettings>();
        container.AddAutocadHelpers();
        container.AddSingleton<IPipeService, PipeService>();
        container.AddSingleton<ITileService, TileService>();
        container.AddSingleton<MainWindow>();
        container.AddSingleton<MainViewModel>();
    }
}