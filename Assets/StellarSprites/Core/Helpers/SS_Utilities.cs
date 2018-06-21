using System.Collections.Generic;
using UnityEngine;

namespace Stellar_Sprites
{
    public static class SS_Utilities
    {
        public static Color Blend(Color color, Color backColor, float amount)
        {
            float r = ((color.r * amount) + backColor.r * (1f - amount));
            float g = ((color.g * amount) + backColor.g * (1f - amount));
            float b = ((color.b * amount) + backColor.b * (1f - amount));
            float a = backColor.a;

            return new Color(r, g, b, a);
        }

        public static SS_Point Centroid(SS_Point[] points)
        {
            Vector2 centroid = new Vector2(0, 0);
            float signedArea = 0.0f;
            float x0 = 0.0f; // Current vertex X
            float y0 = 0.0f; // Current vertex Y
            float x1 = 0.0f; // Next vertex X
            float y1 = 0.0f; // Next vertex Y
            float a = 0.0f;  // Partial signed area

            // For all vertices
            int i = 0;
            for (i = 0; i < points.Length; ++i)
            {
                x0 = points[i].x;
                y0 = points[i].y;
                x1 = points[(i + 1) % points.Length].x;
                y1 = points[(i + 1) % points.Length].y;
                a = x0 * y1 - x1 * y0;
                signedArea += a;
                centroid.x += (x0 + x1) * a;
                centroid.y += (y0 + y1) * a;
            }

            signedArea *= 0.5f;
            centroid.x /= (6.0f * signedArea);
            centroid.y /= (6.0f * signedArea);

            return new SS_Point((short)centroid.x, (short)centroid.y);
        }

        public static SS_Texture ColorWheel()
        {
            int padding = 0;
            int inner_radius = 0;
            int outer_radius = inner_radius + 128;

            int bmp_width = (2 * outer_radius) + (2 * padding);
            int bmp_height = bmp_width;

            SS_Texture spriteTexture = new SS_Texture(bmp_width, bmp_height, Color.black);
            var center = new Vector2(bmp_width / 2, bmp_height / 2);
            var c = Color.red;

            for (int y = 0; y < bmp_width; y++)
            {
                int dy = ((int)center.y - y);

                for (int x = 0; x < bmp_width; x++)
                {
                    int dx = ((int)center.x - x);

                    double dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist >= inner_radius && dist <= outer_radius)
                    {
                        double theta = Mathf.Atan2(dy, dx);
                        // theta can go from -pi to pi

                        double hue = (theta + Mathf.PI) / (2 * Mathf.PI);

                        double dr, dg, db;
                        const double sat = 1.0;
                        const double val = 1.0;
                        HSVToRGB(hue, sat, val, out dr, out dg, out db);

                        dr *= 0.75;
                        dg *= 0.75;
                        db *= 0.75;
                        c = new Color((float)dr, (float)dg, (float)db);

                        // Set
                        spriteTexture.SetPixel(x, y, c);
                    }
                }
            }

            return spriteTexture;
        }

        public static Color[] CreateGradient(Color[] colors, int minSteps, int maxSteps)
        {
            List<Color> tmp = new List<Color>();

            SS_Random random = new SS_Random(0);

            int steps = random.Range(minSteps, maxSteps);

            for (int j = 0; j < colors.Length - 1; j++)
            {
                Color start = colors[j];
                Color end = colors[j + 1];

                for (int i = 0; i < steps; i++)
                {
                    Color result = new
                        Color(
                            start.r + (i * (end.r - start.r) / steps),
                            start.g + (i * (end.g - start.g) / steps),
                            start.b + (i * (end.b - start.b) / steps)
                            );

                    tmp.Add(result);
                }
            }

            return tmp.ToArray();
        }

        public static void HSVToRGB(double H, double S, double V, out double R, out double G, out double B)
        {
            if (H == 1.0)
            {
                H = 0.0;
            }

            double step = 1.0 / 6.0;
            double vh = H / step;

            int i = (int)System.Math.Floor(vh);

            double f = vh - i;
            double p = V * (1.0 - S);
            double q = V * (1.0 - (S * f));
            double t = V * (1.0 - (S * (1.0 - f)));

            switch (i)
            {
                case 0:
                    {
                        R = V;
                        G = t;
                        B = p;
                        break;
                    }
                case 1:
                    {
                        R = q;
                        G = V;
                        B = p;
                        break;
                    }
                case 2:
                    {
                        R = p;
                        G = V;
                        B = t;
                        break;
                    }
                case 3:
                    {
                        R = p;
                        G = q;
                        B = V;
                        break;
                    }
                case 4:
                    {
                        R = t;
                        G = p;
                        B = V;
                        break;
                    }
                case 5:
                    {
                        R = V;
                        G = p;
                        B = q;
                        break;
                    }
                default:
                    {
                        // not possible - if we get here it is an internal error
                        throw new System.ArgumentNullException();
                    }
            }
        }

        public static Color[] GenerateColorWheelColors(int seed, int count)
        {
            SS_Texture colorWheel = ColorWheel();

            Color[] colors = new Color[count];

            SS_Random random = new SS_Random(seed);

            float angle = random.Range(0f, 360f);
            for (int i = 0; i < count; i++)
            {
                float xPos = (colorWheel.Width / 2) + Mathf.Cos(angle) * 32;
                float yPos = (colorWheel.Height / 2) + Mathf.Sin(angle) * 32;

                colors[i] = colorWheel.GetPixel((int)xPos, (int)yPos);
                angle += 360 / count;
            }

            return colors;
        }

        public static float Normalize(float v, float min, float max)
        {
            return (v - min) / (max - min);
        }

    }
}