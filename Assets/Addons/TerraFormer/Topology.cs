using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraFormer
{
    public class Topology : MonoBehaviour
    {

        [SerializeField]
        Terrain terrain;

        void Start()
        {
            if (terrain == null)
            {
                terrain = GetComponent<Terrain>();
            }
        }

        public void Generate()
        {
            int xSize = Mathf.RoundToInt(terrain.terrainData.size.x);
            int zSize = Mathf.RoundToInt(terrain.terrainData.size.z);

            xSize = 1;
            zSize = 1;

            float[,] heights = GetHeightGeneration(0, 0, xSize, zSize, Vector2.one * 0.3f);
            terrain.terrainData.SetHeightsDelayLOD(0, 0, heights);

            terrain.Flush();
        }

        float[,] GetHeightGeneration(int x, int z, int sizeX, int sizeZ, Vector2 scale)
        {
            float[,] heights = new float[sizeX, sizeZ];
            for (int idx=0; idx< sizeX; idx++)
            {
                for(int idz = 0; idz<sizeZ; sizeZ++)
                {
                    heights[idx, idz] = Mathf.Clamp01(Mathf.PerlinNoise(x + idx * scale.x, z + idz * scale.y));
                }
            }
            return heights;
        }
    }

}