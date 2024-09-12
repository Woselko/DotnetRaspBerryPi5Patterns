using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

public class HumanDetectionInVideoYOLO
{
    private readonly string _yoloConfigPath;
    private readonly string _yoloWeightsPath;
    private readonly string _cocoNamesPath;

    public HumanDetectionInVideoYOLO(string yoloConfigPath, string yoloWeightsPath, string cocoNamesPath)
    {
        _yoloConfigPath = yoloConfigPath;
        _yoloWeightsPath = yoloWeightsPath;
        _cocoNamesPath = cocoNamesPath;
    }

    public (int, int) DetectObjectsInVideoCommonTest(string videoPath, string outputPath)
    {
        var net = DnnInvoke.ReadNetFromDarknet(_yoloConfigPath, _yoloWeightsPath);
        net.SetPreferableBackend(Emgu.CV.Dnn.Backend.OpenCV);
        net.SetPreferableTarget(Emgu.CV.Dnn.Target.Cpu);

        var classLabels = File.ReadAllLines(_cocoNamesPath);
        int frameCounter = 0;
        int personDetectedFrame = -1;
        int totalPersonFrames = 0;

        using var videoCapture = new VideoCapture(videoPath);
        using var videoWriter = new VideoWriter(outputPath, VideoWriter.Fourcc('m', 'p', '4', 'v'), 30, new Size(videoCapture.Width, videoCapture.Height), true);

        Mat frame = new Mat();
        while (videoCapture.Read(frame) && !frame.IsEmpty)
        {
            frameCounter++;

            var inputBlob = DnnInvoke.BlobFromImage(frame, 1 / 255.0, new Size(416, 416), new MCvScalar(), true, false);
            net.SetInput(inputBlob);
            VectorOfMat outputBlobs = new VectorOfMat();
            net.Forward(outputBlobs, net.UnconnectedOutLayersNames);

            for (int k = 0; k < outputBlobs.Size; k++)
            {
                using Mat output = outputBlobs[k];
                float[] data = new float[output.Total.ToInt32()];
                output.CopyTo(data);

                for (int i = 0; i < output.Rows; i++)
                {
                    int index = i * output.Cols;
                    float confidence = data[index + 4];

                    if (confidence > 0.7f)
                    {
                        float[] scores = new float[classLabels.Length];
                        for (int j = 5; j < output.Cols; j++)
                        {
                            scores[j - 5] = data[index + j];
                        }

                        int classId = Array.IndexOf(scores, scores.Max());
                        if (scores[classId] > 0.7f)
                        {
                            if (classLabels[classId] == "person")
                            {
                                totalPersonFrames++;
                                if (personDetectedFrame == -1)
                                    personDetectedFrame = frameCounter;
                            }

                            int centerX = (int)(data[index + 0] * frame.Width);
                            int centerY = (int)(data[index + 1] * frame.Height);
                            int width = (int)(data[index + 2] * frame.Width);
                            int height = (int)(data[index + 3] * frame.Height);
                            int x = centerX - width / 2;
                            int y = centerY - height / 2;

                            CvInvoke.Rectangle(frame, new Rectangle(x, y, width, height), new MCvScalar(0, 255, 0), 2);
                            CvInvoke.PutText(frame, classLabels[classId], new Point(x, y - 10), Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.9, new MCvScalar(0, 255, 0), 2);
                        }
                    }
                }
            }

            videoWriter.Write(frame);
        }

        Console.WriteLine("Detection completed. Output saved to " + outputPath);
        return (personDetectedFrame, totalPersonFrames);
    }


    public void DetectObjectsInVideo(string videoPath, string outputPath)
    {

        Emgu.CV.Backend[] backends = CvInvoke.WriterBackends;
        int backend_idx = 0; //any backend;
        foreach (Emgu.CV.Backend be in backends)
        {
            if (be.Name.Equals("MSMF"))
            {
                backend_idx = be.ID;
                break;
            }
        }
        var net = DnnInvoke.ReadNetFromDarknet(_yoloConfigPath, _yoloWeightsPath);
        net.SetPreferableBackend(Emgu.CV.Dnn.Backend.OpenCV);
        net.SetPreferableTarget(Emgu.CV.Dnn.Target.Cpu);

        var classLabels = File.ReadAllLines(_cocoNamesPath);
        int fourcc = VideoWriter.Fourcc('H', '2', '6', '4');
        using var videoCapture = new VideoCapture(videoPath);
        using var writer = new VideoWriter(outputPath, fourcc/*VideoWriter.Fourcc('m', 'p', '4', 'v')*/, 30, new Size(videoCapture.Width, videoCapture.Height), true);

        Mat frame = new Mat();
        while (videoCapture.Read(frame))
        {
            var inputBlob = DnnInvoke.BlobFromImage(frame, 1 / 255.0, new Size(416, 416), new MCvScalar(), true, false);
            net.SetInput(inputBlob);
            VectorOfMat outputBlobs = new VectorOfMat();
            net.Forward(outputBlobs, net.UnconnectedOutLayersNames);

            for (int k = 0; k < outputBlobs.Size; k++)
            {
                using Mat output = outputBlobs[k];
                float[] data = new float[output.Total.ToInt32()];
                output.CopyTo(data);

                for (int i = 0; i < output.Rows; i++)
                {
                    int index = i * output.Cols;
                    float confidence = data[index + 4]; // Confidence znajduje się na 5-tym miejscu

                    if (confidence > 0.7)
                    {
                        float[] scores = new float[classLabels.Length];
                        for (int j = 5; j < output.Cols; j++)
                        {
                            scores[j - 5] = data[index + j];
                        }

                        int classId = Array.IndexOf(scores, scores.Max());
                        if (scores[classId] > 0.7) // Upewnij się, że największa pewność jest również powyżej progu
                        {
                            int centerX = (int)(data[index + 0] * frame.Width);
                            int centerY = (int)(data[index + 1] * frame.Height);
                            int width = (int)(data[index + 2] * frame.Width);
                            int height = (int)(data[index + 3] * frame.Height);
                            int x = centerX - width / 2;
                            int y = centerY - height / 2;

                            CvInvoke.Rectangle(frame, new Rectangle(x, y, width, height), new MCvScalar(0, 255, 0), 2);
                            CvInvoke.PutText(frame, classLabels[classId], new Point(x, y - 10), Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.9, new MCvScalar(0, 255, 0), 2);
                            CvInvoke.Imshow("output", frame);
                            if (CvInvoke.WaitKey(1) == 27)
                                break;
                        }
                    }
                }
            }

            writer.Write(frame);
        }

        Console.WriteLine("Detection completed. Output saved to " + outputPath);
    }

    public void DetectInWebCamera()
    {
        //string videoPath = "test1.avi"; // for video file
        //VideoCapture vc = new VideoCapture(videoPath);
        //yolov3.weights need to be downloaded from https://pjreddie.com/darknet/yolo/ its >200mb
        var net = DnnInvoke.ReadNetFromDarknet(_yoloConfigPath, _yoloWeightsPath);
        net.SetPreferableBackend(Emgu.CV.Dnn.Backend.OpenCV);
        net.SetPreferableTarget(Emgu.CV.Dnn.Target.CpuFp16);
        var classLabels = File.ReadAllLines(_cocoNamesPath);
        var vc = new VideoCapture(0, VideoCapture.API.DShow); // for webcam

        Mat frame = new();
        VectorOfMat output = new();
        VectorOfRect boxes = new();
        VectorOfFloat scores = new();
        VectorOfInt indices = new();
        while (true)
        {
            vc.Read(frame);
            CvInvoke.Resize(frame, frame, new System.Drawing.Size(0, 0), .4, .4);
            boxes = new();
            indices = new();
            scores = new();
            var input = DnnInvoke.BlobFromImage(frame, 1 / 255.0, swapRB: true);
            net.SetInput(input);
            net.Forward(output, net.UnconnectedOutLayersNames);
            for (int i = 0; i < output.Size; i++)
            {
                var mat = output[i];
                var data = (float[,])mat.GetData();
                for (int j = 0; j < data.GetLength(0); j++)
                {
                    float[] row = Enumerable.Range(0, data.GetLength(1))
                                  .Select(x => data[j, x])
                                  .ToArray();
                    var rowScore = row.Skip(5).ToArray();
                    var classId = rowScore.ToList().IndexOf(rowScore.Max());
                    var confidence = rowScore[classId];
                    if (confidence > 0.7f)
                    {
                        var centerX = (int)(row[0] * frame.Width);
                        var centerY = (int)(row[1] * frame.Height);
                        var boxWidth = (int)(row[2] * frame.Width);
                        var boxHeight = (int)(row[3] * frame.Height);
                        var x = (int)(centerX - (boxWidth / 2));
                        var y = (int)(centerY - (boxHeight / 2));
                        boxes.Push(new System.Drawing.Rectangle[] { new System.Drawing.Rectangle(x, y, boxWidth, boxHeight) });
                        indices.Push(new int[] { classId });
                        scores.Push(new float[] { confidence });
                    }
                }
            }
            var bestIndex = DnnInvoke.NMSBoxes(boxes.ToArray(), scores.ToArray(), .9f, .1f);
            var frameOut = frame.ToImage<Bgr, byte>();
            for (int i = 0; i < bestIndex.Length; i++)
            {
                int index = bestIndex[i];
                var box = boxes[index];
                var text = classLabels[indices[index]];
                CvInvoke.Rectangle(frameOut, box, new MCvScalar(0, 255, 0), 2);
                CvInvoke.PutText(frameOut, text, new System.Drawing.Point(box.X, box.Y - 20),
                Emgu.CV.CvEnum.FontFace.HersheyPlain, 1.0, new MCvScalar(0, 0, 255), 2);
            }
            CvInvoke.Resize(frameOut, frameOut, new System.Drawing.Size(0, 0), 4, 4);
            CvInvoke.Imshow("output", frameOut);
            if (CvInvoke.WaitKey(1) == 27)
                break;
        }
    }
}
