using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CameraStreamServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;
using System.IO;
using Microsoft.AspNetCore.Http.Features;
using System.Buffers;
using System.Text;
using System.Diagnostics;
using RaspCameraLibrary.Settings;
using RaspCameraLibrary.Settings.Enumerations;
using RaspCameraLibrary.Settings.Codecs;
using RaspCameraLibrary.Settings.Types;
using RaspCameraLibrary;

[ApiController]
[Route("[controller]")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetVideo")]
    public void Get()
    {
        var bufferingFeature =
            HttpContext.Response.HttpContext.Features.Get<IHttpResponseBodyFeature>();
        bufferingFeature?.DisableBuffering();

        HttpContext.Response.StatusCode = 200;
        HttpContext.Response.ContentType = "multipart/x-mixed-replace; boundary=--frame";
        HttpContext.Response.Headers.Add("Connection", "Keep-Alive");
        HttpContext.Response.Headers.Add("CacheControl", "no-cache");

        //List<Camera>? Cameras = RaspCameraLibrary.Video.ListCameras().Result;
        //var mode = Cameras.FirstOrDefault(c => c.Id == 0).Modes.FirstOrDefault();

        var cancellationTokenSource = new CancellationTokenSource();
        // VideoSettings Settings = new H264()
        // {
        //     Camera = 0,
        //     Width = 640,
        //     Height = 480,
        //     Timeout = 0,
        //     HFlip = true,
        //     VFlip = true,
        //     Framerate = 4,
        //     WhiteBalance = WhiteBalance.Incandescent,
        //     //Mode = mode,
        //     //Output = "/home/woselko/CameraStreamServer" //This line is important to pipe the output
        //     //Output = "/home/woselko/test.avi"
        //     Output = "/dev/stdout"
        // };
        VideoSettings Settings = new Mjpeg()
        {
            Camera = 0,
            Width = 800,
            Height = 600,
            Timeout = 0,
            Flush = true,
            HFlip = true,
            VFlip = true,
            Framerate = 4,
            WhiteBalance = WhiteBalance.Incandescent,
            Output = "/dev/stdout"
        };

        ProcessStartInfo captureStartInfo = RaspCameraLibrary.VideoStream.CaptureStartInfo(Settings);
        var client = new VideoStream();
        client.NewImageReceived += NewImageReceived;

        try
        {
            _logger.LogWarning($"Start streaming video");
            var task = client.StartFrameReaderAsync(captureStartInfo, cancellationTokenSource.Token);

            while (!HttpContext.RequestAborted.IsCancellationRequested) { }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception in streaming: {ex}");
        }
        finally
        {
            HttpContext.Response.Body.Close();
            _logger.LogInformation("Stop streaming video");
        }

        client.NewImageReceived -= NewImageReceived;
        
    }

    private byte[] CreateHeader(int length)
    {
        string header =
            $"--frame\r\nContent-Type:image/jpeg\r\nContent-Length:{length}\r\n\r\n";
        return System.Text.Encoding.ASCII.GetBytes(header);
    }

    private byte[] CreateFooter()
    {
        return System.Text.Encoding.ASCII.GetBytes("\r\n");
    }

    private async void NewImageReceived(byte[] imageData)
    {
        try
        {
            await HttpContext.Response.BodyWriter.WriteAsync(CreateHeader(imageData.Length));
            await HttpContext.Response.BodyWriter.WriteAsync(imageData.AsMemory().Slice(0, imageData.Length));
            await HttpContext.Response.BodyWriter.WriteAsync(CreateFooter());
        }
        catch (ObjectDisposedException)
        {
            // ignore this as its thrown when the stream is stopped
        }

        //ArrayPool<byte>.Shared.Return(imageData);
    }
}
