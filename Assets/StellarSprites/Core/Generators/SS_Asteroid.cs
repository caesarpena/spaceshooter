using UnityEngine;

using LibNoise;
using LibNoise.Generator;

namespace Stellar_Sprites
{
    public class SS_Asteroid
    {
        public int Seed { get; set; }
        public int Size { get; set; }

        public SS_Texture Sprite;

        public SS_Asteroid(int seed, int size, Color[] colors, bool minerals, Color mineralColor, float lightAngle)
        {
            // Set Seed and Size
            Seed = seed;
            Size = size;

            // Create sprite texture
            Sprite = new SS_Texture(Size, Size, Color.clear);

            // Generate a color gradient
            Color[] gradientColors = SS_Utilities.CreateGradient(colors, 4, 8);

            // Initialize noise with parameters
            Perlin perlin = new Perlin(0.01, 2, 0.5, 8, Seed, QualityMode.Low);
            Voronoi mineralNoise = new Voronoi(0.1, 0.25, Seed + 1, true);

            // Set the light position based on the angle parameter
            Vector2 lightPosition = new Vector2(
                Sprite.Center.x + (Mathf.Cos(lightAngle * Mathf.Deg2Rad) * (Size / 4)),
                Sprite.Center.y + (Mathf.Sin(lightAngle * Mathf.Deg2Rad) * (Size / 4)));

            // Begin generating color data
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    // Distance of current pixel to the center of the sprite
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(Sprite.Center.x, Sprite.Center.y));

                    // Get a noise value for the edge of the asteroid - adds roughness instead of a perfect circle
                    float edgeNoise = (float)perlin.GetValue(x, y, 0);
                    edgeNoise = (edgeNoise + 1.0f) * 0.5f;
                    edgeNoise = Mathf.Clamp(edgeNoise, 0f, 1f);
                    edgeNoise *= 16;

                    if (dist < (Size / 2) - edgeNoise)
                    {
                        float pixelNoise = (float)perlin.GetValue(x, y, 0);
                        pixelNoise = (pixelNoise + 1.0f) * 0.5f;
                        pixelNoise = Mathf.Clamp(pixelNoise, 0f, 1f);

                        float n = pixelNoise * (gradientColors.Length - 1);

                        // Generate color and noise so land doesn't look to smooth
                        Color pixelColor = gradientColors[(int)n];
                        pixelColor.a = 1.0f;

                        // Minerals
                        if (minerals)
                        {
                            float mineralAlpha = (float)mineralNoise.GetValue(x, y, 0);
                            mineralAlpha = (1.0f + mineralAlpha) * 0.5f;
                            mineralAlpha = Mathf.Clamp(mineralAlpha, 0.0f, 1.0f);
                            if (mineralAlpha > 0.65f)
                            {
                                pixelColor.r = mineralAlpha * mineralColor.r + (1 - mineralAlpha) * pixelColor.r;
                                pixelColor.g = mineralAlpha * mineralColor.g + (1 - mineralAlpha) * pixelColor.g;
                                pixelColor.b = mineralAlpha * mineralColor.b + (1 - mineralAlpha) * pixelColor.b;
                                pixelColor.a = 1f;
                            }
                        }

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
