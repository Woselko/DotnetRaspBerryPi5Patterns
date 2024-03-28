// using System;
// using System.IO;
// using System.Threading.Tasks;
// using Unosquare.RaspberryIO;
// using Unosquare.RaspberryIO.Camera;
// using Unosquare.RaspberryIO.Computer;
// using Unosquare.WiringPi;
// using static System.Net.Mime.MediaTypeNames;

// class Program
// {
//     static async Task Main()
//     {
//         //Pi.Init<BootstrapWiringPi>();

//         var fileName = "photo.jpg";

//         using (var camera = new CameraController())
//         {

//             // Capture a new photo from the camera
//             using (var imageStream = new FileStream(fileName, FileMode.Create))
//             {
//                 var bytes = await camera.CaptureImageJpegAsync(800,600);
//                 imageStream.Write(bytes, 0, bytes.Length);
//             }
//         }

//         Console.WriteLine($"Photo saved as '{fileName}'");
//     }
// }




// using System.Diagnostics;
// using System.Drawing;
// using System.Drawing.Imaging;
// using Iot.Device.Media;
// using MMALSharp;
// using MMALSharp.Common;
// using MMALSharp.Handlers;



// VideoConnectionSettings settings = new VideoConnectionSettings(busId: 0, captureSize: (640, 480), VideoPixelFormat.NV12);
// VideoConnectionSettings settings = new VideoConnectionSettings(busId: 0, captureSize: (640, 480));
// settings.PixelFormat = VideoPixelFormat.JPEG;

// using VideoDevice device = VideoDevice.Create(settings);
// var formats = device.GetSupportedPixelFormats();
// if(formats != null && formats.Any()){
//     foreach(var format in formats){
//         var resolutions = device.GetPixelFormatResolutions(format);
//     }
// }
// device.Capture("file.jpeg");
// using var fs = new FileStream("file", FileMode.Open);
// Bitmap bmp = new Bitmap(fs);
// bmp.Save("fileJPEG.jpeg", ImageFormat.Jpeg);




// VideoConnectionSettings settings = 
// new VideoConnectionSettings(busId: 0, captureSize: (640, 480), VideoPixelFormat.NV12);

// using VideoDevice device = VideoDevice.Create(settings);

// MemoryStream ms = new MemoryStream(device.Capture());
// Color[] colors = VideoDevice.Nv12ToRgb(ms,settings.CaptureSize);
// Iot.Device.Graphics.BitmapImage bitmap = VideoDevice.RgbToBitmap(settings.CaptureSize, colors);
// bitmap.SaveToFile("Camera.jpeg", Iot.Device.Graphics.ImageFileType.Jpg);
//bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);



using System.Diagnostics;
using RaspCameraLibrary.Settings;
using RaspCameraLibrary.Settings.Enumerations;
using RaspCameraLibrary.Settings.Codecs;
using RaspCameraLibrary.Settings.Types;
using Newtonsoft.Json;
using RaspCameraLibrary;


List<Camera>? Cameras = await RaspCameraLibrary.Video.ListCameras();

if (Cameras is null)
{
    Console.Error.WriteLine("Failed to list cameras");

    return 1;
}

Console.WriteLine("Found {0} camera(s)", Cameras.Count);

if (Cameras.Count == 0)
{
    return 1;
}

Console.WriteLine(JsonConvert.SerializeObject(Cameras, Formatting.Indented));

Console.WriteLine("Select camera:");

Camera? Camera = null;

while (true)
{
    string? Input = "0";//Console.ReadLine();

    if (Input != null && int.TryParse(Input, out int Id) && Id >= 0 && Cameras.FirstOrDefault(c => c.Id == Id) is Camera SelectedCamera)
    {
        Camera = SelectedCamera;

        break;
    }

    Console.WriteLine("Please enter a valid camera ID");
}

if (Camera is null)
{
    Console.Error.WriteLine("Failed to select camera");

    return 1;
}

VideoSettings Settings = new H264()
{
    Camera = 0,
    Width = 1280,
    Height = 720,
    Timeout = 0,
    HFlip = true,
    VFlip = true,
    WhiteBalance = WhiteBalance.Incandescent,
    Output = "test.avi"
};

ProcessStartInfo CaptureStartInfo = RaspCameraLibrary.Video.CaptureStartInfo(Settings);

Process? CaptureProcess = null;

try
{
    CaptureProcess = Process.Start(CaptureStartInfo);
}
catch (Exception ex)
{
    Console.Error.WriteLine("Failed to start process: {0}", ex.Message);
}

if (CaptureProcess is null)
{
    return 1;
}

Console.WriteLine("Started process with ID {0}", CaptureProcess.Id);

Console.WriteLine("Press any key to stop recording");

Console.ReadLine();

if (CaptureProcess.HasExited)
{
    Console.Error.WriteLine("Process has already exited");

    return 1;
}

try
{
    CaptureProcess.Kill();
}
catch (Exception ex)
{
    Console.Error.WriteLine("Failed to kill process: {0}", ex.Message);

    return 1;
}

Console.WriteLine("Stopped process with ID {0}", CaptureProcess.Id);

return 0;








// // Singleton initialized lazily. Reference once in your application.
// MMALCamera cam = MMALCamera.Instance;
// using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "jpg"))        
// {            
//     await cam.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
// }

// // Cleanup disposes all unmanaged resources and unloads Broadcom library. To be called when no more processing is to be done
// // on the camera.
// cam.Cleanup();