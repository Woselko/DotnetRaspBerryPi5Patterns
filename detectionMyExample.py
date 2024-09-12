# import cv2
# from ultralytics import YOLO

# # Załaduj model YOLOv8
# model = YOLO('yolov8m-pose.pt')  # Możesz zmienić na inną wersję modelu np. 'yolov8s.pt', 'yolov8m.pt', 'yolov8l.pt', 'yolov8x.pt'

# # Uruchom kamerę
# cap = cv2.VideoCapture(0)

# if not cap.isOpened():
#     print("Nie udało się otworzyć kamery")
#     exit()

# while True:
#     # Przechwyć obraz z kamery
#     ret, frame = cap.read()

#     if not ret:
#         print("Nie udało się przechwycić obrazu")
#         break

#     # Wykrywanie obiektów za pomocą YOLOv8
#     results = model(frame)

#     # Pobierz wykryte obiekty
#     for result in results:
#         for box in result.boxes:
#             # if box.conf[0] < 1:
#             #     continue
#             # Rozpakuj koordynaty i etykiety
#             x1, y1, x2, y2 = map(int, box.xyxy[0])
#             label = model.names[int(box.cls[0])]
#             confidence = box.conf[0]

#             # Rysowanie prostokątów wokół wykrytych obiektów
#             cv2.rectangle(frame, (x1, y1), (x2, y2), (255, 0, 0), 2)
#             cv2.putText(frame, f'{label} {confidence:.2f}', (x1, y1 - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255, 0, 0), 2)
#     # Wyświetlanie obrazu z kamerki z zaznaczonymi obiektami
#     frame = cv2.resize(frame, None, fx=2, fy=2, interpolation=cv2.INTER_LINEAR)
#     cv2.imshow('YOLOv8 Detekcja Obiektów', frame)

#     # Wyjdź po naciśnięciu klawisza 'q'
#     if cv2.waitKey(1) & 0xFF == ord('q'):
#         break

# # Zwolnij kamerę i zamknij okna
# cap.release()
# cv2.destroyAllWindows()


import cv2
import numpy as np
import time

from ultralytics import YOLO

# # Load the YOLOv8 model
# model = YOLO("yolov8n.pt")

# # Export the model to ONNX format
# model.export(format="onnx")  # creates 'yolov8n.onnx'

# # Load the exported ONNX model
# onnx_model = YOLO("yolov8n.onnx")

# # Załaduj model YOLOv8
model = YOLO('yolov8m-pose.pt') 
model.predict(0, show = True)