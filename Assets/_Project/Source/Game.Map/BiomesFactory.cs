using System;
using System.Collections;
using System.Collections.Generic;
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
                biomes.SeedsPair.Add(randomPosition, i);

            }

            return biomes;
        }

        public static Biomes NewRRTSeeds(Vector2[,] points, int treeSize, int amountOfBIomes)
        {

            Biomes biomes = new Biomes();

            biomes.SeedsPair = CreateRRT(points, treeSize, amountOfBIomes);

            return biomes;
        }

        private static Dictionary<Vector3, int> CreateRRT(Vector2[,] points, int treeSize, int amountOfBiomes)
        {
            var paddingRow = points.GetLength(0) / 6;
            var paddingColunm = points.GetLength(1) / 6;

            Vector2 root = getCenterPoint(points);


            Dictionary<Vector3, int> rrt = new Dictionary<Vector3, int>
        {
            { root, 0 }
        };

            for (int i = 0; i < treeSize; i++)
            {

                Vector3 randomPoint = new Vector3(
                   UnityEngine.Random.Range(paddingRow, points.GetLength(0) - paddingRow),
                   UnityEngine.Random.Range(paddingColunm, points.GetLength(1) - paddingColunm), 0);

                Vector3 pointFar = nearest(root, randomPoint, rrt);

                Vector3 direction = randomPoint - pointFar;
                Vector3 newPoint = pointFar + direction.normalized * 2;

                if (!rrt.ContainsKey(randomPoint))
                {
                    rrt.Add(randomPoint, UnityEngine.Random.Range(0, amountOfBiomes));
                }

                var distance = Vector2.Distance(newPoint, randomPoint);

                while (distance >= 5)
                {
                    if (Vector3.Distance(newPoint, pointFar) > Vector3.Distance(newPoint, randomPoint))
                    {
                        if (!rrt.ContainsKey(newPoint))
                            rrt.Add(newPoint, rrt[randomPoint]);
                    }
                    else
                    {
                        if (!rrt.ContainsKey(newPoint))
                            rrt.Add(newPoint, rrt[pointFar]);


                    }



                    newPoint += direction.normalized * 2;
                    distance = Vector3.Distance(newPoint, randomPoint);
                    Debug.DrawLine(pointFar, newPoint, Color.red, 100f);
                    Debug.DrawLine(newPoint, randomPoint, Color.red, 100f);

                }

            }
            Vector2 getCenterPoint(Vector2[,] points)
            {
                return points[points.GetLength(0) / 2, points.GetLength(1) / 2];
            }

            Vector2 nearest(Vector2 root, Vector2 randomPoint, Dictionary<Vector3, int> rrt)
            {
                float distance = Vector2.Distance(randomPoint, root);

                Vector2 pointFar = root;

                foreach (KeyValuePair<Vector3, int> point in rrt)
                {
                    var tempDistance = Vector2.Distance(randomPoint, point.Key);

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
        public Dictionary<Vector3, int> SeedsPair;

        public Biomes()
        {
            SeedsPair = new Dictionary<Vector3, int>();
        }
    }
}