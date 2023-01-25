using UnityEngine;
using UnityEditor;
using System.Collections;

public class ScreenshotScript : EditorWindow
{
    
    
    
    [MenuItem("Window/Screenshot")]
    public static void ShowWindow()
    {
        GetWindow<ScreenshotScript>("Screenshot");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Take Screenshot"))
        {
            TakeScreenshot();
        }
    }

    private void TakeScreenshot()
    {
        // Get the main camera
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("No main camera found");
            return;
        }
        // Create a RenderTexture to render the camera's view to
        RenderTexture tempRT = new RenderTexture(Screen.width, Screen.height, 24);
        mainCam.targetTexture = tempRT;
        mainCam.Render();
        RenderTexture.active = tempRT;

        // Create a new Texture2D to copy the rendered data to
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();

        // Reset the camera's target texture
        mainCam.targetTexture = null;
        RenderTexture.active = null;
        // Create a file name for the screenshot
       

        // Encode the texture as a PNG file
        byte[] bytes = screenShot.EncodeToPNG();

        // Save the screenshot to the "Assets/Screenshots" folder
#if UNITY_EDITOR
        string destination = "Assets/Icons/Inventory/Weapons/" + "Axe.png";
        System.IO.File.WriteAllBytes(destination, bytes);
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}