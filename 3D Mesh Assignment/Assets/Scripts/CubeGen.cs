using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeGen : MonoBehaviour
{
    public int xLength, yLength, zLength;
    public int roundness;
    public bool drawTriangles;

    private Mesh mesh;
    private Vector3[] verts;
    private Vector3[] normals;

    private void Awake()
    {
        Generate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Generate();
        }
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.Clear();
        mesh.name = "Generated Cube";
        CreateVerts();

        if (drawTriangles == true)
        {
            CreateTris();
        }
    }

    private void CreateVerts()
    {
        int cornerVertices = 8;
        int edgeVertices = (xLength + yLength + zLength - 3) * 4;
        int faceVertices = (
            (xLength - 1) * (yLength - 1) +
            (xLength - 1) * (zLength - 1) +
            (yLength - 1) * (zLength - 1)) * 2;
        verts = new Vector3[cornerVertices + edgeVertices + faceVertices];
        normals = new Vector3[verts.Length];

        int v = 0;
        for (int y = 0; y <= yLength; y++)
        {
            for (int x = 0; x <= xLength; x++)
            {
                SetVertex(v++, x, y, 0);
            }
            for (int z = 1; z <= zLength; z++)
            {
                SetVertex(v++, xLength, y, z);
            }
            for (int x = xLength - 1; x >= 0; x--)
            {
                SetVertex(v++, x, y, zLength);
            }
            for (int z = zLength - 1; z > 0; z--)
            {
                SetVertex(v++, 0, y, z);
            }
        }
        for (int z = 1; z < zLength; z++)
        {
            for (int x = 1; x < xLength; x++)
            {
                SetVertex(v++, x, yLength, z);
            }
        }
        for (int z = 1; z < zLength; z++)
        {
            for (int x = 1; x < xLength; x++)
            {
                SetVertex(v++, x, 0, z);
            }
        }

        mesh.vertices = verts;
        mesh.normals = normals;
    }

    private void CreateTris()
    {
        int[] trianglesZ = new int[(xLength * yLength) * 12];
        int[] trianglesX = new int[(yLength * zLength) * 12];
        int[] trianglesY = new int[(xLength * zLength) * 12];
        int ring = (xLength + zLength) * 2;
        int tZ = 0, tX = 0, tY = 0, v = 0;

        for (int y = 0; y < yLength; y++, v++)
        {
            for (int q = 0; q < xLength; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < zLength; q++, v++)
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < xLength; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < zLength - 1; q++, v++)
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);
        }

        tY = CreateTopFace(trianglesY, tY, ring);
        tY = CreateBottomFace(trianglesY, tY, ring);

        mesh.subMeshCount = 3;
        mesh.SetTriangles(trianglesZ, 0);
        mesh.SetTriangles(trianglesX, 1);
        mesh.SetTriangles(trianglesY, 2);
    }

    private int CreateTopFace(int[] triangles, int t, int ring)
    {
        int v = ring * yLength;
        for (int x = 0; x < xLength - 1; x++, v++)
        {
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
        }
        t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

        int vMin = ring * (yLength + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (int z = 1; z < zLength - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + xLength - 1);
            for (int x = 1; x < xLength - 1; x++, vMid++)
            {
                t = SetQuad(
                    triangles, t,
                    vMid, vMid + 1, vMid + xLength - 1, vMid + xLength);
            }
            t = SetQuad(triangles, t, vMid, vMax, vMid + xLength - 1, vMax + 1);
        }

        int vTop = vMin - 2;
        t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
        for (int x = 1; x < xLength - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
        }
        t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

        return t;
    }

    private int CreateBottomFace(int[] triangles, int t, int ring)
    {
        int v = 1;
        int vMid = verts.Length - (xLength - 1) * (zLength - 1);
        t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
        for (int x = 1; x < xLength - 1; x++, v++, vMid++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
        }
        t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

        int vMin = ring - 2;
        vMid -= xLength - 2;
        int vMax = v + 2;

        for (int z = 1; z < zLength - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid + xLength - 1, vMin + 1, vMid);
            for (int x = 1; x < xLength - 1; x++, vMid++)
            {
                t = SetQuad(
                    triangles, t,
                    vMid + xLength - 1, vMid + xLength, vMid, vMid + 1);
            }
            t = SetQuad(triangles, t, vMid + xLength - 1, vMax + 1, vMid, vMax);
        }

        int vTop = vMin - 1;
        t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
        for (int x = 1; x < xLength - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
        }
        t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

        return t;
    }

    private static int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
        return i + 6;
    }

    private void SetVertex(int i, int x, int y, int z)
    {
        Vector3 inner = verts[i] = new Vector3(x, y, z);

        if (x < roundness)
        {
            inner.x = roundness;
        }
        else if (x > xLength - roundness)
        {
            inner.x = xLength - roundness;
        }
        if (y < roundness)
        {
            inner.y = roundness;
        }
        else if (y > yLength - roundness)
        {
            inner.y = yLength - roundness;
        }
        if (z < roundness)
        {
            inner.z = roundness;
        }
        else if (z > zLength - roundness)
        {
            inner.z = zLength - roundness;
        }

        normals[i] = (verts[i] - inner).normalized;
        verts[i] = inner + normals[i] * roundness;
    }

    private void OnDrawGizmos()
    {
        if (verts == null)
        {
            return;
        }
        for (int i = 0; i < verts.Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(verts[i], 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(verts[i], normals[i]);
        }
    }
}
