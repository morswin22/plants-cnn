using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PoissonDiskSampling;

public class Forestation : MonoBehaviour
{
    GameObject sp;
    GameObject ground;
    List<Tree> trees = new List<Tree>();
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
            posGround.x + groundScale.x * groundSize.x / 8,
            posGround.y + groundScale.y * groundSize.y / 8, // 0
            posGround.z + groundScale.z * groundSize.z / 8);

        minimumCoordinates = new Vector3(
            posGround.x - groundScale.x * groundSize.x / 8,
            posGround.y - groundScale.y * groundSize.y / 8, // 0
            posGround.z - groundScale.z * groundSize.z / 8);

        // I've assumed that the Ground object isn't rotated
        // In case of rotating it, we'll be forced to do some fancy maths
        // Correct coordinates are (x1, y1, z1) where
        // (minimumCoordinates.x < x1 < maximumCoordinates.y) etc.
        // Also name of this method assumes that Earth is flat ;)
    }

    void LoadSingleTree()
    {
        // TODO: think how to select .obj files 
        var path = Application.dataPath + "/Resources/ForestObjects";
        var tree = new Tree(path, "EU43_5");
        tree.prefab.name = $"Tree_{trees.Count + 1}";

        tree.prefab.transform.position = new Vector3(
            Random.Range(minimumCoordinates.x + minimumDistanceFromTheEdge, maximumCoordinates.x - minimumDistanceFromTheEdge),
            Random.Range(minimumCoordinates.y, maximumCoordinates.y) + 0.5f,
            Random.Range(minimumCoordinates.z + minimumDistanceFromTheEdge, maximumCoordinates.z - minimumDistanceFromTheEdge));

        // .tif extension is a problem - to import properly with textures,
        // I've converted every .tif file to .png and I've edited .mtl file
        // (replaced every occurrence of '.tif' with '.png')
    }

    void LoadTrees(string path, List<string> names)
    {
        // Load all trees from the names list
        foreach (var name in names)
        {
            var tree = new Tree(path, name);
            tree.prefab.transform.position = new Vector3(0.0f, -tree.size.y, 0.0f);
            tree.prefab.transform.localScale = new Vector3(
                tree.prefab.transform.localScale.x / 8,
                tree.prefab.transform.localScale.y / 8,
                tree.prefab.transform.localScale.z / 8
            );
            trees.Add(tree);
        }
    }

    void PlaceTreesRandom(int count)
    {
        // Place trees randomly
        for (int i = 0; i < count; i++)
        {
            var tree = Instantiate(trees[Random.Range(0, trees.Count)].prefab);
            tree.transform.position = new Vector3(
                Random.Range(minimumCoordinates.x + minimumDistanceFromTheEdge, maximumCoordinates.x - minimumDistanceFromTheEdge),
                Random.Range(minimumCoordinates.y, maximumCoordinates.y) + 0.5f,
                Random.Range(minimumCoordinates.z + minimumDistanceFromTheEdge, maximumCoordinates.z - minimumDistanceFromTheEdge));
        }
    }

    void PlaceTreesGrid(float gap, float maxRandomOffset)
    {
        // Constrain parameters
        if (gap < 0.0f)
            gap = 0.0f;
        if (maxRandomOffset < 0.0f)
            maxRandomOffset = 0.0f;
        // Find max size of trees from Tree.size
        Vector3 maxSize = new Vector3();
        foreach (var tree in trees)
        {
            if (tree.size.x > maxSize.x)
                maxSize.x = tree.size.x / 8;
            if (tree.size.z > maxSize.z)
                maxSize.z = tree.size.z / 8;
        }
        // Calculate step and offset
        float xStep = maxSize.x + gap;
        float zStep = maxSize.z + gap;
        float xOffset = (((maximumCoordinates.x - minimumCoordinates.x - 2 * minimumDistanceFromTheEdge) / xStep) % 1) * xStep * 0.5f;
        float zOffset = (((maximumCoordinates.z - minimumCoordinates.z - 2 * minimumDistanceFromTheEdge) / zStep) % 1) * zStep * 0.5f;
        // Place trees in a grid
        for (float x = minimumCoordinates.x + minimumDistanceFromTheEdge + xOffset; x <= maximumCoordinates.x - minimumDistanceFromTheEdge; x += xStep)
        {
            for (float z = minimumCoordinates.z + minimumDistanceFromTheEdge + zOffset; z <= maximumCoordinates.z - minimumDistanceFromTheEdge; z += zStep)
            {
                var tree = Instantiate(trees[Random.Range(0, trees.Count)].prefab);
                tree.transform.position = new Vector3(
                    x + Random.Range(-maxRandomOffset, maxRandomOffset),
                    Random.Range(minimumCoordinates.y, maximumCoordinates.y) + 0.5f,
                    z + Random.Range(-maxRandomOffset, maxRandomOffset));
            }
        }
    }

    void PlaceTreesPoisson(float gap, int numSamplesBeforeRejection)
    {
        // Find max size of trees from Tree.size
        Vector3 maxSize = new Vector3();
        foreach (var tree in trees)
        {
            if (tree.size.x > maxSize.x)
                maxSize.x = tree.size.x;
            if (tree.size.z > maxSize.z)
                maxSize.z = tree.size.z;
        }
        // Calculate radius and generate points using Poisson disk sampling
        float radius = (maxSize.magnitude + gap) * 0.5f;
        List<Vector2> points = PoissonDiskSampling.GeneratePoints(
            radius,
            new Vector2(
                maximumCoordinates.x - minimumCoordinates.x - 2 * minimumDistanceFromTheEdge,
                maximumCoordinates.z - minimumCoordinates.z - 2 * minimumDistanceFromTheEdge),
            numSamplesBeforeRejection);
        // Place trees at the generated points
        foreach (var point in points)
        {
            var tree = Instantiate(trees[Random.Range(0, trees.Count)].prefab);
            tree.transform.position = new Vector3(
                minimumCoordinates.x + minimumDistanceFromTheEdge + point.x,
                Random.Range(minimumCoordinates.y, maximumCoordinates.y) + 0.5f,
                minimumCoordinates.z + minimumDistanceFromTheEdge + point.y);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GetEdgesOfTheWorld();
        // LoadSingleTree();
        LoadTrees(Application.dataPath + "/Resources/ForestObjects", new List<string> { "EU43_5" });
        // PlaceTreesRandom(10);
        PlaceTreesGrid(2f, 0.15f);
        // PlaceTreesPoisson(25.0f, 5);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
