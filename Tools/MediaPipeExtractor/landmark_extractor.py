import sys
import cv2
import mediapipe as mp
import json
from tqdm import tqdm

def landmark_list_to_dict(landmarks):
    return [
        {
            "x": lm.x,
            "y": lm.y,
            "z": lm.z,
            "visibility": getattr(lm, "visibility", 1.0)  
        } for lm in landmarks.landmark
    ]

def extract_holistic_landmarks(video_path, output_json_path):
    mp_holistic = mp.solutions.holistic
    results_data = {}

    cap = cv2.VideoCapture(video_path)
    if not cap.isOpened():
        raise IOError(f"Cannot open video: {video_path}")

    total_frames = int(cap.get(cv2.CAP_PROP_FRAME_COUNT))

    with mp_holistic.Holistic(
        static_image_mode=False,
        model_complexity=1,
        smooth_landmarks=True,
        enable_segmentation=False,
        refine_face_landmarks=True,
        min_detection_confidence=0.5,
        min_tracking_confidence=0.5
    ) as holistic:

       for frame_idx in tqdm(range(total_frames), desc="Processing frames", file=sys.stdout):
    
            ret, frame = cap.read()
            if not ret:
                break

            image_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            results = holistic.process(image_rgb)

            frame_data = {}

            if results.pose_landmarks:
                frame_data["pose"] = landmark_list_to_dict(results.pose_landmarks)

            if results.pose_world_landmarks:
                frame_data["poseWorld"] = landmark_list_to_dict(results.pose_world_landmarks)

            if results.face_landmarks:
                frame_data["face"] = landmark_list_to_dict(results.face_landmarks)

            if results.left_hand_landmarks:
                frame_data["leftHand"] = landmark_list_to_dict(results.left_hand_landmarks)

            if results.right_hand_landmarks:
                frame_data["rightHand"] = landmark_list_to_dict(results.right_hand_landmarks)


            if frame_data:
                results_data[f"frame_{frame_idx}"] = frame_data

    cap.release()
    with open(output_json_path, "w") as f:
        json.dump(results_data, f, indent=2)

    print(f"JSON file saved successfully at {output_json_path}")

if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("Usage: python extract_landmarks.py <video_path> <output_json_path>")
        sys.exit(1)
    video_path = sys.argv[1]
    output_json_path = sys.argv[2]

    extract_holistic_landmarks(video_path, output_json_path)
