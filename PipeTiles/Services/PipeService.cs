using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using RxBim.Tools;
using RxBim.Tools.Autocad;

namespace PipeTiles;

/// <inheritdoc />
public class PipeService : IPipeService
{
    private readonly ITransactionService _transactionService;
    private readonly Tolerance _tolerance = new (0.1, 0.1);

    public PipeService(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    public List<Network> GetNetworks()
    {
        return _transactionService.RunInDocumentTransaction((d, _) =>
        {
            var civil = CivilDocument.GetCivilDocument(d.Unwrap<Document>().Database);

            return civil.GetPipeNetworkIds()
                .GetObjectsOf<Autodesk.Civil.DatabaseServices.Network>()
                .Select(n => new Network(n.Name, n.Id))
                .ToList();
        });
    }

    public List<TilePipe> GetTilePipes(Network network, PluginSettings settings)
    {
        return _transactionService.RunInDocumentTransaction(() =>
        {
            var pipes = network.Id
                .GetObjectAs<Autodesk.Civil.DatabaseServices.Network>()
                .GetPipeIds()
                .GetObjectsOf<Pipe>()
                .ToList();

            if (!pipes.Any())
                throw new Exception("Трубы сети не найдены");

            var canals = pipes
                .Where(p => PartSizeMatch(p.PartSizeName, settings.PartSizeCanalMatch))
                .ToList();
            if (!canals.Any())
                throw new Exception("Трубы каналов не найдены");

            var protects = settings.PartSizeProtectMatch.IsNullOrEmpty()
                ? new List<Pipe>()
                : pipes.Where(p => PartSizeMatch(p.PartSizeName, settings.PartSizeProtectMatch)).ToList();

            return canals.SelectMany(c => GetTilePipes(c, protects)).ToList();
        });
    }

    private IEnumerable<TilePipe> GetTilePipes(Pipe pipe, List<Pipe> allProtects)
    {
        var pipeVec = pipe.EndPoint - pipe.StartPoint;
        var tilePipes = new List<TilePipe> {new(pipe)};

        foreach (var protect in allProtects)
        {
            // Проверка, что защитная труба попадает на канал.
            if (!CheckProtectIsOnStraightLine(pipe, pipeVec, protect, out var protectStart, out var protectEnd, out var fullProtect))
                continue;

            if (fullProtect)
                return Enumerable.Empty<TilePipe>();

            tilePipes = tilePipes
                .SelectMany(t => SplitTilePipes(t, protectStart, protectEnd, pipeVec))
                .ToList();
        }

        return tilePipes;
    }

    private IEnumerable<TilePipe> SplitTilePipes(
        TilePipe tilePipe,
        Point3d protectStart,
        Point3d protectEnd,
        Vector3d pipeVec)
    {
        var vecTileStartToProtectStart = protectStart - tilePipe.StartPoint;
        if (vecTileStartToProtectStart.IsCodirectionalTo(pipeVec, _tolerance) &&
            vecTileStartToProtectStart.Length > 0.05)
            yield return new (tilePipe, tilePipe.StartPoint, tilePipe.StartPoint + vecTileStartToProtectStart);

        var vecTileEndToProtectEnd = protectEnd - tilePipe.EndPoint;
        if (!vecTileEndToProtectEnd.IsCodirectionalTo(pipeVec, _tolerance) &&
            vecTileEndToProtectEnd.Length > 0.05)
            yield return new (tilePipe, tilePipe.EndPoint + vecTileEndToProtectEnd, tilePipe.EndPoint);
    }

    private bool CheckProtectIsOnStraightLine(
        Pipe pipe,
        Vector3d pipeVec,
        Pipe protect,
        out Point3d protectStart,
        out Point3d protectEnd,
        out bool fullProtect)
    {
        fullProtect = false;
        protectStart = protect.StartPoint;
        protectEnd = protect.EndPoint;
        var protectVec = protectEnd - protectStart;

        if (!pipeVec.IsParallelTo(protectVec, _tolerance))
            return false;

        if (!pipeVec.IsCodirectionalTo(protectVec, _tolerance))
        {
            protectStart = protect.EndPoint;
            protectEnd = protect.StartPoint;
        }

        if (pipe.EndPoint.IsEqualTo(protectStart, _tolerance) ||
            pipe.StartPoint.IsEqualTo(protectEnd, _tolerance))
            return false;

        var isStartEquals = pipe.StartPoint.IsEqualTo(protectStart, _tolerance);
        var isEndEquals = pipe.EndPoint.IsEqualTo(protectEnd, _tolerance);

        if (isStartEquals && isEndEquals)
        {
            fullProtect = true;
            return true;
        }

        if (!isStartEquals)
        {
            var vecProtectEndToStart = protectEnd - pipe.StartPoint;
            if (!pipeVec.IsParallelTo(vecProtectEndToStart, _tolerance) ||
                !vecProtectEndToStart.IsCodirectionalTo(pipeVec, _tolerance))
                return false;
        }

        if (!isEndEquals)
        {
            var vecProtectStartToEnd = protectStart - pipe.EndPoint;
            if (!pipeVec.IsParallelTo(vecProtectStartToEnd, _tolerance) ||
                vecProtectStartToEnd.IsCodirectionalTo(pipeVec, _tolerance))
                return false;
        }

        return true;
    }

    private bool PartSizeMatch(string partSize, string match) =>
        Regex.IsMatch(partSize, match, RegexOptions.IgnoreCase);
}