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

    /// <summary>
    /// New VideoStream instance
    /// </summary>
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
    /// Actual process running libcamera-vid
    /// </summary>
    public static Process CaptureStreamProcess { get; set; }

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
        CaptureStreamProcess = new Process
        {
            StartInfo = processStartInfo,
        };
        CaptureStreamProcess.ErrorDataReceived += ProcessDataReceived;
        CaptureStreamProcess.Start();
        CaptureStreamProcess.BeginErrorReadLine();

        using (var frameOutputStream = CaptureStreamProcess.StandardOutput.BaseStream)
        {
            var buffer = new byte[65536];
            var imageData = new List<byte>();

            while (!cancellationToken.IsCancellationRequested)
            {
                if(CaptureStreamProcess.HasExited)
                {
                    break;
                }
                
                var length = await frameOutputStream.ReadAsync(buffer, 0, buffer.Length);

                if (length == 0)
                {
                    break;
                }

                imageData.AddRange(buffer.Take(length));
                if (imageData.Count > 0)
                {
                    SendExtractedJPEGFrames(imageData.ToArray());
                   //this.NewImageReceived?.Invoke(imageData.ToArray());
                   imageData.Clear();
                }
            }

            frameOutputStream.Close();
        }
        CaptureStreamProcess.ErrorDataReceived -= this.ProcessDataReceived;
        CaptureStreamProcess.WaitForExit(1000);
        if (!CaptureStreamProcess.HasExited)
        {
            CaptureStreamProcess.Kill();
        }
    }

    /// <summary>
    /// Start Frame reader
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="useShellExecute">Use the Operating System shell to start the process</param>
    /// <returns></returns>
    public async Task StartVideoStream(
        ProcessStartInfo processStartInfo,
        CancellationToken cancellationToken = default,
        bool useShellExecute = false)
    {
        var args = processStartInfo.Arguments;
        args += " --libav-format mjpeg";
        processStartInfo.Arguments = args;
        CaptureStreamProcess = new Process
        {
            StartInfo = processStartInfo,
        };
        CaptureStreamProcess.ErrorDataReceived += ProcessDataReceived;
        CaptureStreamProcess.Start();
        CaptureStreamProcess.BeginErrorReadLine();
    }

    public async Task StopVideoStream()
    {
        if (CaptureStreamProcess != null && !CaptureStreamProcess.HasExited)
        {
            CaptureStreamProcess.Kill();
        }
    }

    private void ProcessDataReceived(object sender, DataReceivedEventArgs e)
    {
        Console.WriteLine(e.Data);
    }

    public void SendExtractedJPEGFrames(byte[] videoBytes)
    {
        int frameStartIndex = -1;
        for (int i = 0; i < videoBytes.Length - 1; i++)
        {
            if (videoBytes[i] == 0xFF && videoBytes[i + 1] == 0xD8)
            {
                if (frameStartIndex != -1)
                {
                    //if frame started, add it to list
                    byte[] frame = new byte[i - frameStartIndex];
                    Array.Copy(videoBytes, frameStartIndex, frame, 0, i - frameStartIndex);
                    this.NewImageReceived?.Invoke(frame);
                }
                // start new frame
                frameStartIndex = i;
            }
            else if (videoBytes[i] == 0xFF && videoBytes[i + 1] == 0xD9)
            {
                // end of frame found
                if (frameStartIndex != -1)
                {
                    byte[] frame = new byte[i - frameStartIndex + 2];
                    Array.Copy(videoBytes, frameStartIndex, frame, 0, i - frameStartIndex + 2);
                    this.NewImageReceived?.Invoke(frame);
                    frameStartIndex = -1;
                }
            }
        }
    }

    public List<byte[]> ExtractJPEGFrames(byte[] videoBytes)
    {
        List<byte[]> frames = new List<byte[]>();
        int frameStartIndex = -1;
        for (int i = 0; i < videoBytes.Length - 1; i++)
        {
            if (videoBytes[i] == 0xFF && videoBytes[i + 1] == 0xD8)
            {
                if (frameStartIndex != -1)
                {
                    //if frame started, add it to list
                    byte[] frame = new byte[i - frameStartIndex];
                    Array.Copy(videoBytes, frameStartIndex, frame, 0, i - frameStartIndex);
                    frames.Add(frame);
                }
                // start new frame
                frameStartIndex = i;
            }
            else if (videoBytes[i] == 0xFF && videoBytes[i + 1] == 0xD9)
            {
                // end of frame found
                if (frameStartIndex != -1)
                {
                    byte[] frame = new byte[i - frameStartIndex + 2];
                    Array.Copy(videoBytes, frameStartIndex, frame, 0, i - frameStartIndex + 2);
                    frames.Add(frame);
                    frameStartIndex = -1;
                }
            }
        }
        return frames;
    }

}
