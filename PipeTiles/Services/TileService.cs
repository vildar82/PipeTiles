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
    private readonly PluginSettings _pluginSettings;
    private double _tileLength;
    private double _tileWidth;
    private double _tileThickness;

    public TileService(
        ITransactionService transactionService,
        PluginSettings pluginSettings)
    {
        _transactionService = transactionService;
        _pluginSettings = pluginSettings;
    }

    public void CreateTiles(List<TilePipe> pipes, string networkName)
    {
        _tileLength = _pluginSettings.TileLength.ConvertMmToM();
        _tileWidth = _pluginSettings.TileWidth.ConvertMmToM();
        _tileThickness = _pluginSettings.TileThickness.ConvertMmToM();

        _transactionService.RunInDocumentTransaction((d, tw) =>
        {
            var doc = d.Unwrap<Document>();
            var ms = doc.Database.GetModelSpace(true);
            var t = tw.Unwrap<Transaction>();
            var layerId = networkName.GetOrCreateLayer(doc.Database);

            foreach (var pipe in pipes)
            {
                var tileSolids = CreatePipeTiles(pipe);

                foreach (var tileSolid in tileSolids)
                {
                    tileSolid.LayerId = layerId;
                    tileSolid.ColorIndex = _pluginSettings.TileColorIndex;
                    AppendEntity(ms, tileSolid, t);
                }
            }

            doc.TransactionManager.QueueForGraphicsFlush();
        });
    }

    private IEnumerable<Solid3d> CreatePipeTiles(TilePipe pipe)
    {
        var offsetY = _pluginSettings.VerticalOffsetTiles.ConvertMmToM() + pipe.Height;
        var pipeVec = pipe.EndPoint - pipe.StartPoint;
        var xVec = pipeVec.GetNormal();
        var yVec = xVec.GetPerpendicularVector();
        var zVec = xVec.CrossProduct(yVec);
        var point = pipe.StartPoint;

        while ((point - pipe.StartPoint).Length <= pipeVec.Length)
        {
            var remainPipeLength = (point - pipe.EndPoint).Length;
            if (remainPipeLength.IsZero())
                yield break;

            var currentTileLength = remainPipeLength >= _tileLength ? _tileLength : remainPipeLength;
            var tileMovePoint = point + xVec * currentTileLength * 0.5;

            var matrixAlign = Matrix3d.AlignCoordinateSystem(
                Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis,
                tileMovePoint.Move(0, 0, offsetY), xVec, yVec, zVec);

            var solid = new Solid3d();
            solid.CreateBox(_tileLength, _tileWidth, _tileThickness);

            solid.TransformBy(matrixAlign);
            yield return solid;

            point += xVec * currentTileLength;
        }
    }

    private void AppendEntity(BlockTableRecord ms, Entity entity, Transaction t)
    {
        ms.AppendEntity(entity);
        t.AddNewlyCreatedDBObject(entity, true);
    }
}