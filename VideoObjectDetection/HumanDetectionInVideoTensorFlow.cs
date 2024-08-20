using System;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV.Util;

public class HumanDetectionInVideoTensorFlow
{
    private readonly Net _net;
    private readonly string[] _classLabels;

    public HumanDetectionInVideoTensorFlow(string modelPath, string config, string labelsPath)
    {
        // Wczytanie modelu TFLite lub TensorFlow
        _net = DnnInvoke.ReadNetFromTensorflow(modelPath, config);
        _net.SetPreferableBackend(Emgu.CV.Dnn.Backend.OpenCV);
        _net.SetPreferableTarget(Target.Cpu);

        // Wczytanie etykiet klas
        _classLabels = File.ReadAllLines(labelsPath);
    }

    public (int, int) DetectObjectsInVideoCommonTest(string inputVideoPath, string outputVideoPath)
    {
        int frameCounter = 0;
        int personDetectedFrame = -1;
        int totalPersonFrames = 0;

        using (var videoCapture = new VideoCapture(inputVideoPath))
        {
            int frameWidth = (int)videoCapture.Get(CapProp.FrameWidth);
            int frameHeight = (int)videoCapture.Get(CapProp.FrameHeight);
            double fps = videoCapture.Get(CapProp.Fps);
            int codec = VideoWriter.Fourcc('m', 'p', '4', 'v');

            using (var videoWriter = new VideoWriter(outputVideoPath, codec, fps, new System.Drawing.Size(frameWidth, frameHeight), true))
            {
                Mat frame = new Mat();
                VectorOfMat output = new VectorOfMat();

                while (videoCapture.Read(frame) && !frame.IsEmpty)
                {
                    frameCounter++;
                    var originalSize = frame.Size;
                    var input = DnnInvoke.BlobFromImage(frame, 1.0 / 255.0, new System.Drawing.Size(320, 320), new MCvScalar(0, 0, 0), true, false);

                    _net.SetInput(input);
                    _net.Forward(output, _net.UnconnectedOutLayersNames);

                    for (int i = 0; i < output.Size; i++)
                    {
                        var mat = output[i];
                        var data = (float[,,,])mat.GetData();

                        for (int detection = 0; detection < data.GetLength(2); detection++)
                        {
                            float confidence = data[0, 0, detection, 2];
                            if (confidence > 0.5f)
                            {
                                int classId = (int)data[0, 0, detection, 1];
                                int x1 = (int)(data[0, 0, detection, 3] * originalSize.Width);
                                int y1 = (int)(data[0, 0, detection, 4] * originalSize.Height);
                                int x2 = (int)(data[0, 0, detection, 5] * originalSize.Width);
                                int y2 = (int)(data[0, 0, detection, 6] * originalSize.Height);

                                var rect = new System.Drawing.Rectangle(x1, y1, x2 - x1, y2 - y1);

                                if (_classLabels[classId] == "person")
                                {
                                    totalPersonFrames++;
                                    if (personDetectedFrame == -1)
                                        personDetectedFrame = frameCounter;
                                }

                                CvInvoke.Rectangle(frame, rect, new MCvScalar(0, 255, 0), 2);
                                string label = _classLabels[classId];
                                CvInvoke.PutText(frame, label, new System.Drawing.Point(x1, y1 - 10),
                                    FontFace.HersheyPlain, 1.0, new MCvScalar(0, 0, 255), 2);
                            }
                        }
                    }

                    videoWriter.Write(frame);
                }
            }
        }

        Console.WriteLine("Video processing completed.");
        return (personDetectedFrame, totalPersonFrames);
    }


    public void DetectObjectsInVideo(string inputVideoPath, string outputVideoPath)
    {
        using (var videoCapture = new VideoCapture(inputVideoPath))
        {
            int frameWidth = (int)videoCapture.Get(CapProp.FrameWidth);
            int frameHeight = (int)videoCapture.Get(CapProp.FrameHeight);
            double fps = videoCapture.Get(CapProp.Fps);
            int codec = VideoWriter.Fourcc('m', 'p', '4', 'v');

            using (var videoWriter = new VideoWriter(outputVideoPath, codec, fps, new System.Drawing.Size(frameWidth, frameHeight), true))
            {
                Mat frame = new Mat();
                VectorOfMat output = new VectorOfMat();

                while (videoCapture.Read(frame) && !frame.IsEmpty)
                {
                    var originalSize = frame.Size;
                    var input = DnnInvoke.BlobFromImage(frame, 1.0 / 255.0, new System.Drawing.Size(320, 320), new MCvScalar(0, 0, 0), true, false);

                    _net.SetInput(input);
                    _net.Forward(output, _net.UnconnectedOutLayersNames);

                    for (int i = 0; i < output.Size; i++)
                    {
                        var mat = output[i];
                        var data = (float[,,,])mat.GetData();

                        for (int detection = 0; detection < data.GetLength(2); detection++)
                        {
                            float confidence = data[0, 0, detection, 2];
                            if (confidence > 0.6f)
                            {
                                int classId = (int)data[0, 0, detection, 1];
                                int x1 = (int)(data[0, 0, detection, 3] * originalSize.Width);
                                int y1 = (int)(data[0, 0, detection, 4] * originalSize.Height);
                                int x2 = (int)(data[0, 0, detection, 5] * originalSize.Width);
                                int y2 = (int)(data[0, 0, detection, 6] * originalSize.Height);

                                var rect = new System.Drawing.Rectangle(x1, y1, x2 - x1, y2 - y1);

                                CvInvoke.Rectangle(frame, rect, new MCvScalar(0, 255, 0), 2);
                                string label = _classLabels[classId];
                                CvInvoke.PutText(frame, label, new System.Drawing.Point(x1, y1 - 10),
                                    FontFace.HersheyPlain, 1.0, new MCvScalar(0, 0, 255), 2);
                            }
                        }
                    }

                    videoWriter.Write(frame);
                }
            }
        }

        Console.WriteLine("Video processing completed.");
    }



    public void DetectObjectsFromWebcam()
    {
        using (var videoCapture = new VideoCapture(0, VideoCapture.API.DShow))
        {
            Mat frame = new Mat();
            VectorOfMat output = new VectorOfMat();

            while (true)
            {
                if (!videoCapture.Read(frame) || frame.IsEmpty)
                {
                    Console.WriteLine("Unable to capture video from webcam or video is empty.");
                    break;
                }

                var originalSize = frame.Size;
                var input = DnnInvoke.BlobFromImage(frame, 1.0 / 255.0, new System.Drawing.Size(320, 320), new MCvScalar(0, 0, 0), true, false);

                _net.SetInput(input);
                _net.Forward(output, _net.UnconnectedOutLayersNames);

                for (int i = 0; i < output.Size; i++)
                {
                    var mat = output[i];
                    var data = (float[,,,])mat.GetData();

                    for (int detection = 0; detection < data.GetLength(2); detection++)
                    {
                        float confidence = data[0, 0, detection, 2];
                        if (confidence > 0.6f)
                        {
                            int classId = (int)data[0, 0, detection, 1];
                            int x1 = (int)(data[0, 0, detection, 3] * originalSize.Width);
                            int y1 = (int)(data[0, 0, detection, 4] * originalSize.Height);
                            int x2 = (int)(data[0, 0, detection, 5] * originalSize.Width);
                            int y2 = (int)(data[0, 0, detection, 6] * originalSize.Height);

                            var rect = new System.Drawing.Rectangle(x1, y1, x2 - x1, y2 - y1);

                            CvInvoke.Rectangle(frame, rect, new MCvScalar(0, 255, 0), 2);
                            string label = _classLabels[classId];
                            CvInvoke.PutText(frame, label, new System.Drawing.Point(x1, y1 - 10),
                                FontFace.HersheyPlain, 1.0, new MCvScalar(0, 0, 255), 2);
                        }
                    }
                }

                CvInvoke.Imshow("Live Object Detection", frame);

                if (CvInvoke.WaitKey(1) == 'q')
                {
                    break;
                }
            }
        }

        CvInvoke.DestroyAllWindows();
        Console.WriteLine("Live detection completed.");
    }


}
