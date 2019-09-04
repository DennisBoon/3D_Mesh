using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeGen : MonoBehaviour
{
    public int xLength, yLength, zLength;

    private Mesh mesh;
    private Vector3[] verts;

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        //mesh.Clear();
        mesh.name = "Generated Cube";
        CreateVerts();
        CreateTris();
    }

    private void CreateVerts()
    {
        int cornerVerts = 8;
        int edgeVerts = (xLength + yLength + zLength - 3) * 4;
        int faceVerts = (
            (xLength - 1) * (yLength - 1) +
            (xLength - 1) * (zLength - 1) +
            (yLength - 1) * (zLength - 1)) * 2;
        verts = new Vector3[cornerVerts + edgeVerts + faceVerts];

        int v = 0;

        for (int y = 0; y <= yLength; y++)
        {
            for (int x = 0; x <= xLength; x++)
            {
                verts[v++] = new Vector3(x, y, 0);
            }
            for (int z = 1; z <= zLength; z++)
            {
                verts[v++] = new Vector3(xLength, y, z);
            }
            for (int x = xLength - 1; x >= 0; x--)
            {
                verts[v++] = new Vector3(x, y, zLength);
            }
            for (int z = zLength - 1; z > 0; z--)
            {
                verts[v++] = new Vector3(0, y, z);
            }
        }

        for (int z = 1; z < zLength; z++)
        {
            for (int x = 1; x < xLength; x++)
            {
                verts[v++] = new Vector3(x, yLength, z);
            }
        }
        for (int z = 1; z < zLength; z++)
        {
            for (int x = 1; x < xLength; x++)
            {
                verts[v++] = new Vector3(x, 0, z);
            }
        }

        mesh.vertices = verts;
    }

    private void CreateTris()
    {
        int quads = (xLength * yLength + xLength * zLength + yLength * zLength) * 2;
        int[] triangles = new int[quads * 6];
        int ring = (xLength + zLength) * 2;
        int t = 0, v = 0;

        for (int y = 0; y < yLength; y++, v++)
        {
            for (int q = 0; q < ring - 1; q++, v++)
            {
                t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
            }
            t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
        }

        mesh.triangles = triangles;
    }

    private static int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
        return i + 6;
    }

    private void OnDrawGizmos()
    {
        if (verts == null)
        {
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < verts.Length; i++)
        {
            Gizmos.DrawSphere(verts[i], 0.1f);
        }
    }
}
