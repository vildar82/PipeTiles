namespace PipeTiles;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

/// <summary>
/// Расширения для Point3d
/// </summary>
public static class Point3dExtensions
{
    /// <summary>
    /// Возвращает центр между точками
    /// </summary>
    /// <param name="pt1">Первая точка</param>
    /// <param name="pt2">Вторая точка</param>
    public static Point3d GetCenter(this Point3d pt1, Point3d pt2)
    {
        return pt1 + (pt2 - pt1) * 0.5;
    }

    /// <summary>
    /// Рамка вокруг точки
    /// </summary>
    /// <param name="pt">Точка</param>
    /// <param name="width">Ширина рамки</param>
    /// <param name="height">Высота рамки</param>
    public static Extents3d GetExtents(this Point3d pt, double width, double? height = null)
    {
        height ??= width;
        return new Extents3d(
            pt.Move(-width * 0.5, -height.Value * 0.5),
            pt.Move(width * 0.5, height.Value * 0.5));
    }

    /// <summary>
    /// Точка со смещением
    /// </summary>
    /// <param name="pt">Точка</param>
    /// <param name="x">Смещение по X</param>
    /// <param name="y">Смещение по Y</param>
    /// <param name="z">Смещение по Z</param>
    public static Point3d Move(this Point3d pt, double x, double y, double z = 0)
    {
        return pt + new Vector3d(x, y, z);
    }
}