using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] World world;
    [SerializeField] WorldConfig worldConfig;

    private void Start()
    {
        //world = this.GetComponent<World>();
        //worldConfig = this.GetComponent<WorldConfig>();
    }

    public void GenerateWorldMap()
    {
        worldConfig.InitializeAllVariables();

        for (int x = 0; x < worldConfig.worldLengthVoxel; x++)
        {
            for (int z = 0; z < worldConfig.worldLengthVoxel; z++)
            {
                Vector3 tempV = new Vector3(x, 0, z);
                world.PlaceVoxel(tempV, BlockConfigs.blue);
            }
        }
    }
}
