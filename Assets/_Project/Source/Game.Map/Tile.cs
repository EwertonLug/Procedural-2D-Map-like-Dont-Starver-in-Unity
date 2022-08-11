using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Map
{
    public class Tile : MonoBehaviour
    {
        public int HeightId;
        public int BiomeId;
        public int X, Y;
        public int Bitmask;
        public Tile Left;
        public Tile Right;
        public Tile Top;
        public Tile Bottom;

        public TileData Data;

        public void UpdateBitmask()
        {
            int count = 0;

            if (Top == null || Top.BiomeId != BiomeId)
                count += 1;
            if (Right == null || Right.BiomeId != BiomeId)
                count += 2;
            if (Bottom == null || Bottom.BiomeId != BiomeId)
                count += 4;
            if (Left == null || Left.BiomeId != BiomeId)
                count += 8;

            Bitmask = count;

            if (Data.Bitmasks.Count == 16)
            {
                GetComponent<SpriteRenderer>().sprite = Data.Bitmasks[Bitmask].Sprite;
            }
            Populate();

        }


        public void ApplyDefaultSprite()
        {
            GetComponent<SpriteRenderer>().sprite = Data.DefaultSprite;
        }

        private void Populate()
        {
            GameObject environment = Data.GetRandomEnvironment();
            if (environment != null)
            {
                Instantiate(environment, transform.position, Quaternion.identity, transform);
            }
        }
    }
}