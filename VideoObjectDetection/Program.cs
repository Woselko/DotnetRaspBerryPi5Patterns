using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;

internal class Program
{
    static void Main(string[] args)
    {
        //yolov3.weights need to be downloaded from https://pjreddie.com/darknet/yolo/ its >200mb
        var net = Emgu.CV.Dnn.DnnInvoke.ReadNetFromDarknet("yolov3.cfg", "yolov3.weights");
        var classLabels = File.ReadAllLines("coco.names");
        net.SetPreferableBackend(Emgu.CV.Dnn.Backend.OpenCV);
        net.SetPreferableTarget(Emgu.CV.Dnn.Target.Cpu);
        //var vc = new VideoCapture(0, VideoCapture.API.DShow);
        string videoPath = "test1.avi";
        VideoCapture vc = new VideoCapture(videoPath);
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
            var image = frame.ToImage<Bgr, byte>();
            var input = DnnInvoke.BlobFromImage(image, 1 / 255.0, swapRB: true);
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
                    if (confidence > 0.8f)
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
            var bestIndex = DnnInvoke.NMSBoxes(boxes.ToArray(), scores.ToArray(), .8f, .8f);
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
