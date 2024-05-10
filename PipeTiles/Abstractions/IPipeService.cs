using System.Collections.Generic;

namespace PipeTiles;

/// <summary>
/// Сервис работы с трубами.
/// </summary>
public interface IPipeService
{
    List<Network> GetNetworks();
    List<TilePipe> GetTilePipes(Network network);
}