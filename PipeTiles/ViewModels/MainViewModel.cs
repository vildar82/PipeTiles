using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace PipeTiles;

public class MainViewModel : ObservableObject
{
    private static PluginSettings? _settings;
    private readonly IPipeService _pipeService;
    private readonly ITileService _tileService;
    private Network? _network;

    public MainViewModel(
        IPipeService pipeService,
        ITileService tileService,
        PluginSettings pluginSettings)
    {
        _settings ??= pluginSettings;
        _pipeService = pipeService;
        _tileService = tileService;

        Networks = pipeService.GetNetworks();
        Network = Networks.FirstOrDefault();
    }

    public PluginSettings Settings => _settings ?? new();

    public List<Network> Networks { get; set; }

    public Network? Network
    {
        get => _network;
        set => Set(ref _network, value);
    }

    public ICommand CreateTilesCommand => new RelayCommand(CreateTilesExecute, () => Network != null);

    private void CreateTilesExecute()
    {
        try
        {
            var pipes = _pipeService.GetTilePipes(Network!, Settings);
            var tileCount = _tileService.CreateTiles(pipes, Network!.Name, Settings);
            MessageBox.Show($"Готово. Построено {tileCount} плиток.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}