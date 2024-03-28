using RaspCameraLibrary.Helpers;

namespace RaspCameraLibrary.Settings.Types;

/// <summary>
/// Thumbnail settings
/// </summary>
public class Thumb : Stringable
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Thumb()
    {
    }

    /// <summary>
    /// Constructor with parameters
    /// </summary>
    public Thumb
    (
        uint? Width = null,
        uint? Height = null,
        uint? Quality = null
    )
    {
        this.Width = Width;
        this.Height = Height;
        this.Quality = Quality;
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
    /// Quality (0-100)
    /// </summary>
    public uint? Quality
    {
        get => _quality;
        set => _quality = value is null ? null : uint.Clamp(value.Value, 0, 100);
    }
    private uint? _quality;

    /// <summary>
    /// Convert to string
    /// </summary>
    public override string ToString() => $"{Width}:{Height}:{Quality}";
}