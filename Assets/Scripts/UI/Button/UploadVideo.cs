using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using System.IO;

public class UploadVideo : MonoBehaviour
{
    [SerializeField] private Button uploadButton;

    [SerializeField] private PythonRunner pythonRunner;

    void Start()
    {
        if (uploadButton != null)
        {
            uploadButton.onClick.AddListener(OpenFileExplorer);
        }
        else
        {
            Debug.LogError("UploadButton is not assigned in the Inspector!");
        }

        if (pythonRunner == null)
        {
            Debug.LogError("PythonRunner is not assigned in the Inspector!");
        }

    }

    public void OpenFileExplorer()
    {
        var extensions = new[] {
            new ExtensionFilter("Video Files", "mp4", "mov", "avi", "mkv")
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Select a Video", "", extensions, false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            Debug.Log("Selected video: " + paths[0]);
            StartCoroutine(RunExtractionWithLog(paths[0]));
        }
    }

    private IEnumerator RunExtractionWithLog(string path)
    {
        Debug.Log("Starting landmark extraction...");
        yield return pythonRunner.RunLandmarkExtraction(path);
        Debug.Log("Landmark extraction coroutine finished.");
    }
}
