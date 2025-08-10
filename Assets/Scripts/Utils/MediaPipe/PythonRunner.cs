using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class PythonRunner : MonoBehaviour
{
    [SerializeField] private LandmarkLoader landmarkLoader; 

    public IEnumerator RunLandmarkExtraction(string videoPath)
    {

        string scriptPath = Path.Combine(Application.dataPath, "../Tools/MediaPipeExtractor/landmark_extractor.py");
        string outputJsonPath = Path.Combine(Application.persistentDataPath, $"{Path.GetFileNameWithoutExtension(videoPath)}_landmarks.json");

        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python";
        start.Arguments = $"\"{scriptPath}\" \"{videoPath}\" \"{outputJsonPath}\"";
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        start.CreateNoWindow = true;

        UnityEngine.Debug.Log("Starting Python process...");

        using (Process process = new Process())
        {
            process.StartInfo = start;

            process.OutputDataReceived += (sender, args) => {
                if (!string.IsNullOrEmpty(args.Data))
                    UnityEngine.Debug.Log("[PYTHON OUT] " + args.Data);
            };
            process.ErrorDataReceived += (sender, args) => {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    if (args.Data.StartsWith("WARNING"))
                    {
                        UnityEngine.Debug.LogWarning("[PYTHON WARNING] " + args.Data);
                    }
                    else if (args.Data.StartsWith("INFO") || args.Data.Contains("tqdm"))
                    {
                        UnityEngine.Debug.Log("[PYTHON LOG] " + args.Data);
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("[PYTHON ERR] " + args.Data);
                    }
                }
            };


            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            while (!process.HasExited)
            {
                yield return null;
            }

            UnityEngine.Debug.Log("Python process exited.");

            if (File.Exists(outputJsonPath))
            {
                UnityEngine.Debug.Log($"Output JSON created successfully:\n{outputJsonPath}");

                HandlePythonOutput(videoPath);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Output JSON not found at:\n{outputJsonPath}");
            }
        }

        yield return null;
    }

    private void HandlePythonOutput(string videoPath)
    {
        string videoFileName = Path.GetFileName(videoPath);
        landmarkLoader.LoadLandmarksFromJson(videoFileName);
    }
}
