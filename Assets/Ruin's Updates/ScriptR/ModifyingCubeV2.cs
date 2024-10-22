using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyingCubeV2 : MonoBehaviour
{
    GameObject cube;
    GameObject plan;
    public Vector3 dimension;
    public int subdiv;
    public float VerticeSpace;
    public bool create = false;
    public bool cubeType = false;
    public bool useRelDimension = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (create)
        {
            CreateCube(dimension, VerticeSpace, subdiv);
            create = false;
        }
    }

    public void CreateCube(Vector3 dimensionXYZ, float VerticeSpace = 7, int subdivisions = 1)
    {
        // 1) Create an empty GameObject with the required Components
        cube = new GameObject("Cube");
        cube.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = cube.AddComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        // 2) Define the cube's dimensions
        float length = dimensionXYZ[0];
        float height = dimensionXYZ[1];
        float thickness = dimensionXYZ[2];

        // 3) Define the co-ordinates of each Corner of the cube 
        Vector3[] c = new Vector3[8];
        c[0] = new Vector3(-length * VerticeSpace / 2, -height * VerticeSpace / 2, thickness * VerticeSpace / 2);
        c[1] = new Vector3(length * VerticeSpace / 2, -height * VerticeSpace / 2, thickness * VerticeSpace / 2);
        c[2] = new Vector3(length * VerticeSpace / 2, -height * VerticeSpace / 2, -thickness * VerticeSpace / 2);
        c[3] = new Vector3(-length * VerticeSpace / 2, -height * VerticeSpace / 2, -thickness * VerticeSpace / 2);
        c[4] = new Vector3(-length * VerticeSpace / 2, height * VerticeSpace / 2, thickness * VerticeSpace / 2);
        c[5] = new Vector3(length * VerticeSpace / 2, height * VerticeSpace / 2, thickness * VerticeSpace / 2);
        c[6] = new Vector3(length * VerticeSpace / 2, height * VerticeSpace / 2, -thickness * VerticeSpace / 2);
        c[7] = new Vector3(-length * VerticeSpace / 2, height * VerticeSpace / 2, -thickness * VerticeSpace / 2);

        // Subdivide faces (front, back, left, right, top, bottom)
        List<Vector3> verticesList = new List<Vector3>();
        List<int> trianglesList = new List<int>();
        List<Vector3> normalsList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();

        // Subdivide each face by calling the helper function
        SubdivideFace(verticesList, trianglesList, c[0], c[1], c[5], c[4], Vector3.forward, (int)dimensionXYZ[0]); // Front
        SubdivideFace(verticesList, trianglesList, c[1], c[2], c[6], c[5], Vector3.right, (int)dimensionXYZ[2]);   // Right
        SubdivideFace(verticesList, trianglesList, c[2], c[3], c[7], c[6], Vector3.back, (int)dimensionXYZ[0]);    // Back
        SubdivideFace(verticesList, trianglesList, c[3], c[0], c[4], c[7], Vector3.left, (int)dimensionXYZ[2]);    // Left
        SubdivideFace(verticesList, trianglesList, c[4], c[5], c[6], c[7], Vector3.up, (int)dimensionXYZ[1]);      // Top
        SubdivideFace(verticesList, trianglesList, c[3], c[2], c[1], c[0], Vector3.down, (int)dimensionXYZ[1]);    // Bottom

        // 8) Build the Mesh
        mesh.Clear();
        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglesList.ToArray();
        mesh.RecalculateNormals(); // Automatically calculate normals
        mesh.uv = uvsList.ToArray();
        mesh.Optimize();

        // 9) Give it a Material
        Material cubeMaterial = new Material(Shader.Find("Standard"));
        cube.GetComponent<Renderer>().material = cubeMaterial;
    }

    // Helper function to subdivide a face
    private void SubdivideFace(List<Vector3> verticesList, List<int> trianglesList, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 normal, int subdivisions)
    {
        // Subdivide the face by adding new vertices and triangles
        for (int i = 0; i <= subdivisions; i++)
        {
            for (int j = 0; j <= subdivisions; j++)
            {
                float t1 = (float)i / subdivisions;
                float t2 = (float)j / subdivisions;

                // Interpolate between the 4 corners of the face
                Vector3 vA = Vector3.Lerp(v0, v1, t1);
                Vector3 vB = Vector3.Lerp(v3, v2, t1);
                Vector3 point = Vector3.Lerp(vB, vA, t2);

                verticesList.Add(point);

                if (i < subdivisions && j < subdivisions)
                {
                    int vertIndex = verticesList.Count - 1;
                    // Define 2 triangles for each quad
                    trianglesList.Add(vertIndex);
                    trianglesList.Add(vertIndex + 1);
                    trianglesList.Add(vertIndex + subdivisions + 1);

                    trianglesList.Add(vertIndex + 1);
                    trianglesList.Add(vertIndex + subdivisions + 2);
                    trianglesList.Add(vertIndex + subdivisions + 1);
                }
            }
        }
    }
}
