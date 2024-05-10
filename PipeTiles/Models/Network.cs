using Autodesk.AutoCAD.DatabaseServices;

namespace PipeTiles;

/// <summary>
/// Сеть.
/// </summary>
public record Network(string Name, ObjectId Id);