using YoloDotNet;
using YoloDotNet.Enums;
using YoloDotNet.Models;
using YoloDotNet.Extensions;
using SkiaSharp;

public class YoloDotNetNuget
{
    string _modelPath;
    public YoloDotNetNuget(string modelPath)
    {
        _modelPath = modelPath;
    }

    public (int, int) DetectObjectsInVideoCommonTest(string videoPath, string outputPath)
    {
        var results = DetectObjectsInVideo(videoPath, outputPath);

        int personCount = 0;
        int firstFrameWithPerson = -1;

        foreach (var frame in results)
        {
            int frameNumber = frame.Key;
            var detections = frame.Value;

            foreach (var detection in detections)
            {
                if (detection.Label.Name == "person")
                {
                    personCount++;

                    // Ustawiamy numer pierwszej klatki z wykryciem "person" tylko raz
                    if (firstFrameWithPerson == -1)
                    {
                        firstFrameWithPerson = frameNumber;
                    }
                }
            }
        }
        return (firstFrameWithPerson, personCount);
    }
    public Dictionary<int, List<ObjectDetection>> DetectObjectsInVideo(string videoPath, string outputPath)
    {
        // Instantiate a new Yolo object
        using var yolo = new Yolo(new YoloOptions
        {
            OnnxModel = _modelPath,      // Your Yolov8 or Yolov10 model in onnx format
            ModelType = ModelType.ObjectDetection,  // Model type
            Cuda = false,                           // Use CPU or CUDA for GPU accelerated inference. Default = true
            GpuId = 0,                               // Select Gpu by id. Default = 0
            PrimeGpu = false,                       // Pre-allocate GPU before first. Default = false
        });


        var options = new VideoOptions
        {
            VideoFile = videoPath,
            OutputDir = outputPath,
            GenerateVideo = true,
            DrawLabels = true,
            //FPS = 15,
            //Width = 640,  // Resize video...
            Height = -2,  // -2 automatically calculate dimensions to keep proportions
            Quality = 28,
            DrawConfidence = true,
            KeepAudio = false,
            KeepFrames = false,

            DrawSegment = DrawSegment.Default,
            //PoseOptions = MyPoseMarkerConfiguration // Your own pose marker configuration...
        };

        // Run inference on video
        var results = yolo.RunObjectDetection(options, 0.7);
        return results;

        // Load image
        //using var image = SKImage.FromEncodedData(@"BrooklynWillowSt.jpeg");

        //// Run inference and get the results
        //var results = yolo.RunObjectDetection(image, confidence: 0.7, iou: 0.7);
        //return results;
        //// Draw results
        //using var resultsImage = image.Draw(results);

        //// Save to file
        //resultsImage.Save(@"detected.jpg", SKEncodedImageFormat.Jpeg, 80);
    }
}
