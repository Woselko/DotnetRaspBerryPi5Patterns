using RaspCameraLibrary.Helpers;
using RaspCameraLibrary.Settings.Enumerations;

namespace RaspCameraLibrary.Settings.Types;

/// <summary>
/// Camera mode
/// </summary>
public class Mode : Stringable
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Mode()
    {
    }

    /// <summary>
    /// Constructor with parameters
    /// </summary>
    public Mode
    (
        string? Name = null,
        uint? Width = null,
        uint? Height = null,
        double? Fps = null,
        Crop? Crop = null,
        BitDepth? BitDepth = null,
        Packing? Packed = null
    )
    {
        this.Name = Name;
        this.Width = Width;
        this.Height = Height;
        this.Fps = Fps;
        this.Crop = Crop;
        this.BitDepth = BitDepth;
        this.Packed = Packed;
    }

    /// <summary>
    /// Name S(Bayer order)(Bit-depth)_(Optional packing)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Width
    /// </summary>
    public uint? Width { get; set; }

    /// <summary>
    /// Height
    /// </summary>
    public uint? Height { get; set; }

    /// <summary>
    /// Frames per second
    /// </summary>
    public double? Fps { get; set; }

    /// <summary>
    /// Crop is the location of the crop window of size Width x Height in the sensor array. The units remain native sensor pixels, even if the sensor is being used in a binning or skipping mode.
    /// </summary>
    public Crop? Crop { get; set; }

    /// <summary>
    /// Bit depth
    /// </summary>
    /// <warning>
    /// Is not being parsed
    /// </warning>
    public BitDepth? BitDepth { get; set; }

    /// <summary>
    /// Packing
    /// </summary>
    /// <warning>
    /// Is not being parsed
    /// </warning>
    public Packing? Packed { get; set; }

    /// <summary>
    /// Convert to string
    /// </summary>
    public override string ToString()
    {
        return $"{Width}:{Height}" + (BitDepth != null ? $":{BitDepth.Value}" : "") + (Packed != null ? $":{Packed.Value}" : "");
    }
}