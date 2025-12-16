import cv2
from utils.pose_detector import PoseDetector
from utils.udp_sender import UDPSender
from utils.mapper import extract_arm_vectors, extract_upper_body


def main():
    cap = cv2.VideoCapture(0)
    detector = PoseDetector()
    sender = UDPSender()

    while True:
        ret, frame = cap.read()
        if not ret:
            break

        frame = cv2.flip(frame, 1)

        results = detector.detect(frame)

        if results.pose_landmarks:
            landmarks = results.pose_landmarks.landmark

            arms = extract_arm_vectors(landmarks)
            upper = extract_upper_body(landmarks)

            data = {**arms, **upper}
            sender.send(data)

        cv2.imshow("Body Tracking", frame)
        if cv2.waitKey(1) & 0xFF == 27:
            break

    cap.release()
    cv2.destroyAllWindows()


if __name__ == "__main__":
    main()
