using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Map
{
    [CreateAssetMenu(fileName = "Biome Data", menuName = "New Biome Data")]
    public class BiomeData : ScriptableObject
    {
        [SerializeField] private int _id;

        [Header("Default Tile Prefab")]
        [SerializeField] private GameObject _tilePrefab;

        [Header("Way Material")]
        [SerializeField] string _waySortingLayer = "Default";
        [SerializeField] int _wayOrderInLayer;
        [SerializeField] private Material _wayMaterial;

        [Header("Tiles Types (Selected by Perlin Noise)")]
        public TileData[] tiles;

        [Header("Resources")]
        [SerializeField] private GameObject[] _resourcesPrefab;

        [Header("Resources Collected")]
        [SerializeField] private GameObject[] _resourcesCollectedPrefab;
        [Range(0, 1)]
        [SerializeField] private float _resourcePercent;
        [Range(0, 1)]
        [SerializeField] private float _resourceCollectedPercent;
        public GameObject TilePrefab => _tilePrefab;
        public Material WayMaterial => _wayMaterial;
        public string WaySortingLayer => _waySortingLayer;
        public int WayOrderInLayer => _wayOrderInLayer;
        public int Id => _id;

        public TileData GetTileById(int index)
        {
            return tiles[index];
        }

        public GameObject GetRandomResource()
        {
            float change = Random.value;

            if (change < _resourcePercent)
            {
                return _resourcesPrefab[Random.Range(0, _resourcesPrefab.Length)];
            }
            else
            {
                change = Random.value;
                if (change < _resourceCollectedPercent)
                {
                    return _resourcesCollectedPrefab[Random.Range(0, _resourcesCollectedPrefab.Length)];
                }
            }

            return null;

        }


    }
}