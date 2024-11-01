using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

// This Unity script demonstrates how to create a Mesh (in this case a Cube) purely through code.
// Simply, create a new Scene, add this script to the Main Camera, and run.  

public class MeshCreator : MonoBehaviour
{ 
    public GameObject cube;

    [Header("Cube")]
    public Vector3 dimension;
    public float verticeSpace=1;
    public bool create = false;
                                    //  4 ______7   Cube & Anchor's space disposition
                                    //   |\5_____\6
                                    //   | |   | |
                                    //   |_|___| | 3
                                    //  0 \|____\| 
                                    //     1     2      x  y
                                    //                z _\|
    [Header("Hole")]
    public bool hole = false;
    public Vector3 holeLoc;
    public Vector3 holescale;
    public Transform[] listPrefabAnchor;

    [Header("Subdiv")]
    public bool subdiv = false;
    public int subdivision = 1;
    public Face[] faceNameList;

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

    public void SubdivideOneFaceOfExisitingCube(GameObject cube, Face[] faceNameList, int subdivision)
    {
        Vector3 size = cube.GetComponent<MeshFilter>().mesh.bounds.size;
        int space = Mathf.CeilToInt(Mathf.Sqrt(cube.GetComponent<MeshFilter>().mesh.vertices.Length / 4));
        Vector3 location = cube.transform.position;

        Destroy(cube);
        CreateUniformCube(size, space, faceNameList, subdivision);

        cube.transform.position = location;
    }

    private bool IsPointInCube(Vector3 pointInWorld, Vector3 holeCenter, Vector3 holeDimensions, float tolerence = 0.01f)
    {
        Vector3 localDim = pointInWorld - holeCenter;
        //Debug.Log($"${Mathf.Abs(localDim.x) - holeDimensions.x/2 - tolerence}, ${Mathf.Abs(localDim.y) - holeDimensions.y / 2 - tolerence}, ${Mathf.Abs(localDim.z) - holeDimensions.z / 2 - tolerence}");
        return Mathf.Abs(localDim.x) < holeDimensions.x/2 + tolerence && Mathf.Abs(localDim.y) < holeDimensions.y/2 + tolerence && Mathf.Abs(localDim.z) < holeDimensions.z/2 + tolerence;
    }

    public void FillTheCubeWithPrefabAnchors(GameObject cube, Transform[] PrefabAnchorListInWorld, bool alreadySubDiv = false)
    {
        Mesh cubeMesh = cube.GetComponent<MeshFilter>().mesh;
        Vector3 cubePosition = cube.transform.position;
        Vector3 cubeScale = cubeMesh.bounds.size;
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

            if (areCollidingAnchors) 
            { 
                SubdivideOneFaceOfExisitingCube(cube, facesToSubdiv.ToArray(), (int)Mathf.Max(cubeScale.x, cubeScale.y, cubeScale.z) * 5); 
                CreateHoleInMesh(cube.GetComponent<MeshFilter>().mesh, position, scale); 
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (create) { CreateUniformCube(dimension, verticeSpace); create = false; }
        if(hole) 
        {
            //CreateHoleInMesh(cube.GetComponent<MeshFilter>().mesh, holeLoc, holescale);
            FillTheCubeWithPrefabAnchors(cube, listPrefabAnchor);
            hole = false; }
        if (subdiv) { SubdivideOneFaceOfExisitingCube(cube, faceNameList, subdivision); subdiv = false; }
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
}
