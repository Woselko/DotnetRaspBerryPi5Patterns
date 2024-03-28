namespace RaspCameraLibrary.Settings.Types;

/// <summary>
/// Position
/// </summary>
public class Position
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Position()
    {
    }

    /// <summary>
    /// Constructor with parameters
    /// </summary>
    public Position
    (
        uint? X = null,
        uint? Y = null
    )
    {
        this.X = X;
        this.Y = Y;
    }

    /// <summary>
    /// X coordinate
    /// </summary>
    public uint? X { get; set; }

    /// <summary>
    /// Y coordinate
    /// </summary>
    public uint? Y { get; set; }
}