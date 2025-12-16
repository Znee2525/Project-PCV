import numpy as np
import mediapipe as mp

PoseLandmark = mp.solutions.pose.PoseLandmark

def vec(a, b):
    return np.array([b.x - a.x, b.y - a.y, b.z - a.z])

def normalize(v):
    n = np.linalg.norm(v)
    if n < 1e-6:
        return np.zeros(3)
    return v / n

def mp_to_unity(v):
    # MediaPipe → Unity
    return np.array([
        -v[0],   # mirror X
         v[1],
        -v[2]    # depth fix
    ])

def extract_arm_vectors(landmarks):
    ls = landmarks[PoseLandmark.LEFT_SHOULDER]
    le = landmarks[PoseLandmark.LEFT_ELBOW]
    lw = landmarks[PoseLandmark.LEFT_WRIST]

    rs = landmarks[PoseLandmark.RIGHT_SHOULDER]
    re = landmarks[PoseLandmark.RIGHT_ELBOW]
    rw = landmarks[PoseLandmark.RIGHT_WRIST]

    left_upper = mp_to_unity(normalize(vec(ls, le)))
    left_lower = mp_to_unity(normalize(vec(le, lw)))

    right_upper = mp_to_unity(normalize(vec(rs, re)))
    right_lower = mp_to_unity(normalize(vec(re, rw)))

    return {
        "left_upper_arm": left_upper.tolist(),
        "left_lower_arm": left_lower.tolist(),
        "right_upper_arm": right_upper.tolist(),
        "right_lower_arm": right_lower.tolist()
    }

def extract_upper_body(landmarks):
    ls = landmarks[PoseLandmark.LEFT_SHOULDER]
    rs = landmarks[PoseLandmark.RIGHT_SHOULDER]
    lh = landmarks[PoseLandmark.LEFT_HIP]
    rh = landmarks[PoseLandmark.RIGHT_HIP]
    nose = landmarks[PoseLandmark.NOSE]

    shoulder = np.array([(ls.x + rs.x)/2, (ls.y + rs.y)/2, (ls.z + rs.z)/2])
    hip = np.array([(lh.x + rh.x)/2, (lh.y + rh.y)/2, (lh.z + rh.z)/2])

    spine = mp_to_unity(normalize(shoulder - hip))
    head = mp_to_unity(normalize(np.array([nose.x, nose.y, nose.z]) - shoulder))

    return {
        "spine": spine.tolist(),
        "hips": [0,1,0],
        "head": head.tolist()
    }
