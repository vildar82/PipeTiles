using System;
using System.Windows;
using RxBim.Command.Civil;
using RxBim.Shared;
using RxBim.Shared.Autocad;

namespace PipeTiles;

/// <inheritdoc />
[RxBimCommandClass("PipeTiles")]
public class Cmd : RxBimCommand
{
    public PluginResult ExecuteCommand(MainWindow win)
    {
        try
        {
            win.ShowDialog();
            return PluginResult.Succeeded;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return PluginResult.Failed;
        }
    }
}