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
z_depths = [0.1526258241623249, 0.12205837126506186, 0.10233207486937718, 0.08807323717451279, 0.07303728178026642, 0.06150556084676804, 0.05517650265243355, 0.04999865818834215, 0.044872238322046544, 0.03856167149333192]
x = [7, 9, 11, 13, 15, 17, 19, 21, 23, 25]
fztop = interp1d(z_depths, x, fill_value="extrapolate")
z_depths = [0.11582572005352186, 0.09265950413045486, 0.08100572795518543, 0.06603013687370445, 0.0565349264208336, 0.05003494598183491, 0.04430843136027134, 0.040908615784036734, 0.03708028171831715, 0.03394205491123189]
x = [3, 5, 7, 9, 11, 13, 15, 17, 19, 21]
fzbot = interp1d(z_depths, x, fill_value="extrapolate")
# X distance measurement
xs = [0.6277118921279907, 0.5994348526000977, 0.5774940848350525, 0.5560979843139648, 0.5163061022758484, 0.4905257225036621, 0.46397367119789124, 0.424101859331131, 0.40318360924720764, 0.37451615929603577, 0.34918156266212463, 0.31031671166419983, 0.26478245854377747]
dists = [1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25]
fxtop = interp1d(xs, dists, fill_value="extrapolate")
xs = [0.7824405431747437, 0.755464494228363, 0.7242898941040039, 0.6889817118644714, 0.6555604338645935, 0.6117358803749084, 0.5615237355232239, 0.5125985145568848, 0.4609793424606323, 0.4154091477394104, 0.36626389622688293, 0.3274620473384857, 0.29657915234565735, 0.26027345657348633]
dists = [0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26]
fxbot = interp1d(xs, dists, fill_value="extrapolate")
# Y distance estimation
ys = [0.27391868829727173, 0.3342598080635071, 0.3886343240737915, 0.451821506023407, 0.5036942958831787, 0.5516012907028198, 0.6277080774307251]
dists = [9, 7, 5, 3, 1, -1, -3]
fytop = interp1d(ys, dists, fill_value="extrapolate")
ys = [0.5770764946937561, 0.624933660030365, 0.6714627838134766, 0.7123582363128662, 0.7515438103675842, 0.7926002740859985, 0.8279182410240173, 0.8587551307678223]
# dists = [5, 3, 1, -1, -3, -5, -7, -9]
dists = [3, 2, 1, -1, -2, -3, -4, -5]
fybot = interp1d(ys, dists, fill_value="extrapolate")

# Unwarping parameters
# top camera
DIM=(1024, 700)
K = np.array([[456.47095121, 0.0, 450.34975221], [0.0, 518.37877053, 65.50468751], [0.0, 0.0, 1.0]])
D = np.array([[0.1507154 ],[0.01015095], [0.01468287], [0.00592813]])
dim1 = [1024, 700]
scaled_K = K * dim1[0] / DIM[0]
scaled_K[2][2] = 1.0
new_K = cv2.fisheye.estimateNewCameraMatrixForUndistortRectify(scaled_K, D, dim1, np.eye(3), balance=1.0)
map1_top, map2_top = cv2.fisheye.initUndistortRectifyMap(scaled_K, D, np.eye(3), new_K, dim1, cv2.CV_16SC2)
# bot camera
DIM=(1300, 1200)
K=np.array([[787.8524863867165, 0.0, 744.7118000552183], [0.0, 829.5071163774452, 561.3314451453386], [0.0, 0.0, 1.0]])
D=np.array([[-0.052595202508066485], [0.03130776521577516], [-0.04104704724832264], [0.015343014605793348]])
dim1 = [1300, 1200]
scaled_K = K * dim1[0] / DIM[0]
scaled_K[2][2] = 1.0
new_K = cv2.fisheye.estimateNewCameraMatrixForUndistortRectify(scaled_K, D, dim1, np.eye(3), balance=1.0)
map1_bot, map2_bot = cv2.fisheye.initUndistortRectifyMap(scaled_K, D, np.eye(3), new_K, dim1, cv2.CV_16SC2)
def undistort(topimg, botimg):
    undistort_top = cv2.remap(topimg, map1_top, map2_top, interpolation=cv2.INTER_LINEAR, borderMode=cv2.BORDER_CONSTANT)
    undistort_bot = cv2.remap(botimg, map1_bot, map2_bot, interpolation=cv2.INTER_LINEAR, borderMode=cv2.BORDER_CONSTANT)
    return undistort_top, undistort_bot

# gets euclidean distance of two points
def get_dist(p1x, p1y, p1z, p2x, p2y, p2z):
    return math.sqrt((p1x-p2x)**2 + (p1y-p2y)**2 + (p1z-p2z)**2)
# def get_dist(p1, p2):
    # return math.sqrt((p1.x-p2.x)**2 + (p1.y-p2.y)**2 + (p1.z-p2.z)**2)

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
                continue
            smoothed = np.average(np.array(smooth_landmark), axis=0)
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

image = np.zeros((1200, 2324, 3), dtype=np.float32)

topcap = cv2.VideoCapture(0)
topcap.set(cv2.CAP_PROP_FRAME_WIDTH, 1600)
topcap.set(cv2.CAP_PROP_FRAME_HEIGHT, 1200)
botcap = cv2.VideoCapture(2)
botcap.set(cv2.CAP_PROP_FRAME_WIDTH, 1600)
botcap.set(cv2.CAP_PROP_FRAME_HEIGHT, 1200)
hands = mp_hands.Hands(max_num_hands=1, min_detection_confidence=0.5, min_tracking_confidence=0.5)
while botcap.isOpened():
    if noclients:
        print('Socket now listening')
        conn,addr=s.accept()
        print('Connected to :', addr[0], ':', addr[1])
        noclients = False
        start_new_thread(threaded, (conn,))

    success, topimage = topcap.read()
    success2, botimage = botcap.read()
    if not success or not success2: continue
    utop, ubot = undistort(topimage[500:, 288:-288], botimage[:, 150:-150])
    image[:, 0:1300, :] = ubot
    image[:700, 1300:, :] = utop
    image = image.astype(np.uint8)

    image.flags.writeable = False
    image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    results = hands.process(image)
    image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

    if results.multi_hand_world_landmarks:
        if results.multi_hand_landmarks[0].landmark[0].x < 0.565: # IF BOT CAM
            for i in range(21):
                landmarks[i] = np.array([results.multi_hand_world_landmarks[0].landmark[i].x,
                    results.multi_hand_world_landmarks[0].landmark[i].y,
                    results.multi_hand_world_landmarks[0].landmark[i].z])

            wrist_x = (results.multi_hand_landmarks[0].landmark[0].x * 2324) / 1300
            dist = get_dist(wrist_x, results.multi_hand_landmarks[0].landmark[0].y, results.multi_hand_landmarks[0].landmark[0].z, (results.multi_hand_landmarks[0].landmark[1].x*2324)/1300, results.multi_hand_landmarks[0].landmark[1].y, results.multi_hand_landmarks[0].landmark[1].z)
            zdepth = fzbot(dist)/100
            xdepth = (fxbot(wrist_x)/100-0.12)*zdepth/0.075
            ydepth = fybot(results.multi_hand_landmarks[0].landmark[0].y)/100*zdepth/0.075
            dist = get_dist(wrist_x, results.multi_hand_landmarks[0].landmark[0].y, results.multi_hand_landmarks[0].landmark[0].z, (results.multi_hand_landmarks[0].landmark[1].x*2324)/1300, results.multi_hand_landmarks[0].landmark[1].y, results.multi_hand_landmarks[0].landmark[1].z)
            landmarks[:, 0] += -xdepth
            landmarks[:, 1] += -ydepth + 0.08
            landmarks[:, 2] += zdepth
        else: # IF TOP CAM
            for i in range(21):
                landmarks[i] = np.array([results.multi_hand_world_landmarks[0].landmark[i].x,
                    results.multi_hand_world_landmarks[0].landmark[i].z,
                    results.multi_hand_world_landmarks[0].landmark[i].y])

            wrist_x = (results.multi_hand_landmarks[0].landmark[0].x * 2324 - 1300) / 1024
            wrist_y = (results.multi_hand_landmarks[0].landmark[0].y * 1200) / 700
            dist = get_dist(wrist_x, wrist_y, results.multi_hand_landmarks[0].landmark[0].z, (results.multi_hand_landmarks[0].landmark[1].x*2324-1300)/1024, (results.multi_hand_landmarks[0].landmark[1].y*1200)/700, results.multi_hand_landmarks[0].landmark[1].z)
            # dist = get_dist(results.multi_hand_landmarks[0].landmark[0], results.multi_hand_landmarks[0].landmark[1])
            ydepth = fztop(dist)/100
            xdepth = (fxtop(wrist_x)/100-0.12)*ydepth/0.12
            zdepth = fytop(wrist_y)/100*ydepth/0.12
            landmarks[:, 0] += xdepth
            landmarks[:, 1] += ydepth + 0.046
            landmarks[:, 2] += zdepth + 0.071

        if get_dist_2d(results.multi_hand_landmarks[0].landmark[4], results.multi_hand_landmarks[0].landmark[8]) < pinchThresh: isPinched = True
        else:   isPinched = False
        image.flags.writeable = True
        for hand_landmarks in results.multi_hand_landmarks:
            mp_drawing.draw_landmarks(
                image,
                hand_landmarks,
                mp_hands.HAND_CONNECTIONS)
        smooth_landmark.append(landmarks.copy())
        if len(smooth_landmark) > 5: smooth_landmark = smooth_landmark[1:]
    else:
        landmarks = np.zeros((21, 3), dtype=np.float32)

    # Flip the image horizontally for a selfie-view display.
    cv2.imshow('MediaPipe Hands', cv2.resize(image, None, fx=0.5, fy=0.5))
    if cv2.waitKey(5) & 0xFF == 27:
        break
topcap.release()
botcap.release()
s.close()
