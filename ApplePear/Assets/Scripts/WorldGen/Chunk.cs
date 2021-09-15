using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public bool isActive;

    int chunkX;
    int chunkZ;

    public void CreateChunkObject(World worldParent, int tempChunkX, int tempChunkZ)
    {
        chunkX = tempChunkX;
        chunkZ = tempChunkZ;

        GameObject chunkObject = new GameObject();
        chunkObject.name = $"Chunk ({tempChunkX}, {tempChunkZ})";

        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
       
        meshRenderer.material = worldParent.worldMaterial;
        chunkObject.transform.SetParent(worldParent.transform);

        chunkObject.transform.position = new Vector3(tempChunkX * chunkUnit, 0, tempChunkZ * chunkUnit);

        PopulateVoxelMap();
        CreateChunk();
    }

    public void DestroyMesh()
    {
        meshFilter.mesh.Clear();
    }

    #region Chunk Creation

    #region Chunk Creation Fields

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    static int chunkUnit = 20;
    
    BlockConfigs.BlockDataConfig[,,] voxelData = new BlockConfigs.BlockDataConfig[chunkUnit, chunkUnit, chunkUnit];

    #endregion

    private void PopulateVoxelMap()
    {
        for (int x = 0; x < chunkUnit; x++)
        {
            for (int y = 0; y < chunkUnit; y++)
            {
                for (int z = 0; z < chunkUnit; z++)
                {
                    voxelData[x, y, z] = BlockConfigs.air;
                }
            }
        }

        for (int x = 0; x < chunkUnit; x++)
        {
            for (int z = 0; z < chunkUnit; z++)
            {
                voxelData[x, 0, z] = BlockConfigs.red;
            }
        }
    }

    public void CreateChunk()
    {
        for (int x = 0; x < chunkUnit; x++)
        {
            for (int z = 0; z < chunkUnit; z++)
            {
                CreateVoxel(new Vector3(x, 0, z), BlockConfigs.blue);
            }
        }

        for (int x = 0; x < chunkUnit - 2; x++)
        {
            for (int z = 0; z < chunkUnit - 2; z++)
            {
                CreateVoxel(new Vector3(x + 1, 0, z + 1), BlockConfigs.red);
            }
        }

        //FillInChunk();

        UpdateMesh();
    }

    //Doesn't Work to my knowledge \/ \/ \/
    public void FillInChunk()
    {
        for (int x = 0; x < chunkUnit; x++)
        {
            for (int y = 0; y < chunkUnit; y++)
            {
                for (int z = 0; z < chunkUnit; z++)
                {
                    CreateVoxel(new Vector3(x,y,z), voxelData[x,y,z]);
                }
            }
        }
    }

    private bool CheckVoxel(Vector3 voxelPosition, int currentFace)
    {
        Vector3 positionToCheck = voxelPosition;

        if (currentFace == 0)
        {
            positionToCheck += new Vector3(0, 0, -1);
        }
        else if (currentFace == 1)
        {
            positionToCheck += new Vector3(-1, 0, 0);
        }
        else if (currentFace == 2)
        {
            positionToCheck += new Vector3(0, 0, 1);
        }
        else if (currentFace == 3)
        {
            positionToCheck += new Vector3(1, 0, 0);
        }
        else if (currentFace == 4)
        {
            positionToCheck += new Vector3(0, 1, 0);
        }
        else if (currentFace == 5)
        {
            positionToCheck += new Vector3(0, -1, 0);
        }

        int checkX = (int)positionToCheck.x;
        int checkY = (int)positionToCheck.y;
        int checkZ = (int)positionToCheck.z;

        if (checkX == -1 || checkX == chunkUnit || checkY == -1 || checkY == chunkUnit|| checkZ == -1 || checkZ == chunkUnit)
        {
            return false;
        }
        else
        {
            if (voxelData[checkX, checkY, checkZ].transparency == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    #endregion

    #region Voxel Creation

    #region Voxel Creation Fields

    [SerializeField] List<Vector3> vertices = new List<Vector3>();
    [SerializeField] List<int> triangles = new List<int>();
    [SerializeField] List<Vector2> uvs = new List<Vector2>();

    int voxelCount = 0;

    [SerializeField] int totalVertexes = 0;

    int currentTextureID = 0;

    #endregion

    private void CreateVoxel(Vector3 voxelPosition, BlockConfigs.BlockDataConfig blockData)
    {
        // Front -> Left -> Back -> Right -> Top -> Bottom

        for (int i = 0; i < 6; i++)
        {
            if (CheckVoxel(voxelPosition, i) == false)
            {
                AddVoxelVertexes(i, voxelPosition);
                AddVoxelTriangles(i);
                AddVoxelUVs(i, blockData);
            }
        }
        voxelCount++;
    }

    private void AddVoxelVertexes(int currentFace, Vector3 voxelPosition)
    {
        int x = (int)voxelPosition.x;
        int y = (int)voxelPosition.y;
        int z = (int)voxelPosition.z;

        if (currentFace == 0)
        {
            vertices.Add(new Vector3(0 + x, 0 + y, 0 + z));
            vertices.Add(new Vector3(0 + x, 1 + y, 0 + z));
            vertices.Add(new Vector3(1 + x, 1 + y, 0 + z));
            vertices.Add(new Vector3(1 + x, 0 + y, 0 + z));
        }
        else if (currentFace == 1)
        {
            vertices.Add(new Vector3(0 + x, 0 + y, 1 + z));
            vertices.Add(new Vector3(0 + x, 1 + y, 1 + z));
            vertices.Add(new Vector3(0 + x, 1 + y, 0 + z));
            vertices.Add(new Vector3(0 + x, 0 + y, 0 + z));
        }
        else if (currentFace == 2)
        {
            vertices.Add(new Vector3(1 + x, 0 + y, 1 + z));
            vertices.Add(new Vector3(1 + x, 1 + y, 1 + z));
            vertices.Add(new Vector3(0 + x, 1 + y, 1 + z));
            vertices.Add(new Vector3(0 + x, 0 + y, 1 + z));
        }
        else if (currentFace == 3)
        {
            vertices.Add(new Vector3(1 + x, 0 + y, 0 + z));
            vertices.Add(new Vector3(1 + x, 1 + y, 0 + z));
            vertices.Add(new Vector3(1 + x, 1 + y, 1 + z));
            vertices.Add(new Vector3(1 + x, 0 + y, 1 + z));
        }
        else if (currentFace == 4)
        {
            vertices.Add(new Vector3(0 + x, 1 + y, 0 + z));
            vertices.Add(new Vector3(0 + x, 1 + y, 1 + z));
            vertices.Add(new Vector3(1 + x, 1 + y, 1 + z));
            vertices.Add(new Vector3(1 + x, 1 + y, 0 + z));
        }
        else if (currentFace == 5)
        {
            vertices.Add(new Vector3(0 + x, 0 + y, 0 + z));
            vertices.Add(new Vector3(0 + x, 0 + y, 1 + z));
            vertices.Add(new Vector3(1 + x, 0 + y, 1 + z));
            vertices.Add(new Vector3(1 + x, 0 + y, 0 + z));
        }

        totalVertexes += 4;
    }

    private void AddVoxelTriangles(int currentFace)
    {
        int first = totalVertexes - 4;
        int second = totalVertexes - 4 + 1;
        int third = totalVertexes - 4 + 2;
        int fourth = totalVertexes - 4 + 3;

        if (currentFace == 5)
        {
            triangles.Add(second);
            triangles.Add(first);
            triangles.Add(third);

            triangles.Add(first);
            triangles.Add(fourth);
            triangles.Add(third);
        }
        else
        {
            triangles.Add(first);
            triangles.Add(second);
            triangles.Add(fourth);

            triangles.Add(second);
            triangles.Add(third);
            triangles.Add(fourth);
        }
    }

    [SerializeField] int tileSheetLength = 4;

    private void AddVoxelUVs(int currentFace, BlockConfigs.BlockDataConfig blockData)
    {
        if (currentFace == 0)
        {
            currentTextureID = blockData.frontTextureID;
        }
        else if (currentFace == 1 || currentFace == 2 || currentFace == 3)
        {
            currentTextureID = blockData.sideTextureID;
        }
        else if (currentFace == 4)
        {
            currentTextureID = blockData.topTextureID;
        }
        else if (currentFace == 5)
        {
            currentTextureID = blockData.bottomTextureID;
        }

        float tileLength = 1 / (float)tileSheetLength;

        int currentRow = currentTextureID / 4;
        int currentColumn = (currentTextureID + 4) % tileSheetLength;

        Vector2 bottomLeft = new Vector2(currentColumn * tileLength, currentRow * tileLength);

        uvs.Add(bottomLeft + new Vector2(0, 0));
        uvs.Add(bottomLeft + new Vector2(0, tileLength));
        uvs.Add(bottomLeft + new Vector2(tileLength, tileLength));
        uvs.Add(bottomLeft + new Vector2(tileLength, 0));
    }

    private void UpdateMesh()
    {
        meshFilter.mesh.Clear();

        meshFilter.mesh.vertices = vertices.ToArray();
        meshFilter.mesh.triangles = triangles.ToArray();
        meshFilter.mesh.uv = uvs.ToArray();

        meshFilter.mesh.RecalculateNormals();
    }

    #endregion
}
