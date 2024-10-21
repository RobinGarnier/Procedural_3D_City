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
    public bool cubeType = false;
    public bool useRelDimension = false;



    public void CreateCube(Vector3 dimensionXYZ, float VerticeSpace=7)
    {

        //1) Create an empty GameObject with the required Components
        cube = new GameObject("Cube");
        cube.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = cube.AddComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;


        //Create a 'Cube' mesh...

        //2) Define the cube's dimensions
        float length = dimensionXYZ[0];
        float height = dimensionXYZ[1];
        float thickness = dimensionXYZ[2];


        //3) Define the co-ordinates of each Corner of the cube 
        Vector3[] c = new Vector3[8];

        c[0] = new Vector3(-length * VerticeSpace/2, -height * VerticeSpace / 2, thickness * VerticeSpace / 2);
        c[1] = new Vector3(length * VerticeSpace / 2, -height * VerticeSpace / 2, thickness * VerticeSpace / 2);
        c[2] = new Vector3(length * VerticeSpace / 2, -height * VerticeSpace / 2, -thickness * VerticeSpace / 2);
        c[3] = new Vector3(-length * VerticeSpace / 2, -height * VerticeSpace / 2, -thickness * VerticeSpace / 2);

        c[4] = new Vector3(-length * VerticeSpace / 2, height * VerticeSpace / 2, thickness * VerticeSpace / 2);
        c[5] = new Vector3(length * VerticeSpace / 2, height * VerticeSpace / 2, thickness * VerticeSpace / 2);
        c[6] = new Vector3(length * VerticeSpace / 2, height * VerticeSpace / 2, -thickness * VerticeSpace / 2);
        c[7] = new Vector3(-length * VerticeSpace / 2, height * VerticeSpace / 2, -thickness * VerticeSpace / 2);


        //4) Define the vertices that the cube is composed of:
        //I have used 16 vertices (4 vertices per side). 
        //This is because I want the vertices of each side to have separate normals.
        //(so the object renders light/shade correctly) 
        Vector3[] vertices = new Vector3[]
        {
            c[0], c[1], c[2], c[3], // Bottom
	        c[7], c[4], c[0], c[3], // Left
	        c[4], c[5], c[1], c[0], // Front
	        c[6], c[7], c[3], c[2], // Back
	        c[5], c[6], c[2], c[1], // Right
	        c[7], c[6], c[5], c[4]  // Top
        };


        //5) Define each vertex's Normal
        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 forward = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;


        Vector3[] normals = new Vector3[]
        {
            down, down, down, down,             // Bottom
	        left, left, left, left,             // Left
	        forward, forward, forward, forward,	// Front
	        back, back, back, back,             // Back
	        right, right, right, right,         // Right
	        up, up, up, up	                    // Top
        };
         

        //6) Define each vertex's UV co-ordinates
        Vector2 uv00 = new Vector2(0f, 0f);
        Vector2 uv10 = new Vector2(1f, 0f);
        Vector2 uv01 = new Vector2(0f, 1f);
        Vector2 uv11 = new Vector2(1f, 1f);

        Vector2[] uvs = new Vector2[]
        {
            uv11, uv01, uv00, uv10, // Bottom
	        uv11, uv01, uv00, uv10, // Left
	        uv11, uv01, uv00, uv10, // Front
	        uv11, uv01, uv00, uv10, // Back	        
	        uv11, uv01, uv00, uv10, // Right 
	        uv11, uv01, uv00, uv10  // Top
        };


        //7) Define the Polygons (triangles) that make up the our Mesh (cube)
        //IMPORTANT: Unity uses a 'Clockwise Winding Order' for determining front-facing polygons.
        //This means that a polygon's vertices must be defined in 
        //a clockwise order (relative to the camera) in order to be rendered/visible.
        int[] triangles = new int[]
        {
            3, 1, 0,        3, 2, 1,        // Bottom	
	        7, 5, 4,        7, 6, 5,        // Left
	        11, 9, 8,       11, 10, 9,      // Front
	        15, 13, 12,     15, 14, 13,     // Back
	        19, 17, 16,     19, 18, 17,	    // Right
	        23, 21, 20,     23, 22, 21,	    // Top
        };


        //8) Build the Mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.Optimize();
        //mesh.RecalculateNormals();

        //cube.transform.Translate(0f, 1f, -8f);

        //9) Give it a Material
        Material cubeMaterial = new Material(Shader.Find("Standard"));
        //cubeMaterial.SetColor("_Color", new Color(0f, 0.7f, 0f)); //green main color
        cube.GetComponent<Renderer>().material = cubeMaterial;
    }

    
    public void CreatePlan(Vector3 dimensionXYZ, float VerticeSpace = 7, bool useRealDimension = false)
    {
        //1) Create an empty GameObject with the required Components
        plan = new GameObject("Plan");
        plan.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = plan.AddComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;


        //Create a 'Cube' mesh...

        //2) Define the cube's dimensions
        float length = dimensionXYZ[0];
        float height = dimensionXYZ[1];
        float width = dimensionXYZ[2];


        //3) Define the co-ordinates of each Corner of the cube 
        List<int> trianglesList = new List<int>();
        List<Vector3> verticesList = new List<Vector3>();
        List<Vector3> normalsList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();

        Vector3 up = Vector3.up;
        Vector2 uv00 = new Vector2(0f, 0f);
        Vector2 uv10 = new Vector2(1f, 0f);
        Vector2 uv01 = new Vector2(0f, 1f);
        Vector2 uv11 = new Vector2(1f, 1f);

        Vector3[] c = new Vector3[(int)((length+1)*(width+1))];
        if (useRealDimension)
        {
            c[0] = new Vector3(-length * VerticeSpace / 2, 0, width * VerticeSpace / 2);
            c[1] = new Vector3(length * VerticeSpace / 2, 0, width * VerticeSpace / 2);
            c[2] = new Vector3(length * VerticeSpace / 2, 0, -width * VerticeSpace / 2);
            c[3] = new Vector3(-length * VerticeSpace / 2, 0, -width * VerticeSpace / 2);
        }
        else
        {
            int index = 0;
            for( int i = 0; i < length + 1; i++)
            {
                for(int j = 0; j < width + 1; j++)
                {
                    c[index] = new Vector3(i * VerticeSpace, 0, j * VerticeSpace);
                    if(i!= 0) 
                    {
                        trianglesList.Add((int)(index-width));
                        trianglesList.Add((int)(index-width-1));
                        trianglesList.Add(index);
                        if (j != 0)
                        {
                            verticesList.Add(c[(int)(index-width)]);
                            verticesList.Add(c[(int)(index-width+1)]);
                            verticesList.Add(c[index-1]);
                            verticesList.Add(c[index]);

                            uvsList.Add(uv11);
                            uvsList.Add(uv01);
                            uvsList.Add(uv00);
                            uvsList.Add(uv10);

                            normalsList.Add(up);
                            normalsList.Add(up);
                            normalsList.Add(up);
                            normalsList.Add(up);
                        }
                    }
                    index++;
                }
            }
        }
        


        //4) Define the vertices that the cube is composed of:
        //I have used 16 vertices (4 vertices per side). 
        //This is because I want the vertices of each side to have separate normals.
        //(so the object renders light/shade correctly) 
        Vector3[] vertices = new Vector3[]
        {
            c[0], c[(int)width], c[c.Length-1], c[c.Length-1-(int)width]
        };
        if (useRealDimension ==false)
        {
            vertices = verticesList.ToArray();
        }
        Debug.Log($"vertice : ${vertices.Length}");


        //5) Define each vertex's Normal
        //Vector3 up = Vector3.up;


        Vector3[] normals = new Vector3[] {up, up, up, up	};
        //normals = new List<Vector3> { vertices.Length * up }.ToArray();
        if (useRealDimension==false)
        {
            normals = normalsList.ToArray();
        }
        Debug.Log($"normal : ${normals.Length}");


        //6) Define each vertex's UV co-ordinates
        Vector2[] uvs = new Vector2[] { uv11, uv01, uv00, uv10 };// Top};
        if (useRealDimension == false) { uvs = uvsList.ToArray(); }
        Debug.Log($"uv : ${uvs.Length}");

        //7) Define the Polygons (triangles) that make up the our Mesh (cube)
        //IMPORTANT: Unity uses a 'Clockwise Winding Order' for determining front-facing polygons.
        //This means that a polygon's vertices must be defined in 
        //a clockwise order (relative to the camera) in order to be rendered/visible.
        int[] triangles = new int[] { 3, 0, 1,        3, 1, 2, };
        if (useRealDimension==false)
        {
            triangles = trianglesList.ToArray();
        }foreach (int elem in triangles) { Debug.Log($"tri : ${elem}");}

        //8) Build the Mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.Optimize();
        //mesh.RecalculateNormals();

        //cube.transform.Translate(0f, 1f, -8f);

        //9) Give it a Material
        Material cubeMaterial = new Material(Shader.Find("Standard"));
        //cubeMaterial.SetColor("_Color", new Color(0f, 0.7f, 0f)); //green main color
        plan.GetComponent<Renderer>().material = cubeMaterial;
    }
    
    
    
    // Update is called once per frame
    private void Update()
    {
        if (create) { if (cubeType) { CreateCube(dimension, VerticeSpace); } else { CreatePlan(dimension, VerticeSpace, useRelDimension); } create = false; }
    }
}
