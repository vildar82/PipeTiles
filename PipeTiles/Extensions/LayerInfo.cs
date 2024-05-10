namespace PipeTiles;

using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using RxBim.Tools.Autocad;

/// <summary>
/// Данные слоя
/// </summary>
public class LayerInfo
{
    /// <summary>
    /// Конструктор
    /// </summary>
    public LayerInfo()
    {
    }

    /// <summary>
    /// Конструктор из объекта слоя
    /// </summary>
    /// <param name="idLayer">Id слоя</param>
    public LayerInfo(ObjectId idLayer)
    {
        using var layer = idLayer.OpenAs<LayerTableRecord>();
        Name = layer.Name;
        Color = layer.Color;
        using var lt = layer.LinetypeObjectId.OpenAs<LinetypeTableRecord>();
        LineType = lt.Name;
        LineWeight = layer.LineWeight;
        IsPlottable = layer.IsPlottable;
        IsFrozen = layer.IsFrozen;
        IsLocked = layer.IsLocked;
        IsOff = layer.IsOff;
        Description = layer.Description;
        LayerId = idLayer;
    }

    /// <summary>
    /// Цвет
    /// </summary>
    public Color? Color { get; set; }

    /// <summary>
    /// Заморожен
    /// </summary>
    public bool? IsFrozen { get; set; }

    /// <summary>
    /// Заблокирован
    /// </summary>
    public bool? IsLocked { get; set; }

    /// <summary>
    /// Отключен
    /// </summary>
    public bool? IsOff { get; set; }

    /// <summary>
    /// Печатаемый
    /// </summary>
    public bool? IsPlottable { get; set; }

    /// <summary>
    /// Id слоя
    /// </summary>
    public ObjectId LayerId { get; set; }

    /// <summary>
    /// Тип линии
    /// </summary>
    public string? LineType { get; set; }

    /// <summary>
    /// Id типа линии
    /// </summary>
    public ObjectId LineTypeObjectId { get; set; }

    /// <summary>
    /// Вес линий
    /// </summary>
    public LineWeight? LineWeight { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    public string? Description { get; set; }
}