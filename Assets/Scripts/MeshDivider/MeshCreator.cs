using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using Parabox.CSG;

// This Unity script demonstrates how to create a Mesh (in this case a Cube) purely through code.
// Simply, create a new Scene, add this script to the Main Camera, and run.  

public class MeshCreator : MonoBehaviour
{ 
    GameObject cube;

    [Header("Cube")]
    Vector3 dimension;
    float verticeSpace=1;
    bool create = false;
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
        if(subdivFace == null)
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
        if(dimensionXYZ == Vector3.one && subdivFace == null) { mesh.name = "Cube"; }
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

    public void SubdivideFacesOfExisitingCube(GameObject cubeToSubdiv, Face[] faceNameList, int subdivision)
    {
        Vector3 size = cubeToSubdiv.GetComponent<MeshFilter>().mesh.bounds.size;
        int space = Mathf.CeilToInt(Mathf.Sqrt(cubeToSubdiv.GetComponent<MeshFilter>().mesh.vertices.Length / 4));
        Vector3 location = cubeToSubdiv.transform.position;

        Destroy(cubeToSubdiv);
        CreateUniformCube(size, space, faceNameList, subdivision);

        cube.transform.position = location;
    }

    private bool IsPointInCube(Vector3 pointInWorld, Vector3 holeCenter, Vector3 holeDimensions, float tolerence = 0.01f)
    {
        Vector3 localDim = pointInWorld - holeCenter;
        //Debug.Log($"${Mathf.Abs(localDim.x) - holeDimensions.x/2 - tolerence}, ${Mathf.Abs(localDim.y) - holeDimensions.y / 2 - tolerence}, ${Mathf.Abs(localDim.z) - holeDimensions.z / 2 - tolerence}");
        return Mathf.Abs(localDim.x) < holeDimensions.x/2 + tolerence && Mathf.Abs(localDim.y) < holeDimensions.y/2 + tolerence && Mathf.Abs(localDim.z) < holeDimensions.z/2 + tolerence;
    }

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
                        if (IsPointInCube(listVerticeOfAnchor[corner], cubePosition, cubeScale)==false)
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

    // Update is called once per frame
    private void Update()
    {
        if (create) { CreateUniformCube(dimension, verticeSpace); create = false; }
        if(hole) 
        {
            //CreateHoleInMesh(cube.GetComponent<MeshFilter>().mesh, holeLoc, holescale);
            //FillTheCubeWithPrefabAnchors(cube, listPrefabAnchor);

            //meshToBore.mesh = BooleanDifference(meshToBore.mesh, holeAnchor);
            Model result = CSG.Subtract(meshToBore.gameObject, holeAnchor.gameObject);

            // Create a gameObject to render the result
            var composite = new GameObject();
            composite.AddComponent<MeshFilter>().sharedMesh = result.mesh;
            composite.AddComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();

            hole = false; }
        if (subdiv) { SubdivideFacesOfExisitingCube(cube, faceNameList, subdivision); subdiv = false; }
    }

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

    public static Mesh BooleanDifference(Mesh originalMesh, Transform holeTransform)
    {
        // Step 1: Get original mesh data
        Vector3[] originalVertices = originalMesh.vertices;
        int[] originalTriangles = originalMesh.triangles;
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();

        // Step 2: Transform hole dimensions and bounding box
        Matrix4x4 holeMatrix = holeTransform.localToWorldMatrix; // Transform of the hole
        Vector3 holeScale = holeTransform.localScale;
        Bounds holeBounds = new Bounds(holeTransform.position, holeScale);

        // Step 3: Iterate through the triangles
        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            int v0 = originalTriangles[i];
            int v1 = originalTriangles[i + 1];
            int v2 = originalTriangles[i + 2];

            Vector3 p0 = originalVertices[v0];
            Vector3 p1 = originalVertices[v1];
            Vector3 p2 = originalVertices[v2];

            // Step 4: Check if the triangle is inside or intersects the hole
            if (IsTriangleIntersectingOrInsideHole(p0, p1, p2, holeBounds, holeMatrix))
            {
                // This triangle is within the hole - skip it
                continue;
            }

            // Add triangle to new mesh
            AddTriangle(newVertices, newTriangles, p0, p1, p2);
        }

        // Step 5: Generate boundary faces for the intersection edges
        GenerateBoundaryFaces(originalVertices, originalTriangles, newVertices, newTriangles, holeBounds, holeMatrix);

        // Step 6: Create new mesh
        Mesh resultMesh = new Mesh();
        resultMesh.vertices = newVertices.ToArray();
        resultMesh.triangles = newTriangles.ToArray();
        resultMesh.RecalculateNormals(); // Ensure normals are updated for lighting
        return resultMesh;
    }

    // Helper function to check if a triangle intersects or is inside the hole
    private static bool IsTriangleIntersectingOrInsideHole(Vector3 p0, Vector3 p1, Vector3 p2, Bounds holeBounds, Matrix4x4 holeMatrix)
    {
        // Transform triangle vertices into the hole's local space
        Vector3 localP0 = holeMatrix.inverse.MultiplyPoint3x4(p0);
        Vector3 localP1 = holeMatrix.inverse.MultiplyPoint3x4(p1);
        Vector3 localP2 = holeMatrix.inverse.MultiplyPoint3x4(p2);

        // Check if any vertex is inside the hole
        if (holeBounds.Contains(localP0) || holeBounds.Contains(localP1) || holeBounds.Contains(localP2))
        {
            return true;
        }

        // Check if any edge of the triangle intersects the bounding box
        if (IsEdgeIntersectingBox(localP0, localP1, holeBounds) ||
            IsEdgeIntersectingBox(localP1, localP2, holeBounds) ||
            IsEdgeIntersectingBox(localP2, localP0, holeBounds))
        {
            return true;
        }

        // If no vertices are inside and no edges intersect, the triangle is outside the hole
        return false;
    }



    // Helper function to check if an edge intersects the hole bounding box
    private static bool IsEdgeIntersectingBox(Vector3 edgeStart, Vector3 edgeEnd, Bounds box)
    {
        // Perform the Slab Method to test for line-box intersection
        Vector3 boxMin = box.min;
        Vector3 boxMax = box.max;

        float tMin = 0.0f;
        float tMax = 1.0f;

        Vector3 direction = edgeEnd - edgeStart;

        for (int i = 0; i < 3; i++) // Check against each axis (X, Y, Z)
        {
            if (Mathf.Abs(direction[i]) < Mathf.Epsilon)
            {
                // The line is parallel to the slab. Check if the start point is outside the slab.
                if (edgeStart[i] < boxMin[i] || edgeStart[i] > boxMax[i])
                    return false;
            }
            else
            {
                // Compute intersection times for the slabs
                float invDir = 1.0f / direction[i];
                float t1 = (boxMin[i] - edgeStart[i]) * invDir;
                float t2 = (boxMax[i] - edgeStart[i]) * invDir;

                if (t1 > t2) // Swap t1 and t2 if needed
                {
                    float temp = t1;
                    t1 = t2;
                    t2 = temp;
                }

                // Update tMin and tMax
                tMin = Mathf.Max(tMin, t1);
                tMax = Mathf.Min(tMax, t2);

                // If tMin > tMax, there is no intersection
                if (tMin > tMax)
                    return false;
            }
        }

        // If we reach here, the line segment intersects the box
        return true;
    }


    // Helper function to check if a line segment intersects a box plane
    private static bool IntersectsPlane(Vector3 edgeStart, Vector3 edgeEnd, Vector3 boxMin, Vector3 boxMax, Vector3 planeNormal)
    {
        Vector3 boxCenter = (boxMin + boxMax) / 2;
        float planeD = Vector3.Dot(planeNormal, boxCenter);

        float startDist = Vector3.Dot(planeNormal, edgeStart) - planeD;
        float endDist = Vector3.Dot(planeNormal, edgeEnd) - planeD;

        // If signs of distances are different, the edge crosses the plane
        return startDist * endDist <= 0;
    }


    // Helper function to add a triangle to the new mesh
    private static void AddTriangle(List<Vector3> vertices, List<int> triangles, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        int index = vertices.Count;
        vertices.Add(p0);
        vertices.Add(p1);
        vertices.Add(p2);
        triangles.Add(index);
        triangles.Add(index + 1);
        triangles.Add(index + 2);
    }

    // Helper function to generate boundary faces for the intersection
    private static void GenerateBoundaryFaces(Vector3[] originalVertices, int[] originalTriangles,
        List<Vector3> newVertices, List<int> newTriangles, Bounds holeBounds, Matrix4x4 holeMatrix)
    {
        // Placeholder for boundary generation logic
        // You'd need to identify edges on the intersection boundary and create quads/triangles to close the mesh.
        // For simplicity, this part can be left as a future enhancement or added if required.

        Debug.Log("Boundary face generation is currently not implemented. Add logic here if needed.");
    }
}
