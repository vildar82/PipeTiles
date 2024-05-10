using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;

namespace PipeTiles;

/// <summary>
/// Участок трубы для построения плиток.
/// </summary>
public class TilePipe
{
    public TilePipe(Pipe pipe)
    {
        PartSize = pipe.PartSizeName;
        StartPoint = pipe.StartPoint;
        EndPoint = pipe.EndPoint;
        Height = pipe.OuterHeight > 0 ? pipe.OuterHeight : pipe.OuterDiameterOrWidth;
    }

    public TilePipe(TilePipe tilePipe, Point3d startPoint, Point3d endPoint)
    {
        PartSize = tilePipe.PartSize;
        Height = tilePipe.Height;
        StartPoint = startPoint;
        EndPoint = endPoint;
    }

    public string PartSize { get; }
    public Point3d StartPoint { get; }
    public Point3d EndPoint { get; }
    public double Height { get; }
}