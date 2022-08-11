using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Map
{
    [Serializable]
    public class PerlinNoise
    {
        [SerializeField] private float _magnification = 20f;

        private float _originX;
        private float _originY;
        /** Using a grid coordinate input, generate a Perlin noise value to be
               converted into a tile ID code. Rescale the normalised Perlin value
               to the number of tiles available. **/
        public int GetIdUsingPerlin(int x, int y, int tilesetCount)
        {

            float xCoord = (x + _originX) / _magnification;
            float yCoord = (y + _originY) / _magnification;
            float raw_perlin = Mathf.PerlinNoise(xCoord, yCoord);
            float clamp_perlin = Mathf.Clamp01(raw_perlin);
            float scaled_perlin = clamp_perlin * tilesetCount;

            // Replaced 4 with tileset.Count to make adding tiles easier
            if (scaled_perlin == tilesetCount)
            {
                scaled_perlin = tilesetCount - 1;
            }
            return Mathf.FloorToInt(scaled_perlin);
        }

        public void UpdateOrign()
        {
            _originX = UnityEngine.Random.Range(0, 50);
            _originY = UnityEngine.Random.Range(0, 50);

        }
    }
}