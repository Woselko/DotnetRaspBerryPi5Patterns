//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace VideoObjectDetection
//{
//    internal class PythonNet
//    {
//        public static void DetectObjects(string imagePath)
//        {
//            using (Py.GIL())
//            {
//                dynamic torch = Py.Import("torch");
//                dynamic torchvision = Py.Import("torchvision");
//                dynamic cv2 = Py.Import("cv2");
//                dynamic model = torch.load("model.pt");
//                model.eval();

//                // Załaduj obraz
//                dynamic img = cv2.imread(imagePath);
//                img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB);

//                // Przekształć obraz do formatu używanego przez model
//                dynamic transform = torchvision.transforms.Compose(new PyList(
//                    new PyObject[] { torchvision.transforms.ToTensor() }));
//                dynamic input = transform(img).unsqueeze(0);

//                // Przewiduj obiekty na obrazie
//                dynamic output = model(input);

//                // Przetwórz wynik (np. wyświetl go lub zwróć do C#)
//            }
//        }
//    }
//}
