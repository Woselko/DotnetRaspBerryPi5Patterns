using RaspCameraLibrary.Helpers;

namespace RaspCameraLibrary.Settings.Types;

/// <summary>
/// Size settings
/// </summary>
public class Size : Stringable
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Size()
    {
    }

    /// <summary>
    /// Constructor with parameters
    /// </summary>
    public Size
    (
        uint? Width = null,
        uint? Height = null
    )
    {
        this.Width = Width;
        this.Height = Height;
    }

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
    public override string ToString() => $"{Width}x{Height}";
}