using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SKCell;

public class LevelLoader : SKMonoSingleton<LevelLoader>
{
    //The array of the map
    public static MapData mapData;
    //This is the scale of the block that depends on the size of the map
    public static Vector3 BLOCK_SCALE_MAP;

    //The array that keeps records of how many blocks are inside the map
    GameObject[] blocks;
    //References to the Actual map
    [SerializeField] Transform mapLeft, mapRight;
    [SerializeField] Transform LeftSide, RightSide;
    [SerializeField] Transform blockContainerLeft, blockContainerRight;
    //This is how big the level is, it can be 3x3 or 4x4, or 5x5, depending on need;
    public Vector2Int levelDimensions = new Vector2Int(3, 3);

    public Bounds boundsLeft, boundsRight;


    // A Dictionary to check whether there are already blocks on a position or not.
    private Dictionary<Vector3, Block> blockPos = new Dictionary<Vector3, Block>();
    // This is the array for the best position of each block in both maps
    public Vector3[,] blockposLeft, blockposRight;
    // This is the center point between two maps
    public static Vector3 center;


    const float BLOCK_SCALE_RATIO = 3f;
    const float DRAGGABLE_BLOCK_SCALE = 0.7f;
    const float DRAGGABLE_BLOCK_GAP = 12.0f;

    //This is the matrix code created for left and right map
    public Block[,] blocksLeftMap, blocksRightMap;

    void Start()
    {
        boundsLeft = mapLeft.GetComponent<BoxCollider>().bounds;
        boundsRight = mapRight.GetComponent<BoxCollider>().bounds;
        center = (LeftSide.position + RightSide.position) / 2.0f;

        LoadMap();
    }

    public void LoadMap()
    {
        //init map data
        mapData = new MapData();
        mapData.map = new int[levelDimensions.x, levelDimensions.y];

        // Initialize the matrices with the dimensions of the level
        blocksLeftMap = new Block[levelDimensions.x, levelDimensions.y];
        blocksRightMap = new Block[levelDimensions.x, levelDimensions.y];

        //Initialize The position array of blocks
        blockposLeft = new Vector3[levelDimensions.x, levelDimensions.y];
        blockposRight = new Vector3[levelDimensions.x, levelDimensions.y];

        //Leftside calculation
        float cellGapX = (boundsLeft.max.x - boundsLeft.min.x) / (2 * levelDimensions.x);
        float cellGapZ = (boundsLeft.max.z - boundsLeft.min.z) / (2 * levelDimensions.y);

        float startX = boundsLeft.min.x + cellGapX;
        float startZ = boundsLeft.min.z + cellGapZ;
        for (int i = 0; i < levelDimensions.y; i++)
        {
            for (int j = 0; j < levelDimensions.x; j++)
            {
                Vector3 pos = new Vector3(startX + 2 * cellGapX * j, 0, startZ + 2 * cellGapZ * i);
                blockposLeft[i, j] = pos;
            }
        }

        //right
        cellGapX = (boundsRight.max.x - boundsRight.min.x) / (2 * levelDimensions.x);
        cellGapZ = (boundsRight.max.z - boundsRight.min.z) / (2 * levelDimensions.y);

        startX = boundsRight.min.x + cellGapX;
        startZ = boundsRight.min.z + cellGapZ;
        for (int i = 0; i < levelDimensions.y; i++)
        {
            for (int j = 0; j < levelDimensions.x; j++)
            {
                Vector3 pos = new Vector3(startX + 2 * cellGapX * j, 0, startZ + 2 * cellGapZ * i);
                blockposRight[i, j] = pos;
            }
        }


        //Starting Aligning Blocks
        blocks = GameObject.FindGameObjectsWithTag("Block");
        BLOCK_SCALE_MAP = Vector3.one * BLOCK_SCALE_RATIO / levelDimensions.x;
        foreach (GameObject block in blocks)
        {
            Vector3 pos = block.transform.position;

            float min_delta = float.MaxValue;
            Vector3 best_pos = Vector3.zero;
            Vector3[,] to_comp = pos.x < center.x ? blockposLeft : blockposRight;
            foreach (Vector3 comp in to_comp)
            {
                float delta = Mathf.Abs(comp.x - pos.x) + Mathf.Abs(comp.z - pos.z);
                if (delta < min_delta)
                {
                    min_delta = delta;
                    best_pos = comp;
                }
            }
            block.transform.position = best_pos;
            //Inseting a key to the dictionary, letting the system know there is a block
            SKUtils.InsertOrUpdateKeyValueInDictionary(blockPos, best_pos, block.GetComponent<Block>());


            block.transform.localScale = BLOCK_SCALE_MAP;
            block.GetComponent<Block>().SyncLocalScale(BLOCK_SCALE_MAP);

        }
        Debug.Log(blockposLeft[0, 1]);

    }
    public void OnMoveBlock(Block block, Vector3 from, Vector3 to)
    {
        if (blockPos.ContainsKey(from))
        {
            blockPos.Remove(from);
        }
        SKUtils.InsertOrUpdateKeyValueInDictionary(blockPos, to, block);
 
    }

    public static Vector3 WorldToCellPos(Vector3 wpos)
    {
        LevelLoader l = LevelLoader.instance;
        float min_delta = float.MaxValue;
        Vector3 best_pos = Vector3.zero;
        Vector3[,] to_comp = wpos.x < center.x ? l.blockposLeft : l.blockposRight;
        foreach (Vector3 comp in to_comp)
        {
            float delta = Mathf.Abs(comp.x - wpos.x) + Mathf.Abs(comp.z - wpos.z);
            if (delta < min_delta)
            {
                min_delta = delta;
                best_pos = comp;
            }
        }
        return best_pos;
    }

    public static Vector3 CellToWorldPos(bool isLeft, int x, int y)
    {
        LevelLoader l = LevelLoader.instance;
        return isLeft ? l.blockposLeft[x, y] : l.blockposRight[x, y];
    }

    public static bool HasBlockOnCellPos(Vector3 cpos)
    {
        LevelLoader l = LevelLoader.instance;
        return l.blockPos.ContainsKey(cpos);
    }





    public class MapData
    {
        public int[,] map;
    }

}
