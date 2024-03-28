using RaspCameraLibrary.Helpers;

namespace RaspCameraLibrary.Settings.Types;

/// <summary>
/// White balance gains
/// </summary>
public class WhiteBalanceGains : Stringable
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public WhiteBalanceGains()
    {
    }

    /// <summary>
    /// Constructor with parameters
    /// </summary>
    public WhiteBalanceGains
    (
        uint? Red = null,
        uint? Blue = null
    )
    {
        this.Red = Red;
        this.Blue = Blue;
    }

    /// <summary>
    /// Red gain
    /// </summary>
    public uint? Red { get; set; }

    /// <summary>
    /// Blue gain
    /// </summary>
    public uint? Blue { get; set; }

    /// <summary>
    /// Convert to string
    /// </summary>
    public override string ToString() => $"{Red},{Blue}";
}