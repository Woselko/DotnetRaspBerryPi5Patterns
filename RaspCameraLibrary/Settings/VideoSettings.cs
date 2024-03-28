using RaspCameraLibrary.Helpers;
using RaspCameraLibrary.Settings.Codecs;
using RaspCameraLibrary.Settings.Enumerations;
using RaspCameraLibrary.Settings.Types;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings;

/// <summary>
/// Parameters for libcamera-vid
/// </summary>
[JsonConverter(typeof(VideoSettingsConverter<VideoSettings>))]
public class VideoSettings : Arguments
{
    /// <summary>
    /// Default settings
    /// </summary>
    public static VideoSettings Default(
        uint Camera = 0,
        uint Width = 1280,
        uint Height = 720,
        uint Timeout = 0,
        bool HFlip = true,
        bool VFlip = true,
        uint Framerate = 30,
        string? Output = null
    ) => new()
    {
        Camera = Camera,
        Width = Width,
        Height = Height,
        Timeout = Timeout,
        HFlip = HFlip,
        VFlip = VFlip,
        Framerate = Framerate,
        Output = Output
    };

    /// <summary>
    /// Default settings for H264 Codec
    /// </summary>
    public static H264 H264(
        uint Camera = 0,
        uint Width = 1280,
        uint Height = 720,
        uint Timeout = 0,
        bool HFlip = true,
        bool VFlip = true,
        uint Bitrate = 1000000,
        uint Framerate = 30,
        string? Output = null
    ) => new()
    {
        Camera = Camera,
        Width = Width,
        Height = Height,
        Timeout = Timeout,
        HFlip = HFlip,
        VFlip = VFlip,
        Bitrate = Bitrate,
        Framerate = Framerate,
        Output = Output
    };

    /// <summary>
    /// Default settings for Mjpeg Codec
    /// </summary>
    public static Mjpeg Mjpeg(
        uint Camera = 0,
        uint Width = 1280,
        uint Height = 720,
        uint Timeout = 0,
        bool HFlip = true,
        bool VFlip = true,
        uint Framerate = 30,
        string? Output = null
    ) => new()
    {
        Camera = Camera,
        Width = Width,
        Height = Height,
        Timeout = Timeout,
        HFlip = HFlip,
        VFlip = VFlip,
        Framerate = Framerate,
        Output = Output
    };

    /// <summary>
    /// Default settings for Yuv420 Codec
    /// </summary>
    public static Yuv420 Yuv420(
        uint Camera = 0,
        uint Width = 1280,
        uint Height = 720,
        uint Timeout = 0,
        bool HFlip = true,
        bool VFlip = true,
        uint Framerate = 30,
        string? Output = null
    ) => new()
    {
        Camera = Camera,
        Width = Width,
        Height = Height,
        Timeout = Timeout,
        HFlip = HFlip,
        VFlip = VFlip,
        Framerate = Framerate,
        Output = Output
    };

    /// <summary>
    /// Chooses the camera to use. To list the available indexes
    /// </summary>
    [Argument("--camera")]
    public uint? Camera { get; set; } = null;

    /// <summary>
    /// Set verbosity level
    /// </summary>
    [Argument("--verbose")]
    public VerboseLevel? Verbose { get; set; } = null;

    /// <summary>
    /// Set the width of the image
    /// </summary>
    [Argument("--width")]
    public uint? Width { get; set; } = null;

    /// <summary>
    /// Set the height of the image
    /// </summary>
    [Argument("--height")]
    public uint? Height { get; set; } = null;

    /// <summary>
    /// Time (in ms) for which program runs
    /// </summary>
    [Argument("--timeout")]
    public uint? Timeout { get; set; } = null;

    /// <summary>
    /// Output filename or '-' for stdout
    /// </summary>
    [Argument("--output")]
    public string? Output { get; set; } = null;

    /// <summary>
    /// Set the file name for configuring the post-processing
    /// </summary>
    [Argument("--post-process-file")]
    public string? PostProcessFile { get; set; } = null;

    /// <summary>
    /// Force use of full resolution raw frames
    /// </summary>
    [Argument("--rawfull")]
    public bool? Rawfull { get; set; } = null;

    /// <summary>
    /// Request a horizontal flip transform
    /// </summary>
    [Argument("--hflip")]
    public bool? HFlip { get; set; } = null;

    /// <summary>
    /// Request a vertical flip transform
    /// </summary>
    [Argument("--vflip")]
    public bool? VFlip { get; set; } = null;

    /// <summary>
    /// Request an image rotation, 0 or 180
    /// </summary>
    [Argument("--rotation")]
    public int? Rotation { get; set; } = null;

    /// <summary>
    /// Set region of interest (digital zoom)
    /// </summary>
    [Argument("--roi")]
    public RegionOfInterest? RegionOfInterest { get; set; } = null;

    /// <summary>
    /// Set a fixed shutter speed in microseconds
    /// </summary>
    [Argument("--shutter")]
    public uint? Shutter { get; set; } = null;

    /// <summary>
    /// Set a fixed gain value
    /// </summary>
    [Argument("--gain")]
    public uint? Gain { get; set; } = null;

    /// <summary>
    /// Set the metering mode
    /// </summary>
    [Argument("--metering")]
    public Metering? Metering { get; set; } = null;

    /// <summary>
    /// Set the exposure mode
    /// </summary>
    [Argument("--exposure")]
    public Exposure? Exposure { get; set; } = null;

    /// <summary>
    /// Set the EV exposure compensation, where 0 = no change
    /// </summary>
    [Argument("--ev")]
    public uint? ExposureCompensation { get; set; } = null;

    /// <summary>
    /// Set the AWB mode
    /// </summary>
    [Argument("--awb")]
    public WhiteBalance? WhiteBalance { get; set; } = null;

    /// <summary>
    /// Set explict red and blue gains
    /// </summary>
    [Argument("--awbgains")]
    public WhiteBalanceGains? WhiteBalanceGains { get; set; } = null;

    /// <summary>
    /// Flush output data as soon as possible
    /// </summary>
    [Argument("--flush")]
    public bool? Flush { get; set; } = null;

    /// <summary>
    /// When writing multiple output files, reset the counter when it reaches this number
    /// </summary>
    [Argument("--wrap")]
    public uint? Wrap { get; set; } = null;

    /// <summary>
    /// Adjust the brightness of the output images, in the range -1.0 to 1.0
    /// </summary>
    [Argument("--brightness")]
    public decimal? Brightness
    {
        get => _brightness;
        set => _brightness = value is null ? null : decimal.Clamp(value.Value, -1, 1);
    }
    private decimal? _brightness = null;

    /// <summary>
    /// Adjust the contrast of the output image, where 1.0 = normal contrast
    /// </summary>
    [Argument("--contrast")]
    public decimal? Contrast
    {
        get => _contrast;
        set => _contrast = value is null ? null : decimal.Clamp(value.Value, 0, 1);
    }
    private decimal? _contrast = null;

    /// <summary>
    /// Adjust the saturation of the output image, where 1.0 = normal saturation
    /// </summary>
    [Argument("--saturation")]
    public decimal? Saturation
    {
        get => _saturation;
        set => _saturation = value is null ? null : decimal.Clamp(value.Value, 0, 1);
    }
    private decimal? _saturation = null;

    /// <summary>
    /// Adjust the sharpness of the output image, where 1.0 = normal sharpness
    /// </summary>
    [Argument("--sharpness")]
    public decimal? Sharpness
    {
        get => _sharpness;
        set => _sharpness = value is null ? null : decimal.Clamp(value.Value, 0, 1);
    }
    private decimal? _sharpness = null;

    /// <summary>
    /// Set the fixed framerate for preview and video modes
    /// </summary>
    [Argument("--framerate")]
    public uint? Framerate { get; set; } = null;

    /// <summary>
    /// Sets the Denoise operating mode
    /// </summary>
    [Argument("--denoise")]
    public Denoise? Denoise { get; set; } = null;

    /// <summary>
    /// Width of viewfinder frames from the camera (distinct from the preview window size
    /// </summary>
    [Argument("--viewfinder-width")]
    public uint? ViewfinderWidth { get; set; } = null;

    /// <summary>
    /// Height of viewfinder frames from the camera (distinct from the preview window size)
    /// </summary>
    [Argument("--viewfinder-height")]
    public uint? ViewfinderHeight { get; set; } = null;

    /// <summary>
    /// Name of camera tuning file to use, omit this option for libcamera default behaviour
    /// </summary>
    [Argument("--tuning-file")]
    public string? TuningFile { get; set; } = null;

    /// <summary>
    /// Width of low resolution frames (use 0 to omit low resolution stream)
    /// </summary>
    [Argument("--lores-width")]
    public uint? LowResolutionWidth { get; set; } = null;

    /// <summary>
    /// Height of low resolution frames (use 0 to omit low resolution stream)
    /// </summary>
    [Argument("--lores-height")]
    public uint? LowResolutionHeight { get; set; } = null;

    /// <summary>
    /// Camera mode as W:H:bit-depth:packing, where packing is P (packed) or U (unpacked)
    /// </summary>
    [Argument("--mode")]
    public Mode? Mode { get; set; } = null;

    /// <summary>
    /// Camera mode for preview as W:H:bit-depth:packing, where packing is P (packed) or U (unpacked)
    /// </summary>
    [Argument("---viewfinder-mode")]
    public Mode? ViewFinderMode { get; set; } = null;

    /// <summary>
    /// Number of in-flight requests (and buffers) configured for video, raw, and still.
    /// </summary>
    [Argument("--buffer-count")]
    public uint? BufferCount { get; set; } = null;

    /// <summary>
    /// Number of in-flight requests (and buffers) configured for preview window.
    /// </summary>
    [Argument("--viewfinder-buffer-count")]
    public uint? ViewFinderBufferCount { get; set; } = null;

    /// <summary>
    /// Control to set the mode of the AF (autofocus) algorithm
    /// </summary>
    [Argument("--autofocus-mode")]
    public AutofocusMode? AutofocusMode { get; set; } = null;

    /// <summary>
    /// Set the range of focus distances that is scanned
    /// </summary>
    [Argument("--autofocus-range")]
    public AutofocusRange? AutofocusRange { get; set; } = null;

    /// <summary>
    /// Control that determines whether the AF algorithm is to move the lens as quickly as possible or more steadily
    /// </summary>
    [Argument("--autofocus-speed")]
    public AutofocusSpeed? AutofocusSpeed { get; set; } = null;

    /// <summary>
    /// Sets AfMetering to  AfMeteringWindows an set region used, e.g. 0.25,0.25,0.5,0.5
    /// </summary>
    [Argument("--autofocus-window")]
    public RegionOfInterest? AutofocusWindow { get; set; } = null;

    /// <summary>
    /// Set the lens to a particular focus position, expressed as a reciprocal distance (0 moves the lens to infinity)
    /// </summary>
    [Argument("--lens-position")]
    public decimal? LensPosition { get; set; } = null;

    /// <summary>
    /// High Dynamic Range, where supported
    /// </summary>
    [Argument("--hdr")]
    public bool? Hdr { get; set; } = null;

    /// <summary>
    /// Save captured image metadata to a file or "-" for stdout
    /// </summary>
    [Argument("--metadata")]
    public string? Metadata { get; set; } = null;

    /// <summary>
    /// Format to save the metadata in, either txt or json (requires --metadata)
    /// </summary>
    [Argument("--metadata-format")]
    public MetadataFormat? MetadataFormat { get; set; } = null;

    /// <summary>
    /// Save a timestamp file with this name
    /// </summary>
    [Argument("--save-pts")]
    public string? SavePts { get; set; } = null;

    /// <summary>
    /// Listen for an incoming client network connection before sending data to the client
    /// </summary>
    [Argument("--listen")]
    public bool? Listen { get; set; } = null;

    /// <summary>
    /// Pause or resume video recording when ENTER pressed
    /// </summary>
    [Argument("--keypress")]
    public bool? Keypress { get; set; } = null;

    /// <summary>
    /// Pause or resume video recording when signal received
    /// </summary>
    [Argument("--signal")]
    public bool? Signal { get; set; } = null;

    /// <summary>
    /// Use 'pause' to pause the recording at startup, otherwise 'record'
    /// </summary>
    [Argument("--initial")]
    public string? Initial { get; set; } = null;

    /// <summary>
    /// Create a new output file every time recording is paused and then resumed
    /// </summary>
    [Argument("--split")]
    public bool? Split { get; set; } = null;

    /// <summary>
    /// Break the recording into files of approximately this many milliseconds
    /// </summary>
    [Argument("--segment")]
    public uint? Segment { get; set; } = null;

    /// <summary>
    /// Write output to a circular buffer of the given size (in MB) which is saved on exit
    /// </summary>
    [Argument("--circular")]
    public uint? Circular { get; set; } = null;

    /// <summary>
    /// Run for the exact number of frames specified. This will override any timeout set
    /// </summary>
    [Argument("--frames")]
    public uint? Frames { get; set; } = null;

    /// <summary>
    /// Set the video codec to use
    /// </summary>
    [Argument("--codec")]
    public virtual Codec? Codec { get; set; } = null;
}