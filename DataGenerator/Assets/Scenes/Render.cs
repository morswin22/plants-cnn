using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Render : MonoBehaviour
{
    GameObject textureCamera;
    Vector2Int textureCameraSize = new Vector2Int(800,600);
    public int ScreenshotsPerSecond = 6;
    int FrameCounter = 0;

    IEnumerator CaptureAndSaveFrames()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            RenderTexture current = RenderTexture.active;
			
            RenderTexture.active = textureCamera.GetComponent<Camera>().targetTexture;

            textureCamera.GetComponent<Camera>().Render();
			
            Texture2D offscreenTexture = new Texture2D(textureCamera.GetComponent<Camera>().targetTexture.width, textureCamera.GetComponent<Camera>().targetTexture.height, TextureFormat.RGB24, false);
            offscreenTexture.ReadPixels(new Rect(0, 0, textureCamera.GetComponent<Camera>().targetTexture.width, textureCamera.GetComponent<Camera>().targetTexture.height), 0, 0, false);
            offscreenTexture.Apply();
		
            RenderTexture.active = current;
            ++FrameCounter;

            byte[] bytes = offscreenTexture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/../Output/capturedFrame" + FrameCounter.ToString() + ".png", bytes);
		
            UnityEngine.Object.Destroy(offscreenTexture);

            yield return new WaitForSeconds(1.0f / ScreenshotsPerSecond);
        }
    }

    public void StopCapturing()
    {
        StopCoroutine("CaptureAndSaveFrames");
        FrameCounter = 0;
    }

    public void ResumeCapturing()
    {
        StartCoroutine("CaptureAndSaveFrames");
    }

    void Start()
    {
        textureCamera = new GameObject();
        textureCamera.AddComponent<Camera>();
        textureCamera.transform.position = new Vector3(0, 10.0f, 20.0f);
        textureCamera.transform.Rotate(10.0f, 180.0f, 0.0f, Space.Self);
        textureCamera.name = "Texture Camera";
        textureCamera.tag = "Camera";
        Rect rect = new Rect(0, 0, textureCameraSize.x , textureCameraSize.y);
        RenderTexture renderTexture = new RenderTexture(textureCameraSize.x, textureCameraSize.y, 24);
        textureCamera.GetComponent<Camera>().targetTexture = renderTexture;

        StartCoroutine("CaptureAndSaveFrames");
    }

    void Update()
    {
    }
}
