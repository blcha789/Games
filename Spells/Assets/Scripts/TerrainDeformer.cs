using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDeformer : MonoBehaviour {

    private Terrain terrain;
    protected int hmWidth;
    protected int hmHeight;

    // Use this for initialization
    void Start()
    {
        terrain = GetComponent<Terrain>();
        hmWidth = terrain.terrainData.heightmapWidth;
        hmHeight = terrain.terrainData.heightmapHeight;
    }
	

    public void DestroyTerrain(Vector3 pos, int defSize, int i)
    {
        if (i == 1)
            DeformTerrainWall(pos, defSize);
        else
            DeformTerrainHole(pos, defSize);
    }

    void DeformTerrainWall(Vector3 pos, int defSize)
    {
        Vector3 terrainPos = GetRelativeTerrainPositionFromPos(pos, terrain, hmWidth, hmHeight);

        int heightMapCraterWidth = defSize;
        int heightMapCraterLength = (int)(defSize / 3);

        int heightMapStartPosX = (int)(terrainPos.x - (heightMapCraterWidth / 2));
        int heightMapStartPosZ = (int)(terrainPos.z - (heightMapCraterLength / 2f));

        float[,] heights = terrain.terrainData.GetHeights(heightMapStartPosX, heightMapStartPosZ, heightMapCraterWidth, heightMapCraterLength);

        for (int i = 0; i < heightMapCraterLength; i++) //width
        {
            for (int j = 0; j < heightMapCraterWidth; j++) //height
            {
                heights[i, j] = Mathf.Clamp(heights[i, j] + 0.005f, 0, 1);
            }
        }

        terrain.terrainData.SetHeights(heightMapStartPosX, heightMapStartPosZ, heights);
    }

    void DeformTerrainHole(Vector3 pos, int defSize)
    {
        Vector3 terrainPos = GetRelativeTerrainPositionFromPos(pos, terrain, hmWidth, hmHeight);

        int heightMapCraterWidth = defSize;
        int heightMapCraterLength = defSize;

        int heightMapStartPosX = (int)(terrainPos.x - (heightMapCraterWidth / 2));
        int heightMapStartPosZ = (int)(terrainPos.z - (heightMapCraterLength / 2f));

        float[,] heights = terrain.terrainData.GetHeights(heightMapStartPosX, heightMapStartPosZ, heightMapCraterWidth, heightMapCraterLength);

        for (int i = 0; i < heightMapCraterLength; i++) //width
        {
            for (int j = 0; j < heightMapCraterWidth; j++) //height
            {
                heights[i, j] = Mathf.Clamp(heights[i, j] - 0.002f, 0, 1);
            }
        }

        terrain.terrainData.SetHeights(heightMapStartPosX, heightMapStartPosZ, heights);
    }

    protected Vector3 GetRelativeTerrainPositionFromPos(Vector3 pos, Terrain terrain, int mapWidth, int mapHeight)
    {
        Vector3 coord = GetNormalizedPositionRelativeToTerrain(pos, terrain);
        // get the position of the terrain heightmap where this game object is
        return new Vector3((coord.x * mapWidth), 0, (coord.z * mapHeight));
    }

    protected Vector3 GetNormalizedPositionRelativeToTerrain(Vector3 pos, Terrain terrain)
    {
        //code based on: http://answers.unity3d.com/questions/3633/modifying-terrain-height-under-a-gameobject-at-runtime
        // get the normalized position of this game object relative to the terrain
        Vector3 tempCoord = (pos - terrain.gameObject.transform.position);
        Vector3 coord;
        coord.x = tempCoord.x / terrain.terrainData.size.x;
        coord.y = tempCoord.y / terrain.terrainData.size.y;
        coord.z = tempCoord.z / terrain.terrainData.size.z;

        return coord;
    }
}
