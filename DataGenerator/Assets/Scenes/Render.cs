using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Render : MonoBehaviour
{
    GameObject sun;
    GameObject[] cameras;
    Camera textureCameraComponent;
    Entropedia.Sun sunComponent;
    public int screenshotsPerSecond = 3;
    int frameCounter = 0;
    Dictionary<string, Range> ranges = new Dictionary<string, Range>();

    void Start()
    {
        sun = GameObject.Find("Sun");
        sunComponent = sun.GetComponent<Entropedia.Sun>();

        cameras = GameObject.FindGameObjectsWithTag("Camera");
        textureCameraComponent = GameObject.Find("Texture Camera").GetComponent<Camera>();

        // time
        ranges["hour"] = new Range(0.0f, 24.0f);
        ranges["minute"] = new Range(0.0f, 60.0f);

        // location
        ranges["latitude"] = new Range(-90.0f, 90.0f);
        ranges["longitude"] = new Range(-180.0f, 180.0f);

        // camera
        ranges["x"] = new Range(-50.0f, 50.0f);
        ranges["y"] = new Range( 15.0f, 25.0f);
        ranges["z"] = new Range(-50.0f, 50.0f);

        // TODO: Add camera angle range

        StartCoroutine("Capture");
    }

    void Randomize()
    {
        // Update sun
        sunComponent.SetTime((int)ranges["hour"].Random(), (int)ranges["minute"].Random());
        sunComponent.SetLocation(ranges["latitude"].Random(), ranges["longitude"].Random());

        // Generate random camera position
        var newPosition = new Vector3(ranges["x"].Random(), ranges["y"].Random(), ranges["z"].Random());
        var newLookAt = new Vector3(ranges["x"].Random(), 1.0f, ranges["z"].Random());

        // Update cameras
        foreach (GameObject camera in cameras)
        {
            camera.transform.position = newPosition;
            camera.transform.LookAt(newLookAt);
        }
    }

    void RenderToFile()
    {
        // Switch to texture camera
        var current = RenderTexture.active;
        RenderTexture.active = textureCameraComponent.targetTexture;
        textureCameraComponent.Render();
        
        // Create screenshot
        var offscreenTexture = new Texture2D(textureCameraComponent.targetTexture.width, textureCameraComponent.targetTexture.height, TextureFormat.RGB24, false);
        offscreenTexture.ReadPixels(new Rect(0, 0, textureCameraComponent.targetTexture.width, textureCameraComponent.targetTexture.height), 0, 0, false);
        offscreenTexture.Apply();
    
        // Switch to main camera
        RenderTexture.active = current;
        ++frameCounter;

        // Save screenshot
        byte[] bytes = offscreenTexture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../../Output/capturedFrame" + frameCounter.ToString() + ".png", bytes);
    
        // Clean up
        UnityEngine.Object.Destroy(offscreenTexture);
    }

    IEnumerator Capture()
    {
        while (true)
        {
            // Randomize scene
            Randomize();

            // Render to file
            RenderToFile();

            // Wait for next frame or more
            if (screenshotsPerSecond > 0)
                yield return new WaitForSeconds(1.0f / screenshotsPerSecond);
            else
                yield return new WaitForEndOfFrame();
        }
    }

    public void StopCapturing()
    {
        StopCoroutine("Capture");
        frameCounter = 0;
    }

    public void ResumeCapturing()
    {
        StartCoroutine("Capture");
    }

    void Update()
    {
    }
}
