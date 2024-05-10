namespace PipeTiles;

public class PluginSettings
{
    /// <summary>
    /// Отступ плиток по вертикале от труб.
    /// </summary>
    public double VerticalOffsetTiles { get; set; }

    /// <summary>
    /// Ширина плитки.
    /// </summary>
    public double TileWidth { get; set; }

    /// <summary>
    /// Длина плитки.
    /// </summary>
    public double TileLength { get; set; }

    /// <summary>
    /// Толщина плитки.
    /// </summary>
    public double TileThickness { get; set; }

    /// <summary>
    /// Цвет плитки.
    /// </summary>
    public int TileColorIndex { get; set; }

    /// <summary>
    /// Соответствие имени типоразмера каналу.
    /// </summary>
    public string PartSizeCanalMatch { get; set; } = string.Empty;

    /// <summary>
    /// Соответствие имени типоразмера защитной трубы.
    /// </summary>
    public string PartSizeProtectMatch { get; set; } = string.Empty;
}