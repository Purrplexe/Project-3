import numpy as np                          # for ARRAYSs
import cv2                    # for OpenCV
from UnityConnector import UnityConnector
from ultralytics import YOLO
import time

def on_timeout():
    print("timeout")
    
def on_stopped():
    print("stopped by Unity")

connector = UnityConnector(
    on_timeout=on_timeout,
    on_stopped=on_stopped
)

def on_data_received(data_type, data):
    print(data_type, data)

print("connecting...")

connector.start_listening(
    on_data_received
)

print("connected")
video_capture = cv2.VideoCapture(0)
#load our trained model
model = YOLO(r"D:\Project 3\Project 3\yolov5\runs\detect\train\weights\best.pt")
while(True):
    # Create data structure
    data = { "Points": [] }
    # Capture frame from the VideoCapture object:
    ret, frame = video_capture.read()
    if ret:
        results = model(frame, conf = 0.7)
        for result in results:
            boxes = result.boxes
            if len(boxes) == 0:
                continue
            for box in boxes:
                #if box is a 2x6 (https://github.com/ultralytics/ultralytics/issues/2868)
                class_label = result.names[int(box.cls[0])]
                if class_label != "brick_2x4":
                    continue
                # extract box coordinates
                x1, y1, x2, y2 = box.xyxy[0]
                #get center point
                cx = (x1 + x2) / 2
                cy = (y1 + y2) / 2
                #add point to data structure
                data["Points"].append({ "x": float(cx), "y": float(cy) })
                # send data to Unity
            connector.send("box_coordinates", data)
            data = { "Points": [] }
    time.sleep(10)
            
