using RaspCameraLibrary.Settings.Types;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RaspCameraLibrary;

/// <summary>
/// libcamera-hello Wrapper
/// </summary>
public partial class Hello : RaspCamera
{
    /// <summary>
    /// Binary name
    /// </summary>
    protected override string Executable => "libcamera-hello";

    private const string ListCamerasCommand = "--list-cameras";

    [GeneratedRegex(@"(\d+) : (.+) \[(\d+)x(\d+)(.*)\] \((.+)\)", RegexOptions.IgnoreCase)]
    private static partial Regex CameraRegex();

    [GeneratedRegex(@"'(.+)' : (.+)", RegexOptions.IgnoreCase)]
    private static partial Regex ModeRegex();

    [GeneratedRegex(@"(\d+)x(\d+) \[(.+) fps - \((\d+), (\d+)\)\/(\d+)x(\d+) crop\]", RegexOptions.IgnoreCase)]
    private static partial Regex ResolutionRegex();

    /// <summary>
    /// List cameras
    /// </summary>
    protected async Task<List<Camera>?> List()
    {
        List<Camera> Cameras = [];

        Process? ListCamerasProcess = null;

        try
        {
            ProcessStartInfo startInfo = StartInfo(ListCamerasCommand);

            Console.WriteLine("Starting process with command: {0} {1}", startInfo.FileName, startInfo.Arguments);

            ListCamerasProcess = Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Failed to start process: {0}", ex.Message);
        }

        if (ListCamerasProcess is null)
        {
            return null;
        }

        Camera? camera = null;
        string? name = null;

        while (true)
        {
            string? Line = await ListCamerasProcess.StandardOutput.ReadLineAsync();

            if (Line is null)
            {
                break;
            }

            bool CamaraDefinition = CameraRegex().IsMatch(Line);

            if (CamaraDefinition && camera is not null)
            {
                Cameras.Add(camera);

                camera = null;
            }

            if (CamaraDefinition && camera is null)
            {
                Match cameraMatch = CameraRegex().Match(Line);

                camera = new(
                    Id: uint.Parse(cameraMatch.Groups[1].Value),
                    Name: cameraMatch.Groups[2].Value,
                    Width: uint.Parse(cameraMatch.Groups[3].Value),
                    Height: uint.Parse(cameraMatch.Groups[4].Value),
                    Path: cameraMatch.Groups[6].Value
                );

                continue;
            }

            bool ModeDefinition = ModeRegex().IsMatch(Line);

            if (ModeDefinition && camera is not null && name is not null)
            {
                name = null;
            }

            if (ModeDefinition && camera is not null && name is null)
            {
                Match modeMatch = ModeRegex().Match(Line);

                name = modeMatch.Groups[1].Value;
            }

            bool ResolutionDefinition = ResolutionRegex().IsMatch(Line);

            if (ResolutionDefinition && camera is not null && name is not null)
            {
                Match resolutionMatch = ResolutionRegex().Match(Line);

                camera.Modes.Add(
                    new Mode(
                        Name: name,
                        Width: uint.Parse(resolutionMatch.Groups[1].Value),
                        Height: uint.Parse(resolutionMatch.Groups[2].Value),
                        Fps: double.Parse(resolutionMatch.Groups[3].Value),
                        Crop: new(
                            X: uint.Parse(resolutionMatch.Groups[4].Value),
                            Y: uint.Parse(resolutionMatch.Groups[5].Value),
                            Width: uint.Parse(resolutionMatch.Groups[6].Value),
                            Height: uint.Parse(resolutionMatch.Groups[7].Value)
                        )
                    )
                );
            }
        }

        if (camera is not null)
        {
            Cameras.Add(camera);
        }

        return Cameras;
    }

    private static Hello Instance { get; } = new Hello();

    /// <summary>
    /// List cameras
    /// </summary>
    public static async Task<List<Camera>?> ListCameras() => await Instance.List();
}
