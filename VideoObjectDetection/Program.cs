using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Intrinsics.X86;
using SkiaSharp;
using Emgu.CV.Features2D;
using System.Drawing;
using VideoObjectDetection;
using Emgu.CV.Linemod;
using static System.Formats.Asn1.AsnWriter;
using System.Numerics;
using System.Diagnostics;

internal class Program
{
    static void Main(string[] args)
    {

        //YOLOv3(2018): This version introduced multi-scale predictions, allowing it to detect objects of different sizes more effectively.
        //YOLOv4(2020): YOLOv4 introduced new techniques, like CSPDarknet53, making it faster and better at handling complex tasks.
        //YOLOv5(2020): Although not officially part of the YOLO series, YOLOv5, developed by Ultralytics, became famous for its ease of use and better training features.
        //YOLOv6(2022): Released by Meituan, YOLOv6 made further improvements in speed and efficiency.
        //YOLOv7(2022): This version, created by Chien - Yao Wang, brought faster and more accurate object detection through an improved backbone.
        // Uzyskanie bieżącego katalogu roboczego
        string currentDirectory = Directory.GetCurrentDirectory();

        // Tworzenie obiektu DirectoryInfo
        DirectoryInfo directoryInfo = new DirectoryInfo(currentDirectory);

        // Przechodzenie do katalogu nadrzędnego, aż osiągniemy katalog projektu
        DirectoryInfo projectDirectory = directoryInfo.Parent?.Parent?.Parent;
        string videoPath = Path.Combine(projectDirectory.FullName, "testVideo15fps720p.mp4");

        //HumanDetectionInVideoMobileNetSSD v2 = new HumanDetectionInVideoMobileNetSSD(projectDirectory + @"\mobileSsdNet\fullfacedetection.caffemodel", projectDirectory + @"\mobileSsdNet\fullface_deploy.prototxt");
        //v2.DetectInWebCamera();

        //HumanDetectionInVideoYOLO yolo = new HumanDetectionInVideoYOLO(projectDirectory + @"\yolov8_10Onnx\csdarknet53-omega.cfg", projectDirectory + @"\yolov8_10Onnx\csdarknet53-omega_final.weights", projectDirectory + @"\yolov3\coco.names");
        //yolo.DetectInWebCamera();

        //HumanDetectionInVideoYOLO yolo = new HumanDetectionInVideoYOLO(projectDirectory + @"\yolov3\yolov3.cfg", projectDirectory + @"\yolov3\yolov3.weights", projectDirectory + @"\yolov3\coco.names");
        //yolo.DetectObjectsInVideo(videoPath, "YOLOdetected.mp4");
        //yolo.DetectInWebCamera();

        //HumanDetectionInVideoYOLO yolo7 = new HumanDetectionInVideoYOLO(projectDirectory + @"\yolov7\yolov7.cfg", projectDirectory + @"\yolov7\yolov7.weights", projectDirectory + @"\yolov7\coco.names");
        //yolo7.DetectObjectsInVideoCommonTest(videoPath, "YOLOV7detected.mp4");
        //yolo7.DetectInWebCamera();

        //HumanDetectionInVideoYOLO yoloTinyV2 = new HumanDetectionInVideoYOLO(projectDirectory + @"\yolov2tiny\yolov2-tiny.cfg", projectDirectory + @"\yolov2tiny\yolov2-tiny.weights", projectDirectory + @"\yolov2tiny\coco.names");
        //yoloTinyV2.DetectObjectsInVideo(videoPath, "YOLOTINYV2detected.mp4");
        //yoloTinyV2.DetectInWebCamera();

        //HumanDetectionInVideoYOLO yoloTinyV3 = new HumanDetectionInVideoYOLO(projectDirectory + @"\yolov3tiny\yolov3-tiny.cfg", projectDirectory + @"\yolov3tiny\yolov3-tiny.weights", projectDirectory + @"\yolov3tiny\coco.names");
        //yoloTinyV3.DetectObjectsInVideo(videoPath, "YOLOTINYV3detected.mp4");
        //yoloTinyV3.DetectInWebCamera();

        //HumanDetectionInVideoYOLO yolov4 = new HumanDetectionInVideoYOLO(projectDirectory + @"\yolov4\yolov4.cfg", projectDirectory + @"\yolov4\yolov4.weights", projectDirectory + @"\yolov4\coco.names");
        //yolov4.DetectObjectsInVideo(videoPath, "YOLOTINYV4detected.mp4");
        //yolov4.DetectInWebCamera();

        //HumanDetectionInVideoYOLO yoloTinyV4 = new HumanDetectionInVideoYOLO(projectDirectory + @"\yolov4tiny\yolov4-tiny.cfg", projectDirectory + @"\yolov4tiny\yolov4-tiny.weights", projectDirectory + @"\yolov4tiny\coco.names");
        //yoloTinyV4.DetectObjectsInVideo(videoPath, "YOLOTINYV4detected.mp4");
        //yoloTinyV4.DetectInWebCamera();

        HumanDetectionInVideoYOLO yoloTinyV7 = new HumanDetectionInVideoYOLO(projectDirectory + @"\yolov7tiny\yolov7-tiny.cfg", projectDirectory + @"\yolov7tiny\yolov7-tiny.weights", projectDirectory + @"\yolov7tiny\coco.names");
        yoloTinyV7.DetectObjectsInVideo(videoPath, "YOLOTINYV7detected.mp4");
        //yoloTinyV7.DetectInWebCamera();

        //HumanDetectionInVideoMobileNetSSD ssd = new HumanDetectionInVideoMobileNetSSD(projectDirectory + @"\mobileSsdNet\MobileNetSSD_deploy.caffemodel", projectDirectory + @"\mobileSsdNet\MobileNetSSD_deploy.prototxt.txt");
        //ssd.DetectObjectsInVideo(videoPath, "MOBILESSDNETdetected.mp4");
        //ssd.DetectInWebCamera();

        //HumanDetectionInVideoMobileNetSSDV2 ssdv2caffe = new HumanDetectionInVideoMobileNetSSDV2(projectDirectory + @"\MobileNetV2Caffe\mobilenet_v2.caffemodel", projectDirectory + @"\MobileNetV2Caffe\mobilenet_v2_deploy.prototxt", projectDirectory + @"\MobileNetV2Caffe\labels.txt");
        //ssdv2caffe.DetectObjectsInVideo(videoPath, "MOBILENETV2Caffedetected.mp4");
        //ssdv2caffe.DetectObjectsInVideo_MobileNetCam();

        //var V3TensorFlow = new HumanDetectionInVideoTensorFlow(projectDirectory + @"\MobileNetV3TensorFlow\frozen_inference_graph.pb", projectDirectory + @"\MobileNetV3TensorFlow\ssd_mobilenet_v3_large_coco_2020_01_14.pbtxt", projectDirectory + @"\MobileNetV3TensorFlow\labelmap.txt");
        //V3TensorFlow.DetectObjectsInVideo(videoPath, "MOBILENETV3detected.mp4");
        //V3TensorFlow.DetectObjectsFromWebcam();

        //YOLOv8 yolo8 = new YOLOv8(projectDirectory + @"\yolov8_10Onnx\yolov8n.onnx");
        //yolo8.DetectObjectsFromWebcamColorCorrection();

        //MeasurePerformance("YOLOv3", () =>
        //{
        //    HumanDetectionInVideoYOLO yolo = new HumanDetectionInVideoYOLO(projectDirectory+@"\yolov3\yolov3.cfg", projectDirectory + @"\yolov3\yolov3.weights", projectDirectory + @"\yolov3\coco.names");
        //    return yolo.DetectObjectsInVideoCommonTest(videoPath, "YOLOV3detected.mp4");
        //});

        //MeasurePerformance("YOLOv4", () =>
        //{
        //    HumanDetectionInVideoYOLO yolo = new HumanDetectionInVideoYOLO(projectDirectory + @"\yolov4\yolov4.cfg", projectDirectory + @"\yolov4\yolov4.weights", projectDirectory + @"\yolov4\coco.names");
        //    return yolo.DetectObjectsInVideoCommonTest(videoPath, "YOLOV4detected.mp4");
        //});

        //MeasurePerformance("YOLOv7", () =>
        //{
        //    HumanDetectionInVideoYOLO yolo = new HumanDetectionInVideoYOLO(projectDirectory + @"\yolov7\yolov7.cfg", projectDirectory + @"\yolov7\yolov7.weights", projectDirectory + @"\yolov7\coco.names");
        //    return yolo.DetectObjectsInVideoCommonTest(videoPath, "YOLOV7detected.mp4");
        //});

        //MeasurePerformance("YOLOv2 Tiny", () =>
        //{
        //    HumanDetectionInVideoYOLO yoloTinyV2 = new HumanDetectionInVideoYOLO(projectDirectory + @"\yolov2tiny\yolov2-tiny.cfg", projectDirectory + @"\yolov2tiny\yolov2-tiny.weights", projectDirectory + @"\yolov2tiny\coco.names");
        //    return yoloTinyV2.DetectObjectsInVideoCommonTest(videoPath, "YOLOTINYV2detected.mp4");
        //});

        //MeasurePerformance("YOLOv3 Tiny", () =>
        //{
        //    HumanDetectionInVideoYOLO yoloTinyV3 = new HumanDetectionInVideoYOLO(projectDirectory + @"\yolov3tiny\yolov3-tiny.cfg", projectDirectory + @"\yolov3tiny\yolov3-tiny.weights", projectDirectory + @"\yolov3tiny\coco.names");
        //    return yoloTinyV3.DetectObjectsInVideoCommonTest(videoPath, "YOLOTINYV3detected.mp4");
        //});

        //MeasurePerformance("YOLOv4 Tiny", () =>
        //{
        //    HumanDetectionInVideoYOLO yoloTinyV4 = new HumanDetectionInVideoYOLO(projectDirectory + @"\yolov4tiny\yolov4-tiny.cfg", projectDirectory + @"\yolov4tiny\yolov4-tiny.weights", projectDirectory + @"\yolov4tiny\coco.names");
        //    return yoloTinyV4.DetectObjectsInVideoCommonTest(videoPath, "YOLOTINYV4detected.mp4");
        //});

        //MeasurePerformance("YOLOv7 Tiny", () =>
        //{
        //    HumanDetectionInVideoYOLO yoloTinyV7 = new HumanDetectionInVideoYOLO(projectDirectory + @"\yolov7tiny\yolov7-tiny.cfg", projectDirectory + @"\yolov7tiny\yolov7-tiny.weights", projectDirectory + @"\yolov7tiny\coco.names");
        //    return yoloTinyV7.DetectObjectsInVideoCommonTest(videoPath, "YOLOTINYV7detected.mp4");
        //});

        //MeasurePerformance("YOLOv10nano", () =>
        //{
        //    YoloDotNetNuget yoloDotNetNuget = new YoloDotNetNuget(projectDirectory + @"\yolov8_10Onnx\yolov10n.onnx");
        //    return yoloDotNetNuget.DetectObjectsInVideoCommonTest(videoPath, "YOLOV10NanoDetected.mp4");
        //});

        //MeasurePerformance("YOLOv8nano", () =>
        //{
        //    YOLOv8 yolo8 = new YOLOv8(projectDirectory + @"/yolov8_10Onnx/yolov8n.onnx");
        //    return yolo8.DetectObjectsInVideoCommonTest(videoPath, "YOLOV8NanoDetected.mp4");
        //});

        //MeasurePerformance("MobileNetSSD", () =>
        //{
        //    HumanDetectionInVideoMobileNetSSD ssd = new HumanDetectionInVideoMobileNetSSD(projectDirectory + @"\mobileSsdNet\MobileNetSSD_deploy.caffemodel", projectDirectory + @"\mobileSsdNet\MobileNetSSD_deploy.prototxt.txt");
        //    return ssd.DetectObjectsInVideoCommonTest(videoPath, "MOBILESSDNETdetected.mp4");
        //});

        //MeasurePerformance("MobileNetSSDV2", () =>
        //{
        //    HumanDetectionInVideoGoogleNetCaffe ssdv2 = new HumanDetectionInVideoGoogleNetCaffe(projectDirectory + @"\MobileNetV2Caffe\mobilenet_v2.caffemodel", projectDirectory + @"\MobileNetV2Caffe\mobilenet_v2_deploy.prototxt", projectDirectory + @"\MobileNetV2Caffe\labels.txt");
        //    return ssdv2.DetectObjectsInVideoCommonTest(videoPath, "MOBILENETV2detected.mp4");
        //});

        //MeasurePerformance("MobileNetV3 TensorFlow", () =>
        //{
        //    var V3TensorFlow = new HumanDetectionInVideoTensorFlow(projectDirectory + @"\MobileNetV3TensorFlow\frozen_inference_graph.pb", projectDirectory + @"\MobileNetV3TensorFlow\ssd_mobilenet_v3_large_coco_2020_01_14.pbtxt", projectDirectory + @"\MobileNetV3TensorFlow\labelmap.txt");
        //    return V3TensorFlow.DetectObjectsInVideoCommonTest(videoPath, "MOBILENETV3detected.mp4");
        //});
    }

    static void MeasurePerformance(string methodName, Func<(int, int)> detectionMethod)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        (int, int) outputFramesInfo = detectionMethod();
        stopwatch.Stop();
        Console.WriteLine($"{methodName} execution time: {stopwatch.Elapsed.TotalSeconds} seconds");
        Console.WriteLine($"{methodName} Person detected in frame number: {outputFramesInfo.Item1}, total frames with person: {outputFramesInfo.Item2}");
    }

}
