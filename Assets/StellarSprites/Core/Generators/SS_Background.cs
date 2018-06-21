using UnityEngine;

using LibNoise;
using LibNoise.Generator;

namespace Stellar_Sprites
{
    public class SS_Background {


        public SS_Texture Sprite;
        
        public SS_Background(int seed, int size, SS_NoiseGenerator backgroundNoise, double frequency, double lacunarity, double persistence, int octaves, Color tint, float brightness)
        {
            // Create sprite texture
            Sprite = new SS_Texture(size, size, Color.white);

            // Initialize noise with parameters
            ModuleBase myModule;

            if (backgroundNoise == SS_NoiseGenerator.Perlin)
            {
                Perlin perlin = new Perlin(frequency, lacunarity, persistence, octaves, seed, QualityMode.Low);
                myModule = perlin; 
            }
            else
            {
                RidgedMultifractal ridgedMultifractal = new RidgedMultifractal(frequency, lacunarity, octaves, seed, QualityMode.Low);
                myModule = ridgedMultifractal;
            }   

            // Create seemless tiling noise
            Noise2D noise = new Noise2D(size, size, myModule);
            noise.GeneratePlanar(0, size, 0, size, false);

            // Get noise data
            float[,] noiseData = noise.GetNormalizedData(false, 0, 0);

            // Create cloud
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float n = noiseData[y, x]; ;
                    Color pixelColor = tint * n * brightness;

                    pixelColor.a = ((pixelColor.r + pixelColor.g + pixelColor.b) / 3.0f);

                    Sprite.SetPixel(x, y, pixelColor);
                }
            }
        }
    }
}
