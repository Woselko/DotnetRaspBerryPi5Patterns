using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Drawing;
using SixLabors.Fonts.WellKnownIds;
//using SixLabors.ImageSharp.Processing.Text;
using SixLabors.Fonts;
using Emgu.CV;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Yolov8Net;
using Emgu.CV;
using Emgu.CV.CvEnum;
//using System.Drawing;

namespace VideoObjectDetection
{
    public class YOLOv8
    {
        string _modelPath;

        public YOLOv8(string modelPath)
        {
            _modelPath = modelPath;
        }

        public (int, int) DetectObjectsInVideoCommonTest(string videoPath, string outputPath)
        {
            // Inicjalizacja modelu ONNX przy użyciu SixLabors.ImageSharp
            using var yolo = YoloV8Predictor.Create(_modelPath);

            // Inicjalizacja obiektów Emgu CV do pracy z wideo
            using var videoCapture = new VideoCapture(videoPath);
            using var videoWriter = new VideoWriter(outputPath, VideoWriter.Fourcc('m', 'p', '4', 'v'), 30, new System.Drawing.Size(videoCapture.Width, videoCapture.Height), true);

            int frameCounter = 0;
            int personDetectedFrame = -1;
            int totalPersonFrames = 0;

            Mat frame = new Mat();
            while (videoCapture.Read(frame) && !frame.IsEmpty)
            {
                frameCounter++;
                // Konwersja klatki Mat na Image<Rgba32> (ImageSharp)
                using var imageSharp = Image.LoadPixelData<Rgba32>(frame.ToImage<Bgra, byte>().Bytes, frame.Width, frame.Height);

                // Predykcja z użyciem modelu YoloV8
                var predictions = yolo.Predict(imageSharp);

                // Rysowanie wyników predykcji na obrazie
                foreach (var pred in predictions)
                {
                    if (pred.Score < 0.7f)
                        continue;
                    var x = Math.Max(pred.Rectangle.X, 0);
                    var y = Math.Max(pred.Rectangle.Y, 0);
                    var width = Math.Min(imageSharp.Width - x, pred.Rectangle.Width);
                    var height = Math.Min(imageSharp.Height - y, pred.Rectangle.Height);

                    // Tekst dla ramki
                    string text = $"{pred.Label.Name} [{pred.Score}]";

                    // Tworzenie czcionki
                    var font = SystemFonts.CreateFont("Consolas", 11);
                    var textSize = TextMeasurer.MeasureSize(text, new TextOptions(font));
                    if (pred.Label.Name == "person")
                    {
                        totalPersonFrames++;
                        if (personDetectedFrame == -1)
                            personDetectedFrame = frameCounter;
                    }
                    // Definiowanie pióra i pędzla
                    var pen = Pens.Solid(SixLabors.ImageSharp.Color.Yellow, 2.0f);
                    var colorBrush = Brushes.Solid(SixLabors.ImageSharp.Color.Yellow);

                    // Rysowanie na obrazie
                    imageSharp.Mutate(ctx =>
                    {
                        ctx.Fill(colorBrush, new RectangleF(x, y - textSize.Height - 1, textSize.Width, textSize.Height));
                        ctx.DrawText(text, font, SixLabors.ImageSharp.Color.Black, new PointF(x, y - textSize.Height - 1));
                        ctx.Draw(pen, new RectangleF(x, y, width, height));
                    });
                }

                // Konwersja z powrotem do Mat (Emgu CV) - konwersja z RGBA na BGR
                var bgrBytes = new byte[imageSharp.Width * imageSharp.Height * 3];
                imageSharp.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        var pixelRow = accessor.GetRowSpan(y);
                        for (int x = 0; x < accessor.Width; x++)
                        {
                            var pixel = pixelRow[x];
                            int offset = (y * accessor.Width + x) * 3;
                            bgrBytes[offset + 0] = pixel.R; // Red na Blue
                            bgrBytes[offset + 1] = pixel.G; // Green na Green
                            bgrBytes[offset + 2] = pixel.B; // Blue na Red
                        }
                    }
                });

                frame = new Mat(imageSharp.Height, imageSharp.Width, DepthType.Cv8U, 3);
                frame.SetTo(bgrBytes);

                videoWriter.Write(frame);
            }

            Console.WriteLine("Processing completed. Output saved to " + outputPath);
            return (personDetectedFrame, totalPersonFrames);
        }

        public void DetectObjectsInVideo(string videoPath, string outputPath)
        {
            // Inicjalizacja modelu ONNX przy użyciu SixLabors.ImageSharp
            using var yolo = YoloV8Predictor.Create(_modelPath);

            // Inicjalizacja obiektów Emgu CV do pracy z wideo
            using var videoCapture = new VideoCapture(videoPath);
            using var videoWriter = new VideoWriter(outputPath, VideoWriter.Fourcc('m', 'p', '4', 'v'), 30, new System.Drawing.Size(videoCapture.Width, videoCapture.Height), true);

            Mat frame = new Mat();
            while (videoCapture.Read(frame) && !frame.IsEmpty)
            {
                // Konwersja klatki Mat na Image<Rgba32> (ImageSharp)
                using var imageSharp = Image.LoadPixelData<Rgba32>(frame.ToImage<Bgra, byte>().Bytes, frame.Width, frame.Height);

                // Predykcja z użyciem modelu YoloV8
                var predictions = yolo.Predict(imageSharp);

                // Rysowanie wyników predykcji na obrazie
                foreach (var pred in predictions)
                {
                    if (pred.Score < 0.7f)
                        continue;
                    var x = Math.Max(pred.Rectangle.X, 0);
                    var y = Math.Max(pred.Rectangle.Y, 0);
                    var width = Math.Min(imageSharp.Width - x, pred.Rectangle.Width);
                    var height = Math.Min(imageSharp.Height - y, pred.Rectangle.Height);

                    // Tekst dla ramki
                    string text = $"{pred.Label.Name} [{pred.Score}]";

                    // Tworzenie czcionki
                    var font = SystemFonts.CreateFont("Consolas", 11);
                    var textSize = TextMeasurer.MeasureSize(text, new TextOptions(font));

                    // Definiowanie pióra i pędzla
                    var pen = Pens.Solid(SixLabors.ImageSharp.Color.Yellow, 2.0f);
                    var colorBrush = Brushes.Solid(SixLabors.ImageSharp.Color.Yellow);

                    // Rysowanie na obrazie
                    imageSharp.Mutate(ctx =>
                    {
                        ctx.Fill(colorBrush, new RectangleF(x, y - textSize.Height - 1, textSize.Width, textSize.Height));
                        ctx.DrawText(text, font, SixLabors.ImageSharp.Color.Black, new PointF(x, y - textSize.Height - 1));
                        ctx.Draw(pen, new RectangleF(x, y, width, height));
                    });
                }

                // Konwersja z powrotem do Mat (Emgu CV) - konwersja z RGBA na BGR
                var bgrBytes = new byte[imageSharp.Width * imageSharp.Height * 3];
                imageSharp.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        var pixelRow = accessor.GetRowSpan(y);
                        for (int x = 0; x < accessor.Width; x++)
                        {
                            var pixel = pixelRow[x];
                            int offset = (y * accessor.Width + x) * 3;
                            bgrBytes[offset + 0] = pixel.R; // Red na Blue
                            bgrBytes[offset + 1] = pixel.G; // Green na Green
                            bgrBytes[offset + 2] = pixel.B; // Blue na Red
                        }
                    }
                });

                frame = new Mat(imageSharp.Height, imageSharp.Width, DepthType.Cv8U, 3);
                frame.SetTo(bgrBytes);

                videoWriter.Write(frame);
            }

            Console.WriteLine("Processing completed. Output saved to " + outputPath);
        }

        public void DetectObjectsFromWebcamColorCorrection()
        {
            // Inicjalizacja modelu ONNX przy użyciu SixLabors.ImageSharp
            using var yolo = YoloV8Predictor.Create(_modelPath);

            // Inicjalizacja obiektów Emgu CV do pracy z kamerą internetową
            var vc = new VideoCapture(0, VideoCapture.API.DShow); // Kamera internetowa

            if (!vc.IsOpened)
            {
                Console.WriteLine("Nie udało się otworzyć kamery.");
                return;
            }

            Mat frame = new Mat();
            while (true)
            {
                vc.Read(frame);
                if (frame.IsEmpty)
                    continue;

                // Konwersja klatki Mat na Image<Rgba32> (ImageSharp)
                using var imageSharp = Image.LoadPixelData<Rgba32>(frame.ToImage<Bgra, byte>().Bytes, frame.Width, frame.Height);

                // Predykcja z użyciem modelu YoloV8
                var predictions = yolo.Predict(imageSharp);

                // Rysowanie wyników predykcji na obrazie
                foreach (var pred in predictions)
                {
                    if (pred.Score < 0.7f)
                        continue;
                    var x = Math.Max(pred.Rectangle.X, 0);
                    var y = Math.Max(pred.Rectangle.Y, 0);
                    var width = Math.Min(imageSharp.Width - x, pred.Rectangle.Width);
                    var height = Math.Min(imageSharp.Height - y, pred.Rectangle.Height);

                    // Tekst dla ramki
                    string text = $"{pred.Label.Name} [{pred.Score}]";

                    // Tworzenie czcionki
                    var font = SystemFonts.CreateFont("Consolas", 11);
                    var textSize = TextMeasurer.MeasureSize(text, new TextOptions(font));

                    // Definiowanie pióra i pędzla
                    var pen = Pens.Solid(SixLabors.ImageSharp.Color.Yellow, 2.0f);
                    var colorBrush = Brushes.Solid(SixLabors.ImageSharp.Color.Yellow);

                    // Rysowanie na obrazie
                    imageSharp.Mutate(ctx =>
                    {
                        ctx.Fill(colorBrush, new RectangleF(x, y - textSize.Height - 1, textSize.Width, textSize.Height));
                        ctx.DrawText(text, font, SixLabors.ImageSharp.Color.Black, new PointF(x, y - textSize.Height - 1));
                        ctx.Draw(pen, new RectangleF(x, y, width, height));
                    });
                }

                // Konwersja z powrotem do Mat (Emgu CV) - konwersja z RGBA na BGR
                var bgrBytes = new byte[imageSharp.Width * imageSharp.Height * 3];
                imageSharp.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        var pixelRow = accessor.GetRowSpan(y);
                        for (int x = 0; x < accessor.Width; x++)
                        {
                            var pixel = pixelRow[x];
                            int offset = (y * accessor.Width + x) * 3;
                            bgrBytes[offset + 0] = pixel.R; // Red na Blue
                            bgrBytes[offset + 1] = pixel.G; // Green na Green
                            bgrBytes[offset + 2] = pixel.B; // Blue na Red
                        }
                    }
                });

                frame = new Mat(imageSharp.Height, imageSharp.Width, DepthType.Cv8U, 3);
                frame.SetTo(bgrBytes);

                // Wyświetlenie przetworzonej klatki
                CvInvoke.Imshow("YOLOv8 Webcam Detection", frame);

                // Wyjście po naciśnięciu klawisza 'q'
                if (CvInvoke.WaitKey(1) == 'q')
                    break;
            }

            vc.Release();
            CvInvoke.DestroyAllWindows();
        }


        public void DetectObjectsFromWebcam()
        {
            // Inicjalizacja modelu ONNX przy użyciu SixLabors.ImageSharp
            using var yolo = YoloV8Predictor.Create(_modelPath);

            // Inicjalizacja obiektów Emgu CV do pracy z kamerą internetową
            var vc = new VideoCapture(0, VideoCapture.API.DShow); // Kamera internetowa

            if (!vc.IsOpened)
            {
                Console.WriteLine("Nie udało się otworzyć kamery.");
                return;
            }

            Mat frame = new Mat();
            while (true)
            {
                vc.Read(frame);
                if (frame.IsEmpty)
                    continue;

                // Konwersja klatki Mat na Image<Rgba32> (ImageSharp)
                using var imageSharp = Image.LoadPixelData<Rgba32>(frame.ToImage<Bgra, byte>().Bytes, frame.Width, frame.Height);

                // Predykcja z użyciem modelu YoloV8
                var predictions = yolo.Predict(imageSharp);

                // Rysowanie wyników predykcji na obrazie
                foreach (var pred in predictions)
                {
                    if (pred.Score < 0.7f)
                        continue;
                    var x = Math.Max(pred.Rectangle.X, 0);
                    var y = Math.Max(pred.Rectangle.Y, 0);
                    var width = Math.Min(imageSharp.Width - x, pred.Rectangle.Width);
                    var height = Math.Min(imageSharp.Height - y, pred.Rectangle.Height);

                    // Tekst dla ramki
                    string text = $"{pred.Label.Name} [{pred.Score}]";

                    // Tworzenie czcionki
                    var font = SystemFonts.CreateFont("Consolas", 11);
                    var textSize = TextMeasurer.MeasureSize(text, new TextOptions(font));

                    // Definiowanie pióra i pędzla
                    var pen = Pens.Solid(SixLabors.ImageSharp.Color.Yellow, 2.0f);
                    var colorBrush = Brushes.Solid(SixLabors.ImageSharp.Color.Yellow);

                    // Rysowanie na obrazie
                    imageSharp.Mutate(ctx =>
                    {
                        ctx.Fill(colorBrush, new RectangleF(x, y - textSize.Height - 1, textSize.Width, textSize.Height));
                        ctx.DrawText(text, font, SixLabors.ImageSharp.Color.Black, new PointF(x, y - textSize.Height - 1));
                        ctx.Draw(pen, new RectangleF(x, y, width, height));
                    });
                }

                // Konwersja z powrotem do Mat (Emgu CV) - konwersja z RGBA na BGR
                var bgrBytes = new byte[imageSharp.Width * imageSharp.Height * 3];
                imageSharp.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        var pixelRow = accessor.GetRowSpan(y);
                        for (int x = 0; x < accessor.Width; x++)
                        {
                            var pixel = pixelRow[x];
                            int offset = (y * accessor.Width + x) * 3;
                            bgrBytes[offset + 0] = pixel.B;
                            bgrBytes[offset + 1] = pixel.G;
                            bgrBytes[offset + 2] = pixel.R;
                        }
                    }
                });

                frame = new Mat(imageSharp.Height, imageSharp.Width, DepthType.Cv8U, 3);
                frame.SetTo(bgrBytes);

                // Wyświetlenie przetworzonej klatki
                CvInvoke.Imshow("YOLOv8 Webcam Detection", frame);

                // Wyjście po naciśnięciu klawisza 'q'
                if (CvInvoke.WaitKey(1) == 'q')
                    break;
            }

            vc.Release();
            CvInvoke.DestroyAllWindows();
        }


        public void DetectImg(string fileInput = "BrooklynWillowSt.jpeg", string fileOutput = "yolov8Onnx/detected.jpg")
        {
            // Utwórz nowy predyktor YoloV8, podając model (w formacie ONNX)
            using var yolo = YoloV8Predictor.Create("yolov8Onnx/yolov8m.onnx");

            // Załaduj obraz
            using var image = Image.Load<Rgba32>(fileInput);
            var predictions = yolo.Predict(image);

            // Rysowanie obramowań
            foreach (var pred in predictions)
            {
                var originalImageHeight = image.Height;
                var originalImageWidth = image.Width;

                var x = Math.Max(pred.Rectangle.X, 0);
                var y = Math.Max(pred.Rectangle.Y, 0);
                var width = Math.Min(originalImageWidth - x, pred.Rectangle.Width);
                var height = Math.Min(originalImageHeight - y, pred.Rectangle.Height);

                // Tekst dla ramki
                string text = $"{pred.Label.Name} [{pred.Score}]";

                // Tworzenie czcionki
                var font = SystemFonts.CreateFont("Consolas", 11);

                var textSize = TextMeasurer.MeasureSize(text, new TextOptions(font));

                // Definiowanie pióra i pędzla
                var pen = Pens.Solid(Color.Yellow, 2.0f);
                var colorBrush = Brushes.Solid(Color.Yellow);

                // Rysowanie na obrazie
                image.Mutate(ctx =>
                {
                    ctx.Fill(colorBrush, new RectangleF(x, y - textSize.Height - 1, textSize.Width, textSize.Height));
                    ctx.DrawText(text, font, Color.Black, new PointF(x, y - textSize.Height - 1));

                    // Rysowanie prostokąta (obramowania)
                    ctx.Draw(pen, new RectangleF(x, y, width, height));
                });
            }
            // Opcjonalnie, zapisz obraz
            image.Save(fileOutput);
        }
    }
}
