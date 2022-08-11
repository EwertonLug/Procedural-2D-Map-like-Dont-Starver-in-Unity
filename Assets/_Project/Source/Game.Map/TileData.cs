using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Map
{
    [CreateAssetMenu(fileName = "Tile Data", menuName = "New Tile Data")]
    public class TileData : ScriptableObject
    {

        [Header("Environment")]
        [SerializeField] private GameObject[] _environmentPrefabs;

        [Header("Bitmasks")]
        [SerializeField] private List<BitmaskData> _bitmasks;

        public IReadOnlyList<BitmaskData> Bitmasks => _bitmasks;
        public Sprite DefaultSprite => _bitmasks[0].Sprite;

        public GameObject GetRandomEnvironment()
        {
            if (_environmentPrefabs.Length == 0)
            {
                return null;
            }
            return _environmentPrefabs[Random.Range(0, _environmentPrefabs.Length)];
        }

        [System.Serializable]
        public class BitmaskData
        {
            public int Variant;
            public Sprite Sprite;
        }

    }
}