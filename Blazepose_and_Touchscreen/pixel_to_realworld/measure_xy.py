import math
import cv2
import numpy as np
import mediapipe as mp
mp_drawing = mp.solutions.drawing_utils
mp_drawing_styles = mp.solutions.drawing_styles
mp_hands = mp.solutions.hands

pix_xs = []
pix_ys = []

# TOP CAMERA
DIM=(1024, 700)
K = np.array([[456.47095121, 0.0, 450.34975221], [0.0, 518.37877053, 65.50468751], [0.0, 0.0, 1.0]])
D = np.array([[0.1507154 ],[0.01015095], [0.01468287], [0.00592813]])
dim1 = [1024, 700]
scaled_K = K * dim1[0] / DIM[0]
scaled_K[2][2] = 1.0
new_K = cv2.fisheye.estimateNewCameraMatrixForUndistortRectify(scaled_K, D, dim1, np.eye(3), balance=1.0)
map1_top, map2_top = cv2.fisheye.initUndistortRectifyMap(scaled_K, D, np.eye(3), new_K, dim1, cv2.CV_16SC2)

# BOT CAMERA
DIM=(1300, 1200)
K=np.array([[787.8524863867165, 0.0, 744.7118000552183], [0.0, 829.5071163774452, 561.3314451453386], [0.0, 0.0, 1.0]])
D=np.array([[-0.052595202508066485], [0.03130776521577516], [-0.04104704724832264], [0.015343014605793348]])
dim1 = [1300, 1200]
scaled_K = K * dim1[0] / DIM[0]
scaled_K[2][2] = 1.0
new_K = cv2.fisheye.estimateNewCameraMatrixForUndistortRectify(scaled_K, D, dim1, np.eye(3), balance=1.0)
map1_bot, map2_bot = cv2.fisheye.initUndistortRectifyMap(scaled_K, D, np.eye(3), new_K, dim1, cv2.CV_16SC2)

def undistort(img):
    undistort = cv2.remap(img, map1_bot, map2_bot, interpolation=cv2.INTER_LINEAR, borderMode=cv2.BORDER_CONSTANT)
    return undistort

counter = 0
cap = cv2.VideoCapture(2)
cap.set(cv2.CAP_PROP_FRAME_WIDTH, 1600)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 1200)
hands = mp_hands.Hands(max_num_hands=1, min_detection_confidence=0.5, min_tracking_confidence=0.5)
while cap.isOpened():
    success, image = cap.read()
    if not success:
        print("Ignoring empty camera frame.")
        # If loading a video, use 'break' instead of 'continue'.
        continue
    # image = image[500:, 288:-288]
    image = image[:, 150:-150]

    # To improve performance, optionally mark the image as not writeable to
    # pass b reference.
    image.flags.writeable = False
    image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    image = undistort(image)
    results = hands.process(image)

    # Draw the hand annotations on the image.
    image.flags.writeable = True
    image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
    if results.multi_hand_landmarks:
        for hand_landmarks in results.multi_hand_landmarks:
            mp_drawing.draw_landmarks(
                image,
                hand_landmarks,
                mp_hands.HAND_CONNECTIONS,
                mp_drawing_styles.get_default_hand_landmarks_style(),
                mp_drawing_styles.get_default_hand_connections_style())
    # Flip the image horizontally for a selfie-view display.
    cv2.imshow('MediaPipe Hands', cv2.resize(image, None, fx=0.5, fy=0.5))
    key = cv2.waitKey(5)
    if key == ord('s'):
        x, y = results.multi_hand_landmarks[0].landmark[0].x, results.multi_hand_landmarks[0].landmark[0].y
        pix_xs.append(x)
        pix_ys.append(y)
        print(counter, ": ", " x: ", x,  " y: ", y)
        counter += 1
    if key == ord('q'):
        print(pix_xs)
        print(pix_ys)
        break
cap.release()
