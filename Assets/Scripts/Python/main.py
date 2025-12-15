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
model = YOLO(r"C:\Users\piow3\Documents\GitHub\Project-3\Assets\Scripts\Python\best.pt")
while(True):
    # Create data structure
    data = { "Points": [] }
    # Capture frame from the VideoCapture object:
    ret, frame = video_capture.read()
    if ret:
        results = model(frame, conf = 0.4)  
        for result in results:
            boxes = result.boxes
            if len(boxes) == 0:
                continue
            for box in boxes:
                # (https://github.com/ultralytics/ultralytics/issues/2868)
                class_label = result.names[int(box.cls[0])]
                # extract box coordinates
                x1, y1, x2, y2 = box.xyxy[0]
                #get center point
                cx = (x1 + x2) / 2
                cy = (y1 + y2) / 2
                # Draw bounding box on frame
                cv2.rectangle(frame, 
                          (int(x1), int(y1)), 
                          (int(x2), int(y2)), 
                          (0, 255, 0), 2)
                cv2.putText(frame, class_label, 
                        (int(x1), int(y1) - 10),
                        cv2.FONT_HERSHEY_SIMPLEX, 
                        0.7, (0, 255, 0), 2)
                

                cv2.circle(frame, 
                           (int(cx),int(cy))
                           ,1,(0, 0, 255),1)
                #invert y axis https://stackoverflow.com/questions/39953263/get-video-dimension-in-python-opencv
                cy = video_capture.get(cv2.CAP_PROP_FRAME_HEIGHT) -cy
                #add point to data structure
                
                isHorizontal = bool((abs(x1-x2) < abs(y1-y2) ))
                length = abs(x1-x2) if bool((abs(x1-x2) <  abs(y1-y2))) else abs(y1-y2)
                data["Points"].append({ "brickType": str(class_label),"x": float(cx), "y": float(cy), "isHorizontal": bool(isHorizontal), "length": float(length)})
                # send data to Unity
        connector.send("box_coordinates", data)
        data = { "Points": [] }
    cv2.imshow("Camera with Bounding Boxes", frame)
    cv2.waitKey(5000)

            
