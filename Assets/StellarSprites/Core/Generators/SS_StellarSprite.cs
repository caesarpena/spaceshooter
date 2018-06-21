using UnityEngine;
using LibNoise;
using LibNoise.Generator;

namespace Stellar_Sprites
{
    public enum SS_NoiseGenerator
    {
        Perlin,
        RigidMultifractal
    }

    public enum SS_Mirror
    {
        TopRight,
        Vertical
    }

    public static class SS_StellarSprite
    {
        // Global constants
        public static Color FillColor = Color.magenta;
        public static Color OutlineColor = Color.black;

        public static bool Threaded = true;
        public static FilterMode filterMode = FilterMode.Bilinear;

        // Generate a generally boring metallic-like texture (ships and stations use this)
        public static SS_Texture GenerateBaseTexture(int seed, int textureWidth, int textureHeight, int detail)
        {
            SS_Texture tmpTexture = new SS_Texture(textureWidth, textureHeight, Color.clear);

            Perlin noise = new Perlin(0.005, 2, 0.5, 6, seed, QualityMode.Low);

            int offsetX = tmpTexture.Width / detail;
            int offsetY = tmpTexture.Height / detail;
            for (int y = 0; y < tmpTexture.Height; y += offsetY)
            {
                for (int x = 0; x < tmpTexture.Width; x += offsetX)
                {
                    float n1 = (float)noise.GetValue(x, y, 0);
                    n1 = (n1 + 3.0f) * 0.25f;
                    n1 = Mathf.Clamp(n1, 0.5f, 1f);

                    float n2 = (float)noise.GetValue(x, y, 0);
                    n2 = (n2 + 3.0f) * 0.25f;
                    n2 = Mathf.Clamp(n2, 0.95f, 1f);

                    Color c = new Color(n1, n1, n1, 1f);

                    SS_Drawing.RectangleFill(tmpTexture, x, y, offsetX, offsetY, c, c);

                    Color current = tmpTexture.GetPixel(x, y);
                    Color c3 = new Color(current.r * n2, current.g * n2, current.b * n2);
                    SS_Drawing.Line(tmpTexture, x, y, x, y + detail, c3);
                    SS_Drawing.Line(tmpTexture, x, y, x + detail, y, c3);
                }
            }

            return tmpTexture;
        }
        
        public static void Mirror(SS_Texture texture, SS_Mirror location)
        {
            // Mirror top right horizontally and vertically
            if (location == SS_Mirror.TopRight)
            {
                // Vertical
                for (int y = (texture.Height / 2); y < texture.Height; y++)
                {
                    for (int x = texture.Width / 2; x < texture.Width; x++)
                    {
                        Color target = texture.GetPixel(x, y);
                        if (target != Color.clear)
                        {
                            int y1 = texture.Height - y - 1;
                            texture.SetPixel(x, y1, texture.GetPixel(x, y));
                        }
                    }
                }

                // Horizontal
                for (int y = 0; y < texture.Height; y++)
                {
                    for (int x = (texture.Width / 2); x < texture.Width; x++)
                    {
                        Color target = texture.GetPixel(x, y);
                        if (target != Color.clear)
                        {
                            int x1 = texture.Width - x - 1;
                            texture.SetPixel(x1, y, texture.GetPixel(x, y));
                        }
                    }
                }
            }
            else if (location == SS_Mirror.Vertical)
            {
                // Vertical
                for (int y = (texture.Height / 2); y < texture.Height; y++)
                {
                    for (int x = 0; x < texture.Width; x++)
                    {
                        Color target = texture.GetPixel(x, y);
                        if (target != Color.clear)
                        {
                            int y1 = texture.Height - y - 1;
                            texture.SetPixel(x, y1, texture.GetPixel(x, y));
                        }
                    }
                }
            }
        }

        // Darken any pixels next the black borders
        public static void ShadeEdge(SS_Texture texture)
        {
            Color[] tmpColors = new Color[texture.Width * texture.Height];
            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    tmpColors[x + y * texture.Width] = texture.GetPixel(x, y);
                }
            }

            for (int y = 1; y < texture.Height - 1; y++)
            {
                for (int x = 1; x < texture.Width - 1; x++)
                {
                    Color c = texture.GetPixel(x, y);

                    if (c != Color.clear && c != Color.black)
                    {
                        Color tL = texture.GetPixel(x - 1, y - 1);
                        Color tM = texture.GetPixel(x, y - 1);
                        Color tR = texture.GetPixel(x + 1, y - 1);

                        Color mL = texture.GetPixel(x - 1, y);
                        Color mR = texture.GetPixel(x + 1, y);

                        Color bL = texture.GetPixel(x - 1, y + 1);
                        Color bM = texture.GetPixel(x, y + 1);
                        Color bR = texture.GetPixel(x + 1, y + 1);

                        if ((tL == Color.black || tM == Color.black || tR == Color.black ||
                             mL == Color.black || mR == Color.black ||
                             bL == Color.black || bM == Color.black || bR == Color.black))
                        {
                            c.r *= 0.5f;
                            c.g *= 0.5f;
                            c.b *= 0.5f;

                            tmpColors[x + y * texture.Width] = c;
                        }
                    }
                }
            }

            for (int x = 0; x < texture.Width; x++)
            {
                if (texture.GetPixel(x, 0) != Color.clear)
                {
                    texture.SetPixel(x, 0, Color.black);

                }
                if (texture.GetPixel(x, texture.Height - 1) != Color.clear)
                {
                    texture.SetPixel(x, texture.Height - 1, Color.black);
                }
            }

            texture.SetPixels(tmpColors);
        }

        // Light pixels as they approach black borders
        public static void PixelLighting(SS_Texture texture, int x, int y, ref Color hullShade)
        {
            int Width = texture.Width;
            int Height = texture.Height;

            float eastShading = 1.5f;
            float westShading = 1.5f;
            float northShading = 1.5f;
            float southShading = 1.5f;
            int shadingThickness = 4;

            bool hasEastBorder = false;
            int cntr = 0;
            while (!hasEastBorder)
            {
                int currentX = x + cntr;
                if (currentX < 0) currentX = 0;
                if (currentX > Width - 1) currentX = Width - 1;
                Color shadingPixel = texture.GetPixel(currentX, y);
                if (shadingPixel == Color.black)
                {
                    hasEastBorder = true;
                }

                cntr++;
                if (cntr > shadingThickness)
                {
                    break;
                }
            }
            if (hasEastBorder)
            {
                hullShade *= eastShading;
            }

            bool hasWestBorder = false;
            cntr = 0;
            while (!hasWestBorder)
            {
                int currentX = x - cntr;
                if (currentX < 0) currentX = 0;
                if (currentX > Width - 1) currentX = Width - 1;
                Color shadingPixel = texture.GetPixel(currentX, y);
                if (shadingPixel == Color.black)
                {
                    hasWestBorder = true;
                }

                cntr++;
                if (cntr > shadingThickness)
                {
                    break;
                }
            }
            if (hasWestBorder)
            {
                hullShade *= westShading;
            }

            bool hasNorthBorder = false;
            cntr = 0;
            while (!hasNorthBorder)
            {
                int currentY = y - cntr;
                if (currentY < 0) currentY = 0;
                if (currentY > Height - 1) currentY = Height - 1;
                Color shadingPixel = texture.GetPixel(x, currentY);
                if (shadingPixel == Color.black)
                {
                    hasNorthBorder = true;
                }

                cntr++;
                if (cntr > shadingThickness)
                {
                    break;
                }
            }
            if (hasNorthBorder)
            {
                hullShade *= northShading;
            }

            bool hasSouthBorder = false;
            cntr = 0;
            while (!hasSouthBorder)
            {
                int currentY = y + cntr;
                if (currentY < 0) currentY = 0;
                if (currentY > Height - 1) currentY = Height - 1;
                Color shadingPixel = texture.GetPixel(x, currentY);
                if (shadingPixel == Color.black)
                {
                    hasSouthBorder = true;
                }

                cntr++;
                if (cntr > shadingThickness)
                {
                    break;
                }
            }
            if (hasSouthBorder)
            {
                hullShade *= southShading;
            }
        }
    }
}