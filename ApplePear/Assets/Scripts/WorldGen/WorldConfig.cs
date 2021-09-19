using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldConfig : MonoBehaviour
{
    public int worldChunkLength;
    public int chunkRenderRange;
    public int chunkVoxelLength;

    [HideInInspector] public int worldLengthVoxel;

    [HideInInspector] public Vector3 startChunkPosition;
    [HideInInspector] public Vector3 startVoxelPosition;

    private void Start()
    {
        InitializeStartPositions();
        InitializeWorldLengthVoxel();
    }

    private void InitializeStartPositions()
    {
        startChunkPosition = new Vector3(worldChunkLength / 2, 1, worldChunkLength / 2);
        startVoxelPosition = (startChunkPosition * chunkVoxelLength) + new Vector3(chunkVoxelLength / 2, 1, chunkVoxelLength / 2);
    }

    private void InitializeWorldLengthVoxel()
    {
        worldLengthVoxel = worldChunkLength * chunkVoxelLength;
    }
}
