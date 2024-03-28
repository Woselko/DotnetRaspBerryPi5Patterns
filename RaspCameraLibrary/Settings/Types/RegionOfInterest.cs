using RaspCameraLibrary.Helpers;

namespace RaspCameraLibrary.Settings.Types;

/// <summary>
/// Region of interest
/// </summary>
public class RegionOfInterest : Stringable
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public RegionOfInterest()
    {
    }

    /// <summary>
    /// Constructor with parameters
    /// </summary>
    public RegionOfInterest
    (
        decimal? X = null,
        decimal? Y = null,
        decimal? Width = null,
        decimal? Height = null
    )
    {
        this.X = X;
        this.Y = Y;
        this.Width = Width;
        this.Height = Height;
    }

    /// <summary>
    /// X start coordinate
    /// </summary>
    public decimal? X { get; set; }

    /// <summary>
    /// Y start coordinate
    /// </summary>
    public decimal? Y { get; set; }

    /// <summary>
    /// Width
    /// </summary>
    public decimal? Width { get; set; }

    /// <summary>
    /// Height
    /// </summary>
    public decimal? Height { get; set; }

    /// <summary>
    /// Convert to string
    /// </summary>
    public override string ToString() => $"{X},{Y},{Width},{Height}";
}