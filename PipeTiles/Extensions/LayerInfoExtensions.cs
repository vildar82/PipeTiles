namespace PipeTiles;

using System;
using Autodesk.AutoCAD.DatabaseServices;
using RxBim.Tools.Autocad;

/// <summary>
/// Расширения для LayerInfo
/// </summary>
public static class LayerInfoExtensions
{
    /// <summary>
    /// Получает или создаёт слой в чертеже
    /// </summary>
    /// <param name="layerName">Имя слоя</param>
    /// <param name="db">Чертёж</param>
    public static ObjectId GetOrCreateLayer(this string layerName, Database db)
    {
        var layerInfo = new LayerInfo { Name = layerName };
        return GetOrCreateLayer(layerInfo, db);
    }

    /// <summary>
    /// Получает или создаёт слой в чертеже
    /// </summary>
    /// <param name="layerInfo">Данные слоя</param>
    /// <param name="db">Чертёж</param>
    /// <param name="overwrite">Переписать свойства из данных в слой</param>
    public static ObjectId GetOrCreateLayer(this LayerInfo layerInfo, Database db, bool overwrite = false)
    {
        ObjectId layerId;
        if (layerInfo.LayerId.Database == db)
        {
            layerId = layerInfo.LayerId;
        }
        else
        {
            if (layerInfo.Name.IsNullOrEmpty())
                throw new Exception("Пустое имя слоя.");

            using var lt = db.LayerTableId.OpenAs<LayerTable>(true);
            if (lt.Has(layerInfo.Name))
            {
                layerId = lt[layerInfo.Name];

                if (overwrite)
                {
                    using var layer = layerId.OpenAs<LayerTableRecord>(true);
                    Overwrite(layer, layerInfo, db);
                }
            }
            else
            {
                using var layer = new LayerTableRecord();
                layer.Name = layerInfo.Name;
                layerId = lt.Add(layer);
                Overwrite(layer, layerInfo, db);
            }
        }

        return layerId;
    }

    private static void Overwrite(LayerTableRecord layer, LayerInfo layerInfo, Database db)
    {
        if (layerInfo.IsFrozen != null)
            layer.IsFrozen = layerInfo.IsFrozen.Value;

        if (layerInfo.IsLocked != null)
            layer.IsLocked = layerInfo.IsLocked.Value;

        if (layerInfo.IsOff != null)
            layer.IsOff = layerInfo.IsOff.Value;

        if (layerInfo.IsPlottable != null)
            layer.IsPlottable = layerInfo.IsPlottable.Value;

        if (!layerInfo.Description.IsNullOrEmpty())
            layer.Description = layerInfo.Description;

        if (layerInfo.Color != null)
            layer.Color = layerInfo.Color;

        if (layerInfo.LineWeight != null)
            layer.LineWeight = layerInfo.LineWeight.Value;

        if (!layerInfo.LineTypeObjectId.IsNull && layerInfo.LineTypeObjectId.Database == db)
            layer.LinetypeObjectId = layerInfo.LineTypeObjectId;
    }
}