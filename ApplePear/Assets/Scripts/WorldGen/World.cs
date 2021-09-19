using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public WorldConfig worldConfig;
    [SerializeField] public Material worldMaterial;

    Chunk currentChunk;

    public Chunk[,] totalChunks;
    List<Chunk> activeChunkList = new List<Chunk>();

    private void Start()
    {
        worldConfig = this.GetComponent<WorldConfig>();

        InitializeChunkArray();

        WorldGeneration worldGeneration = this.GetComponent<WorldGeneration>();
        worldGeneration.GenerateWorldMap();

        CreateStartChunks();

        Camera.main.transform.position = worldConfig.startVoxelPosition;
    }

    private void Update()
    {        
        if (IsChunkInWorldFromPosition(Camera.main.transform) == false)
        {
            return;
        }

        RenderHandle();
    }

    #region Initialize Chunk Array && Create Start Chunks

    private void InitializeChunkArray()
    {
        //World Length In Chunks
        int worldCL = worldConfig.worldChunkLength;

        totalChunks = new Chunk[worldCL, worldCL];

        for (int x = 0; x < worldCL; x++)
        {
            for (int z = 0; z < worldCL; z++)
            {
                totalChunks[x, z] = null;
            }
        }
    }

    private void CreateStartChunks()
    {
        //Render Range In Chunks
        int chunkRR = worldConfig.chunkRenderRange;

        for (int x = 0; x < chunkRR * 2 + 1; x++)
        {
            for (int y = 0; y < chunkRR * 2 + 1; y++)
            {
                int chunkXDisplacementFromCenter = (int)worldConfig.startChunkPosition.x - chunkRR + x;
                int chunkZDisplacementFromCenter = (int)worldConfig.startChunkPosition.x - chunkRR + y;

                totalChunks[chunkXDisplacementFromCenter, chunkZDisplacementFromCenter].parentChunkObject.SetActive(true);
            }
        }
    }

    #endregion

    #region Render/Derender

    private void RenderHandle()
    {
        Vector3 playerPosition = Camera.main.transform.position;

        int chunkX = (int)ChunkFromCoords(playerPosition).x;
        int chunkZ = (int)ChunkFromCoords(playerPosition).y;

        Chunk playerChunk = totalChunks[chunkX, chunkZ];

        if (playerChunk != currentChunk)
        {
            if (currentChunk == null)
            {
                currentChunk = playerChunk;

                RenderChunks();
            }
            else
            {
                currentChunk = playerChunk;

                RenderChunks();

                DeRenderChunks();
            }
        }
    }

    private void RenderChunks()
    {
        int chunkRR = worldConfig.chunkRenderRange;

        for (int x = 0; x < chunkRR * 2 + 1; x++)
        {
            for (int z = 0; z < chunkRR * 2 + 1; z++)
            {
                int xOffset = x - chunkRR;
                int zOffset = z - chunkRR;

                int chunkX = xOffset + (int)ChunkFromCoords(Camera.main.transform.position).x;
                int chunkZ = zOffset + (int)ChunkFromCoords(Camera.main.transform.position).y;

                if (IsChunkInWorldFromVector2(new Vector2(chunkX, chunkZ)) == false || IsChunkInList(totalChunks[chunkX, chunkZ]) == true)
                {
                    continue;
                }

                //Run through every chunk in render distance that's not in chunkList

                if (totalChunks[chunkX, chunkZ] == null)
                {
                    Chunk newChunk = MakeChunk(chunkX, chunkZ);

                    activeChunkList.Add(newChunk);
                    totalChunks[chunkX, chunkZ] = newChunk;
                }
                else
                {
                    activeChunkList.Add(totalChunks[chunkX, chunkZ]);
                    totalChunks[chunkX, chunkZ].parentChunkObject.SetActive(true);
                }

                totalChunks[chunkX, chunkZ].CreateChunk();
            }
        }
    }

    private void DeRenderChunks()
    {
        //Render Range In Chunks
        int chunkRR = worldConfig.chunkRenderRange;

        List<Chunk> previouslyActiveChunks = new List<Chunk>(activeChunkList);

        for (int x = 0; x < chunkRR * 2 + 1; x++)
        {
            for (int z = 0; z < chunkRR * 2 + 1; z++)
            {
                int xOffset = x - chunkRR;
                int zOffset = z - chunkRR;

                int chunkX = xOffset + (int)ChunkFromCoords(Camera.main.transform.position).x;
                int chunkZ = zOffset + (int)ChunkFromCoords(Camera.main.transform.position).y;

                //Run through every chunk in render distance

                if (IsChunkInWorldFromVector2(new Vector2(chunkX, chunkZ)) == false)
                {
                    continue;
                }

                Chunk curChunk = totalChunks[chunkX, chunkZ];
                previouslyActiveChunks.Remove(curChunk);
            }
        }

        foreach (Chunk tempChunk in previouslyActiveChunks)
        {
            tempChunk.parentChunkObject.SetActive(false);
            activeChunkList.Remove(tempChunk);
        }
    }

    #endregion

    #region Utilities

    public void PlaceVoxel(Vector3 position, BlockConfigs.BlockDataConfig inputBlock)
    {
        Chunk currentChunk = null;

        int chunkX = (int)ChunkFromCoords(position).x;
        int chunkZ = (int)ChunkFromCoords(position).y;

        if (IsChunkInWorldFromPosition(Camera.main.transform) == false)
        {
            return;
        }

        if (totalChunks[chunkX, chunkZ] == null)
        {
            currentChunk = MakeChunk(chunkX, chunkZ);
        }

        Chunk targetChunk = totalChunks[chunkX, chunkZ];

        int xPos = (int)(position.x % worldConfig.chunkVoxelLength);
        int yPos = (int)position.y;
        int zPos = (int)(position.z % worldConfig.chunkVoxelLength);

        Vector3 positionInChunk = new Vector3(xPos, yPos, zPos);

        targetChunk.AddVoxelToMap(positionInChunk, inputBlock);

        if (currentChunk != null)
        {
            currentChunk.parentChunkObject.SetActive(false);
        }   
    }

    private Chunk MakeChunk(int chunkX, int chunkZ)
    {
        Chunk newChunk = new Chunk();
        newChunk.CreateChunkObject(this, chunkX, chunkZ);

        totalChunks[chunkX, chunkZ] = newChunk;

        return newChunk;
    }

    public Vector2 ChunkFromCoords(Vector3 playerPosition)
    {
        int curX = (int)playerPosition.x;
        int curZ = (int)playerPosition.z;

        curX = curX / worldConfig.chunkVoxelLength;
        curZ = curZ / worldConfig.chunkVoxelLength;

        return new Vector2(curX, curZ);
    }

    private bool IsChunkInList(Chunk inputChunk)
    {
        foreach (Chunk chunk in activeChunkList)
        {
            if (inputChunk == chunk)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsChunkInWorldFromVector2(Vector2 chunkCoords)
    {
        if (chunkCoords.x < 0 || chunkCoords.x > worldConfig.worldChunkLength - 1 || chunkCoords.y < 0 || chunkCoords.y > worldConfig.worldChunkLength - 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool IsChunkInWorldFromPosition(Transform position)
    {
        Vector3 curChunk = ChunkFromCoords(position.position);

        if (IsChunkInWorldFromVector2(curChunk) == true)
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