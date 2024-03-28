namespace RaspCameraLibrary.Settings.Types;

/// <summary>
/// Camera connected
/// </summary>
public class Camera
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Camera()
    {
    }

    /// <summary>
    /// Constructor with parameters
    /// </summary>
    public Camera
    (
        uint? Id = null,
        string? Name = null,
        uint? Width = null,
        uint? Height = null,
        string? Path = null
    )
    {
        this.Id = Id;
        this.Name = Name;
        this.Width = Width;
        this.Height = Height;
        this.Path = Path;
    }

    /// <summary>
    /// Camera index
    /// </summary>
    public uint? Id { get; set; }

    /// <summary>
    /// Camera sensor name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Width of the camera sensor
    /// </summary>
    public uint? Width { get; set; }

    /// <summary>
    /// Height of the camera sensor
    /// </summary>
    public uint? Height { get; set; }

    /// <summary>
    /// Path to the camera device
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Camera modes
    /// </summary>
    public List<Mode> Modes { get; set; } = [];
}