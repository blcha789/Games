using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//this script is for setup level (size of map, create map, create grid, spawn buyers, spawn deposits)

public class LevelSetup : MonoBehaviour
{
    [Header("Main")]
    public bool sandbox = false;
    public GameObject sandboxPanel; //panel if is sandbox level
    public GameObject constructionSignPrefab; //decorative
    public GameObject billBoardPrefab; //decorative
    public Transform tileMap; //grid
    public Transform buildingsParent;// parent of buildings
    public Transform depositsParent; //parent of deposits
    public Transform Ground;
    public Transform[] GroundSides;

    public int sizeX;
    public int sizeZ;
    public float tileSize = 1.0f;

    [Header("Raw materials-------------")]
    public Deposits[] oreDeposits;
    public Deposits[] fluidDeposits;

    [Header("Output---------------------")]
    [Header("Conveyor")]
    public GameObject conveyorOutputPrefab;
    public ConveyorOutputSetup[] conveyorOutput;

    [Header("Pipe")]
    public GameObject pipeOutputPrefab;
    public PipeOutputSetup[] pipeOutput;

    [Header("Buyers--------------------")]
    public GameObject buyerPrefab;
    public BuyerSetup[] buyerInput;

    [Header("PowerPlant----------------")]
    public GameObject powerPlant;
    public PowerPlantSetup[] powerPlantSetup;

    private GameLogic gameLogic;

    private void Start()
    {
        gameLogic = GetComponent<GameLogic>();

        if (!sandbox) //if is not sandbox then setup game
        {
            GameSetup();
        }
        else //if is sandbox then show panel with parameters that you want to change
        {
            sandboxPanel.SetActive(true);
        }
    }

    public void GameSetup()
    {
        sandboxPanel.SetActive(false);

        GameObject.FindGameObjectWithTag("Hierarchy/Camera").transform.position = new Vector3(sizeX / 2, 0, sizeZ / 2);

        TileMap();
        GroundBlock();
        Decoratives();
        OutputsInputs();

        if (gameLogic.isPowerInLevel)
            SetupPowerPlant();

        GroundBlockSideX1();
        GroundBlockSideX2();
        GroundBlockSideZ1();
        GroundBlockSideZ2();
    }

    private void TileMap()
    {
        //creating grid

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
        MeshFilter meshFilter = tileMap.GetComponent<MeshFilter>();
        //MeshRenderer meshRenderer = tileMap.GetComponent<MeshRenderer>();
        MeshCollider meshCollider = tileMap.GetComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        BuildTextureTileMap();
    }

    private void BuildTextureTileMap()
    {
        //add grid texture

        MeshRenderer meshRenderer = tileMap.GetComponent<MeshRenderer>();
        meshRenderer.materials[0].mainTextureScale = new Vector2(sizeX + 1, sizeZ + 1);
    }

    //creating map
    private void GroundBlock()
    {
        int groundSizeX = sizeX + 6;
        int groundSizeZ = sizeZ + 6;

        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[2 * 3];
        Vector3[] normals = new Vector3[4];
        Vector2[] uv = new Vector2[4];

        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(groundSizeZ, 0, 0);
        vertices[2] = new Vector3(0, 0, -groundSizeX);
        vertices[3] = new Vector3(groundSizeZ, 0, -groundSizeX);

        triangles[0] = 0;
        triangles[1] = 3;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 1;
        triangles[5] = 3;

        normals[0] = Vector3.up;
        normals[1] = Vector3.up;
        normals[2] = Vector3.up;
        normals[3] = Vector3.up;

        uv[0] = new Vector2(0, 1);
        uv[1] = new Vector2(1, 1);
        uv[2] = new Vector2(0, 0);
        uv[3] = new Vector2(1, 0);

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        //Assign mesh
        MeshFilter meshFilter = Ground.GetComponent<MeshFilter>();
        MeshCollider meshCollider = Ground.GetComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        BuildTextureGround();
    }

    private void BuildTextureGround()
    {
        int textureSizeZ = sizeX + 6;
        int textureSizeX = sizeZ + 6;

        Texture2D texture = new Texture2D(textureSizeX, textureSizeZ);
        bool colorChange = true;
        Color c;

        for (int z = 0; z < textureSizeZ; z++)
        {
            if (textureSizeX % 2 == 0) 
                colorChange = !colorChange;

            for (int x = 0; x < textureSizeX; x++)
            {
                if (colorChange)
                    c = new Color(0.54f, 0.70f, 0.29f);
                else
                    c = new Color(0.6f, 0.74f, 0.38f);

                colorChange = !colorChange;
                texture.SetPixel(x, z, c);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        MeshRenderer meshRenderer = Ground.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterials[0].mainTexture = texture;
    }


    private void GroundBlockSideX1()
    {
        int groundSizeX = sizeX + 6;
        int groundSizeZ = 2;

        int numTiles = groundSizeX * groundSizeZ;
        int numTris = numTiles * 2;//jeden stvorec sa sklada z dvoch trojuholnikov

        int vSizeX = groundSizeX + 1;
        int vSizeZ = groundSizeZ + 1;
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

        for (z = 0; z < groundSizeZ; z++)
        {
            for (x = 0; x < groundSizeX; x++)
            {
                int squareIndex = z * groundSizeX + x;
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
        MeshFilter meshFilter = GroundSides[0].GetComponent<MeshFilter>();
        MeshCollider meshCollider = GroundSides[0].GetComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        BuildTextureGroundBlockSideX1();
    }

    private void BuildTextureGroundBlockSideX1()
    {
        int textureSizeX = sizeX + 7;
        int textureSizeZ = 2;

        Texture2D texture = new Texture2D(textureSizeX, textureSizeZ);
        bool colorChange = false;
        bool isDirtDone = false;
        Color c;

        for (int z = 0; z < textureSizeZ; z++)
        {
            for (int x = 0; x < textureSizeX; x++)
            {
                if (isDirtDone)
                {
                    if (colorChange)
                        c = new Color(0.6f, 0.74f, 0.38f);
                    else
                        c = new Color(0.54f, 0.70f, 0.29f);
                }
                else
                    c = new Color(0.77f, 0.61f, 0.46f);

                colorChange = !colorChange;
                texture.SetPixel(x, z, c);

                if(x+1 == textureSizeX)
                    isDirtDone = true;
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        MeshRenderer meshRenderer = GroundSides[0].GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterials[0].mainTexture = texture;
    }


    private void GroundBlockSideZ1()
    {
        GroundSides[2].transform.position = new Vector3(-3.5f, -0.5f, sizeZ + 2.5f);

        int groundSizeX = sizeZ + 6;
        int groundSizeZ = 2;

        int numTiles = groundSizeX * groundSizeZ;
        int numTris = numTiles * 2;//jeden stvorec sa sklada z dvoch trojuholnikov

        int vSizeX = groundSizeX + 1;
        int vSizeZ = groundSizeZ + 1;
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

        for (z = 0; z < groundSizeZ; z++)
        {
            for (x = 0; x < groundSizeX; x++)
            {
                int squareIndex = z * groundSizeX + x;
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
        MeshFilter meshFilter = GroundSides[2].GetComponent<MeshFilter>();
        MeshCollider meshCollider = GroundSides[2].GetComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        BuildTextureGroundBlockSideZ1();
    }

    private void BuildTextureGroundBlockSideZ1()
    {
        int textureSizeX = sizeZ + 7;
        int textureSizeZ = 2;

        Texture2D texture = new Texture2D(textureSizeX, textureSizeZ);
        bool colorChange = false;
        bool isDirtDone = false;
        Color c;

        if (sizeX % 2 == 0)
        {
            if (sizeZ % 2 == 0)
                colorChange = false;
            else
                colorChange = true;
        }
        else
        {
            if (sizeZ % 2 == 0)
                colorChange = true;
            else
                colorChange = false;
        }

        for (int z = 0; z < textureSizeZ; z++)
        {
            for (int x = 0; x < textureSizeX; x++)
            {
                if (isDirtDone)
                {
                    if (colorChange)
                        c = new Color(0.6f, 0.74f, 0.38f);
                    else
                        c = new Color(0.54f, 0.70f, 0.29f);

                    colorChange = !colorChange;
                }
                else
                    c = new Color(0.77f, 0.61f, 0.46f);

                texture.SetPixel(x, z, c);

                if (x + 1 == textureSizeX)
                    isDirtDone = true;
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        MeshRenderer meshRenderer = GroundSides[2].GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterials[0].mainTexture = texture;
    }


    private void GroundBlockSideX2()
    {
        GroundSides[1].transform.position = new Vector3(sizeX + 2.5f, -0.5f, sizeZ + 2.5f);

        int groundSizeX = sizeX + 6;
        int groundSizeZ = 2;

        int numTiles = groundSizeX * groundSizeZ;
        int numTris = numTiles * 2;//jeden stvorec sa sklada z dvoch trojuholnikov

        int vSizeX = groundSizeX + 1;
        int vSizeZ = groundSizeZ + 1;
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

        for (z = 0; z < groundSizeZ; z++)
        {
            for (x = 0; x < groundSizeX; x++)
            {
                int squareIndex = z * groundSizeX + x;
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
        MeshFilter meshFilter = GroundSides[1].GetComponent<MeshFilter>();
        MeshCollider meshCollider = GroundSides[1].GetComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        BuildTextureGroundBlockSideX2();
    }

    private void BuildTextureGroundBlockSideX2()
    {
        int textureSizeX = sizeX + 7;
        int textureSizeZ = 2;

        Texture2D texture = new Texture2D(textureSizeX, textureSizeZ);
        bool colorChange = false;
        bool isDirtDone = false;
        Color c;

        if (sizeX % 2 == 0)
        {
            if (sizeZ % 2 == 0)
                colorChange = true;
            else
                colorChange = false;
        }
        else
        {
            if (sizeZ % 2 == 0)
                colorChange = true;
            else
                colorChange = false;
        }

        for (int z = 0; z < textureSizeZ; z++)
        {
            for (int x = 0; x < textureSizeX; x++)
            {
                if (isDirtDone)
                {
                    if (colorChange)
                        c = new Color(0.6f, 0.74f, 0.38f);
                    else
                        c = new Color(0.54f, 0.70f, 0.29f);

                    colorChange = !colorChange;
                }
                else
                    c = new Color(0.77f, 0.61f, 0.46f);

                texture.SetPixel(x, z, c);

                if (x + 1 == textureSizeX)
                    isDirtDone = true;
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        MeshRenderer meshRenderer = GroundSides[1].GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterials[0].mainTexture = texture;
    }


    private void GroundBlockSideZ2()
    {
        GroundSides[3].transform.position = new Vector3(sizeX + 2.5f, -0.5f, -3.5f);

        int groundSizeX = sizeZ + 6;
        int groundSizeZ = 2;

        int numTiles = groundSizeX * groundSizeZ;
        int numTris = numTiles * 2;//jeden stvorec sa sklada z dvoch trojuholnikov

        int vSizeX = groundSizeX + 1;
        int vSizeZ = groundSizeZ + 1;
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

        for (z = 0; z < groundSizeZ; z++)
        {
            for (x = 0; x < groundSizeX; x++)
            {
                int squareIndex = z * groundSizeX + x;
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
        MeshFilter meshFilter = GroundSides[3].GetComponent<MeshFilter>();
        MeshCollider meshCollider = GroundSides[3].GetComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        BuildTextureGroundBlockSideZ2();
    }

    private void BuildTextureGroundBlockSideZ2()
    {
        int textureSizeX = sizeZ + 7;
        int textureSizeZ = 2;

        Texture2D texture = new Texture2D(textureSizeX, textureSizeZ);
        bool colorChange = false;
        bool isDirtDone = false;
        Color c;

        for (int z = 0; z < textureSizeZ; z++)
        {
            for (int x = 0; x < textureSizeX; x++)
            {
                if (isDirtDone)
                {
                    if (colorChange)
                        c = new Color(0.6f, 0.74f, 0.38f);
                    else
                        c = new Color(0.54f, 0.70f, 0.29f);

                    colorChange = !colorChange;
                }
                else
                    c = new Color(0.77f, 0.61f, 0.46f);

                texture.SetPixel(x, z, c);

                if (x + 1 == textureSizeX)                
                    isDirtDone = true;               
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        MeshRenderer meshRenderer = GroundSides[3].GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterials[0].mainTexture = texture;
    }

    //spawn decoratives
    private void Decoratives()
    {
        Instantiate(constructionSignPrefab, new Vector3(-1, 0.86f, -1), Quaternion.Euler(0, 180, 0));
        Instantiate(constructionSignPrefab, new Vector3(sizeX , 0.86f, -1), Quaternion.Euler(0, 90, 0));
        Instantiate(constructionSignPrefab, new Vector3(-1, 0.86f, sizeZ), Quaternion.Euler(0, -90, 0));
        Instantiate(constructionSignPrefab, new Vector3(sizeX, 0.86f, sizeZ), Quaternion.Euler(0, 0, 0));

        Instantiate(billBoardPrefab, new Vector3(sizeX / 2, 0.5f, sizeZ + 1), Quaternion.identity);
        Instantiate(billBoardPrefab, new Vector3(sizeX + 1, 0.5f, sizeZ / 2), Quaternion.Euler(0, 90, 0));
    }

    //spawn buyers
    private void OutputsInputs()
    {
        for (int i = 0; i < oreDeposits.Length; i++)
        {
            GameObject con = Instantiate(oreDeposits[i].prefab, new Vector3(oreDeposits[i].position.x + 0.5f, oreDeposits[i].position.y, oreDeposits[i].position.z + 0.5f), Quaternion.identity, depositsParent);
            con.GetComponent<OreDeposit>().depositSize = oreDeposits[i].depositSize;
        }

        for (int i = 0; i < fluidDeposits.Length; i++)
        {
            GameObject con = Instantiate(fluidDeposits[i].prefab, new Vector3(fluidDeposits[i].position.x, fluidDeposits[i].position.y, fluidDeposits[i].position.z), Quaternion.identity, depositsParent);
            con.GetComponent<FluidDeposit>().depositSize = fluidDeposits[i].depositSize;
        }

        for (int i = 0; i < conveyorOutput.Length; i++)
        {
            GameObject con = Instantiate(conveyorOutputPrefab, conveyorOutput[i].position, Quaternion.Euler(0, (float)conveyorOutput[i].rotation, 0), buildingsParent);
            con.GetComponent<ConveyorOutput>().item = conveyorOutput[i].item;
            con.GetComponent<BuildingsUI>().buildingsItems[0].GetChild(0).GetChild(0).GetComponent<Image>().sprite = conveyorOutput[i].image;
        }       

        for (int i = 0; i < pipeOutput.Length; i++)
        {
            GameObject con = Instantiate(pipeOutputPrefab, pipeOutput[i].position, Quaternion.Euler(0, (float)pipeOutput[i].rotation, 0), buildingsParent);
            con.GetComponent<PipeOutput>().fluidName = pipeOutput[i].fluidName;
            con.GetComponent<PipeOutput>().fluidColor = pipeOutput[i].fluidColor;
            Color c = pipeOutput[i].fluidColor;
            con.GetComponent<BuildingsUI>().buildingsItems[0].GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(c.r, c.g, c.b, 255);
        }

        for (int i = 0; i < buyerInput.Length; i++)
        {
            GameObject con = Instantiate(buyerPrefab, buyerInput[i].position, Quaternion.Euler(0, (float)buyerInput[i].rotation, 0), buildingsParent);
            con.GetComponent<Buyer>().item = buyerInput[i].item;
            con.GetComponent<Buyer>().fluidName = buyerInput[i].fluidName;

            if (buyerInput[i].item != null)
            {
                con.GetComponent<BuildingsUI>().buildingsItems[0].GetChild(0).GetChild(0).GetComponent<Image>().sprite = buyerInput[i].itemImage;
                con.GetComponent<BuildingsUI>().buildingsItems[0].gameObject.SetActive(true);
                con.GetComponent<Buyer>().inputItem.itemName = buyerInput[i].item.name;

            }
            if (buyerInput[i].fluidName != null && buyerInput[i].fluidName != "")
            {
                con.GetComponent<BuildingsUI>().buildingsItems[1].GetChild(0).GetChild(0).GetComponent<Image>().color = buyerInput[i].fluidColor;
                con.GetComponent<BuildingsUI>().buildingsItems[1].gameObject.SetActive(true);
                con.GetComponent<Buyer>().inputFluid.fluidName = buyerInput[i].fluidName;
            }

            buyerInput[i].buyer = con.GetComponent<Buyer>();

            con.GetComponent<Buyer>().itemCount = buyerInput[i].itemAmount;
            con.GetComponent<Buyer>().fluidCount = buyerInput[i].fluidAmount;
        }
    }

    //spawn PowerPlant
    private void SetupPowerPlant()
    {
        if (sandbox)
        {
            int rotN = Random.Range(0, 3);
            Quaternion rot;

            if (rotN == 0)
                rot = Quaternion.Euler(0, 0, 0);
            else if (rotN == 1)
                rot = Quaternion.Euler(0, 90, 0);
            else if (rotN == 2)
                rot = Quaternion.Euler(0, 180, 0);
            else
                rot = Quaternion.Euler(0, 270, 0);

            Instantiate(powerPlant, new Vector3(Random.Range(4, sizeX - 5), 1f, Random.Range(4, sizeZ - 5)), rot, buildingsParent);
        }
        else
        {
            for (int i = 0; i < powerPlantSetup.Length; i++)
            {
                Instantiate(powerPlant, powerPlantSetup[i].position, Quaternion.Euler(0, (float)powerPlantSetup[i].rotation, 0), buildingsParent);
            }
        }
    }
}