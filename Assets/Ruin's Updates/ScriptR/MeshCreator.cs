using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This Unity script demonstrates how to create a Mesh (in this case a Cube) purely through code.
// Simply, create a new Scene, add this script to the Main Camera, and run.  

public class MeshCreator : MonoBehaviour
{

    GameObject cube;
    GameObject plan;
    public Vector3 dimension;
    public float VerticeSpace;
    public bool create = false;
    public bool useRelDimension = false;


    public void CreateUniformCube(Vector3 dimensionXYZ, float verticeDistance)
    {
        // Calculate subdivisions per face based on the dimension and target vertice distance
        int subdivisionsX = Mathf.CeilToInt(dimensionXYZ.x / verticeDistance);  // Along the X axis
        int subdivisionsY = Mathf.CeilToInt(dimensionXYZ.y / verticeDistance);  // Along the Y axis
        int subdivisionsZ = Mathf.CeilToInt(dimensionXYZ.z / verticeDistance);  // Along the Z axis

        // 1) Create an empty GameObject with the required Components
        cube = new GameObject("UniformCube");
        cube.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = cube.AddComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        // Lists to store all vertices, triangles, and UVs
        List<Vector3> verticesList = new List<Vector3>();
        List<int> trianglesList = new List<int>();
        List<Vector2> uvsList = new List<Vector2>();

        // Subdivide each face of the cube into small, uniform triangles
        // Front face
        SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
            new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Bottom-left corner
            new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Bottom-right corner
            new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),    // Top-right corner
            new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Top-left corner
            subdivisionsX, subdivisionsY);

        // Back face (along X, Y)
        SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
            new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Bottom-left corner
            new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2), // Bottom-right corner
            new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Top-right corner
            new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-left corner
            subdivisionsX, subdivisionsY);  // Subdivide back face (x, y axes)

        // Left face (along Z, Y)
        SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
            new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2), // Bottom-left corner
            new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Bottom-right corner
            new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Top-right corner
            new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Top-left corner
            subdivisionsZ, subdivisionsY);  // Subdivide left face (z, y axes)

        // Right face (along Z, Y)
        SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
            new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Bottom-left corner
            new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Bottom-right corner
            new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-right corner
            new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),    // Top-left corner
            subdivisionsZ, subdivisionsY);  // Subdivide right face (z, y axes)

        // Top face (along X, Z)
        SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
            new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Bottom-left corner
            new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),    // Bottom-right corner
            new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-right corner
            new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Top-left corner
            subdivisionsX, subdivisionsZ);  // Subdivide top face (x, z axes)

        // Bottom face (along X, Z)
        SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
            new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2), // Bottom-left corner
            new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Bottom-right corner
            new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Top-right corner
            new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Top-left corner
            subdivisionsX, subdivisionsZ);  // Subdivide bottom face (x, z axes)

        // Apply the same for other 5 faces of the cube with different normals

        // 8) Build the Mesh
        mesh.Clear();
        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglesList.ToArray();
        mesh.RecalculateNormals();
        mesh.uv = uvsList.ToArray();
        mesh.Optimize();

        // 9) Give it a Material
        Material cubeMaterial = new Material(Shader.Find("Standard"));
        cube.GetComponent<Renderer>().material = cubeMaterial;
    }

    // Subdivide a face into uniform triangles
    private void SubdivideFaceWithUniformTriangles(List<Vector3> verticesList, List<int> trianglesList, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, int subdivisionsX, int subdivisionsY)
    {
        // Calculate the step size for subdivisions
        Vector3 stepX = (v1 - v0) / subdivisionsX;
        Vector3 stepY = (v3 - v0) / subdivisionsY;

        // Loop through each subdivision to create uniform quads and split them into triangles
        for (int i = 0; i < subdivisionsX; i++)
        {
            for (int j = 0; j < subdivisionsY; j++)
            {
                // Bottom-left corner of the current quad
                Vector3 bl = v0 + stepX * i + stepY * j;
                Vector3 br = bl + stepX;  // Bottom-right
                Vector3 tl = bl + stepY;  // Top-left
                Vector3 tr = bl + stepX + stepY;  // Top-right

                // Add vertices to the list
                verticesList.Add(bl);  // 0
                verticesList.Add(br);  // 1
                verticesList.Add(tl);  // 2
                verticesList.Add(tr);  // 3

                int vertIndex = verticesList.Count - 4;  // Index of the bottom-left vertex of the quad

                // Split the quad into two triangles (uniform size)
                trianglesList.Add(vertIndex);       // Triangle 1: Bottom-left
                trianglesList.Add(vertIndex + 1);   // Triangle 1: Bottom-right
                trianglesList.Add(vertIndex + 2);   // Triangle 1: Top-left
                

                trianglesList.Add(vertIndex + 1);   // Triangle 2: Bottom-right
                trianglesList.Add(vertIndex + 3);   // Triangle 2: Top-right
                trianglesList.Add(vertIndex + 2);   // Triangle 2: Top-left
                
            }
        }
    }


    // Update is called once per frame
    private void Update()
    {
        if (create) { CreateUniformCube(dimension, VerticeSpace); create = false; }
    }
}
