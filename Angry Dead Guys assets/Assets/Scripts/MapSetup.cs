using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSetup : MonoBehaviour
{
    public Transform mapGrid;

    public int sizeX;
    public int sizeZ;
    public float tileSize = 1.0f;

    void Start()
    {
        CreateGridMap();
    }

    private void CreateGridMap()
    {
        int numTiles = sizeX * sizeZ;
        int numTris = numTiles * 2;

        int vSizeX = sizeX + 1;
        int vSizeZ = sizeZ + 1;
        int numVerts = vSizeX * vSizeZ;

        //mesh data
        Vector3[] vertices = new Vector3[numVerts];
        int[] triangles = new int[numTris * 3];
        Vector3[] normals = new Vector3[numVerts];
        Vector2[] uv = new Vector2[numVerts];

        int x, z;
        for (z = 0; z < vSizeZ; z++)
        {
            for (x = 0; x < vSizeX; x++)
            {
                vertices[z * vSizeX + x] = new Vector3(x * tileSize, 0, z * tileSize);
                normals[z * vSizeX + x] = Vector3.up;
                uv[z * vSizeX + x] = new Vector2((float)x / vSizeX, (float)z / vSizeZ);
            }
        }

        for (z = 0; z < sizeZ; z++)
        {
            for (x = 0; x < sizeX; x++)
            {
                int squareIndex = z * sizeX + x;
                int trisOffest = squareIndex * 6;

                triangles[trisOffest + 0] = z * vSizeX + x + 0;
                triangles[trisOffest + 1] = z * vSizeX + x + vSizeX + 0;
                triangles[trisOffest + 2] = z * vSizeX + x + vSizeX + 1;

                triangles[trisOffest + 3] = z * vSizeX + x + 0;
                triangles[trisOffest + 4] = z * vSizeX + x + vSizeX + 1;
                triangles[trisOffest + 5] = z * vSizeX + x + 1;
            }
        }

        //Create mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        //Assign mesh
        MeshFilter meshFilter = mapGrid.GetComponent<MeshFilter>();
        //MeshRenderer meshRenderer = tileMap.GetComponent<MeshRenderer>();
        MeshCollider meshCollider = mapGrid.GetComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        BuildTextureTileMap();
    }

    private void BuildTextureTileMap()
    {
        //add grid texture
        MeshRenderer meshRenderer = mapGrid.GetComponent<MeshRenderer>();
        meshRenderer.materials[0].mainTextureScale = new Vector2(sizeX + 1, sizeZ + 1);
    }
}
