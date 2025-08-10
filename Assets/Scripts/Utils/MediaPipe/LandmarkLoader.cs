using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class LandmarkLoader : MonoBehaviour
{
    public void LoadLandmarksFromJson(string videoFileName)
    {
        string jsonFileName = $"{Path.GetFileNameWithoutExtension(videoFileName)}_landmarks.json";
        string jsonFilePath = Path.Combine(Application.persistentDataPath, jsonFileName);
        string jsonText = File.ReadAllText(jsonFilePath);
       

        if (jsonText == null)
        {
            Debug.LogError($"JSON file named '{jsonFileName}' was not found in the Resources folder.");
            return;
        }

        Debug.Log($"JSON file '{jsonFileName}.json' was found and loaded successfully.");

        // אם תרצה לשלב את ההמרה בעתיד:
        //MyLandmarkData data = JsonConvert.DeserializeObject<MyLandmarkData>(jsonText.text);
        //Debug.Log(" JSON parsed successfully. Ready to use the landmark data.");
    }
}
