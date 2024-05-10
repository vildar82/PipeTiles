namespace PipeTiles;

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

/// <summary>
/// Расширения для строк
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Объединение списка объектов в одну строку, с разделителем и методом получения строки из объекта.
    /// </summary>
    /// <param name="array">Список объектов</param>
    /// <param name="separator">Разделитель</param>
    /// <param name="getString">Получение строки из объекта</param>
    /// <typeparam name="T">Тип объекта</typeparam>
    public static string JoinToString<T>(
        this IEnumerable<T>? array,
        string separator = ", ",
        Func<T, string>? getString = null)
    {
        return array == null
            ? string.Empty
            : string.Join(separator, array.Select(o => getString == null ? o?.ToString() : getString(o)));
    }

    /// <summary>
    /// Пустая строка?
    /// </summary>
    /// <param name="value">Строка</param>
    [UsedImplicitly]
    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Строка пустая или с одними пробелами?
    /// </summary>
    /// <param name="value">Строка</param>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Пустые и null строки считаются равными
    /// </summary>
    /// <param name="s1">Строка 1</param>
    /// <param name="s2">Строка 2</param>
    /// <returns></returns>
    public static bool IsBothStringIsNullOrEmpty(this string? s1, string? s2)
    {
        return string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2);
    }

    /// <summary>
    /// Равна ли строка любой из списка
    /// </summary>
    /// <param name="value">Строка</param>
    /// <param name="values">Список</param>
    public static bool EqualsAny(this string value, params string[] values)
    {
        return values.Contains(value);
    }

    /// <summary>
    /// Равна ли строка любой из списка
    /// </summary>
    /// <param name="value">Строка</param>
    /// <param name="comparer">Сравнение</param>
    /// <param name="values">Список</param>
    public static bool EqualsAny(this string value, IEqualityComparer<string> comparer, params string[] values)
    {
        return EqualsAny(value, comparer, (IEnumerable<string>)values);
    }

    /// <summary>
    /// Равна ли строка любой из списка
    /// </summary>
    /// <param name="target">Строка</param>
    /// <param name="comparer">Сравнение</param>
    /// <param name="values">Список</param>
    public static bool EqualsAny(this string target, IEqualityComparer<string> comparer, IEnumerable<string> values)
    {
        return values.Contains(target, comparer);
    }

    /// <summary>
    /// Равна ли строка любой из списка без учета регистра
    /// </summary>
    /// <param name="value">Строка</param>
    /// <param name="values">Список</param>
    public static bool EqualsAnyIgnoreCase(this string value, params string[] values)
    {
        return EqualsAny(value, StringComparer.OrdinalIgnoreCase, (IEnumerable<string>)values);
    }

    /// <summary>
    /// Равна ли строка любой из списка без учета регистра
    /// </summary>
    /// <param name="target">Строка</param>
    /// <param name="values">Список</param>
    public static bool EqualsAnyIgnoreCase(this string target, IEnumerable<string> values)
    {
        return values.Contains(target, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Сравнение строк без учета регистра.
    /// Внимание! null, "", и строка только с пробелами считаются равными!
    /// </summary>
    /// <param name="string1">Строка 1</param>
    /// <param name="string2">Строка 2</param>
    public static bool EqualsIgnoreCase(this string? string1, string? string2)
    {
        return string.Equals(string1, string2, StringComparison.OrdinalIgnoreCase) ||
               IsBothStringIsNullOrEmpty(string1, string2);
    }

    /// <summary>
    /// Разбить строку на части
    /// </summary>
    /// <param name="value">Строка</param>
    /// <param name="length">Длина части</param>
    public static IEnumerable<string> Split(this string value, int length)
    {
        var skip = 0;
        do
        {
            var part = string.Concat(value.Skip(skip).Take(length));
            if (string.IsNullOrEmpty(part))
                break;
            yield return part;
            skip += length;
        }
        while (true);
    }
}