using UnityEngine;

using LibNoise;
using LibNoise.Generator;

namespace Stellar_Sprites
{
    public class SS_Moon
    {
        public int Seed { get; set; }
        public int Size { get; set; }

        public SS_Texture Sprite;

        private Color[] gradientColors;

        public SS_Moon(int seed, int size, float frequency, float lacunarity, int octaves, float roughness, Color[] colors, float lightAngle)
        {
            Seed = seed;
            Size = size;

            Sprite = new SS_Texture(size, size, Color.clear);
            gradientColors = SS_Utilities.CreateGradient(colors, 16, 32);

            Perlin shapeNoise = new Perlin(0.01, 2, 0.5, 8, seed, QualityMode.High);
            RidgedMultifractal noise = new RidgedMultifractal(frequency, lacunarity, octaves, seed, QualityMode.Low);
            
            Vector2 lightPosition = new Vector2(
                Sprite.Center.x + (Mathf.Cos(lightAngle * Mathf.Deg2Rad) * (Size / 4)),
                Sprite.Center.y + (Mathf.Sin(lightAngle * Mathf.Deg2Rad) * (Size / 4)));
            
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(Sprite.Center.x, Sprite.Center.y));
					float edgeNoise = (float)shapeNoise.GetValue(x, y, 0);
					edgeNoise = (edgeNoise + 1.0f) * 0.5f;
					edgeNoise = Mathf.Clamp(edgeNoise, 0f, 1f);
					edgeNoise *= (8 * roughness);

					if (dist < (Size / 2) - edgeNoise)
                    {
                        float pixelNoise = (float)noise.GetValue(x, y, 0);
                        pixelNoise = (pixelNoise + 1.0f) * 0.5f;
                        pixelNoise = Mathf.Clamp(pixelNoise, 0f, 1f);

                        float n = pixelNoise * (gradientColors.Length - 1);

                        // Generate color and noise so land doesn't look to smooth
                        Color pixelColor = gradientColors[(int)n];
                        pixelColor.a = 1.0f;

                        Sprite.SetPixel(x, y, pixelColor);

                        // Shadow
                        float lightDistance = Vector2.Distance(new Vector2(x, y), lightPosition);
                        lightDistance = 1.25f - (lightDistance / (Size / 2));
                        if (lightDistance < 0.025f)
                            lightDistance = 0.025f;

                        pixelColor.r *= lightDistance;
                        pixelColor.g *= lightDistance;
                        pixelColor.b *= lightDistance;
                        Sprite.SetPixel(x, y, pixelColor);
                    }
                }
            }

        }
    }
}
