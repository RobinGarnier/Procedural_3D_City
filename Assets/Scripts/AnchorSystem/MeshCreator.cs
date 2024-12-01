using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using Parabox.CSG;

public class MeshCreator : MonoBehaviour
{
    GameObject cube;

    [Header("Cube")]
    Vector3 dimension;
    float verticeSpace = 1;
    bool create = false;

    public enum Face
    {
        none,
        top,
        front,
        right,
        back,
        left,
        bot
    }
    //  4 ______7   Cube & Anchor's space disposition
    //   |\5_____\6
    //   | |   | |
    //   |_|___| | 3
    //  0 \|____\| 
    //     1     2      x  y
    //                z _\|

    [Header("Hole")]
    public bool hole = false;
    Vector3 holeLoc;
    Vector3 holescale;
    public Transform holeAnchor;
    public MeshFilter meshToBore;

    [Header("Subdiv")]
    public bool subdiv = false;
    int subdivision = 1;
    Face[] faceNameList;

    [Header("Wall")]
    public List<Vector3> points;
    float thickness = 0.5f;
    float height = 2f;
    public bool wall;


    //Create a cube 
    public void CreateUniformCube(Vector3 dimensionXYZ, float verticeDistance, Face[] subdivFace = null, int subdiv = 0)
    {
        // Calculate subdivisions per face based on the dimension and target vertice distance
        int subdivisionsX = Mathf.CeilToInt(dimensionXYZ.x / verticeDistance);  // Along the X axis
        int subdivisionsY = Mathf.CeilToInt(dimensionXYZ.y / verticeDistance);  // Along the Y axis
        int subdivisionsZ = Mathf.CeilToInt(dimensionXYZ.z / verticeDistance);  // Along the Z axis
        Vector3 subdivisionVector = new(subdivisionsX, subdivisionsY, subdivisionsZ);
        Vector3 vectorSubDiv = new(subdiv, subdiv, subdiv);

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
        if (subdivFace == null)
        {
            //Front
            SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                    new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Bottom-left corner
                    new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Bottom-right corner
                    new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),    // Top-right corner
                    new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Top-left corner
                    subdivisionsX, subdivisionsY);
            //Back
            SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                    new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Bottom-left corner
                    new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2), // Bottom-right corner
                    new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Top-right corner
                    new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-left corner
                    subdivisionsX, subdivisionsY);
            //Left
            SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                    new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2), // Bottom-left corner
                    new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Bottom-right corner
                    new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Top-right corner
                    new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Top-left corner
                    subdivisionsZ, subdivisionsY);
            //Right
            SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                    new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Bottom-left corner
                    new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Bottom-right corner
                    new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-right corner
                    new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),    // Top-left corner
                    subdivisionsZ, subdivisionsY);
            //Top
            SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                    new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Bottom-left corner
                    new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),    // Bottom-right corner
                    new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-right corner
                    new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Top-left corner
                    subdivisionsX, subdivisionsZ);
            //Bot
            SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                    new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2), // Bottom-left corner
                    new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Bottom-right corner
                    new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-right corner
                    new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Top-left corner
                    subdivisionsX, subdivisionsZ);
        }
        else
        {
            // Front face (along X, Y)
            if (subdivFace.Contains(Face.front))
            {
                SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Bottom-left corner
                new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Bottom-right corner
                new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),    // Top-right corner
                new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Top-left corner
                subdiv, subdiv);
            }
            else
            {
                SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                    new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Bottom-left corner
                    new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Bottom-right corner
                    new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),    // Top-right corner
                    new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Top-left corner
                    subdivisionsX, subdivisionsY);
            }

            // Back face (along X, Y)
            if (subdivFace.Contains(Face.back))
            {
                SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Bottom-left corner
                new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2), // Bottom-right corner
                new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Top-right corner
                new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-left corner
                subdiv, subdiv);
            }
            else
            {
                SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                    new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Bottom-left corner
                    new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2), // Bottom-right corner
                    new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Top-right corner
                    new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-left corner
                    subdivisionsX, subdivisionsY);
            }

            // Left face (along Z, Y)
            if (subdivFace.Contains(Face.left))
            {
                SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                    new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2), // Bottom-left corner
                    new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Bottom-right corner
                    new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Top-right corner
                    new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Top-left corner
                    subdiv, subdiv);
            }
            else
            {
                SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                    new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2), // Bottom-left corner
                    new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Bottom-right corner
                    new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Top-right corner
                    new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Top-left corner
                    subdivisionsZ, subdivisionsY);
            }  // Subdivide left face (z, y axes)

            // Right face (along Z, Y)
            if (subdivFace.Contains(Face.right))
            {
                SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                    new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Bottom-left corner
                    new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Bottom-right corner
                    new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-right corner
                    new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),    // Top-left corner
                    subdiv, subdiv);
            }
            else
            {
                SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                    new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Bottom-left corner
                    new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Bottom-right corner
                    new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-right corner
                    new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),    // Top-left corner
                    subdivisionsZ, subdivisionsY);
            }  // Subdivide right face (z, y axes)

            // Top face (along X, Z)
            if (subdivFace.Contains(Face.top))
            {
                SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                    new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Bottom-left corner
                    new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),    // Bottom-right corner
                    new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-right corner
                    new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Top-left corner
                    subdiv, subdiv);
            }
            else
            {
                SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                    new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),   // Bottom-left corner
                    new Vector3(-dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),    // Bottom-right corner
                    new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-right corner
                    new Vector3(dimensionXYZ.x / 2, dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Top-left corner
                    subdivisionsX, subdivisionsZ);
            }  // Subdivide top face (x, z axes)

            // Bottom face (along X, Z)
            if (subdivFace.Contains(Face.bot))
            {
                SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                    new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2), // Bottom-left corner
                    new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Bottom-right corner
                    new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-right corner
                    new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Top-left corner
                    subdiv, subdiv);
            }
            else
            {
                SubdivideFaceWithUniformTriangles(verticesList, trianglesList,
                    new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2), // Bottom-left corner
                    new Vector3(dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),  // Bottom-right corner
                    new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, -dimensionXYZ.z / 2),   // Top-right corner
                    new Vector3(-dimensionXYZ.x / 2, -dimensionXYZ.y / 2, dimensionXYZ.z / 2),  // Top-left corner
                    subdivisionsX, subdivisionsZ);
            }  // Subdivide bottom face (x, z axes)
        }

        // 8) Build the Mesh
        mesh.Clear();
        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglesList.ToArray();
        mesh.RecalculateNormals();
        mesh.uv = uvsList.ToArray();
        if (dimensionXYZ == Vector3.one && subdivFace == null) { mesh.name = "Cube"; }
        //mesh.Optimize();

        // 9) Give it a Material
        Material cubeMaterial = new Material(Shader.Find("Standard"));
        cube.GetComponent<Renderer>().material = cubeMaterial;
    }
    private void SubdivideFaceWithUniformTriangles(List<Vector3> verticesList, List<int> trianglesList, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, int subdivisionsX, int subdivisionsY)
    {
        // p3\_______\ p2
        //   |    y  |
        //   | z_|   |
        //   |   \-x |     this cycle follow the same rotation along the normal of the face to the exterior
        //  \|_______|
        //  p0         p1

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
                trianglesList.Add(vertIndex + 2);   // Triangle 1: Top-left             //before : 3rd
                trianglesList.Add(vertIndex + 1);   // Triangle 1: Bottom-right


                trianglesList.Add(vertIndex + 1);   // Triangle 2: Bottom-right
                trianglesList.Add(vertIndex + 2);   // Triangle 2: Top-left             //before : 3rd
                trianglesList.Add(vertIndex + 3);   // Triangle 2: Top-right

            }
        }
    }


    //Bore a mesh
    public void CreateHoleInMesh(Mesh mesh, Transform[] listHoleTransform)
    {
        // Get the existing vertices and triangles from the mesh
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        List<Vector3> listHolePosition = new List<Vector3>();
        List<Vector3> listHoleScale = new List<Vector3>();
        foreach (Transform t in listHoleTransform)
        {
            listHolePosition.Add(t.position);
            listHoleScale.Add(t.localScale);
        }

        Vector3[] arrayHolePosition = listHolePosition.ToArray();
        Vector3[] arrayHoleScale = listHoleScale.ToArray();

        // Create new lists to hold the modified mesh data
        List<Vector3> newVertices = new List<Vector3>(vertices);
        List<int> newTriangles = new List<int>();
        //Debug.Log($"original tri number : ${triangles.Length}");
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
            bool collidingTriangle = false;
            for (int j = 0; j < listHoleTransform.Length; j++)
            {
                Vector3 holeCenter = arrayHolePosition[j];
                Vector3 holeDimensions = arrayHoleScale[j];
                if (IsPointInCube(p0, holeCenter, holeDimensions) &&
                IsPointInCube(p1, holeCenter, holeDimensions) &&
                IsPointInCube(p2, holeCenter, holeDimensions))
                {
                    collidingTriangle = true;
                }
            }
            if (!collidingTriangle)
            {
                // If the triangle is not in the hole, add it to the new triangle list
                newTriangles.Add(v0);
                newTriangles.Add(v1);
                newTriangles.Add(v2);
            }
            // Otherwise, we skip this triangle, which removes it from the mesh
        }
        //Debug.Log($"new tri number : ${newTriangles.Count}");

        // Update the mesh with the new vertices and triangles
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.RecalculateNormals();  // Recalculate normals to ensure proper lighting
    }
    private bool IsPointInCube(Vector3 pointInWorld, Vector3 holeCenter, Vector3 holeDimensions, float tolerence = 0.01f)
    {
        Vector3 localDim = pointInWorld - holeCenter;
        return Mathf.Abs(localDim.x) < holeDimensions.x / 2 + tolerence && Mathf.Abs(localDim.y) < holeDimensions.y / 2 + tolerence && Mathf.Abs(localDim.z) < holeDimensions.z / 2 + tolerence;
    }
    /*public void CreateHoleInMesh(Mesh mesh, Vector3 holeCenter, Vector3 holeDimensions)
    {
        // Get the existing vertices and triangles from the mesh
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Create new lists to hold the modified mesh data
        List<Vector3> newVertices = new List<Vector3>(vertices);
        List<int> newTriangles = new List<int>();
        Debug.Log($"original tri number : ${triangles.Length}");
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
        //Debug.Log($"new tri number : ${newTriangles.Count}");

        // Update the mesh with the new vertices and triangles
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.RecalculateNormals();  // Recalculate normals to ensure proper lighting
    }*/


    //Create a wall
    public void GenerateWall()
    {
        if (points == null || points.Count < 2)
        {
            Debug.LogError("You need at least two points to generate a wall.");
            return;
        }

        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        // Generate wall vertices
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];

            // Calculate direction and normal
            Vector3 direction = (p2 - p1).normalized;
            Vector3 normal = new Vector3(-direction.z, 0, direction.x); // Perpendicular vector

            // Outer and inner points
            Vector3 p1Outer = p1 + normal * thickness / 2;
            Vector3 p1Inner = p1 - normal * thickness / 2;
            Vector3 p2Outer = p2 + normal * thickness / 2;
            Vector3 p2Inner = p2 - normal * thickness / 2;

            // Top vertices (offset by height)
            Vector3 p1TopOut = p1Outer + Vector3.up * height;
            Vector3 p1TopIn = p1Inner + Vector3.up * height;
            Vector3 p2TopOut = p2Outer + Vector3.up * height;
            Vector3 p2TopIn = p2Inner + Vector3.up * height;

            //Front 
            SubdivideFaceWithUniformTriangles(vertices, triangles, p1Inner, p2Inner, p2TopIn, p1TopIn, 10, 10);//
            //Back 
            SubdivideFaceWithUniformTriangles(vertices, triangles, p1Outer, p1TopOut, p2TopOut,  p2Outer, 10, 10);
            //Top 
            SubdivideFaceWithUniformTriangles(vertices, triangles, p1TopIn, p2TopIn, p2TopOut, p1TopOut, 1, 1);//
            //Bot 
            SubdivideFaceWithUniformTriangles(vertices, triangles, p2Outer, p2Inner, p1Inner, p1Outer, 1, 1);
            //Right
            SubdivideFaceWithUniformTriangles(vertices, triangles, p1Outer, p1Inner, p1TopIn, p1TopOut, 1, 1);
            //Left 
            SubdivideFaceWithUniformTriangles(vertices, triangles, p2Inner, p2Outer, p2TopOut, p2TopIn, 1, 1);
        }

        cube = new GameObject("UniformCube");
        cube.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = cube.AddComponent<MeshFilter>();
        // Assign mesh data
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        // Recalculate normals and bounds
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Assign the mesh to the MeshFilter
        meshFilter.mesh = mesh;
        Material cubeMaterial = new Material(Shader.Find("Standard"));
        cube.GetComponent<Renderer>().material = cubeMaterial;
    }


    //_________________________________HELPERS______________________________________________

    //Subdivide specific faces of an existing cube
    public void SubdivideFacesOfExisitingCube(GameObject cubeToSubdiv, Face[] faceNameList, int subdivision)
    {
        Vector3 size = cubeToSubdiv.GetComponent<MeshFilter>().mesh.bounds.size;
        int space = Mathf.CeilToInt(Mathf.Sqrt(cubeToSubdiv.GetComponent<MeshFilter>().mesh.vertices.Length / 4));
        Vector3 location = cubeToSubdiv.transform.position;

        Destroy(cubeToSubdiv);
        CreateUniformCube(size, space, faceNameList, subdivision);

        cube.transform.position = location;
    }
    //from two anchor, correctly bore the needed cube
    public void FillTheCubeWithPrefabAnchors(GameObject cubeToBore, Transform[] PrefabAnchorListInWorld, bool alreadySubDiv = false)
    {
        Mesh cubeMesh = cubeToBore.GetComponent<MeshFilter>().mesh;
        Vector3 cubePosition = cubeToBore.transform.position;
        Vector3 cubeScale = cubeMesh.bounds.size;

        List<Face> finalFacesToSubdiv = new List<Face>();
        List<Transform> listCollidingAnchors = new List<Transform>();
        bool needHole = false;

        foreach (Transform transformAnchor in PrefabAnchorListInWorld)
        {
            //center coordinate of all anchors to the center of the cube
            //transformAnchor.position = localCoord ? transformAnchor.position : transformAnchor.position - cube.transform.position ;

            //define the anchor as the 8 corners of its boundery
            Vector3 position = transformAnchor.position;
            Vector3 scale = transformAnchor.localScale;
            Vector3[] listVerticeOfAnchor = new Vector3[] {
                position + new Vector3(scale.x/2, -scale.y/2, scale.z/2),//
                position + new Vector3(-scale.x/2, -scale.y/2, scale.z/2),
                position + new Vector3(-scale.x/2, -scale.y/2, -scale.z/2),//
                position + new Vector3(scale.x/2, -scale.y/2, -scale.z/2),
                position + new Vector3(scale.x/2, scale.y/2, scale.z/2),//
                position + new Vector3(-scale.x/2, scale.y/2, scale.z/2),
                position + new Vector3(-scale.x/2, scale.y/2, -scale.z/2),
                position + new Vector3(scale.x/2, scale.y/2, -scale.z/2)
            };
            //Check if the anchor intersect with the cube surface by checking two diagonal corners
            //if(IsPointInCube(listVerticeOfAnchor[2], position, scale) ^ IsPointInCube(listVerticeOfAnchor[4], position, scale)) { CreateHoleInMesh(cubeMesh, position, scale); }
            List<int[]> refIndexForCornerCheck = new List<int[]>() {
                new int[] { 1, 4, 3 },
                new int[] { 0, 5, 2 },
                new int[] { 3, 6, 1 },
                new int[] { 2, 7, 0 },
                new int[] { 5, 0, 7 },
                new int[] { 4, 1, 6 },
                new int[] { 7, 2, 5 },
                new int[] { 6, 3, 4 },
            };
            List<Face[]> refFaceForCornerCheck = new List<Face[]>()
            {
                new Face[] {Face.front, Face.top, Face.right},
                new Face[] {Face.back, Face.top, Face.right},
                new Face[] {Face.back, Face.top, Face.left},
                new Face[] {Face.front, Face.top, Face.left},
                new Face[] {Face.front, Face.bot, Face.right},
                new Face[] {Face.back, Face.bot, Face.right},
                new Face[] {Face.back, Face.bot, Face.left},
                new Face[] {Face.front, Face.bot, Face.left},

            };
            List<Face> facesToSubdiv = new List<Face>();
            int indexCorner = 0;
            bool areCollidingAnchors = false;
            foreach (Vector3 vertice in listVerticeOfAnchor)
            {
                if (IsPointInCube(vertice, cubePosition, cubeScale))
                {
                    Debug.Log($"PointInCube for vertice n°${indexCorner} : ${vertice}");
                    int indexFace = 0;
                    foreach (int corner in refIndexForCornerCheck[indexCorner])
                    {
                        if (IsPointInCube(listVerticeOfAnchor[corner], cubePosition, cubeScale) == false)
                        {
                            facesToSubdiv.Add(refFaceForCornerCheck[indexCorner][indexFace]);
                            areCollidingAnchors = true;
                            Debug.Log(refFaceForCornerCheck[indexCorner][indexFace]);
                        }
                        indexFace++;
                    }
                    break;
                }
                indexCorner++;
            }

            foreach (Face face in facesToSubdiv)
            {
                if (!finalFacesToSubdiv.Contains(face))
                {
                    finalFacesToSubdiv.Add(face);
                }
            }
            if (areCollidingAnchors) { needHole = true; listCollidingAnchors.Add(transformAnchor); }
            /*if (areCollidingAnchors) 
            { 
                SubdivideFacesOfExisitingCube(cube, facesToSubdiv.ToArray(), (int)Mathf.Max(cubeScale.x, cubeScale.y, cubeScale.z) * 5);
                CreateHoleInMesh(cube.GetComponent<MeshFilter>().mesh, position, scale); 
            }*/
        }

        if (needHole)
        {
            SubdivideFacesOfExisitingCube(cube, finalFacesToSubdiv.ToArray(), (int)Mathf.Max(cubeScale.x, cubeScale.y, cubeScale.z) * 5);
            CreateHoleInMesh(cube.GetComponent<MeshFilter>().mesh, listCollidingAnchors.ToArray());
        }
    }


    private void Update()
    {
        if (create) { CreateUniformCube(dimension, verticeSpace); create = false; }
        if (hole)
        {
            Model result = CSG.Subtract(meshToBore.gameObject, holeAnchor.gameObject);

            // Create a gameObject to render the result
            var composite = new GameObject();
            composite.AddComponent<MeshFilter>().sharedMesh = result.mesh;
            composite.AddComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();

            hole = false;
        }
        if (subdiv) { SubdivideFacesOfExisitingCube(cube, faceNameList, subdivision); subdiv = false; }
        if(wall) { GenerateWall(); wall = false; }
    }


}
