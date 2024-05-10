using System.Collections.Generic;

namespace PipeTiles;

/// <summary>
/// Сервис построения плиток.
/// </summary>
public interface ITileService
{
    /// <summary>
    /// Создает плитки.
    /// </summary>
    /// <param name="pipes">Список участков труб.</param>
    /// <param name="networkName">Имя сети.</param>
    void CreateTiles(List<TilePipe> pipes, string networkName);
}