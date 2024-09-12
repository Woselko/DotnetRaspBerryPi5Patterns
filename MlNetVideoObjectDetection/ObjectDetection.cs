//using Emgu.CV;
//using Emgu.CV.Reg;
//using Emgu.CV.Structure;
//using Microsoft.ML;
//using Microsoft.ML.Data;
//using System.Drawing;
//using Emgu.CV;
//using Emgu.CV.CvEnum;
//using System.IO;


//namespace MlNetVideoObjectDetection
//{
//    public class ObjectDetection
//    {
//        private readonly PredictionEngine<ImageInputData, ImageObjectPrediction> _predictionEngine;

//        public ObjectDetection(string modelPath)
//        {
//            var mlContext = new MLContext();
//            var model = mlContext.Model.Load(modelPath, out _);
//            _predictionEngine = mlContext.Model.CreatePredictionEngine<ImageInputData, ImageObjectPrediction>(model);
//        }

//        public ImageObjectPrediction Predict(ImageInputData imageData)
//        {
//            return _predictionEngine.Predict(imageData);
//        }

//        public void ProcessVideo(string videoPath)
//        {
//            using var capture = new VideoCapture(videoPath);
//            var frame = new Mat();
//            var detector = new ObjectDetection("model.onnx");

//            while (true)
//            {
//                capture.Read(frame);
//                if (frame.IsEmpty)
//                    break;

//                // Konwertuj klatkę do Bitmapy
//                Bitmap bitmap = frame.ToBitmap();

//                // Przetwarzaj klatkę za pomocą ML.NET
//                var imageData = new ImageInputData { Image = bitmap };
//                var prediction = detector.Predict(imageData);

//                // Przetwarzanie wyników predykcji (rysowanie obiektów itp.)
//                DrawPredictions(frame, prediction);

//                // Wyświetlanie klatki
//                CvInvoke.Imshow("Object Detection", frame);
//                if (CvInvoke.WaitKey(1) == 27) // Naciśnięcie ESC
//                    break;
//            }
//            CvInvoke.DestroyAllWindows();
//        }

//        private void DrawPredictions(Mat frame, ImageObjectPrediction prediction)
//        {
//            // Zakładając, że mamy bounding boxy w wyniku predykcji
//            foreach (var box in prediction.BoundingBoxes)
//            {
//                CvInvoke.Rectangle(frame, new Rectangle((int)box.X, (int)box.Y, (int)box.Width, (int)box.Height), new MCvScalar(255, 0, 0), 2);
//                CvInvoke.PutText(frame, box.Label, new Point((int)box.X, (int)box.Y - 10), Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.9, new MCvScalar(255, 0, 0), 2);
//            }
//        }
//    }

//    public class ImageInputData
//    {
//        public byte[] Image { get; set; }

//        public static ImageInputData CreateFromMat(Mat mat)
//        {
//            // Konwertuj Mat na tablicę bajtów
//            using (var memoryStream = new MemoryStream())
//            {
//                mat.ToImage<Bgr, byte>().ToBitmap().Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
//                return new ImageInputData { Image = memoryStream.ToArray() };
//            }
//        }
//    }

//    public class ImageObjectPrediction
//    {
//        [ColumnName("grid")] // Zależnie od modelu, może być inna nazwa
//        public float[] PredictedLabels { get; set; }
//    }

//}



