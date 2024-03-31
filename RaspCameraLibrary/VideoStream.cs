using RaspCameraLibrary.Settings;
using RaspCameraLibrary.Settings.Types;
using System.Diagnostics;

namespace RaspCameraLibrary;

/// <summary>
/// libcamera-vid Wrapper
/// </summary>
public class VideoStream : Hello
{
    /// <summary>
    /// Binary name
    /// </summary>
    protected override string Executable => "libcamera-vid";


    /// <summary>
    /// New frame received event
    /// </summary>
    public event Action<byte[]> NewImageReceived;

    /// <summary>
    /// New Video info received event
    /// </summary>
    public event Action<string> VideoInfoReceived;

    private static VideoStream Instance { get; } = new VideoStream();

    /// <summary>
    /// Generate a ProcessStartInfo for libcamera-vid
    /// </summary>
    public static ProcessStartInfo CaptureStartInfo(VideoSettings settings) => Instance.StartInfo(settings);

    /// <summary>
    /// List cameras
    /// </summary>
    public static new async Task<List<Camera>?> ListCameras() => await Instance.List();

    /// <summary>
    /// Start Frame reader
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="useShellExecute">Use the Operating System shell to start the process</param>
    /// <returns></returns>
    public async Task StartFrameReaderAsync(
        ProcessStartInfo processStartInfo,
        CancellationToken cancellationToken = default,
        bool useShellExecute = false)
    {
        var args = processStartInfo.Arguments;
        args += " --libav-format mjpeg";
        processStartInfo.Arguments = args;
        using Process captureStreamProcess = new Process
        {
            StartInfo = processStartInfo,
        };
        //captureStreamProcess.PriorityBoostEnabled = true; //low performance
        captureStreamProcess.ErrorDataReceived += ProcessDataReceived;
        captureStreamProcess.Start();
        captureStreamProcess.BeginErrorReadLine();

        using (var frameOutputStream = captureStreamProcess.StandardOutput.BaseStream)
        {
            //var index = 0;
            //var buffer = new byte[16384];
            //var buffer = new byte[32768];
            //var buffer = new byte[1024];
            var buffer = new byte[65536];
            //var buffer = new byte[131072];
            var imageData = new List<byte>();
            //byte[] imageHeader = null;

            while (!cancellationToken.IsCancellationRequested)
            {
                if(captureStreamProcess.HasExited)
                {
                    break;
                }
                
                //var length = await frameOutputStream.ReadAsync(buffer);
                var length = await frameOutputStream.ReadAsync(buffer, 0, buffer.Length);

                if (length == 0)
                {
                    break;
                }
                //Set Image Header with first data
                // if (imageHeader == null)
                // {
                //     imageHeader = buffer.Take(5).ToArray();
                // }

                // if (buffer.Take(5).SequenceEqual(imageHeader))
                // {
                imageData.AddRange(buffer.Take(length));
                if (imageData.Count > 0)
                {
                   //this.NewImageReceived?.Invoke(buffer);
                   this.NewImageReceived?.Invoke(imageData.ToArray());
                   imageData.Clear();
                   //index++;
                }
                // }

                //imageData.AddRange(buffer.Take(length));
            }

            frameOutputStream.Close();
        }
        captureStreamProcess.ErrorDataReceived -= this.ProcessDataReceived;
        captureStreamProcess.WaitForExit(1000);
        if (!captureStreamProcess.HasExited)
        {
            captureStreamProcess.Kill();
        }
    }

    private void ProcessDataReceived(object sender, DataReceivedEventArgs e)
    {
        //this.VideoInfoReceived?.Invoke(e.Data);
        Console.WriteLine(e.Data);
    }

}
