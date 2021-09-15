using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockConfigs : MonoBehaviour
{
    public struct BlockDataConfig
    {
        public int blockID;
        public string blockName;
        public bool transparency;

        public int frontTextureID;
        public int sideTextureID;
        public int topTextureID;
        public int bottomTextureID;
    }

    public static BlockDataConfig air = new BlockDataConfig { blockID = 0, blockName = "air", transparency = true};
    public static BlockDataConfig green = new BlockDataConfig { blockID = 1, blockName = "green", transparency = false, frontTextureID = 0, sideTextureID = 0, topTextureID = 0, bottomTextureID = 0 };
    public static BlockDataConfig red = new BlockDataConfig { blockID = 1, blockName = "red", transparency = false, frontTextureID = 1, sideTextureID = 1, topTextureID = 1, bottomTextureID = 1 };
    public static BlockDataConfig blue = new BlockDataConfig { blockID = 1, blockName = "blue", transparency = false, frontTextureID = 3, sideTextureID = 3, topTextureID = 3, bottomTextureID = 3 };
}
