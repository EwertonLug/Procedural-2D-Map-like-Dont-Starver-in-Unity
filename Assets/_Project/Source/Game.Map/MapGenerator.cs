using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Game.Map
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private List<BiomeData> _biomesData = new List<BiomeData>();

        [Header("Dimensions")]
        [SerializeField] private int _mapRows = 160;
        [SerializeField] private int _mapColumns = 90;
        [SerializeField] private int _biomeWith = 10;
        [SerializeField] private bool _enableTurbulence;
        [SerializeField] private Turbulence _turbulence;
        [SerializeField] private PerlinNoise _perlinNoise;


        private Dictionary<int, GameObject> _biomesGroup;
        private Biomes _biomes = new Biomes();

        private Tile[,] _tiles;
        Vector2[,] _points;

        private async void Start()
        {
            _perlinNoise.UpdateOrign();
            _points = CreateGrid(_mapRows, _mapColumns);
            _tiles = new Tile[_mapRows, _mapColumns];
            _biomes = BiomesFactory.NewRRTSeeds(_points, 15, _biomesData.Count);
            CreateBiomesSeedGroup();
            await PopulateBiomes();
            GenerateResourceOfBiomes();
            UpdateNeighbors();
            UpdateBitmasks();
            GenerateWays();
        }

        private void GenerateWays()
        {
            GameObject worldWay = new GameObject();
            worldWay.name = "WorldWay";

            foreach (KeyValuePair<BiomeSeed, int> item in _biomes.SeedsPair)
            {

                if (item.Key.Parent != null)
                {
                    GameObject way = new GameObject();
                    way.name = $"{item.Key.Localization} <=> {item.Key.Parent.Localization}";
                    LineRenderer lineRenderer = way.AddComponent<LineRenderer>();
                    lineRenderer.alignment = LineAlignment.TransformZ;
                    lineRenderer.material = _biomesData[item.Value].WayMaterial;
                    lineRenderer.textureMode = LineTextureMode.Tile;
                    lineRenderer.sortingLayerName = _biomesData[item.Value].WaySortingLayer;
                    lineRenderer.sortingOrder = _biomesData[item.Value].WayOrderInLayer;
                    Vector3 startPos = new Vector3(item.Key.Parent.Localization.x, item.Key.Parent.Localization.y, -.01f);
                    Vector3 EndPos = new Vector3(item.Key.Localization.x, item.Key.Localization.y, -.01f);
                    lineRenderer.SetPosition(0, startPos);
                    lineRenderer.SetPosition(1, EndPos);

                    way.transform.SetParent(worldWay.transform);
                }

            }
        }

        private Vector2[,] CreateGrid(int numRows, int numColunms)
        {
            Vector2[,] points = new Vector2[numRows, numColunms];

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColunms; j++)
                {
                    points[i, j] = new Vector2(i, j);
                }
            }

            return points;
        }

        private void GenerateResourceOfBiomes()
        {
            for (int row = 0; row < _mapRows; row++)
            {
                for (int column = 0; column < _mapColumns; column++)
                {
                    Tile t = _tiles[row, column];

                    if (t != null)
                    {
                        BiomeData data = _biomesData.Find(x => x.Id == t.BiomeId);

                        if (data != null)
                        {
                            GameObject resource = data.GetRandomResource();

                            if (resource != null)
                            {
                                Instantiate(resource, new Vector3(t.X, t.Y, -0.02f), Quaternion.identity, t.transform);
                            }
                        }
                    }
                }
            }
        }
        private void CreateBiomesSeedGroup()
        {
            _biomesGroup = new Dictionary<int, GameObject>();

            foreach (KeyValuePair<BiomeSeed, int> _seedsPair in _biomes.SeedsPair)
            {


                if (!_biomesGroup.ContainsKey(_seedsPair.Value))
                {
                    GameObject biomeGroup = new GameObject(_biomesData[_seedsPair.Value].name);

                    biomeGroup.transform.parent = gameObject.transform;
                    biomeGroup.transform.localPosition = new Vector3(0, 0, 0);

                    _biomesGroup.Add(_seedsPair.Value, biomeGroup);
                    CreateTileTypeGroup(_seedsPair, biomeGroup);
                }



            }

            void CreateTileTypeGroup(KeyValuePair<BiomeSeed, int> _seedsPair, GameObject biomeGroup)
            {
                foreach (TileData tileType in _biomesData[_seedsPair.Value].tiles)
                {
                    GameObject tile_group = new GameObject(tileType.name);
                    tile_group.transform.parent = biomeGroup.transform;
                    tile_group.transform.localPosition = new Vector3(0, 0, 0);

                }
            }
        }

        private async Task PopulateBiomes()
        {
            for (int row = 0; row < _mapRows; row++)
            {
                for (int column = 0; column < _mapColumns; column++)
                {
                    await Task.Delay(TimeSpan.FromSeconds(0));

                    Vector3 point = new Vector3(row, column, 0);

                    int closestPointIndex = FindClosestPoint(point);

                    if (closestPointIndex != -1)
                    {
                        var tilesetCount = _biomesData[closestPointIndex].tiles.Length;
                        int tile_id = _perlinNoise.GetIdUsingPerlin(row, column, tilesetCount);

                        GameObject biome_group = _biomesGroup[closestPointIndex];

                        Tile tile = CreateTile(point, closestPointIndex, tile_id, biome_group);
                        _tiles[row, column] = tile;


                    }

                }

            }
        }

        private Tile CreateTile(Vector3 point, int closestPointIndex, int tile_id, GameObject biome_group)
        {
            TileData tileData = _biomesData[closestPointIndex].GetTileById(tile_id);
            GameObject prefab = _biomesData[closestPointIndex].TilePrefab;
            GameObject tile = Instantiate(prefab, biome_group.transform.GetChild(tile_id));
            tile.name = string.Format("tile_x{0}_y{1} = ID{2}", point.x, point.y, tile_id);
            tile.transform.localPosition = new Vector3(point.x, point.y, 0);
            Tile tileScript = tile.AddComponent<Tile>();
            tileScript.X = (int)point.x;
            tileScript.Y = (int)point.y;
            tileScript.HeightId = tile_id;
            tileScript.BiomeId = _biomesData[closestPointIndex].Id;
            tileScript.Data = tileData;
            tileScript.ApplyDefaultSprite();
            return tileScript;
        }

        private int FindClosestPoint(Vector3 point)
        {
            if (_enableTurbulence)
            {
                Vector2 warpedPos = point + _turbulence.Get2DTurbulence(point);
                point = warpedPos;
            }
            int closestPointIndex = 0;

            var distance = Vector3.Distance(point, _biomes.SeedsPair.Keys.FirstOrDefault().Localization);

            foreach (KeyValuePair<BiomeSeed, int> _seedsPair in _biomes.SeedsPair)
            {
                var tempDistance = Vector3.Distance(point, _seedsPair.Key.Localization);
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    closestPointIndex = _seedsPair.Value;
                }
            }
            if (distance >= _biomeWith)
            {
                return -1;
            }

            return closestPointIndex;
        }

        public static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        private Tile GetTop(Tile t)
        {
            return _tiles[t.X, Mod(t.Y + 1, _mapColumns)];
        }
        private Tile GetBottom(Tile t)
        {
            return _tiles[t.X, Mod(t.Y - 1, _mapColumns)];
        }
        private Tile GetRight(Tile t)
        {
            return _tiles[Mod(t.X + 1, _mapRows), t.Y];
        }
        private Tile GetLeft(Tile t)
        {
            return _tiles[Mod(t.X - 1, _mapRows), t.Y];
        }

        private void UpdateNeighbors()
        {
            for (var row = 0; row < _mapRows; row++)
            {
                for (var column = 0; column < _mapColumns; column++)
                {

                    Tile t = _tiles[row, column];

                    if (t != null)
                    {
                        t.Top = GetTop(t);
                        t.Bottom = GetBottom(t);
                        t.Left = GetLeft(t);
                        t.Right = GetRight(t);
                    }

                }
            }
        }

        private void UpdateBitmasks()
        {
            for (var row = 0; row < _mapRows; row++)
            {
                for (var column = 0; column < _mapColumns; column++)
                {

                    Tile t = _tiles[row, column];

                    if (t != null)
                    {
                        t.UpdateBitmask();
                    }


                }
            }
        }


        private void OnDrawGizmosSelected()
        {
            DrawBiomeSeeds();

        }

        private void DrawBiomeSeeds()
        {
    
            foreach (KeyValuePair<BiomeSeed, int> item in _biomes.SeedsPair)
            {

                if (item.Key.Parent != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(item.Key.Parent.Localization, .2f);
                    Gizmos.DrawSphere(item.Key.Localization, .2f);

                    GUIStyle gUIStyle = new GUIStyle();
                    gUIStyle.normal.textColor = Color.red;
                    Handles.Label(item.Key.Parent.Localization, $"Seed:{item.Key.Parent.Localization} Type:{item.Value}", gUIStyle);
                    Handles.Label(item.Key.Localization, $"Seed:{item.Key.Localization} Type:{item.Value}", gUIStyle);
                    Debug.DrawLine(item.Key.Parent.Localization, item.Key.Localization, Color.red, 100f);

                }
                else
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(item.Key.Localization, .2f);
                    GUIStyle gUIStyle = new GUIStyle();
                    gUIStyle.normal.textColor = Color.yellow;
                    Handles.Label(item.Key.Localization, $"Type:{item.Value}", gUIStyle);
                }

            }
        }
    }
}