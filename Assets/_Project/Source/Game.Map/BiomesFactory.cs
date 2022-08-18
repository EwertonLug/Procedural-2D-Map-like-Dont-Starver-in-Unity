using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Map
{
    public class BiomesFactory : MonoBehaviour
    {
        public static Biomes NewRandomSeeds(int _mapRows, int _mapColunms, int amountBiomes)
        {
            Biomes biomes = new Biomes();

            var paddingRow = _mapRows / 5;
            var paddingColunm = _mapColunms / 5;

            for (int i = 0; i < amountBiomes; i++)
            {
                Vector3 randomPosition = new Vector3(
                    UnityEngine.Random.Range(paddingRow, _mapRows - paddingRow),
                    UnityEngine.Random.Range(paddingColunm, _mapColunms - paddingColunm), 0);
                biomes.SeedsPair.Add(new BiomeSeed(randomPosition), i);

            }

            return biomes;
        }

        public static Biomes NewRRTSeeds(Vector2[,] points, int treeSize, int amountOfBIomes)
        {

            Biomes biomes = new Biomes();

            biomes.SeedsPair = CreateRRT(points, treeSize, amountOfBIomes);

            return biomes;
        }


        private static Dictionary<BiomeSeed, int> CreateRRT(Vector2[,] points, int treeSize, int amountOfBiomes)
        {
            var paddingRow = points.GetLength(0) / 6;
            var paddingColunm = points.GetLength(1) / 6;

            BiomeSeed root = getCenterPoint(points);

            Dictionary<BiomeSeed, int> rrt = new Dictionary<BiomeSeed, int>
            {
               { root , 0 }
            };

            List<Vector3> randomPoints = new List<Vector3>();

            for (int i = 0; i < treeSize; i++)
            {
                Vector3 randomPoint = Vector3.zero;

                while (!randomPoints.Contains(randomPoint))
                {
                    randomPoint = new Vector3(
                   UnityEngine.Random.Range(paddingRow, points.GetLength(0) - paddingRow),
                   UnityEngine.Random.Range(paddingColunm, points.GetLength(1) - paddingColunm), 0);
                    randomPoints.Add(randomPoint);
                }


                BiomeSeed newRandomSeed = new BiomeSeed(randomPoint);
                BiomeSeed pointFar = nearest(randomPoint, rrt);

                Vector3 direction = newRandomSeed.Localization - pointFar.Localization;
                Vector3 newPoint = pointFar.Localization + direction.normalized * 2;

                newRandomSeed.SetParent(pointFar);

                BiomeSeed newSeed = new BiomeSeed(newPoint);

                if (!rrt.ContainsKey(newRandomSeed))
                {
                    rrt.Add(newRandomSeed, UnityEngine.Random.Range(0, amountOfBiomes));
                }

                var distance = Vector2.Distance(newSeed.Localization, newRandomSeed.Localization);

                while (distance >= 5)
                {
                    if (Vector3.Distance(newSeed.Localization, pointFar.Localization) > Vector3.Distance(newSeed.Localization, newRandomSeed.Localization))
                    {

                        if (!rrt.ContainsKey(newSeed))
                        {
                            rrt.Add(newSeed, rrt[newRandomSeed]);
                        }
                    }
                    else
                    {
                        if (!rrt.ContainsKey(newSeed))
                        {
                            rrt.Add(newSeed, rrt[pointFar]);
                        }

                    }

                    newPoint += direction.normalized * 2;
                    newSeed = new BiomeSeed(newPoint);
                    distance = Vector3.Distance(newPoint, randomPoint);

                }

            }
            BiomeSeed getCenterPoint(Vector2[,] points)
            {
                return new BiomeSeed(points[points.GetLength(0) / 2, points.GetLength(1) / 2]);
            }

            BiomeSeed nearest(Vector2 randomPoint, Dictionary<BiomeSeed, int> rrt)
            {
                float distance = Vector2.Distance(randomPoint, root.Localization);

                BiomeSeed pointFar = rrt.Keys.FirstOrDefault();

                foreach (KeyValuePair<BiomeSeed, int> point in rrt)
                {
                    var tempDistance = Vector2.Distance(randomPoint, point.Key.Localization);

                    if (tempDistance < distance)
                    {
                        distance = tempDistance;
                        pointFar = point.Key;

                    }
                }

                return pointFar;
            }
            return rrt;
        }
    }

    public class Biomes
    {
        public Dictionary<BiomeSeed, int> SeedsPair;

        public Biomes()
        {
            SeedsPair = new Dictionary<BiomeSeed, int>();
        }
    }
    public class BiomeSeed
    {
        private Vector3 _localization;
        private BiomeSeed _parent;

        public Vector3 Localization => _localization;
        public BiomeSeed Parent => _parent;

        public BiomeSeed(Vector3 localization)
        {
            _localization = localization;
        }

        public void SetParent(BiomeSeed parent)
        {
            _parent = parent;
        }

    }
}