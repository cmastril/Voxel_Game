using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] World world;
    [SerializeField] WorldConfig worldConfig;

    public void GenerateWorldMap()
    {
        worldConfig.InitializeAllVariables();

        for (int x = 0; x < worldConfig.worldLengthVoxel; x++)
        {
            for (int z = 0; z < worldConfig.worldLengthVoxel; z++)
            {
                Vector3 tempV = new Vector3(x, 0, z);

                int ran = Random.Range(0,2);

                if (ran == 0)
                {
                    world.PlaceVoxel(tempV, BlockConfigs.red);
                }
                else if (ran == 1)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        world.PlaceVoxel(tempV + new Vector3(0,i,0), BlockConfigs.blue);
                    }
                }
            }
        }
    }
}
