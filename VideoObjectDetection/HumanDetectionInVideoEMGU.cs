using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Features2D;
using Emgu.CV.Util;
using Emgu.Util.TypeEnum;
using Emgu.CV.XObjdetect;

namespace VideoObjectDetection
{
    static class HumanDetectionInVideoEMGU
    {
        public static void DetectImage(string imagePath = "BrooklynWillowSt.jpeg")
        {
            // Wczytaj obraz
            using var image = new Image<Bgr, byte>(imagePath);

            // Inicjalizacja detektora HOG
            var hog = new HOGDescriptor();
            hog.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());

            // Detekcja osób na obrazie
            MCvObjectDetection[] regions = hog.DetectMultiScale(image);

            //MCvObjectDetection[] regions = hog.DetectMultiScale(image,
            //                                       scale: 1.05,
            //                                       winStride: new Size(8, 8),
            //                                       padding: new Size(16, 16),
            //                                       finalThreshold: 3.0);

            // Narysuj prostokąty wokół wykrytych osób
            foreach (var region in regions)
            {
                // Użyj region.Rect, aby uzyskać prostokąt
                image.Draw(region.Rect, new Bgr(Color.Red), 3);
            }

            // Wyświetl obraz z narysowanymi prostokątami
            CvInvoke.Imshow("Detekcja osób", image);

            // Czekaj na klawisz ESC, aby zamknąć okno
            CvInvoke.WaitKey(0);

            // Zniszcz wszystkie okna
            CvInvoke.DestroyAllWindows();
        } 

        public static void DetectVideo(string videoPath = "test1.mp4")
        {
            // Inicjalizacja detektora HOG
            var hog = new HOGDescriptor();
            hog.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());

            // Otwórz plik wideo
            using var capture = new VideoCapture();

            while (true)
            {
                // Przeczytaj kolejną klatkę wideo
                using var frame = capture.QueryFrame().ToImage<Bgr, byte>();
                if (frame == null) break;

                // Detekcja osób
                MCvObjectDetection[] regions = hog.DetectMultiScale(frame);

                // Narysuj prostokąty wokół wykrytych osób
                foreach (var region in regions)
                {
                    // Użyj region.Rect, aby uzyskać prostokąt
                    frame.Draw(region.Rect, new Bgr(Color.Red), 3);
                }

                // Wyświetl klatkę za pomocą CvInvoke.Imshow
                CvInvoke.Imshow("Detekcja osób", frame);

                // Czekaj na klawisz ESC, aby wyjść
                if (CvInvoke.WaitKey(30) == 27) break;
            }

            // Zniszcz wszystkie okna
            CvInvoke.DestroyAllWindows();
        }

        public static void DetectSift(string imagePath = "BrooklynWillowSt.jpeg")
        {
            using var image = new Image<Bgr, byte>(imagePath);

            // Inicjalizacja SIFT
            var sift = new SIFT();

            // Wykrycie cech (keypoints) i deskryptorów
            VectorOfKeyPoint keyPoints = new VectorOfKeyPoint();
            Mat descriptors = new Mat();
            sift.DetectAndCompute(image, null, keyPoints, descriptors, false);

            // Narysowanie punktów kluczowych na obrazie
            var outputImage = new Mat();
            Features2DToolbox.DrawKeypoints(image, keyPoints, outputImage, new Bgr(Color.Red), Features2DToolbox.KeypointDrawType.Default);

            // Wyświetl obraz
            CvInvoke.Imshow("SIFT Keypoints", outputImage);
            CvInvoke.WaitKey(0);
            CvInvoke.DestroyAllWindows();
        }
    }
}
