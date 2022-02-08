using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PoissonDiskSampling;

public class Forestation : MonoBehaviour
{
    GameObject sp;
    GameObject ground;
    List<Tree> trees = new List<Tree>();
    List<GameObject> placedTrees = new List<GameObject>();
    Vector3 maximumCoordinates;
    Vector3 minimumCoordinates;
    public float minimumDistanceFromTheEdge = 5;
    public float treesAreaScale = 1f/8f;
    public float treesSizeScale = 1f/8f;
    public float treesSinkAmount = -1.0f;

    void GetEdgesOfTheWorld()
    {
        var ground = GameObject.Find("Ground");
        var groundScale = ground.transform.localScale;
        var groundSize = ground.GetComponent<MeshFilter>().mesh.bounds.size;
        var posGround = ground.transform.position;

        // vector with maximum coordinates  
        maximumCoordinates = new Vector3(
            posGround.x + groundScale.x * groundSize.x * treesAreaScale,
            posGround.y + groundScale.y * groundSize.y * treesAreaScale - treesSizeScale * treesSinkAmount * 1.25f,
            posGround.z + groundScale.z * groundSize.z * treesAreaScale);

        minimumCoordinates = new Vector3(
            posGround.x - groundScale.x * groundSize.x * treesAreaScale,
            posGround.y + groundScale.y * groundSize.y * treesAreaScale - treesSizeScale * treesSinkAmount * 0.75f,
            posGround.z - groundScale.z * groundSize.z * treesAreaScale);

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
        var tree = new Tree(path, "EU43_5", treesSizeScale);
        tree.prefab.name = $"Tree_{trees.Count + 1}";

        tree.prefab.transform.position = new Vector3(
            Random.Range(minimumCoordinates.x + minimumDistanceFromTheEdge, maximumCoordinates.x - minimumDistanceFromTheEdge),
            Random.Range(minimumCoordinates.y, maximumCoordinates.y),
            Random.Range(minimumCoordinates.z + minimumDistanceFromTheEdge, maximumCoordinates.z - minimumDistanceFromTheEdge));
        tree.prefab.transform.Rotate(0f, Random.Range(0f, 360f), 0f, Space.Self);

        // .tif extension is a problem - to import properly with textures,
        // I've converted every .tif file to .png and I've edited .mtl file
        // (replaced every occurrence of '.tif' with '.png')
    }

    void LoadTrees(string path, List<string> names)
    {
        // Load all trees from the names list
        foreach (var name in names)
        {
            var tree = new Tree(path, name, treesSizeScale);
            tree.prefab.transform.position = new Vector3(0.0f, -tree.size.y, 0.0f);
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
                Random.Range(minimumCoordinates.y, maximumCoordinates.y),
                Random.Range(minimumCoordinates.z + minimumDistanceFromTheEdge, maximumCoordinates.z - minimumDistanceFromTheEdge));
            tree.transform.Rotate(0f, Random.Range(0f, 360f), 0f, Space.Self);
            placedTrees.Add(tree);
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
                maxSize.x = tree.size.x;
            if (tree.size.z > maxSize.z)
                maxSize.z = tree.size.z;
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
                    Random.Range(minimumCoordinates.y, maximumCoordinates.y),
                    z + Random.Range(-maxRandomOffset, maxRandomOffset));
                tree.transform.localRotation = Quaternion.Euler(-90f, 0, Random.Range(0f, 360f));
                placedTrees.Add(tree);
            }
        }
    }

    void PlaceTreesGrid(int cols, int rows, float maxRandomOffset)
    {
        // Constrain parameters
        if (cols < 1)
            cols = 1;
        if (rows < 1)
            rows = 1;
        if (maxRandomOffset < 0.0f)
            maxRandomOffset = 0.0f;
        // Find max size of trees from Tree.size
        Vector3 maxSize = new Vector3();
        foreach (var tree in trees)
        {
            if (tree.size.x > maxSize.x)
                maxSize.x = tree.size.x;
            if (tree.size.z > maxSize.z)
                maxSize.z = tree.size.z;
        }
        // Calculate step and offset
        float xStep = (maximumCoordinates.x - minimumCoordinates.x - 2 * minimumDistanceFromTheEdge) / cols;
        float zStep = (maximumCoordinates.z - minimumCoordinates.z - 2 * minimumDistanceFromTheEdge) / rows;
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
                    Random.Range(minimumCoordinates.y, maximumCoordinates.y),
                    z + Random.Range(-maxRandomOffset, maxRandomOffset));
                tree.transform.localRotation = Quaternion.Euler(-90f, 0, Random.Range(0f, 360f));
                placedTrees.Add(tree);
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
                Random.Range(minimumCoordinates.y, maximumCoordinates.y),
                minimumCoordinates.z + minimumDistanceFromTheEdge + point.y);
            tree.transform.Rotate(0f, Random.Range(0f, 360f), 0f, Space.Self);
            placedTrees.Add(tree);
        }
    }

    void RemovePlacedTrees()
    {
        foreach (var tree in placedTrees)
        {
            Destroy(tree);
        }
        placedTrees.Clear();
    }

    void RemoveLoadedTrees()
    {
        foreach (var tree in trees)
        {
            Destroy(tree.prefab);
        }
        trees.Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        GetEdgesOfTheWorld();
        // LoadSingleTree();
        LoadTrees(Application.dataPath + "/Resources/ForestObjects", new List<string> { "EU43_5" });
        // PlaceTreesRandom(10);
        // PlaceTreesGrid(2f, 0.15f);
        PlaceTreesGrid(3, 9, 0.15f);
        // PlaceTreesPoisson(25.0f, 5);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
