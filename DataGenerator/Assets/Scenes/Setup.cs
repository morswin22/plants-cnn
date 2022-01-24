using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    GameObject mainCamera;
    public GameObject directionalLight;
    Vector3 lightDir = new Vector3(10.0f, 10.0f, 10.0f);
    public GameObject ground;
    public Material groundMaterial;

    void Start()
    {
        mainCamera = new GameObject();
        mainCamera.AddComponent<Camera>();
        // mainCamera.AddComponent<Movement>();
        // mainCamera.AddComponent<Render>();
        mainCamera.transform.position = new Vector3(0, 10.0f, 20.0f);
        mainCamera.transform.Rotate(10.0f, 180.0f, 0.0f, Space.Self);
        mainCamera.name = "Main Camera";
        mainCamera.tag = "Camera";

        directionalLight = new GameObject();
        directionalLight.AddComponent<Light>();
        directionalLight.name = "Sun";
        directionalLight.GetComponent<Light>().type = LightType.Directional;
        directionalLight.GetComponent<Light>().shadows = LightShadows.Soft;
        directionalLight.GetComponent<Light>().color = Color.white;
        directionalLight.GetComponent<Light>().useColorTemperature = true;
        directionalLight.GetComponent<Light>().colorTemperature = 5900;
        directionalLight.transform.position = new Vector3(0, 10.0f, 0.0f);
        directionalLight.transform.Rotate(lightDir.x, lightDir.y, lightDir.z, Space.Self);
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
