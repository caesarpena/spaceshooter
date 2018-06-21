using UnityEngine;

using LibNoise;
using LibNoise.Generator;

namespace Stellar_Sprites
{
    public class SS_Cloud {

        public SS_Texture Sprite;
        
        public SS_Cloud(int seed, int size, double frequency, double lacunarity, int octaves, Color tint, float brightness)
        {
            // Create sprite texture
            Sprite = new SS_Texture(size, size, Color.white);

            // Initialize noise with parameters
            RidgedMultifractal noise = new RidgedMultifractal(frequency, lacunarity, octaves, seed, QualityMode.Medium);
            
            // Create cloud
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = SS_Point.Distance(new SS_Point(x, y), Sprite.Center);

                    float n = (float)noise.GetValue(x, y, 0);
                    Color pixelColor = tint * n * brightness;

                    float blackness = ((pixelColor.r + pixelColor.g + pixelColor.b) / 3.0f);
                    float fade = distance / (size / 2);

                    pixelColor.a = (1f - fade) * blackness;
                    if (distance > (size / 2))
                        pixelColor.a = 0f;

                    Sprite.SetPixel(x, y, pixelColor);
                }
            }
        }
    }
}
