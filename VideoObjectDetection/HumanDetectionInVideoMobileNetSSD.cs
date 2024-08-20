
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

public class HumanDetectionInVideoMobileNetSSD
{
    private readonly string _modelPath;
    private readonly string _configPath;
    private readonly string[] labels = new string[]
    {
        "background",    // 0
        "aeroplane",     // 1
        "bicycle",       // 2
        "bird",          // 3
        "boat",          // 4
        "bottle",        // 5
        "bus",           // 6
        "car",           // 7
        "cat",           // 8
        "chair",         // 9
        "cow",           // 10
        "diningtable",   // 11
        "dog",           // 12
        "horse",         // 13
        "motorbike",     // 14
        "person",        // 15
        "pottedplant",   // 16
        "sheep",         // 17
        "sofa",          // 18
        "train",         // 19
        "tvmonitor"      // 20
    };

    public HumanDetectionInVideoMobileNetSSD(string modelPath, string configPath)
    {
        _modelPath = modelPath;
        _configPath = configPath;
    }

    public (int, int) DetectObjectsInVideoCommonTest(string videoPath, string outputPath)
    {
        var net = DnnInvoke.ReadNetFromCaffe(_configPath, _modelPath);

        int frameCounter = 0;
        int personDetectedFrame = -1;
        int totalPersonFrames = 0;

        using var videoCapture = new VideoCapture(videoPath);
        using var videoWriter = new VideoWriter(outputPath, VideoWriter.Fourcc('m', 'p', '4', 'v'), videoCapture.Get(CapProp.Fps), new Size(videoCapture.Width, videoCapture.Height), true);

        Mat frame = new Mat();
        while (videoCapture.Read(frame) && !frame.IsEmpty)
        {
            frameCounter++;

            var inputBlob = DnnInvoke.BlobFromImage(frame, 0.007843, new Size(300, 300), new MCvScalar(127.5, 127.5, 127.5), false, false);
            net.SetInput(inputBlob);
            Mat detection = net.Forward();

            float[] data = new float[detection.Total.ToInt32()];
            Marshal.Copy(detection.DataPointer, data, 0, data.Length);

            for (int i = 0; i < data.Length / 7; i++)
            {
                float confidence = data[i * 7 + 2];
                if (confidence > 0.7f)
                {
                    int classId = (int)data[i * 7 + 1];
                    int x1 = (int)(data[i * 7 + 3] * frame.Width);
                    int y1 = (int)(data[i * 7 + 4] * frame.Height);
                    int x2 = (int)(data[i * 7 + 5] * frame.Width);
                    int y2 = (int)(data[i * 7 + 6] * frame.Height);

                    CvInvoke.Rectangle(frame, new Rectangle(x1, y1, x2 - x1, y2 - y1), new MCvScalar(0, 255, 0), 2);
                    CvInvoke.PutText(frame, $"{labels[classId]}: {confidence * 100:0.00}%", new Point(x1, y1 - 10), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 255, 0));

                    if (labels[classId] == "person")
                    {
                        totalPersonFrames++;
                        if (personDetectedFrame == -1)
                            personDetectedFrame = frameCounter;
                    }
                }
            }

            videoWriter.Write(frame);
        }

        Console.WriteLine("Wykrywanie zakończone.");
        return (personDetectedFrame, totalPersonFrames);
    }


    public void DetectObjectsInVideo(string videoPath, string outputPath)
    {
        string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _modelPath);
        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _configPath);

        if (!File.Exists(modelPath) || !File.Exists(configPath))
        {
            Console.WriteLine("One or more files are missing.");
            return;
        }

        // Wczytaj model
        var net = DnnInvoke.ReadNetFromCaffe(configPath, modelPath);

        // Wczytaj wideo
        using (var videoCapture = new VideoCapture(videoPath))
        {
            Mat frame = new Mat();
            videoCapture.Read(frame);

            if (frame.IsEmpty)
            {
                Console.WriteLine("Unable to read the video file or video is empty.");
                return;
            }

            double fps = videoCapture.Get(CapProp.Fps);
            int frameWidth = frame.Width;
            int frameHeight = frame.Height;

            using (var videoWriter = new VideoWriter(outputPath, VideoWriter.Fourcc('m', 'p', '4', 'v'), fps, new Size(frameWidth, frameHeight), true))
            {
                do
                {
                    if (frame.IsEmpty)
                        break;

                    // Przetwórz klatkę
                    var blob = DnnInvoke.BlobFromImage(frame, 0.007843, new Size(300, 300), new MCvScalar(127.5, 127.5, 127.5), false, false);

                    // Przekaż blob do sieci
                    net.SetInput(blob);
                    Mat detection = net.Forward();

                    // Przekopiuj dane z Mat do tablicy float
                    float[] data = new float[detection.Total.ToInt32()];
                    Marshal.Copy(detection.DataPointer, data, 0, data.Length);

                    // Przetwarzaj wykryte obiekty
                    for (int i = 0; i < data.Length / 7; i++)
                    {
                        float confidence = data[i * 7 + 2];
                        if (confidence > 0.7) // próg pewności
                        {
                            int classId = (int)data[i * 7 + 1];

                            // Ewentualne przesunięcie indeksów (jeśli konieczne)
                            // classId = classId - 1; // Odkomentuj, jeśli klasa jest przesunięta o 1

                            if (classId >= 0 && classId < labels.Length)
                            {
                                string label = labels[classId];

                                int x1 = (int)(data[i * 7 + 3] * frame.Width);
                                int y1 = (int)(data[i * 7 + 4] * frame.Height);
                                int x2 = (int)(data[i * 7 + 5] * frame.Width);
                                int y2 = (int)(data[i * 7 + 6] * frame.Height);

                                // Narysuj ramkę wokół wykrytego obiektu
                                CvInvoke.Rectangle(frame, new Rectangle(x1, y1, x2 - x1, y2 - y1), new MCvScalar(0, 255, 0), 2);
                                CvInvoke.PutText(frame, $"{label}: {confidence * 100:0.00}%", new Point(x1, y1 - 10), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 255, 0));
                            }
                        }
                    }

                    // Zapisz klatkę do nowego pliku wideo
                    videoWriter.Write(frame);
                } while (videoCapture.Read(frame));
            }
        }

        Console.WriteLine("Wykrywanie zakończone.");
    }

    public void DetectInWebCamera()
    {
        string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _modelPath);
        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _configPath);

        if (!File.Exists(modelPath) || !File.Exists(configPath))
        {
            Console.WriteLine("One or more files are missing.");
            return;
        }

        // Wczytaj model
        var net = DnnInvoke.ReadNetFromCaffe(configPath, modelPath);

        // Użyj kamery jako źródła wideo
        using (var videoCapture = new VideoCapture(0, VideoCapture.API.DShow)) // Kamera 0, API DirectShow
        {
            Mat frame = new Mat();

            while (true)
            {
                videoCapture.Read(frame);

                if (frame.IsEmpty)
                {
                    Console.WriteLine("Unable to read from camera or the frame is empty.");
                    break;
                }

                // Przetwórz klatkę
                var blob = DnnInvoke.BlobFromImage(frame, 0.007843, new Size(300, 300), new MCvScalar(127.5, 127.5, 127.5), false, false);

                // Przekaż blob do sieci
                net.SetInput(blob);
                Mat detection = net.Forward();

                // Przekopiuj dane z Mat do tablicy float
                float[] data = new float[detection.Total.ToInt32()];
                Marshal.Copy(detection.DataPointer, data, 0, data.Length);

                // Przetwarzaj wykryte obiekty
                for (int i = 0; i < data.Length / 7; i++)
                {
                    float confidence = data[i * 7 + 2];
                    if (confidence > 0.7) // próg pewności
                    {
                        int classId = (int)data[i * 7 + 1];

                        if (classId >= 0 && classId < labels.Length)
                        {
                            string label = labels[classId];

                            int x1 = (int)(data[i * 7 + 3] * frame.Width);
                            int y1 = (int)(data[i * 7 + 4] * frame.Height);
                            int x2 = (int)(data[i * 7 + 5] * frame.Width);
                            int y2 = (int)(data[i * 7 + 6] * frame.Height);

                            // Narysuj ramkę wokół wykrytego obiektu
                            CvInvoke.Rectangle(frame, new Rectangle(x1, y1, x2 - x1, y2 - y1), new MCvScalar(0, 255, 0), 2);
                            CvInvoke.PutText(frame, $"{label}: {confidence * 100:0.00}%", new Point(x1, y1 - 10), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 255, 0));
                        }
                    }
                }

                // Wyświetl klatkę na ekranie
                CvInvoke.Imshow("Camera Object Detection", frame);

                // Przerwij, jeśli użytkownik naciśnie 'q' lub 'ESC'
                if (CvInvoke.WaitKey(1) == 27) // 27 to kod ASCII dla klawisza 'ESC'
                {
                    break;
                }
            }
        }

        CvInvoke.DestroyAllWindows();
        Console.WriteLine("Detekcja zakończona.");
    }

}

