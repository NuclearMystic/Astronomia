using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

[InitializeOnLoad]
public class AutoSaveEditor
{
    // Time interval between autosaves in seconds (300 seconds = 5 minutes)
    private static readonly double saveInterval = 300.0;
    private static double nextSaveTime;

    static AutoSaveEditor()
    {
        // Initialize the next save time
        nextSaveTime = EditorApplication.timeSinceStartup + saveInterval;

        // Subscribe to the Editor's update event
        EditorApplication.update += Update;
    }

    static void Update()
    {
        // Check if it's time to save
        if (EditorApplication.timeSinceStartup >= nextSaveTime)
        {
            SaveScene();
            // Schedule the next save
            nextSaveTime = EditorApplication.timeSinceStartup + saveInterval;
        }
    }

    static void SaveScene()
    {
        string autosaveFolderPath = "Assets/autosaves";

        // Create the autosaves folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(autosaveFolderPath))
        {
            AssetDatabase.CreateFolder("Assets", "autosaves");
            AssetDatabase.Refresh();
        }

        Scene currentScene = EditorSceneManager.GetActiveScene();
        string scenePath = currentScene.path;

        if (string.IsNullOrEmpty(scenePath))
        {
            Debug.LogWarning("AutoSave: No scene is currently open to save.");
            return;
        }

        string sceneName = Path.GetFileNameWithoutExtension(scenePath);
        string autosaveScenePath = $"{autosaveFolderPath}/{sceneName}_AutoSave.unity";

        // Save the scene as a copy to the autosaves folder
        bool saveSuccessful = EditorSceneManager.SaveScene(currentScene, autosaveScenePath, true);

        if (saveSuccessful)
        {
            Debug.Log($"[AutoSave] Scene saved to {autosaveScenePath}");
        }
        else
        {
            Debug.LogWarning("[AutoSave] Failed to save the scene.");
        }
    }
}


