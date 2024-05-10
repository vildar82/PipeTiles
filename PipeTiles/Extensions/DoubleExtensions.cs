namespace PipeTiles;

public static class DoubleExtensions
{
    /// <summary>
    /// Конвертирует мм в м.
    /// </summary>
    /// <param name="valueMm">Значение в мм.</param>
    public static double ConvertMmToM(this double valueMm) => valueMm * 0.001;
}