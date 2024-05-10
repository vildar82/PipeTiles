using System;
using System.Windows;
using RxBim.Command.Civil;
using RxBim.Shared;
using RxBim.Shared.Autocad;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace PipeTiles;

/// <inheritdoc />
[RxBimCommandClass("PipeTiles")]
public class Cmd : RxBimCommand
{
    public PluginResult ExecuteCommand(MainWindow win)
    {
        try
        {
            Application.ShowModalWindow(win);
            return PluginResult.Succeeded;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return PluginResult.Failed;
        }
    }
}