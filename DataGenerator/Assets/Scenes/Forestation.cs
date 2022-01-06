using Dummiesman;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forestation : MonoBehaviour
{
    GameObject sp;
    GameObject ground;
    List<GameObject> trees = new List<GameObject>();
    Vector3 maximumCoordinates;
    Vector3 minimumCoordinates;
    public float minimumDistanceFromTheEdge = 5;

    void GetEdgesOfTheWorld()
    {
        var ground = GameObject.Find("Ground");
        var groundScale = ground.transform.localScale;
        var groundSize = ground.GetComponent<MeshFilter>().mesh.bounds.size;
        var posGround = ground.transform.position;

        // vector with maximum coordinates  
        maximumCoordinates = new Vector3(
            posGround.x + groundScale.x * groundSize.x / 2,
            posGround.y + groundScale.y * groundSize.y / 2, // 0
            posGround.z + groundScale.z * groundSize.z / 2);

        minimumCoordinates = new Vector3(
            posGround.x - groundScale.x * groundSize.x / 2,
            posGround.y - groundScale.y * groundSize.y / 2, // 0
            posGround.z - groundScale.z * groundSize.z / 2);

        // I've assumed that the Ground object isn't rotated
        // In case of rotating it, we'll be forced to do some fancy maths
        // Correct coordinates are (x1, y1, z1) where
        // (minimumCoordinates.x < x1 < maximumCoordinates.y) etc.
        // Also name of this method assumes that Earth is flat ;)
    }

    void LoadSingleTree()
    {
        // TODO: think how to select .obj files 
        var path = Application.dataPath + "/Resources/ForestObjects/EU43_5";
        var tree = new OBJLoader().Load($"{path}.obj", $"{path}.mtl");
        tree.name = $"tree_{trees.Count + 1}";

        tree.transform.position = new Vector3(
            Random.Range(minimumCoordinates.x + minimumDistanceFromTheEdge, maximumCoordinates.x - minimumDistanceFromTheEdge),
            Random.Range(minimumCoordinates.y, maximumCoordinates.y) + 0.5f,
            Random.Range(minimumCoordinates.z + minimumDistanceFromTheEdge, maximumCoordinates.z - minimumDistanceFromTheEdge));

        tree.transform.Rotate(-90f, 0f, 0f, Space.Self);
        trees.Add(tree);

        // .tif extension is a problem - to import properly with textures,
        // I've converted every .tif file to .png and I've edited .mtl file
        // (replaced every occurrence of '.tif' with '.png')
    }

    // Start is called before the first frame update
    void Start()
    {
        GetEdgesOfTheWorld();
        LoadSingleTree();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
