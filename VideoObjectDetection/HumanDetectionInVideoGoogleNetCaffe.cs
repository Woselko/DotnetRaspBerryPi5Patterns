
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Drawing;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

public class HumanDetectionInVideoGoogleNetCaffe
{
    private readonly string _modelPath;
    private readonly string _configPath;
    private readonly string _labelsPath;

    public HumanDetectionInVideoGoogleNetCaffe(string modelPath, string configPath, string labelsPath)
    {
        _modelPath = modelPath;
        _configPath = configPath;
        _labelsPath = labelsPath;
    }

    public (int, int) DetectObjectsInVideoCommonTest(string videoPath, string outputPath)
    {
        var net = DnnInvoke.ReadNetFromCaffe(_configPath, _modelPath);
        var labels = File.ReadAllLines(_labelsPath);

        int frameCounter = 0;
        int personDetectedFrame = -1;
        int totalPersonFrames = 0;

        using var videoCapture = new VideoCapture(videoPath);
        using var videoWriter = new VideoWriter(outputPath, VideoWriter.Fourcc('m', 'p', '4', 'v'), videoCapture.Get(CapProp.Fps), new Size(videoCapture.Width, videoCapture.Height), true);

        Mat frame = new Mat();
        while (videoCapture.Read(frame) && !frame.IsEmpty)
        {
            frameCounter++;

            var inputBlob = DnnInvoke.BlobFromImage(frame, 1, new Size(224, 224), new MCvScalar(), false, false);
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
                    string label = $"{labels[classId]}: {confidence * 100:0.00}%";
                    CvInvoke.PutText(frame, label, new Point(x1, y1 - 10), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 255, 0));

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
        string labelsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _labelsPath);

        if (!File.Exists(modelPath) || !File.Exists(configPath) || !File.Exists(labelsPath))
        {
            Console.WriteLine("One or more files are missing.");
            return;
        }

        // Wczytaj etykiety
        string[] labels = File.ReadAllLines(labelsPath);

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
                    var blob = DnnInvoke.BlobFromImage(frame, 1, new Size(224, 224));

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
                            int x1 = (int)(data[i * 7 + 3] * frame.Width);
                            int y1 = (int)(data[i * 7 + 4] * frame.Height);
                            int x2 = (int)(data[i * 7 + 5] * frame.Width);
                            int y2 = (int)(data[i * 7 + 6] * frame.Height);

                            // Narysuj ramkę wokół wykrytego obiektu
                            CvInvoke.Rectangle(frame, new Rectangle(x1, y1, x2 - x1, y2 - y1), new MCvScalar(0, 255, 0), 2);
                            string label = $"{labels[classId]}: {confidence * 100:0.00}%";
                            CvInvoke.PutText(frame, label, new Point(x1, y1 - 10), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 255, 0));
                        }
                    }

                    // Zapisz klatkę do nowego pliku wideo
                    videoWriter.Write(frame);
                } while (videoCapture.Read(frame));
            }
        }

        Console.WriteLine("Wykrywanie zakończone.");
    }

    public void DetectObjectsInVideo_MobileNetV2(string videoPath, string outputPath)
    {
        string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _modelPath);
        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _configPath);
        string labelsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _labelsPath);

        if (!File.Exists(modelPath) || !File.Exists(configPath) || !File.Exists(labelsPath))
        {
            Console.WriteLine("One or more files are missing.");
            return;
        }

        // Wczytaj etykiety
        string[] labels = File.ReadAllLines(labelsPath);

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
                    var blob = DnnInvoke.BlobFromImage(
                        frame,
                        1.0 / 255.0,  // Skalowanie wartości pikseli
                        new Size(224, 224),  // Rozmiar wejściowy zgodny z modelem MobileNetV2
                        new MCvScalar(103.94, 116.78, 123.68),  // Odejmuje średnie wartości pikseli (mean values)
                        swapRB: true,  // Przełączenie z BGR na RGB
                        crop: false  // Bez przycinania
                    );

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

                            // Debugowanie: Sprawdź, jakie classId są zwracane
                            Console.WriteLine($"Detected classId: {classId}");

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

    public void DetectObjectsInVideo_MobileNetV22(string videoPath, string outputPath)
    {
        string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _modelPath);
        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _configPath);
        string labelsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _labelsPath);

        if (!File.Exists(modelPath) || !File.Exists(configPath) || !File.Exists(labelsPath))
        {
            Console.WriteLine("One or more files are missing.");
            return;
        }

        // Wczytaj etykiety
        string[] labels = File.ReadAllLines(labelsPath);

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
                    var blob = DnnInvoke.BlobFromImage(
                        frame,
                        1.0 / 255.0,  // Skalowanie wartości pikseli
                        new Size(224, 224),  // Rozmiar wejściowy zgodny z modelem MobileNetV2
                        new MCvScalar(103.94, 116.78, 123.68),  // Odejmuje średnie wartości pikseli (mean values)
                        swapRB: true,  // Przełączenie z BGR na RGB
                        crop: false  // Bez przycinania
                    );

                    // Przekaż blob do sieci
                    net.SetInput(blob);
                    Mat detection = net.Forward();

                    // Przekopiuj dane z Mat do tablicy float
                    float[] data = new float[detection.Total.ToInt32()];
                    Marshal.Copy(detection.DataPointer, data, 0, data.Length);

                    // Znajdź indeks z najwyższą wartością (klasa o najwyższym prawdopodobieństwie)
                    int maxIndex = -1;
                    float maxConfidence = -1f;
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i] > maxConfidence)
                        {
                            maxConfidence = data[i];
                            maxIndex = i;
                        }
                    }

                    if (maxIndex >= 0 && maxIndex < labels.Length)
                    {
                        string label = labels[maxIndex];
                        Console.WriteLine($"Detected class: {label}, confidence: {maxConfidence}");

                        // Jeśli potrzebujesz narysować coś na obrazie, możesz to zrobić, ale zazwyczaj w klasyfikatorach nie jest to konieczne.
                        CvInvoke.PutText(frame, $"{label}: {maxConfidence * 100:0.00}%", new Point(10, 50), FontFace.HersheySimplex, 1.0, new MCvScalar(0, 255, 0), 2);
                    }

                    // Zapisz klatkę do nowego pliku wideo
                    videoWriter.Write(frame);
                } while (videoCapture.Read(frame));
            }
        }

        Console.WriteLine("Wykrywanie zakończone.");
    }


    public void DetectObjectsInVideo_MobileNetV222(string videoPath, string outputPath)
    {
        string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _modelPath);
        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _configPath);
        string labelsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _labelsPath);

        if (!File.Exists(modelPath) || !File.Exists(configPath) || !File.Exists(labelsPath))
        {
            Console.WriteLine("One or more files are missing.");
            return;
        }

        // Wczytaj etykiety
        string[] labels = File.ReadAllLines(labelsPath);

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
                    var blob = DnnInvoke.BlobFromImage(
                        frame,
                        1.0 / 255.0,  // Skalowanie wartości pikseli
                        new Size(224, 224),  // Rozmiar wejściowy zgodny z modelem MobileNetV2
                        new MCvScalar(103.94, 116.78, 123.68),  // Odejmuje średnie wartości pikseli (mean values)
                        swapRB: true,  // Przełączenie z BGR na RGB
                        crop: false  // Bez przycinania
                    );

                    // Przekaż blob do sieci
                    net.SetInput(blob);
                    Mat detection = net.Forward();

                    // Przekopiuj dane z Mat do tablicy float
                    float[] data = new float[detection.Total.ToInt32()];
                    Marshal.Copy(detection.DataPointer, data, 0, data.Length);

                    // Znajdź indeks z najwyższą wartością (klasa o najwyższym prawdopodobieństwie)
                    int maxIndex = -1;
                    float maxConfidence = -1f;
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i] > maxConfidence)
                        {
                            maxConfidence = data[i];
                            maxIndex = i;
                        }
                    }

                    if (maxIndex >= 0 && maxIndex < labels.Length)
                    {
                        string label = labels[maxIndex];
                        Console.WriteLine($"Detected class: {label}, confidence: {maxConfidence}");

                        // Jeśli potrzebujesz narysować coś na obrazie, możesz to zrobić, ale zazwyczaj w klasyfikatorach nie jest to konieczne.
                        CvInvoke.PutText(frame, $"{label}: {maxConfidence * 100:0.00}%", new Point(10, 50), FontFace.HersheySimplex, 1.0, new MCvScalar(0, 255, 0), 2);
                    }

                    // Zapisz klatkę do nowego pliku wideo
                    videoWriter.Write(frame);
                } while (videoCapture.Read(frame));
            }
        }

        Console.WriteLine("Wykrywanie zakończone.");
    }

    public void DetectObjectsInVideo_MobileNetCam()
    {
        string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _modelPath);
        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _configPath);
        string labelsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _labelsPath);

        if (!File.Exists(modelPath) || !File.Exists(configPath) || !File.Exists(labelsPath))
        {
            Console.WriteLine("One or more files are missing.");
            return;
        }

        // Wczytaj etykiety
        string[] labels = File.ReadAllLines(labelsPath);

        // Wczytaj model
        var net = DnnInvoke.ReadNetFromCaffe(configPath, modelPath);

        // Użyj kamery jako źródła wideo
        var vc = new VideoCapture(0, VideoCapture.API.DShow); // Kamera 0, API DirectShow
        if (!vc.IsOpened)
        {
            Console.WriteLine("Unable to access the camera.");
            return;
        }

        Mat frame = new Mat();
        while (true)
        {
            vc.Read(frame); // Wczytaj klatkę z kamery

            if (frame.IsEmpty)
            {
                Console.WriteLine("Unable to capture video from camera or video is empty.");
                break;
            }

            // Przetwórz klatkę
            var blob = DnnInvoke.BlobFromImage(
                frame,
                1.0 / 255.0,  // Skalowanie wartości pikseli
                new Size(224, 224),  // Rozmiar wejściowy zgodny z modelem MobileNetV2
                new MCvScalar(103.94, 116.78, 123.68),  // Odejmuje średnie wartości pikseli (mean values)
                swapRB: true,  // Przełączenie z BGR na RGB
                crop: false  // Bez przycinania
            );

            // Przekaż blob do sieci
            net.SetInput(blob);
            Mat detection = net.Forward();

            // Przekopiuj dane z Mat do tablicy float
            float[] data = new float[detection.Total.ToInt32()];
            Marshal.Copy(detection.DataPointer, data, 0, data.Length);

            // Znajdź indeks z najwyższą wartością (klasa o najwyższym prawdopodobieństwie)
            int maxIndex = -1;
            float maxConfidence = -1f;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] > maxConfidence)
                {
                    maxConfidence = data[i];
                    maxIndex = i;
                }
            }

            if (maxIndex >= 0 && maxIndex < labels.Length)
            {
                string label = labels[maxIndex];
                Console.WriteLine($"Detected class: {label}, confidence: {maxConfidence}");

                // Wyświetl etykietę na obrazie
                CvInvoke.PutText(frame, $"{label}: {maxConfidence * 100:0.00}%", new Point(10, 50), FontFace.HersheySimplex, 1.0, new MCvScalar(0, 255, 0), 2);
            }

            // Wyświetl klatkę na żywo
            CvInvoke.Imshow("Camera", frame);

            // Jeśli użytkownik naciśnie 'q', zakończ program
            if (CvInvoke.WaitKey(1) == 'q')
            {
                break;
            }
        }

        // Zwolnij zasoby kamery i zamknij okno
        vc.Release();
        CvInvoke.DestroyAllWindows();

        Console.WriteLine("Wykrywanie zakończone.");
    }

}

