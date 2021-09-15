using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] public Material worldMaterial;

    [SerializeField] int worldChunkUnit = 10;
    [SerializeField] int worldChunkRender = 2;
    [SerializeField] int chunkUnit = 20;

    Vector3 startChunkPosition;
    Vector3 startVoxelPosition;

    Chunk currentChunk;

    Chunk[,] totalChunks;
    List<Chunk> chunkList = new List<Chunk>();

    private void Start()
    {
        InitializeStartPositions();
        InitializeChunkArray();

        CreateStartChunks();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log(chunkList.ToArray().Length);
        }

        Vector3 playerPosition = Camera.main.transform.position;
        int worldLengthVox = worldChunkUnit * chunkUnit;

        if (IsChunkInWorldPosition(Camera.main.transform) == false)
        {
            return;
        }

        int chunkX = (int)ChunkFromCoords(playerPosition).x;
        int chunkZ = (int)ChunkFromCoords(playerPosition).y;

        Chunk playerChunk = totalChunks[chunkX, chunkZ];

        if (playerChunk != currentChunk)
        {
            if (currentChunk == null)
            {
                currentChunk = playerChunk;

                LoadNewRenderChunks();
            }
            else
            {
                currentChunk = playerChunk;

                LoadNewRenderChunks();

                DeRenderChunks();
            }
        }
    }

    private void DeRenderChunks()
    {
        List<Chunk> tempList = new List<Chunk>();
        
        foreach(Chunk tempChunk in chunkList)
        {
            tempList.Add(tempChunk);
        }

        for (int x = 0; x < worldChunkRender * 2 + 1; x++)
        {
            for (int z = 0; z < worldChunkRender * 2 + 1; z++)
            {
                int xOffset = x - worldChunkRender;
                int zOffset = z - worldChunkRender;

                int chunkX = xOffset + (int)ChunkFromCoords(Camera.main.transform.position).x;
                int chunkZ = zOffset + (int)ChunkFromCoords(Camera.main.transform.position).y;

                //Run through every chunk in render distance

                if (IsChunkInWorld(new Vector2(chunkX, chunkZ)) == false)
                {
                    continue;
                }

                Vector2 curChunkCoords = ChunkFromCoords(new Vector2(chunkX, chunkZ));
                Chunk curChunk = totalChunks[chunkX, chunkZ];

                tempList.Remove(curChunk);
            }
        }

        foreach (Chunk tempChunk in tempList)
        {
            tempChunk.DestroyMesh();
            chunkList.Remove(tempChunk);
        }
    }

    private void LoadNewRenderChunks()
    {
        for (int x = 0; x < worldChunkRender * 2 + 1; x ++)
        {
            for (int z = 0; z < worldChunkRender * 2 + 1; z++)
            {
                int xOffset = x - worldChunkRender;
                int zOffset = z - worldChunkRender;

                int chunkX = xOffset + (int)ChunkFromCoords(Camera.main.transform.position).x;
                int chunkZ = zOffset + (int)ChunkFromCoords(Camera.main.transform.position).y;

                //Run through every chunk in render distance

                if(IsChunkInWorld(new Vector2(chunkX, chunkZ)) == false)
                {
                    continue;
                }

                if (IsChunkInList(totalChunks[chunkX, chunkZ]) == true)
                {
                    continue;
                }

                //Run through every chunk in render distance that's not in chunkList

                if (totalChunks[chunkX, chunkZ] == null)
                {
                    //Null Chunk Handle
                    Chunk newChunk = CreateChunk(chunkX, chunkZ);

                    chunkList.Add(newChunk);
                    totalChunks[chunkX, chunkZ] = newChunk;
                }
                else
                {
                    //Fill Chunk Handle
                    chunkList.Add(totalChunks[chunkX, chunkZ]);
                    totalChunks[chunkX, chunkZ].CreateChunk();
                }
            }
        }
    }

    #region Variable Initializations

    private void InitializeStartPositions()
    {
        startChunkPosition = new Vector3(worldChunkUnit / 2, 1, worldChunkUnit / 2);
        startVoxelPosition = (startChunkPosition * chunkUnit) + new Vector3(chunkUnit / 2, 1, chunkUnit / 2);

        Camera.main.transform.position = startVoxelPosition;
    }

    private void InitializeChunkArray()
    {
        totalChunks = new Chunk[worldChunkUnit, worldChunkUnit];

        for (int x = 0; x < worldChunkUnit; x++)
        {
            for (int z = 0; z < worldChunkUnit; z++)
            {
                totalChunks[x, z] = null;
            }
        }
    }

    #endregion

    #region Chunk Generation

    private void CreateStartChunks()
    {
        for (int x = 0; x < worldChunkRender * 2 + 1; x++)
        {
            for (int y = 0; y < worldChunkRender * 2 + 1; y++)
            {
                int chunkXDisplacementFromCenter = (int)startChunkPosition.x - worldChunkRender + x;
                int chunkZDisplacementFromCenter = (int)startChunkPosition.x - worldChunkRender + y;

                CreateChunk(chunkXDisplacementFromCenter, chunkZDisplacementFromCenter);
            }
        }
    }

    private Chunk CreateChunk(int chunkX, int chunkZ)
    {
        Chunk newChunk = new Chunk();
        newChunk.CreateChunkObject(this, chunkX, chunkZ);

        totalChunks[chunkX, chunkZ] = newChunk;

        return newChunk;
    }

    #endregion region

    #region Utilities

    private Vector2 ChunkFromCoords(Vector3 playerPosition)
    {
        int curX = (int)playerPosition.x;
        int curZ = (int)playerPosition.z;

        curX = curX / chunkUnit;
        curZ = curZ / chunkUnit;

        return new Vector2(curX, curZ);
    }

    private bool IsChunkInList(Chunk inputChunk)
    {
        foreach (Chunk chunk in chunkList)
        {
            if (inputChunk == chunk)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsChunkInWorld(Vector2 chunkCoords)
    {
        if (chunkCoords.x < 0 || chunkCoords.x > worldChunkUnit - 1 || chunkCoords.y < 0 || chunkCoords.y > worldChunkUnit - 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool IsChunkInWorldPosition(Transform position)
    {
        Vector3 curChunk = ChunkFromCoords(position.position);

        if (IsChunkInWorld(curChunk) == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
}