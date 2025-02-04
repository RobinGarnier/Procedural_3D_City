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
    public List<Vector3> pointsList;
    float thickness = 0.5f;
    float height = 2f;
    public bool wall;

    [Header("District")]
    public bool district;
    public Transform districtAnchor;
    public List<Vector3> entryPointsList;

    public class Wall
    {
        public List<Vector3> points;
        public float height;
        public float thickness;
        public List<Transform> holeAnchor;
        

        public Wall(List<Vector3> points, float thickness, float height, List<Transform> holeAnchor)
        {
            this.points = points;
            this.thickness = thickness;
            this.height = height;
            this.holeAnchor = holeAnchor;
        }
        public Wall(List<Vector3> points, float thickness, float height)
        {
            this.points = points;
            this.thickness = thickness;
            this.height = height;
            this.holeAnchor = new List<Transform>();
        }
    }

    public class BuildingRef 
    {
        public List<Vector3> anchorLimitPoints;
        public List<Vector3> entryPoints;
        public List<int> openSides;
        public float size;
        public bool dividable;

        public BuildingRef(List<Vector3> listLimitPoint, List<Vector3> listEntryPoint = null, List<int> listOpenSide = null)
        {
            anchorLimitPoints = listLimitPoint;
            entryPoints = listEntryPoint;
            openSides = listOpenSide;
            size = CalculateBuildingArea(anchorLimitPoints);
            dividable = listEntryPoint != null || listOpenSide != null;
        }

        public static float CalculateBuildingArea(List<Vector3> limitPoints)
        {
            if (limitPoints.Count < 3)
            {
                Debug.LogError("A polygon must have at least 3 points to calculate area.");
                return 0f;
            }

            float area = 0f;
            int n = limitPoints.Count;

            for (int i = 0; i < n; i++)
            {
                Vector3 current = limitPoints[i];
                Vector3 next = limitPoints[(i + 1) % n]; // Wrap around to the first point

                //Shoelace formula
                area += (current.x * next.z) - (current.z * next.x);
            }

            return Mathf.Abs(area) / 2f;
        }
    }

    public List<GameObject> obj = new List<GameObject>();

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
    private void SubdivideFaceWithUniformTriangles(List<Vector3> verticesList, List<int> trianglesList, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, int subdivisionsX, int subdivisionsY, bool squareShape = false)
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

        Vector3 step01 = (v1 - v0);
        Vector3 step12 = (v2 - v1);
        Vector3 step32 = (v2 - v3);
        Vector3 step03 = (v3 - v0);

        // Loop through each subdivision to create uniform quads and split them into triangles
        for (int i = 0; i < subdivisionsX; i++)
        {
            for (int j = 0; j < subdivisionsY; j++)
            {
                if (squareShape)// Bottom-left corner of the current quad
                {
                    Vector3 bl = v0 + stepX * i + stepY * j;
                    Vector3 br = bl + stepX;  // Bottom-right
                    Vector3 tl = bl + stepY;  // Top-left
                    Vector3 tr = bl + stepX + stepY;  // Top-right

                    verticesList.Add(bl);  // 0
                    verticesList.Add(br);  // 1
                    verticesList.Add(tl);  // 2
                    verticesList.Add(tr);  // 3 
                }
                else
                {
                    //allow the creation of trapezoïdal shape that can be subdivide
                    Vector3 p0 = v0 + step01 / subdivisionsX * i + (i / subdivisionsX * step03 + (1 - i / subdivisionsX) * step12) / subdivisionsY * j;
                    Vector3 p1 = v0 + step01 / subdivisionsX * (i + 1) + (i / subdivisionsX * step03 + (1 - i / subdivisionsX) * step12) / subdivisionsY * j;
                    Vector3 p2 = v0 + step01 / subdivisionsX * (i + 1) + (i / subdivisionsX * step03 + (1 - i / subdivisionsX) * step12) / subdivisionsY * (j + 1);
                    Vector3 p3 = v0 + step01 / subdivisionsX * i + (i / subdivisionsX * step03 + (1 - i / subdivisionsX) * step12) / subdivisionsY * (j + 1);

                    verticesList.Add(p0);  // 0
                    verticesList.Add(p1);  // 1
                    verticesList.Add(p3);  // 2
                    verticesList.Add(p2);  // 3
                }

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

    //Create a wall
    public void GenerateWall(List<Vector3>points, float heightList = 2f, float thicknessList = 0.1f)
    {
        if (points == null || points.Count < 2)
        {
            Debug.LogError("You need at least two points to generate a wall.");
            return;
        }
        bool roundWall = points[0] == points[^1];//^1 : point.Count -1

        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        Vector3 p0Inner = new Vector3();
        Vector3 p0TopIn = new Vector3();
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 direction = (p2 - p1).normalized;
            Vector3 normal = new Vector3(-direction.z, 0, direction.x);
            int distance = Mathf.RoundToInt((p2 - p1).magnitude);

            // Outer and inner points
            Vector3 p1Outer = p1 + normal * thicknessList / 2;
            Vector3 p1Inner = p1 - normal * thicknessList / 2;
            Vector3 p1TopOut = p1Outer + Vector3.up * heightList;
            Vector3 p1TopIn = p1Inner + Vector3.up * heightList;

            Vector3 p2Outer = p2 + normal * thicknessList / 2;
            Vector3 p2Inner = p2 - normal * thicknessList / 2;
            Vector3 p2TopOut = p2Outer + Vector3.up * heightList;
            Vector3 p2TopIn = p2Inner + Vector3.up * heightList;

            //Front 
            SubdivideFaceWithUniformTriangles(vertices, triangles, p1Inner, p2Inner, p2TopIn, p1TopIn, 1, 1); //distance, Mathf.RoundToInt(heightList[i]));
            //Back 
            SubdivideFaceWithUniformTriangles(vertices, triangles, p1Outer, p1TopOut, p2TopOut,  p2Outer, 1, 1); //Mathf.RoundToInt(heightList[i]), distance);
            //Top 
            SubdivideFaceWithUniformTriangles(vertices, triangles, p1TopIn, p2TopIn, p2TopOut, p1TopOut, 1, 1);
            if (i > 0)
            {
                SubdivideFaceWithUniformTriangles(vertices, triangles, p0Inner, p1Inner, p1TopIn, p0TopIn, 1, 1);
            }
            //Bot 
            SubdivideFaceWithUniformTriangles(vertices, triangles, p2Outer, p2Inner, p1Inner, p1Outer, 1, 1);

            //Right
            if (i > 0) 
            {
                SubdivideFaceWithUniformTriangles(vertices, triangles, p0Inner, p1Inner, p1TopIn, p0TopIn, 1, 1);
            }
            else
            {
                if (roundWall)
                {
                    Vector3 directionFinal = (points[^1] - points[^2]).normalized;
                    Vector3 normalFinal = new Vector3(-directionFinal.z, 0, directionFinal.x);
                    Vector3 pFinalInner = points[0] - normalFinal * thicknessList / 2;
                    Vector3 pFinalTopIn = pFinalInner + Vector3.up * heightList;
                    SubdivideFaceWithUniformTriangles(vertices, triangles, pFinalInner, p1Inner, p1TopIn, pFinalTopIn, 1, 1);
                }
                else
                {
                    SubdivideFaceWithUniformTriangles(vertices, triangles, p1Outer, p1Inner, p1TopIn, p1TopOut, 1, 1);
                }
            }

            //Left 
            if (i < points.Count - 2 || roundWall)
            {
                Vector3 direction23 = roundWall && i >= points.Count - 2 ? (points[1]-points[0]).normalized : (points[i + 2] - p2).normalized;
                int index = roundWall && i >= points.Count - 2 ? 1 : i + 2;
                Vector3 normal23 = new Vector3(-direction23.z, 0, direction23.x);
                Vector3 p2BisOuter = p2 + normal23 * thicknessList / 2;
                Vector3 p2BisTopOut = p2BisOuter + Vector3.up * heightList;
                Vector3 p2BisInner = p2 - normal23 * thicknessList / 2;
                Vector3 p2BisTopIn = p2BisInner + Vector3.up * heightList;

                SubdivideFaceWithUniformTriangles(vertices, triangles, p2BisOuter, p2Outer, p2TopOut, p2BisTopOut, 1, 1);

                //Top and bot faces needed to cover connection holes
                if (Vector3.Cross(direction, direction23).normalized.y > 0)
                {
                    SubdivideFaceWithUniformTriangles(vertices, triangles, p2TopIn, p2BisTopOut, p2TopOut, p2BisTopIn, 1, 1);
                    SubdivideFaceWithUniformTriangles(vertices, triangles, p2BisOuter, p2Inner, p2BisInner, p2Outer, 1, 1);
                }
                else
                {
                    SubdivideFaceWithUniformTriangles(vertices, triangles, p2BisTopOut, p2TopIn, p2BisTopIn, p2TopOut, 1, 1);
                    SubdivideFaceWithUniformTriangles(vertices, triangles, p2Inner, p2BisOuter, p2Outer, p2BisInner, 1, 1);
                }
            }
            else
            {
                SubdivideFaceWithUniformTriangles(vertices, triangles, p2Inner, p2Outer, p2TopOut, p2TopIn, 1, 1);
            }

            p0Inner = p2Inner;
            p0TopIn = p2TopIn;
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
        obj.Add(cube);
    }

    //From anchor to BuildingRef
    List<Vector3>BuildingRefViaAnchor(Transform districtAnchor)
    {
        float bottomHeightRef = districtAnchor.position.y - districtAnchor.localScale.y / 2;
        return new() {
            new Vector3(districtAnchor.position.x - districtAnchor.localScale.x/2, bottomHeightRef, districtAnchor.position.z - districtAnchor.localScale.z/2),
            new Vector3(districtAnchor.position.x + districtAnchor.localScale.x/2, bottomHeightRef, districtAnchor.position.z - districtAnchor.localScale.z/2),
            new Vector3(districtAnchor.position.x + districtAnchor.localScale.x/2, bottomHeightRef, districtAnchor.position.z + districtAnchor.localScale.z/2),
            new Vector3(districtAnchor.position.x - districtAnchor.localScale.x/2, bottomHeightRef, districtAnchor.position.z + districtAnchor.localScale.z/2),
            new Vector3(districtAnchor.position.x - districtAnchor.localScale.x/2, bottomHeightRef, districtAnchor.position.z - districtAnchor.localScale.z/2) //To close the loop
        };
    }
    
    //Divide a surface into list of BuildingReference
    public void SurfaceDivider(List<Vector3> anchorLimitPoints, List<Vector3> entryPoints, float streetWidth = 1f)
    {
        List<List<Vector3>> returnBuildingRefList = new List<List<Vector3>>();

        List<Vector3> overallStreetPointList = new();

        //bool PointOnASegment(Vector3 point, Vector3 s1, Vector3 s2, float approx = 1E-4f)
        //=> Vector2.Dot(point - s1, s2 - s1) > 0 && Vector2.Dot(point - s1, s2 - s1) < (s2 - s1).sqrMagnitude && Vector3.Cross(s2 - s1, point - s1).sqrMagnitude < approx;
        bool PointOnASegment(Vector3 point, Vector3 s1, Vector3 s2, float approx = 1E-4f)
        => Mathf.Abs(Vector3.Distance(s2, s1)-(Vector3.Distance(s2, point)+Vector3.Distance(point, s1))) < approx;

        bool PointOnABuildingRef(Vector3 entryPoint, List<Vector3> buildingRef, int indexSegment)
            => indexSegment == buildingRef.Count - 1 ?
            PointOnASegment(entryPoint, buildingRef[indexSegment], buildingRef[0]) : PointOnASegment(entryPoint, buildingRef[indexSegment], buildingRef[indexSegment + 1]);

        Vector3 ClosestPoint(Vector3 positionReference, List<Vector3> pointList)
        {
            float minDistence = Vector3.Distance(pointList[0], positionReference);
            int minIndex = 0;
            for (int index = 1; index < pointList.Count; index++)
            {
                if (minDistence > Vector3.Distance(pointList[index], positionReference))
                {
                    minDistence = Vector3.Distance(pointList[index], positionReference);
                    minIndex = index;
                }
            }
            return pointList[minIndex];
        }

        Vector3 PivotBetweenTwoPoints(Vector3 point1, Vector3 point2, List<Vector3> avoidPointList = default)
        {
            if (avoidPointList != default)
            {
                Vector3 possiblePivot = new Vector3(point1.x, point1.y, point2.z);
                bool viable = true;
                for (int i = 0; i < avoidPointList.Count - 1; i++)
                {
                    if (PointOnABuildingRef(possiblePivot, avoidPointList, i)) { viable = false; }
                }
                return viable ? possiblePivot : new Vector3(point2.x, point1.y, point1.z);
            }
            else
            {
                return Random.Range(0, 2) == 0 ? new Vector3(point1.x, point1.y, point2.z) : new Vector3(point2.x, point1.y, point1.z);
            }
        }

        List<Vector3> RandomBevelPivot(Vector3 point1, Vector3 pivot, Vector3 point2, float bevelMax = -1, float bevelMin = 0)
        {
            float offset = bevelMax == -1 ? Random.Range(bevelMin, Mathf.Min(Vector3.Distance(point1, pivot), Vector3.Distance(pivot, point2))) : Random.Range(bevelMin, bevelMax); // Random.Range(0, 10)/10
            return new() { point1, point1 + offset * Vector3.Normalize(pivot - point1), pivot + (Mathf.Min(Vector3.Distance(point1, pivot), Vector3.Distance(pivot, point2)) - offset) * Vector3.Normalize(point2 - pivot), point2 };
        }

        List<Vector3> BevelBuildingRef(List<Vector3> buildingRef, int straightDenominatorProbability = 2)
        {
            List<Vector3> returnList = new();
            for (int i = 0; i < buildingRef.Count - 1; i++)
            {
                if (Random.Range(0, straightDenominatorProbability) == 0) { returnList.Add(buildingRef[i]); }
                else
                {
                    int minIndex = i == 0 ? buildingRef.Count - 1 : i - 1;

                    List<Vector3> bevelList = new List<Vector3>();
                    bevelList = i == 0 ? RandomBevelPivot(buildingRef[0], buildingRef[i], buildingRef[i + 1]) : RandomBevelPivot(returnList[^1], buildingRef[i], buildingRef[i + 1]);

                    returnList.Add(bevelList[1]);
                    returnList.Add(bevelList[2]);
                }
            }
            try { returnList.Add(buildingRef[0]); } catch { }
            return returnList;
        }

        List<Vector3> ManageStreetWidth(Vector3 point, float streetWidth, Vector3 streetVector, bool inverted = false, Vector3 streetVectorAfterCorner = default, float setStreetWidth = -1)
        {
            List<Vector3> returnStreetList = new();
            setStreetWidth = setStreetWidth == -1 ? streetWidth : setStreetWidth;

            //StraightStreet
            if (streetVectorAfterCorner == default)
            {
                returnStreetList.Add(point + Mathf.Pow(-1, inverted ? 1:0) * Vector3.Cross(streetVector, Vector3.up).normalized * (setStreetWidth / 2));
                returnStreetList.Add(point - Mathf.Pow(-1, inverted ? 1 : 0) * Vector3.Cross(streetVector, Vector3.up).normalized * (setStreetWidth / 2));
            }
            //Corner
            else
            {
                returnStreetList.Add(point + Mathf.Pow(-1, inverted ? 1 : 0) * Vector3.Cross(streetVector, Vector3.up).normalized * (setStreetWidth / 2) + (point - Mathf.Pow(-1, inverted ? 1 : 0) * Vector3.Cross(streetVectorAfterCorner, Vector3.up).normalized * (setStreetWidth / 2) - point));
                returnStreetList.Add(point - Mathf.Pow(-1, inverted ? 1 : 0) * Vector3.Cross(streetVector, Vector3.up).normalized * (setStreetWidth / 2) + (point + Mathf.Pow(-1, inverted ? 1 : 0) * Vector3.Cross(streetVectorAfterCorner, Vector3.up).normalized * (setStreetWidth / 2) - point));
            }
            return returnStreetList;
        }

        List<List<Vector3>> StreetPointCreation(Vector3 entryPoint0, Vector3 entryPoint1, float streetWidth)
        {
            if (entryPoint0.x == entryPoint1.x || entryPoint0.z == entryPoint1.z)
            {
                return new List<List<Vector3>>() {
                    ManageStreetWidth(entryPoint0, streetWidth, Vector3.Normalize(entryPoint1 - entryPoint0)),
                    ManageStreetWidth(entryPoint1, streetWidth, Vector3.Normalize(entryPoint1 - entryPoint0))
                };
            }
            else
            {
                Vector3 middle = RandomMiddlePoint(entryPoint0 + streetWidth * Vector3.Normalize(entryPoint1 - entryPoint0), entryPoint1 - streetWidth * Vector3.Normalize(entryPoint1 - entryPoint0));
                Vector3 pivot0_Mid = PivotBetweenTwoPoints(entryPoint0, middle, anchorLimitPoints);
                Vector3 pivot1_Mid = PivotBetweenTwoPoints(entryPoint1, middle, anchorLimitPoints);

                overallStreetPointList.Add(middle);
                overallStreetPointList.Add(pivot0_Mid);
                overallStreetPointList.Add(pivot1_Mid);

                return new List<List<Vector3>>()
                {
                    ManageStreetWidth(entryPoint0, streetWidth, Vector3.Normalize(pivot0_Mid - entryPoint0)),
                    ManageStreetWidth(pivot0_Mid, streetWidth, Vector3.Normalize(Vector3.Normalize(middle - pivot0_Mid)+Vector3.Normalize(pivot0_Mid - entryPoint0)), false, default, Mathf.Sqrt(2)*streetWidth),
                    //ManageStreetWidth(middle, streetWidth, Vector3.Normalize(pivot1_Mid - middle)),
                    ManageStreetWidth(pivot1_Mid, streetWidth, Vector3.Normalize(Vector3.Normalize(entryPoint1 - pivot1_Mid)+Vector3.Normalize(pivot1_Mid-middle)), false, default, Mathf.Sqrt(2)*streetWidth),
                    ManageStreetWidth(entryPoint1, streetWidth, Vector3.Normalize(entryPoint1 - pivot1_Mid))
                };
            }
        }

        void DivideABuildingRef(List<Vector3> buildingRef, List<Vector3> entryPointList)
        {
            bool inList1 = true;
            List<Vector3> buildingRefDivision1 = new();
            List<Vector3> buildingRefDivision2 = new();

            BuildingRef building = new(buildingRef, entryPointList);
            List<List<Vector3>> streetPointList = StreetPointCreation(building.entryPoints[0], building.entryPoints[1], streetWidth);

            void CheckingRightLink(List<Vector3> offsetPointList, Vector3 closestPoint, bool inverted = false)
            {
                int BoolToInt(bool TrueToOne, bool invert = false) => TrueToOne ? (invert ? 0 : 1) : (invert ? 1 : 0);
                buildingRefDivision1.Add(offsetPointList[BoolToInt(!(Vector3.Distance(offsetPointList[0], closestPoint) < Vector3.Distance(offsetPointList[1], closestPoint)), inverted)]);
                buildingRefDivision2.Add(offsetPointList[BoolToInt(Vector3.Distance(offsetPointList[0], closestPoint) < Vector3.Distance(offsetPointList[1], closestPoint), inverted)]);
            }

            void DistributeStreetPoints(List<Vector3> buildingRefDiv, int divRef, bool inverted = false)
            {
                if (streetPointList.Count <= 2)
                {
                    return;
                }
                if (inverted)
                {
                    for (int i = streetPointList.Count - 2; i > 0; i--)
                    {
                        buildingRefDiv.Add(streetPointList[i][divRef]);
                    }
                }
                else
                {
                    for (int i = 1; i < streetPointList.Count - 1; i++)
                    {
                        buildingRefDiv.Add(streetPointList[i][divRef]);
                    }
                }
            }


            //Insert the street in each list
            //Avoid a misspicking of offsetedStreetPoints
            bool wrongPlacement = false;
            for (int i = 0; i < building.anchorLimitPoints.Count; i++)
            {
                if (PointOnABuildingRef(building.entryPoints[0], building.anchorLimitPoints, i))
                {
                    wrongPlacement = false;
                    break;
                }
                else if(PointOnABuildingRef(building.entryPoints[1], building.anchorLimitPoints, i))
                {
                    wrongPlacement = true;
                    break;
                }
            }
            
            //Divide building Ref into two buildingRef
            for (int i = 0; i < building.anchorLimitPoints.Count; i++)
            {
                //Put the buildingRefPoint in the correct subdivision
                if (inList1) { buildingRefDivision1.Add(building.anchorLimitPoints[i]); }
                else { buildingRefDivision2.Add(building.anchorLimitPoints[i]); }

                for (int j = 0; j < building.entryPoints.Count; j++)
                { 
                    if (PointOnABuildingRef(building.entryPoints[j], building.anchorLimitPoints, i))
                    {
                        if (building.entryPoints[j] == building.entryPoints[0])
                        {
                            CheckingRightLink(streetPointList[0], building.anchorLimitPoints[i], wrongPlacement);
                        }
                        else
                        {
                            CheckingRightLink(streetPointList[^1], building.anchorLimitPoints[i], !wrongPlacement); // true
                        }

                        if (buildingRefDivision2.Count == 1)
                        {
                            if (building.entryPoints[j] == building.entryPoints[0])
                            {
                                DistributeStreetPoints(buildingRefDivision1, wrongPlacement?1:0); //0
                            }
                            else
                            {
                                DistributeStreetPoints(buildingRefDivision1, wrongPlacement ? 1 : 0, true); //0
                            }
                        }
                        else
                        {
                            if (building.entryPoints[j] == building.entryPoints[0])
                            {
                                DistributeStreetPoints(buildingRefDivision2, wrongPlacement ? 0:1); //1
                            }
                            else
                            {
                                DistributeStreetPoints(buildingRefDivision2, wrongPlacement ? 0 : 1, true); //1
                            }
                        }

                        inList1 = !inList1;
                    }
                }
            }
            try { buildingRefDivision2.Add(buildingRefDivision2[0]); } catch { }

            returnBuildingRefList.Add(buildingRefDivision1);
            returnBuildingRefList.Add(buildingRefDivision2);
        }


        DivideABuildingRef(anchorLimitPoints, new() { entryPoints[0], entryPoints[1] });

        for(int entryPointIndex = 2; entryPointIndex < entryPoints.Count; entryPointIndex++)
        {
            bool buildingRefFound = false;
            for (int i = 0; i < returnBuildingRefList.Count; i++)
            {
                if (buildingRefFound) { break; }

                for (int j = 0; j < returnBuildingRefList[i].Count; j++)
                {
                    if (PointOnABuildingRef(entryPoints[entryPointIndex], returnBuildingRefList[i], j))
                    {
                        List<Vector3> buildingRefToDivide = returnBuildingRefList[i];
                        returnBuildingRefList.Remove(returnBuildingRefList[i]);//RemoveAt(i);

                        //connect then the new entry point to the nearest streetSection
                        Vector3 closestStreetPoint = ClosestPoint(entryPoints[entryPointIndex], overallStreetPointList);
                        Vector3 closestBuildingPoint = ClosestPoint(closestStreetPoint, buildingRefToDivide);
                        int closeIndex = buildingRefToDivide.IndexOf(closestBuildingPoint);
                        int uperIndex = closeIndex == buildingRefToDivide.Count-1 ? 0 : closeIndex + 1;
                        int underIndex = closeIndex == 0 ? buildingRefToDivide.Count - 1 : closeIndex - 1;
                        List<Vector3> listPossibleEntryPoint = new List<Vector3>();
                        for(int a = 0; a < Mathf.Max(Vector3.Distance(buildingRefToDivide[closeIndex], buildingRefToDivide[uperIndex]), Vector3.Distance(buildingRefToDivide[closeIndex], buildingRefToDivide[underIndex]))-streetWidth/2; a++)
                        {
                            if(a+streetWidth/2 < Vector3.Distance(buildingRefToDivide[closeIndex], buildingRefToDivide[uperIndex])) 
                            { listPossibleEntryPoint.Add(closestBuildingPoint + (a + streetWidth / 2) * Vector3.Normalize(buildingRefToDivide[uperIndex]- buildingRefToDivide[closeIndex])); }
                            if (a + streetWidth / 2 < Vector3.Distance(buildingRefToDivide[closeIndex], buildingRefToDivide[underIndex]))
                            { listPossibleEntryPoint.Add(buildingRefToDivide[underIndex] + (a + streetWidth / 2) * Vector3.Normalize(buildingRefToDivide[closeIndex] - buildingRefToDivide[underIndex])); }
                        }
                        Vector3 entryPointOnBuilding = ClosestPoint(entryPoints[entryPointIndex], listPossibleEntryPoint);
                        Debug.Log(entryPointOnBuilding);
                        
                        DivideABuildingRef(buildingRefToDivide, new() { entryPoints[entryPointIndex],  entryPointOnBuilding});

                        buildingRefFound = true;

                        break;
                    }
                }
            }
        }
        

        //Draw the building for every buildingRef
        int indexBuilding = 0;
        for(int objIndex = obj.Count-1;objIndex>=0;objIndex--) { if (obj[objIndex] != null) { GameObject.Destroy(obj[objIndex]); } }
        foreach (List<Vector3> buildingRef in returnBuildingRefList)
        {
            //GenerateWall(buildingRef, districtAnchor.localScale.y - 1 + Random.Range(-1, 1));
            GenerateWall(BevelBuildingRef(buildingRef, 1), districtAnchor.localScale.y);
            indexBuilding++;
        }
    }



        //_________________________________HELPERS______________________________________________
        //Define MiddlePoint
    Vector3 RandomMiddlePoint(Vector3 point1, Vector3 point2, float distanceFromEndSegment = 0) 
        => point1 + Vector3.Normalize(point2 - point1) * 
        ( distanceFromEndSegment == 0 ? Random.Range(0, Vector3.Distance(point2, point1)) : Random.Range(distanceFromEndSegment, Vector3.Distance(point2, point1) - distanceFromEndSegment ));

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
        if(wall) { GenerateWall(pointsList); wall = false; }
        if (district) 
        {
            SurfaceDivider(BuildingRefViaAnchor(districtAnchor), entryPointsList); 
            district = false; 
        }
    }
}
/*
        // p1     pivot             p1   pivot1
        // x-----x                  x---x           *Distence(p1,pivot1)=Distence(p1,pivot)-offset
        //       |        ===>           \
        //       |          z             x pivot2
        //       | p2       |_x           |
        //       x                        x p2
        //  OR
        //  x p1
        //  |
        //  | pivot
        //  x------x p2
        */
