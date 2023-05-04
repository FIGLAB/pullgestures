import cv2

counter = 0
cap = cv2.VideoCapture(2)
cap.set(cv2.CAP_PROP_FRAME_WIDTH, 1600)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 1200)
while cap.isOpened():
    success, image = cap.read()
    if not success:
        print("Ignoring empty camera frame.")
        continue

    # TOP: image[500:, 288:-288]
    # print(image[:, 150:-150].shape)
    cv2.imshow('crop', cv2.resize(image[:, 150:-150], None, fx=0.5, fy=0.5))
    key = cv2.waitKey(5)
    if key == ord('s'):
        cv2.imwrite("frame" + str(counter) + ".png", image)
        counter += 1
        print("counter: ", counter)
    if key == ord('q'):
        break
