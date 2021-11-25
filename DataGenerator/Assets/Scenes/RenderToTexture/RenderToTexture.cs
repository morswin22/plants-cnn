using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RenderToTexture : MonoBehaviour
{
    Vector3 lightDir = new Vector3(10.0f, 10.0f, 10.0f);
    Vector2Int textureCameraSize = new Vector2Int(800,600);
    GameObject mainCamera;
    GameObject textureCamera;
    GameObject directionalLight;
    int ScreenshotsPerSecond = 24;
    int FrameCounter = 0;

    IEnumerator CaptureAndSaveFrames()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();//czeka na koniec renderingu bie��cej ramki

            // Zapami�tanie aktualnie aktywnej tekstury
            RenderTexture currentRT = RenderTexture.active;

            // Ustawienie na aktywn� nowej tekstury (typu RenderTexture) 			
            RenderTexture.active = textureCamera.GetComponent<Camera>().targetTexture;

            // Rendering do nowej tekstury u�ywaj�c nowej kamery
            textureCamera.GetComponent<Camera>().Render();

            // Odczyt nowej tekstury do formatu Texture2D			
            Texture2D offscreenTexture = new Texture2D(textureCamera.GetComponent<Camera>().targetTexture.width, textureCamera.GetComponent<Camera>().targetTexture.height, TextureFormat.RGB24, false);
            offscreenTexture.ReadPixels(new Rect(0, 0, textureCamera.GetComponent<Camera>().targetTexture.width, textureCamera.GetComponent<Camera>().targetTexture.height), 0, 0, false);
            offscreenTexture.Apply();

            // Przywr�cenie poprzednio aktywnej tekstury			
            RenderTexture.active = currentRT;
            ++FrameCounter;

            // Zrzut zapami�tanej tekstury do obrazu PNG
            byte[] bytes = offscreenTexture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/../Output/capturedFrame" + FrameCounter.ToString() + ".png", bytes);

            // Usuni�cie pomocniczej tekstury. 			
            UnityEngine.Object.Destroy(offscreenTexture);

            yield return new WaitForSeconds(1.0f / ScreenshotsPerSecond);
        }
    }

    public void StopCapturing()
    {
        StopCoroutine("CaptureAndSaveFrames");
        FrameCounter = 0;
    }

    /// <summary> 	
    /// Resume image capture. 	
    /// </summary> 	
    public void ResumeCapturing()
    {
        StartCoroutine("CaptureAndSaveFrames");
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = new GameObject();
        mainCamera.AddComponent<Camera>();
        mainCamera.transform.position = new Vector3(0, 10.0f, 20.0f);
        mainCamera.transform.Rotate(10.0f, 180.0f, 0.0f, Space.Self);
        mainCamera.name = "MainCamera";

        textureCamera = new GameObject();
        textureCamera.AddComponent<Camera>();
        textureCamera.transform.position = new Vector3(0, 10.0f, 20.0f);
        textureCamera.transform.Rotate(10.0f, 180.0f, 0.0f, Space.Self);
        textureCamera.name = "TextureCamera";
        //Debug.Log(textureCamera.GetComponent<Camera>().targetTexture);
        Rect rect = new Rect(0, 0, textureCameraSize.x , textureCameraSize.y);
        RenderTexture renderTexture = new RenderTexture(textureCameraSize.x, textureCameraSize.y, 24);
        textureCamera.GetComponent<Camera>().targetTexture = renderTexture;

        GameObject directionalLight = new GameObject();
        directionalLight.AddComponent<Light>();
        directionalLight.name = "Directional Light";
        directionalLight.GetComponent<Light>().type = LightType.Directional;
        directionalLight.GetComponent<Light>().shadows = LightShadows.Soft;
        directionalLight.GetComponent<Light>().color = Color.white;
        directionalLight.GetComponent<Light>().useColorTemperature = true;
        directionalLight.GetComponent<Light>().colorTemperature = 5900;
        directionalLight.transform.position = new Vector3(0, 10.0f, 0.0f);
        directionalLight.transform.Rotate(lightDir.x, lightDir.y, lightDir.z, Space.Self);

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        Color brown = new Color(139f / 255f, 69f / 255f, 19f / 255f, 1f);
        ground.GetComponent<Renderer>().material.color = brown;

        StartCoroutine("CaptureAndSaveFrames");
    }

    // Update is called once per frame
    void Update()
    {
        mainCamera.transform.RotateAround(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f),1);
        textureCamera.transform.RotateAround(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), 1);
    }
}
