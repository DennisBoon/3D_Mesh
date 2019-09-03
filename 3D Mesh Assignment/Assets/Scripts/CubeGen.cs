using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeGen : MonoBehaviour
{
    public int xLength, yLength, zLength;

    private Mesh mesh;
    private Vector3[] verts;

    void Start()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Generated Cube";

        int cornerVerts = 8;
        int edgeVerts = (xLength + yLength + zLength - 3) * 4;
        int faceVerts = (
            (xLength - 1) * (yLength - 1) +
            (xLength - 1) * (zLength - 1) +
            (yLength - 1) * (zLength - 1)) * 2;
        verts = new Vector3[cornerVerts + edgeVerts + faceVerts];

        int v = 0;
        for (int x = 0; x <= xLength; x++)
        {
            verts[v++] = new Vector3(x, 0, 0);
        }
    }
}
