using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Map
{
    [System.Serializable]
    public class Turbulence
    {
        [Header("Settings")]

        [SerializeField] private float _baseScale = 50.0f;
        [SerializeField] private int _octaveCount = 4;
        [SerializeField] private float _amplitude = 5.0f;
        [SerializeField] private float _lacunarity = 2.0f;
        [SerializeField] private float _persistence = 0.5f;

        // Arbitrary numbers to break up visible correlation between octaves / x & y
        [SerializeField] private Vector3 _seed = new Vector2(-71, 37);

        public Vector3 Get2DTurbulence(Vector3 input)
        {

            input = input / _baseScale + _seed;
            float a = 2f * _amplitude;

            Vector2 noise = Vector2.zero;

            for (int octave = 0; octave < _octaveCount; octave++)
            {
                noise.x += a * (Mathf.PerlinNoise(input.x, input.y) - 0.5f);
                noise.y += a * (Mathf.PerlinNoise(input.x + _seed.y, input.y + _seed.y) - 0.5f);
                input = input * _lacunarity + _seed;
                a *= _persistence;
            }

            return noise;
        }
    }
}