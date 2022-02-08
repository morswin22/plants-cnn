using Dummiesman;
using UnityEngine;

class Tree
{
    public string name;
    public GameObject prefab;
    public Vector3 size;

    public Tree(string path, string name, float sizeScale)
    {
        this.name = name;
        // Load the model
        this.prefab = new OBJLoader().Load($"{path}/{name}.obj", $"{path}/{name}.mtl");
        this.prefab.name = $"Tree_{name}";
        this.prefab.transform.Rotate(-90f, 0f, 0f, Space.Self);
        this.prefab.transform.localScale = new Vector3(
            this.prefab.transform.localScale.x * sizeScale,
            this.prefab.transform.localScale.y * sizeScale,
            this.prefab.transform.localScale.z * sizeScale
        );
        // Get max size from prefab's children
        Bounds bounds = new Bounds();
        foreach (Transform child in prefab.transform)
        {
            bounds.Encapsulate(child.gameObject.GetComponent<Renderer>().bounds);
        }
        size = bounds.size;
    }
}
