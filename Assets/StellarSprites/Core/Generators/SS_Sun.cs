using UnityEngine;

using LibNoise;
using LibNoise.Generator;

namespace Stellar_Sprites
{
    public class SS_Sun
    {
        public int Seed { get; set; }
        public int Size { get; set; }
        
        public SS_Texture Sprite;

        private Color[] highlightColors;

        public SS_Sun(int seed, int size, Color mainColor)
        {
            Seed = seed;
            Size = size;

            Sprite = new SS_Texture(Size, Size, Color.clear);

            Perlin noise = new Perlin(0.05, 2, 0.5, 8, Seed, QualityMode.Low);
            Perlin noiseGlow = new Perlin(0.005, 2, 0.5, 6, Seed, QualityMode.Low);

            float radius = Size * 0.75f;

            Color[] tmp = new Color[3];
            tmp[0] = Color.white;
            tmp[1] = mainColor;
            tmp[2] = new Color(255f / 255f, 102f / 255f, 0f);

            Color[] gradient = SS_Utilities.CreateGradient(tmp, 8, 32);

            float atmosphereThickness = Size * 0.125f;
            Color hotnessColor = Color.white;

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(Sprite.Center.x, Sprite.Center.y));

                    if (dist <= (radius / 2))
                    {
                        float n = (float)noise.GetValue(x, y, 0);
                        n = (n + 1.0f) * 0.5f;
                        n = Mathf.Clamp(n, 0f, 1f);
                        n *= (gradient.Length - 1);

                        Color baseColor = gradient[(int)n];

                        // Edge Hotness
                        hotnessColor.a = dist / (radius / 2);
                        hotnessColor.a += 0.0025f;

                        Color c = baseColor;
                        c.r = Mathf.Clamp(c.r + (hotnessColor.r * hotnessColor.a), 0, 1);
                        c.g = Mathf.Clamp(c.g + (hotnessColor.g * hotnessColor.a), 0, 1);
                        c.b = Mathf.Clamp(c.b + (hotnessColor.b * hotnessColor.a), 0, 1);

                        Sprite.SetPixel(x, y, c);
                    }

                    // Create glow
                    Color currentPixel = Sprite.GetPixel(x, y);

                    Color atmosphereColor = Color.white;
                    if (currentPixel == Color.clear)
                    {
                        atmosphereColor.a = 1;
                        float distToEdge = Vector2.Distance(new Vector2(x, y), new Vector2(Sprite.Center.x, Sprite.Center.y));
                        if (distToEdge < (radius / 2) + atmosphereThickness &&
                            distToEdge > (radius / 2))
                        {
                            float dist2 = dist - (radius / 2);
                            dist2 = (atmosphereThickness - dist2) / atmosphereThickness;

                            float glowNoise = (float)noiseGlow.GetValue(x, y, 0);
                            glowNoise = (glowNoise + 1.0f) * 0.5f;
                            glowNoise = Mathf.Clamp(glowNoise, 0f, 1f);
                            atmosphereColor.a = Mathf.Pow(dist2, 2) * glowNoise;

                            Sprite.SetPixel(x, y, atmosphereColor);
                        }
                    }
                }
            }
        }
    }
}
