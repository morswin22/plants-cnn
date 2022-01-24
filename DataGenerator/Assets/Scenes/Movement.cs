using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Range
{
    public float min;
    public float max;
    public Range(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
    public float Random()
    {
        return UnityEngine.Random.Range(min, max);
    }
}

public class Movement : MonoBehaviour
{
  public GameObject sunObject;
  public GameObject[] cameras;
  Dictionary<string, Range> ranges = new Dictionary<string, Range>();

  void Start()
  {
    sunObject = GameObject.Find("Sun");
    cameras = GameObject.FindGameObjectsWithTag("Camera");

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

    // TODO: add range for camera angle
  }

  void Update()
  {
    var sun = sunObject.GetComponent<Entropedia.Sun>();
    sun.SetTime((int)ranges["hour"].Random(), (int)ranges["minute"].Random());
    sun.SetLocation(ranges["latitude"].Random(), ranges["longitude"].Random());

    var newPosition = new Vector3(ranges["x"].Random(), ranges["y"].Random(), ranges["z"].Random());
    var newLookAt = new Vector3(ranges["x"].Random(), 1.0f, ranges["z"].Random());

    foreach (GameObject camera in cameras)
    {
        camera.transform.position = newPosition;
        camera.transform.LookAt(newLookAt);
    }
  }
}