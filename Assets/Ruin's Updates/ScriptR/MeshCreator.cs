using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This Unity script demonstrates how to create a Mesh (in this case a Cube) purely through code.
// Simply, create a new Scene, add this script to the Main Camera, and run.  

public class MeshCreator : MonoBehaviour
{ 
    public GameObject cube;
    GameObject plan;
    [Header("Cube")]
    public Vector3 dimension;
    public float VerticeSpace;
    public bool create = false;
    [Header("Hole")]
    public bool hole = false;
    public Vector3 holeLoc;
    public Vector3 holescale;
    [Header("Subdiv")]
    public bool subdiv = false;
    public Face faceName;
    

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

    public void SubdivideFaceUsingEnum(Mesh mesh, Face faceName, Vector3 subdivision)
    {
        List<Vector3> verticesList = new List<Vector3>(mesh.vertices);
        List<int> trianglesList = new List<int>(mesh.triangles);
        Vector3[] cornerVertices = new Vector3[] {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
        Vector3 dimensionXYZ = Vector3.zero;
        int subdivision1 = 0;
        int subdivision2 = 0;

        switch (faceName)
        {
            case Face.top : cornerVertices = new Vector3[]   //TOP
            {
                new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Bottom-left corner
                new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),    // Bottom-right corner
                new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-right corner
                new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Top-left corner
            }; subdivision1 = (int)subdivision.x; subdivision2 = (int)subdivision.z; break;

            case Face.front: cornerVertices = new Vector3[]   //FRONT
            {
                new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Bottom-left corner
            new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Bottom-right corner
            new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),    // Top-right corner
            new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2)  // Top-left corner
            }; subdivision1 = (int)subdivision.x; subdivision2 = (int)subdivision.y; break;

            case Face.right: cornerVertices = new Vector3[]   //RIGHT
                {
                new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Bottom-left corner
            new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Bottom-right corner
            new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-right corner
            new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2)  // Top-left corner
                }; subdivision1 = (int)subdivision.z; subdivision2 = (int)subdivision.y; break;

            case Face.back: cornerVertices = new Vector3[]   //BACK
                {
                new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Bottom-left corner
            new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2), // Bottom-right corner
            new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Top-right corner
            new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2)  // Top-left corner
                }; subdivision1 = (int)subdivision.x; subdivision2 = (int)subdivision.y; break;

            case Face.left: cornerVertices = new Vector3[]   //LEFT
                {
                new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2), // Bottom-left corner
            new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Bottom-right corner
            new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Top-right corner
            new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2)  // Top-left corner
                }; subdivision1 = (int)subdivision.z; subdivision2 = (int)subdivision.y; break;

            case Face.bot: cornerVertices = new Vector3[]   //BOT
                {
                new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2), // Bottom-left corner
            new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Bottom-right corner
            new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Top-right corner
            new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2)  // Top-left corner
                }; subdivision1 = (int)subdivision.x; subdivision2 = (int)subdivision.z; break;
        }

        SubdivideFaceWithUniformTriangles(verticesList, trianglesList, cornerVertices[0], cornerVertices[1], cornerVertices[2], cornerVertices[3], subdivision1, subdivision2);
    }

    public void CreateHoleInMesh(Mesh mesh, Vector3 holeCenter, Vector3 holeDimensions)
    {
        // Get the existing vertices and triangles from the mesh
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Create new lists to hold the modified mesh data
        List<Vector3> newVertices = new List<Vector3>(vertices);
        List<int> newTriangles = new List<int>();

        // Iterate through the triangles
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Get the vertex indices for the current triangle
            int v0 = triangles[i];
            int v1 = triangles[i + 1];
            int v2 = triangles[i + 2];

            // Get the actual vertex positions
            Vector3 p0 = vertices[v0];
            Vector3 p1 = vertices[v1];
            Vector3 p2 = vertices[v2];

            // Check if this triangle intersects the hole
            if (!(IsPointInCube(p0, holeCenter, holeDimensions) &&
                IsPointInCube(p1, holeCenter, holeDimensions) &&
                IsPointInCube(p2, holeCenter, holeDimensions) ))
            {
                // If the triangle is not in the hole, add it to the new triangle list
                newTriangles.Add(v0);
                newTriangles.Add(v1);
                newTriangles.Add(v2);
            }
            // Otherwise, we skip this triangle, which removes it from the mesh
        }

        // Update the mesh with the new vertices and triangles
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.RecalculateNormals();  // Recalculate normals to ensure proper lighting
    }

    // Helper function to check if a triangle is within the hole area
    private bool IsTriangleInHole(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 faceNormal, Vector3 holeCenter, Vector2 holeDimensions)
    {
        // First, ensure the triangle lies on the specified face of the mesh (based on faceNormal)
        // We check if the triangle is coplanar with the face by projecting vertices onto the faceNormal plane
        // This ensures we're looking at triangles on the same plane as the face we want to modify

        float tolerance = 0.01f;  // Tolerance to check if vertices are coplanar with the face
        if (Mathf.Abs(Vector3.Dot(p0 - holeCenter, faceNormal)) > tolerance ||
            Mathf.Abs(Vector3.Dot(p1 - holeCenter, faceNormal)) > tolerance ||
            Mathf.Abs(Vector3.Dot(p2 - holeCenter, faceNormal)) > tolerance)
        {
            return false;  // Triangle is not in the plane of the face we're modifying
        }

        // Now check if the triangle's vertices are inside the hole area (projected onto the face)
        Vector3 projectedP0 = Vector3.ProjectOnPlane(p0 - holeCenter, faceNormal);
        Vector3 projectedP1 = Vector3.ProjectOnPlane(p1 - holeCenter, faceNormal);
        Vector3 projectedP2 = Vector3.ProjectOnPlane(p2 - holeCenter, faceNormal);

        Vector2 holeHalfSize = holeDimensions / 2;

        // Check if each vertex lies within the bounds of the hole
        bool p0InHole = Mathf.Abs(projectedP0.x) <= holeHalfSize.x && Mathf.Abs(projectedP0.y) <= holeHalfSize.y;
        bool p1InHole = Mathf.Abs(projectedP1.x) <= holeHalfSize.x && Mathf.Abs(projectedP1.y) <= holeHalfSize.y;
        bool p2InHole = Mathf.Abs(projectedP2.x) <= holeHalfSize.x && Mathf.Abs(projectedP2.y) <= holeHalfSize.y;

        // If all three vertices are inside the hole area, the triangle should be removed
        return p0InHole && p1InHole && p2InHole;
    }

    private bool IsPointInCube(Vector3 point, Vector3 holeCenter, Vector3 holeDimensions, float tolerence = 0.01f)
    {
        Vector3 localDim = point - holeCenter;
        return Mathf.Abs(localDim.x) < holeDimensions.x/2 + tolerence && Mathf.Abs(localDim.y) < holeDimensions.y/2 + tolerence && Mathf.Abs(localDim.z) < holeDimensions.z/2 + tolerence;
    }

    // Update is called once per frame
    private void Update()
    {
        if (create) { CreateUniformCube(dimension, VerticeSpace); create = false; }
        if(hole) { CreateHoleInMesh(cube.GetComponent<MeshFilter>().mesh, holeLoc, holescale); hole = false; }
        if (subdiv) { SubdivideFaceUsingEnum(cube.GetComponent<MeshFilter>().mesh, faceName, new Vector3(Mathf.CeilToInt(dimension.x / VerticeSpace), Mathf.CeilToInt(dimension.y / VerticeSpace), Mathf.CeilToInt(dimension.z / VerticeSpace))); subdiv = false; }
    }

    public enum Face
    {
        top,
        front,
        right,
        back,
        left,
        bot
    }
}
