using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateTerrain : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    Vector3[] normals;
    Vector2[] uv;
    int[] triangles;

    public int xLength = 20;
    public int zLength = 20;
    public float scale, noiseAmount, offsetX, offsetZ;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Randomized variable to make the look of the terrain random
        offsetX = Random.Range(0f, 99999f);
        offsetZ = Random.Range(0f, 99999f);
    }

    private void Update()
    {
        CreateTerrain();
        UpdateMesh();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            offsetX = Random.Range(0f, 99999f);
            offsetZ = Random.Range(0f, 99999f);
        }
    }

    void CreateTerrain()
    {
        vertices = new Vector3[(xLength + 1) * (zLength + 1)];
        normals = new Vector3[vertices.Length];
        uv = new Vector2[vertices.Length];

        for (int i = 0, z = 0; z <= zLength; z++)
        {
            for (int x = 0; x <= xLength; x++)
            {
                float xCoord = (float)x / xLength * scale + offsetX;
                float zCoord = (float)z / zLength * scale + offsetZ;
                float y = Mathf.PerlinNoise(xCoord * noiseAmount, zCoord * noiseAmount);
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[xLength * zLength * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zLength; z++)
        {
            for (int x = 0; x < xLength; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xLength + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xLength + 1;
                triangles[tris + 5] = vert + xLength + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
