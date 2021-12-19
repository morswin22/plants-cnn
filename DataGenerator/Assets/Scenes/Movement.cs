using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
  public GameObject[] cameras;

  Vector3[][] presetCameraRanges;
  public int selectedPreset = 1;

  Vector3 getRandomPosition()
  {
    selectedPreset = Mathf.Clamp(selectedPreset, 0, presetCameraRanges.Length - 1);
    return new Vector3(Random.Range(presetCameraRanges[selectedPreset][0].x, presetCameraRanges[selectedPreset][1].x),
                       Random.Range(presetCameraRanges[selectedPreset][0].y, presetCameraRanges[selectedPreset][1].y),
                       Random.Range(presetCameraRanges[selectedPreset][0].z, presetCameraRanges[selectedPreset][1].z));
  }

  void FindCameras()
  {
    cameras = GameObject.FindGameObjectsWithTag("Camera");
  }

  void Start()
  {
    FindCameras();

    presetCameraRanges = new Vector3[2][];
    presetCameraRanges[0] = new Vector3[2];
    presetCameraRanges[0][0] = new Vector3(-50.0f,  5.0f, -50.0f);
    presetCameraRanges[0][1] = new Vector3( 50.0f, 10.0f,  50.0f);
    presetCameraRanges[1] = new Vector3[2];
    presetCameraRanges[1][0] = new Vector3(-50.0f, 15.0f,  50.0f);
    presetCameraRanges[1][1] = new Vector3( 50.0f, 25.0f, -50.0f);
  }

  void Update()
  {
    if (cameras.Length != 2)
      FindCameras();

    Vector3 newPosition = getRandomPosition();
    Vector3 newLookAt = getRandomPosition();
    newLookAt.y = 1.0f;

    foreach (GameObject camera in cameras)
    {
      camera.transform.position = newPosition;
      camera.transform.LookAt(newLookAt);
    }
  }
}