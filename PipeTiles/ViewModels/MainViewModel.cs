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
    private readonly IPipeService _pipeService;
    private readonly ITileService _tileService;
    private Network? _network;

    public MainViewModel(
        IPipeService pipeService,
        ITileService tileService)
    {
        _pipeService = pipeService;
        _tileService = tileService;

        Networks = pipeService.GetNetworks();
        Network = Networks.FirstOrDefault();
    }

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
            var pipes = _pipeService.GetTilePipes(Network!);
            _tileService.CreateTiles(pipes, Network!.Name);
            MessageBox.Show("Готово");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}