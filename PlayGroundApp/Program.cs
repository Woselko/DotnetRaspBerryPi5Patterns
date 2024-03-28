using Iot.Device.Camera;
using Iot.Device.Camera.Settings;
using Iot.Device.Common;
using Iot.Device.Display;
using Iot.Device.Media;

//listing cameras
// var processSettings = ProcessSettingsFactory.CreateForLibcamerastillAndStderr();
// using var proc = new ProcessRunner(processSettings);
// var text = await proc.ExecuteReadOutputAsStringAsync(string.Empty);
// IEnumerable<CameraInfo> cameras = await CameraInfo.From(text);

// var timeout = CommandOptionAndValue.Create(Command.Timeout, "10000");
// var builder = new CommandOptionsBuilder(false)
//             .WithContinuousStreaming()
//             .With(timeout);
            
// var arguments = builder.GetArguments();

// ProcessSettings settings = new()
// {
//     Filename = "libcamera-vid",
//     WorkingDirectory = null,
// };

// using var proc = new ProcessRunner(settings);

// var text = "";
// while (string.IsNullOrEmpty(text))
// {
//     text = await proc.ExecuteReadOutputAsStringAsync(arguments);
// }

// Console.WriteLine(text);

var processSettings = ProcessSettingsFactory.CreateForLibcamerastill();

var builder = new CommandOptionsBuilder()
    .WithVflip()
    .WithHflip()
    .WithPictureOptions(90, "jpg")
    .WithResolution(640, 480);
var argss = builder.GetArguments();

using var proc1 = new ProcessRunner(processSettings);

var filename = CreateFilename("jpg");
using var file = File.OpenWrite(filename);
await proc1.ExecuteAsync(args, file);

System.Console.WriteLine($"File saved as {filename}");

string CreateFilename(string extension)
{
    var now = DateTime.Now;
    return $"{now.Year:00}{now.Month:00}{now.Day:00}_{now.Hour:00}{now.Minute:00}{now.Second:00}{now.Millisecond:0000}.{extension}";
}