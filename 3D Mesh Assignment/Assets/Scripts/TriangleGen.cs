using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TriangleGen : MonoBehaviour
{
    Mesh mesh;
    Vector3[] verts;
    int[] tris;
    public int xLength = 10, yLength = 10;

    void Start()
    {
        // assign objects mesh
        mesh = GetComponent<MeshFilter>().mesh;

        // mesh name
        mesh.name = "Vertex Strip";

        // clear objects mesh to make sure it's empty
        mesh.Clear();

        // create vertices
        verts = new Vector3[(xLength + 1) * (yLength + 1)];
        Vector2[] uv = new Vector2[verts.Length];
        Vector4[] tangents = new Vector4[verts.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, y = 0; y <= yLength; y++)
        {
            for (int x = 0; x <= xLength; x++, i++)
            {
                verts[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / xLength, (float)y / yLength);
                tangents[i] = tangent;
            }
        }

        // assign created vertices to the mesh
        mesh.vertices = verts;
        mesh.uv = uv;
        mesh.tangents = tangents;

        // create triangles
        tris = new int[xLength * yLength * 6];
        for (int i = 0, j = 0, y = 0; y < yLength; y++, j++)
        {
            for (int x = 0; x < xLength; x++, i += 6, j++)
            {
                tris[i] = j;
                tris[i + 3] = tris[i + 2] = j + 1;
                tris[i + 4] = tris[i + 1] = j + xLength + 1;
                tris[i + 5] = j + xLength + 2;
            }
        }

        // faces 
        mesh.triangles = tris;
        mesh.RecalculateNormals();
    }
}
