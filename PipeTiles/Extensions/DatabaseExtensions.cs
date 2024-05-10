namespace PipeTiles;

using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using RxBim.Tools.Autocad;

/// <summary>
/// Расширения для чертежа
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Возвращает масштаб аннотаций
    /// </summary>
    /// <param name="db">Чертёж</param>
    public static double GetAnnotationScale(this Database db)
    {
        try
        {
            return 1 / db.Cannoscale.Scale;
        }
        catch
        {
            return 1;
        }
    }

    /// <summary>
    /// Id Модели чертежа
    /// </summary>
    /// <param name="db">Чертёж</param>
    public static ObjectId GetModelSpaceId(this Database db)
    {
        return SymbolUtilityServices.GetBlockModelSpaceId(db);
    }

    /// <summary>
    /// Модель чертежа
    /// </summary>
    /// <param name="db">Чертеж</param>
    /// <param name="forWrite">С открытием для записи</param>
    public static BlockTableRecord GetModelSpace(this Database db, bool forWrite = false)
    {
        return SymbolUtilityServices.GetBlockModelSpaceId(db).GetObjectAs<BlockTableRecord>(forWrite);
    }

    /// <summary>
    /// Получение табличного стиля по одному из имени или текущий стиль чертежа.
    /// </summary>
    /// <param name="db">Чертеж</param>
    /// <param name="styleNames">Имена табличных стилей</param>
    public static ObjectId GetTableStyle(this Database db, params string[] styleNames)
    {
        using var tableStyleDic = db.TableStyleDictionaryId.OpenAs<DBDictionary>();
        var findStyleName = styleNames.FirstOrDefault(styleName => tableStyleDic.Contains(styleName));
        return findStyleName != null ? tableStyleDic.GetAt(findStyleName) : db.Tablestyle;
    }
}