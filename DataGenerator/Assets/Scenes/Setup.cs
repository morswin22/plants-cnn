using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject textureCamera;
    public GameObject directionalLight;
    public GameObject ground;
    public Material groundMaterial;

    Vector2Int textureCameraSize = new Vector2Int(800, 600);

    void Start()
    {
        mainCamera = new GameObject();
        mainCamera.AddComponent<Camera>();
        mainCamera.transform.position = new Vector3(0, 10.0f, 20.0f);
        mainCamera.transform.Rotate(10.0f, 180.0f, 0.0f, Space.Self);
        mainCamera.name = "Main Camera";
        mainCamera.tag = "Camera";

        textureCamera = new GameObject();
        textureCamera.AddComponent<Camera>();
        textureCamera.transform.position = new Vector3(0, 10.0f, 20.0f);
        textureCamera.transform.Rotate(10.0f, 180.0f, 0.0f, Space.Self);
        textureCamera.name = "Texture Camera";
        textureCamera.tag = "Camera";
        textureCamera.GetComponent<Camera>().targetTexture =  new RenderTexture(textureCameraSize.x, textureCameraSize.y, 24);

        directionalLight = new GameObject();
        directionalLight.AddComponent<Light>();
        directionalLight.name = "Sun";
        directionalLight.GetComponent<Light>().type = LightType.Directional;
        directionalLight.GetComponent<Light>().shadows = LightShadows.Soft;
        directionalLight.GetComponent<Light>().color = Color.white;
        directionalLight.GetComponent<Light>().useColorTemperature = true;
        directionalLight.GetComponent<Light>().colorTemperature = 5900;
        directionalLight.transform.position = new Vector3(0, 10.0f, 0.0f);
        directionalLight.transform.Rotate(10.0f, 10.0f, 10.0f, Space.Self);
        directionalLight.AddComponent<Entropedia.Sun>();

        groundMaterial = Resources.Load("ForestFloor/ForestFloor") as Material;
        ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.localScale = new Vector3(10.0f, 1.0f, 10.0f);
        ground.name = "Ground";
        ground.GetComponent<MeshRenderer>().material = groundMaterial;
        ground.AddComponent<Forestation>();

    }

    void Update()
    {
    }
}
