import math
import socket
import struct
from _thread import *
import threading
import cv2
import numpy as np
from scipy.interpolate import interp1d
import mediapipe as mp
mp_drawing = mp.solutions.drawing_utils
mp_hands = mp.solutions.hands

# Socket only listens if no clients connected
noclients = True
# global landmarks array to send to socket
smooth_landmark = []
landmarks = np.zeros((21, 3), dtype=np.float32)
isPinched = False # if 2D euclidean dist is pinching
pinchThresh = 0.045
# Z distance estimation
zdists = [0.18483491735620391, 0.13680091575495834, 0.10386779102474056, 0.08529673979742568, 0.06965447610932723, 0.05892574374119139, 0.052710802405779425, 0.04641563912204463, 0.041223734793754706, 0.038386375932311985, 0.034576005847017234, 0.03110578475128149]
depths = range(1, 13)
fz = interp1d(zdists, depths, fill_value="extrapolate")
# x distance estimation
xs = [0.8058186769485474, 0.7680327892303467, 0.7422037124633789, 0.7034374475479126, 0.6599224805831909, 0.623870313167572, 0.5784257650375366, 0.5393351912498474, 0.4976162314414978, 0.4452589452266693, 0.3877609372138977, 0.34428590536117554, 0.314473956823349, 0.26358702778816223, 0.2183338701725006, 0.17534014582633972]
xdists = range(0, 16)
fx = interp1d(xs, xdists, fill_value="extrapolate")
# y distance estimation
ys = [0.25875619053840637, 0.3047190010547638, 0.34740883111953735, 0.40508753061294556, 0.4609423875808716, 0.5063245892524719, 0.5696361064910889, 0.6263465285301208, 0.7163373231887817]
ydists = range(2, 11)
ys.reverse()
fy = interp1d(ys, ydists, fill_value="extrapolate")

# Unwarping parameters
DIM=(640, 480)
K=np.array([[166.61079119072195, 0.0, 312.9184096351807], [0.0, 166.36889686101503, 220.81540593092757], [0.0, 0.0, 1.0]])
D=np.array([[0.17835573435561652], [-0.11728804523084109], [0.01697694605881821], [-0.000615972255290476]])
h, w = 640, 480
map1, map2 = cv2.fisheye.initUndistortRectifyMap(K, D, np.eye(3), K, DIM, cv2.CV_16SC2)
def undistort(img):
    undistorted_img = cv2.remap(img, map1, map2, interpolation=cv2.INTER_LINEAR, borderMode=cv2.BORDER_CONSTANT)
    return undistorted_img

# gets euclidean distance of two points
def get_dist(p1, p2):
    return math.sqrt((p1.x-p2.x)**2 + (p1.y-p2.y)**2 + (p1.z-p2.z)**2)

def get_dist_2d(p1, p2):
    return math.sqrt((p1.x-p2.x)**2 + (p1.y-p2.y)**2)

def threaded(c):
    global smooth_landmark, landmarks, noclients, isPinched
    while True:
        # data received from client
        data = c.recv(8000)
        if not data:
            print('Client disconnected.')
            noclients = True
            break
        if b'gimme' in data:
            if len(smooth_landmark) < 5:
                c.send(np.vstack((landmarks, np.array([int(isPinched), 0.0, 0.0]))).astype(np.float32).tobytes())
                # c.send(landmarks.astype(np.float32).tobytes())
                continue
            smoothed = np.average(np.array(smooth_landmark), axis=0)
            # a = (np.vstack((smoothed, np.array([int(isPinched), 0.0, 0.0]))).astype(np.float32))
            # print(a.dtype)
            # print(a.shape)
            # print(a)
            # c.send(smoothed.astype(np.float32).tobytes())
            c.send(np.vstack((smoothed, np.array([int(isPinched), 0.0, 0.0]))).astype(np.float32).tobytes())
    # connection closed
    c.close()

HOST = '172.26.110.197'
PORT = 5005
s=socket.socket(socket.AF_INET,socket.SOCK_STREAM)
print('Socket created')
s.bind((HOST,PORT))
print('Socket bind complete')
s.listen()

cap = cv2.VideoCapture(2)
hands = mp_hands.Hands(max_num_hands=1, min_detection_confidence=0.5, min_tracking_confidence=0.5)
while cap.isOpened():
    if noclients:
        print('Socket now listening')
        conn,addr=s.accept()
        print('Connected to :', addr[0], ':', addr[1])
        noclients = False
        start_new_thread(threaded, (conn,))

    success, image = cap.read()
    if not success: continue
    image.flags.writeable = False
    image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    image = undistort(image)
    results = hands.process(image)

    image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
    if results.multi_hand_world_landmarks:
        for i in range(21):
            landmarks[i] = np.array([results.multi_hand_world_landmarks[0].landmark[i].x,
                results.multi_hand_world_landmarks[0].landmark[i].y,
                results.multi_hand_world_landmarks[0].landmark[i].z])
        image.flags.writeable = True
        for hand_landmarks in results.multi_hand_landmarks:
            mp_drawing.draw_landmarks(
                image,
                hand_landmarks,
                mp_hands.HAND_CONNECTIONS)

        if get_dist_2d(results.multi_hand_landmarks[0].landmark[4], results.multi_hand_landmarks[0].landmark[8]) < pinchThresh: isPinched = True
        else:   isPinched = False
        dist = get_dist(results.multi_hand_landmarks[0].landmark[0], results.multi_hand_landmarks[0].landmark[1])
        zdepth = fz(dist)*0.0254
        xdepth = (fx(results.multi_hand_landmarks[0].landmark[0].x)-8)*0.0254*zdepth/0.1016
        ydepth = fy(results.multi_hand_landmarks[0].landmark[0].y)*0.0254*zdepth/0.1016 * 0.5
        print("x-inch: ", fx(results.multi_hand_landmarks[0].landmark[0].x), " y-inch: ", fy(results.multi_hand_landmarks[0].landmark[0].y), " z_inch: ", fz(dist))
        landmarks[:, 0] += xdepth
        landmarks[:, 1] += ydepth
        landmarks[:, 2] += zdepth
        smooth_landmark.append(landmarks.copy())
        if len(smooth_landmark) > 5: smooth_landmark = smooth_landmark[1:]
    else:
        landmarks = np.zeros((21, 3), dtype=np.float32)

    # Flip the image horizontally for a selfie-view display.
    cv2.imshow('MediaPipe Hands', cv2.flip(image, 1))
    if cv2.waitKey(5) & 0xFF == 27:
        break
cap.release()
s.close()
