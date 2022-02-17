using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public double cubeEdge = 7d;
    List<GameObject> cameras;
    Camera textureCameraComponent;
    Forestation forestation;
    List<GameObject> trees;
    System.Random rnd = new System.Random();

    #region helpers
    void SetCameras(Vector3 position, Vector3 lookAt)
    {
        cameras.ForEach(x =>
            {
                x.transform.position = position;
                x.transform.LookAt(lookAt);
            });
    }
    double GetRandomDoubleInRage(double minimum, double maximum)
    {
        return rnd.NextDouble() * (maximum - minimum) + minimum;
    }


    GameObject GetRandomTree()
    {
        return trees[rnd.Next(trees.Count)];
    }

    Vector3 GetCameraLocationOnCube(double edge, Vector3 treePosition, double treeHeight)
    {
        var halfOfEdge = edge;
        return new Vector3(
            (float)GetRandomDoubleInRage(-halfOfEdge, halfOfEdge) + treePosition.x,
            (float)GetRandomDoubleInRage(treeHeight * halfOfEdge, halfOfEdge) + treePosition.y,
            (float)GetRandomDoubleInRage(-halfOfEdge, halfOfEdge) + treePosition.z
        );
    }
    #endregion


    void FocusCameraOnRandomTree()
    {
        var tree = GetRandomTree();
        var treePosition = tree.transform.position;
        var treeHeight = tree.transform.lossyScale.y / 2;
        var cameraLocation = GetCameraLocationOnCube(cubeEdge, treePosition, treeHeight);
        treePosition.y += treeHeight;
        SetCameras(cameraLocation, treePosition);
    }

    void Start()
    {
        cameras = GameObject.FindGameObjectsWithTag("Camera").ToList();
        textureCameraComponent = GameObject.Find("Texture Camera").GetComponent<Camera>();
        forestation = (Forestation)GameObject.Find("Ground").GetComponent("Forestation");
        trees = forestation.placedTrees;
    }

    // Update is called once per frame
    void Update()
    {
        FocusCameraOnRandomTree();
    }
}
