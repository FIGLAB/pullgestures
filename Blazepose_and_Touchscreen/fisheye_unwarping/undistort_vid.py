import numpy as np
import sys
import cv2
# You should replace these 3 lines with the output in calibration step
# DIM=(640, 480)
# K=np.array([[166.61079119072195, 0.0, 312.9184096351807], [0.0, 166.36889686101503, 220.81540593092757], [0.0, 0.0, 1.0]])
# D=np.array([[0.17835573435561652], [-0.11728804523084109], [0.01697694605881821], [-0.000615972255290476]])
# h, w = 640, 480
# DIM=(1024, 700)
# oldK=np.array([[467.97095120791715, 0.0, 454.8497522081019], [0.0, 468.3787705262718, 41.00468750861216], [0.0, 0.0, 1.0]])
# oldD=np.array([[0.05071540349267734], [-0.009849050475532521], [-0.015317127429080844], [0.005928125411691629]])

DIM=(1300, 1200)
# K=np.array([[466.75752327548236, 0.0, 648.0633979665595], [0.0, 465.18867705922446, 538.8849849629175], [0.0, 0.0, 1.0]])
# D=np.array([[0.07396562341304336], [-0.07532960259252966], [0.05226379649097869], [-0.017783877763008164]])
# oldK = np.array([[517.97095121, 0.0, 504.84975221], [0.0, 418.37877053, -8.99531249], [0.0, 0.0, 1.0]])
# oldD = np.array([[ 0.7307154 ], [-0.20984905], [-0.10531713], [ 0.22592813]])
oldK=np.array([[787.8524863867165, 0.0, 744.7118000552183], [0.0, 829.5071163774452, 561.3314451453386], [0.0, 0.0, 1.0]])
oldD=np.array([[-0.052595202508066485], [0.03130776521577516], [-0.04104704724832264], [0.015343014605793348]])

bal = 10.0
K00, K02, K11, K12, = oldK[0][0], oldK[0][2], oldK[1][1], oldK[1][2]
D0, D1, D2, D3 = oldD[0][0], oldD[1][0], oldD[2][0], oldD[3][0]
def dbal(val):
    global bal
    bal = 0.0 + val/10
def dK00(val):
    global K00
    K00 = oldK[0][0] - 0.5*100 + val*0.5
def dK02(val):
    global K02
    K02 = oldK[0][2] - 0.5*100 + val*0.5
def dK11(val):
    global K11
    K11 = oldK[1][1] - 0.5*100 + val*0.5
def dK12(val):
    global K12
    K12 = oldK[1][2] - 0.5*100 + val*0.5
def dD0(val):
    global D0
    D0 = oldD[0][0] - 0.01*100 + val*0.01
def dD1(val):
    global D1
    D1 = oldD[1][0] - 0.01*100 + val*0.01
def dD2(val):
    global D2
    D2 = oldD[2][0] - 0.01*100 + val*0.01
def dD3(val):
    global D3
    D3 = oldD[3][0] - 0.01*100 + val*0.01

cv2.namedWindow("image")
# cv2.createTrackbar("balance", "image", 0, 10, dbal)
cv2.createTrackbar("K00", "image", 0, 200, dK00)
cv2.createTrackbar("K02", "image", 0, 200, dK02)
cv2.createTrackbar("K11", "image", 0, 200, dK11)
cv2.createTrackbar("K12", "image", 0, 200, dK12)
cv2.createTrackbar("D0", "image", 0, 200, dD0)
cv2.createTrackbar("D1", "image", 0, 200, dD1)
cv2.createTrackbar("D2", "image", 0, 200, dD2)
cv2.createTrackbar("D3", "image", 0, 200, dD3)

def undistort_basic(img):
    h,w = img.shape[:2]
    map1, map2 = cv2.fisheye.initUndistortRectifyMap(K, D, np.eye(3), K, DIM, cv2.CV_16SC2)
    undistorted_img = cv2.remap(img, map1, map2, interpolation=cv2.INTER_LINEAR, borderMode=cv2.BORDER_CONSTANT)
    return undistorted_img

def undistort(img, balance=0.0, dim2=None, dim3=None):
    dim1 = img.shape[:2][::-1]  #dim1 is the dimension of input image to un-distort
    assert dim1[0]/dim1[1] == DIM[0]/DIM[1], "Image to undistort needs to have same aspect ratio as the ones used in calibration"
    if not dim2:
        dim2 = dim1
    if not dim3:
        dim3 = dim1
    scaled_K = K * dim1[0] / DIM[0]  # The values of K is to scale with image dimension.
    scaled_K[2][2] = 1.0  # Except that K[2][2] is always 1.0
    # This is how scaled_K, dim2 and balance are used to determine the final K used to un-distort image. OpenCV document failed to make this clear!
    new_K = cv2.fisheye.estimateNewCameraMatrixForUndistortRectify(scaled_K, D, dim2, np.eye(3), balance=balance)
    map1, map2 = cv2.fisheye.initUndistortRectifyMap(scaled_K, D, np.eye(3), new_K, dim3, cv2.CV_16SC2)
    undistorted_img = cv2.remap(img, map1, map2, interpolation=cv2.INTER_LINEAR, borderMode=cv2.BORDER_CONSTANT)
    return undistorted_img

counter = 0
cap = cv2.VideoCapture(2)
cap.set(cv2.CAP_PROP_FRAME_WIDTH, 1600)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 1200)
while cap.isOpened():
    success, image = cap.read()
    # image = image[500:, 288:-288]
    image = image[:, 150:-150]
    if not success:
        print("Ignoring empty camera frame.")
        continue

    K=np.array([[K00, 0.0, K02], [0.0, K11, K12], [0.0, 0.0, 1.0]])
    D=np.array([[D0], [D1], [D2], [D3]])
    # image = undistort_basic(image)
    image = undistort(image, bal, dim2=None, dim3=None)
    # Flip the image horizontally for a selfie-view display.
    cv2.imshow('image', cv2.resize(image, None, fx=0.6, fy=0.6))
    key = cv2.waitKey(5)
    if key == ord('q'):
        break
    if key == ord('p'):
        print("K=np.array(" + str(K.tolist()) + ")")
        print("D=np.array(" + str(D.tolist()) + ")")
