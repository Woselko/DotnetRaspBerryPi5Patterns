using RaspCameraLibrary.Helpers;

namespace RaspCameraLibrary.Settings.Types;

/// <summary>
/// Crop
/// </summary>
public class Crop : Stringable
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Crop()
    {
    }

    /// <summary>
    /// Constructor with parameters
    /// </summary>
    public Crop
    (
        uint? X = null,
        uint? Y = null,
        uint? Width = null,
        uint? Height = null)
    {
        this.X = X;
        this.Y = Y;
        this.Width = Width;
        this.Height = Height;
    }

    /// <summary>
    /// X start coordinate
    /// </summary>
    public uint? X { get; set; }

    /// <summary>
    /// Y start coordinate
    /// </summary>
    public uint? Y { get; set; }

    /// <summary>
    /// Width
    /// </summary>
    public uint? Width { get; set; }

    /// <summary>
    /// Height
    /// </summary>
    public uint? Height { get; set; }

    /// <summary>
    /// Convert to string
    /// </summary>
    public override string ToString() => $"{X},{Y},{Width},{Height}";
}