﻿using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using RxBim.Tools;
using RxBim.Tools.Autocad;

namespace PipeTiles;

public class TileService : ITileService
{
    private readonly ITransactionService _transactionService;
    private double _tileLength;
    private double _tileWidth;
    private double _tileThickness;

    public TileService(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    public int CreateTiles(List<TilePipe> pipes, string networkName, PluginSettings settings)
    {
        var tileCount = 0;
        _tileLength = settings.TileLength.ConvertMmToM();
        _tileWidth = settings.TileWidth.ConvertMmToM();
        _tileThickness = settings.TileThickness.ConvertMmToM();

        _transactionService.RunInDocumentTransaction((d, tw) =>
        {
            var doc = d.Unwrap<Document>();
            var ms = doc.Database.GetModelSpace(true);
            var t = tw.Unwrap<Transaction>();
            var layerId = networkName.GetOrCreateLayer(doc.Database);

            foreach (var pipe in pipes)
            {
                var tileSolids = CreatePipeTiles(pipe, settings);

                foreach (var tileSolid in tileSolids)
                {
                    tileSolid.LayerId = layerId;
                    tileSolid.ColorIndex = settings.TileColorIndex;
                    AppendEntity(ms, tileSolid, t);
                    tileCount++;
                }
            }

            doc.TransactionManager.QueueForGraphicsFlush();
        });

        return tileCount;
    }

    private IEnumerable<Solid3d> CreatePipeTiles(TilePipe pipe, PluginSettings settings)
    {
        var offsetY = settings.VerticalOffsetTiles.ConvertMmToM() + pipe.Height;
        var pipeVec = pipe.EndPoint - pipe.StartPoint;
        var xVec = pipeVec.GetNormal();
        var yVec = xVec.GetPerpendicularVector();
        var zVec = xVec.CrossProduct(yVec);
        var tilesCount = Math.Ceiling(pipeVec.Length / _tileLength);
        var tilesLength = tilesCount * _tileLength;
        var tilePoint = GetStartTilePoint(pipeVec, pipe.StartPoint, tilesLength);

        for (var i = 0; i < tilesCount; i++)
        {
            var tileMovePoint = tilePoint + xVec * _tileLength * 0.5;

            var matrixAlign = Matrix3d.AlignCoordinateSystem(
                Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis,
                tileMovePoint.Move(0, 0, offsetY), xVec, yVec, zVec);

            var solid = new Solid3d();
            solid.CreateBox(_tileLength, _tileWidth, _tileThickness);

            solid.TransformBy(matrixAlign);
            yield return solid;

            tilePoint += xVec * _tileLength;
        }
    }

    private static Point3d GetStartTilePoint(Vector3d pipeVec, Point3d startPoint, double tilesLength)
    {
        var pipeLength = pipeVec.Length;

        if (tilesLength.IsEqualOrLess(pipeLength))
            return startPoint;

        var overLength = tilesLength - pipeLength;
        return startPoint + pipeVec.Negate().GetNormal() * overLength * 0.5;
    }

    private static void AppendEntity(BlockTableRecord ms, Entity entity, Transaction t)
    {
        ms.AppendEntity(entity);
        t.AddNewlyCreatedDBObject(entity, true);
    }
}