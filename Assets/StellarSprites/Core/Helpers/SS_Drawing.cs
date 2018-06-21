using UnityEngine;
using System.Collections.Generic;

namespace Stellar_Sprites
{
    public class SS_Texture
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private Color[] TextureData;

        public SS_Point Center
        {
            get { return new SS_Point(Width / 2, Height / 2); }
        }

        public SS_Texture(int width, int height, Color color)
        {
            Width = width;
            Height = height;

            TextureData = new Color[Width * Height];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    SetPixel(x, y, color);
                }
            }
        }

        //public void Crop(Color backgroundColor)
        //{
        //    int minX = Width;
        //    int maxX = 0;
        //    int minY = Height;
        //    int maxY = 0;

        //    for (int y = 0; y < Height; y++)
        //    {
        //        for (int x = 0; x < Width; x++)
        //        {
        //            Color c = GetPixel(x, y);
        //            if (c != backgroundColor)
        //            {
        //                if (x < minX) minX = x;
        //                if (y < minY) minY = y;
        //                if (x > maxX) maxX = x;
        //                if (y > maxY) maxY = y;
        //            }
        //        }
        //    }

        //    maxX++;
        //    maxY++;

        //    Debug.Log("MinX: " + minX + "\n");
        //    Debug.Log("MinY: " + minY + "\n");
        //    Debug.Log("MaxX: " + maxX + "\n");
        //    Debug.Log("MaxY: " + maxY + "\n");

        //    SS_Texture texture = new SS_Texture(maxX - minX, maxY - minY, backgroundColor);
        //    for (int y = 0; y < texture.Height; y++)
        //    {
        //        for (int x = 0; x < texture.Width; x++)
        //        {
        //            texture.SetPixel(x, y, GetPixel(minX + x, minY + y));
        //        }
        //    }

        //    Width = texture.Width;
        //    Height = texture.Height;
        //    TextureData = new Color[Width * Height];
        //    TextureData = texture.GetPixels();
        //}

        public Color GetPixel(int x, int y)
        {
            if (x >= 0 && x <= Width - 1 && y >= 0 && y <= Height - 1)
                return TextureData[x + (y * Width)];

            return Color.clear;
        }

        public Color[] GetPixels()
        {
            return TextureData;
        }

        public void SetPixel(int x, int y, Color c)
        {
            if (x >= 0 && x <= Width - 1 && y >= 0 && y <= Height - 1)
                TextureData[x + (y * Width)] = c;
        }

        public void SetPixels(Color[] data)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    SetPixel(x, y, data[x + (y * Width)]);
                }
            }
        }
    }

    public static class SS_Drawing
    {
        public static void Blur(ref SS_Texture texture)
        {
            SS_Texture tmp = new SS_Texture(texture.Width, texture.Height, Color.clear);

            float v = 1.0f / 9.0f;
            float[,] kernel = { { v, v, v },
                                { v, v, v },
                                { v, v, v }};

            // Loop through every pixel in the image
            for (int y = 1; y < texture.Height - 1; y++)
            {
                for (int x = 1; x < texture.Width - 1; x++)
                {
                    float alpha = texture.GetPixel(x, y).a;

                    Color sum = Color.clear; // Kernel sum for this pixel
                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            sum += kernel[ky + 1, kx + 1] * texture.GetPixel(x + kx, y + ky);
                        }
                    }

                    sum.a = alpha;
                    tmp.SetPixel(x, y, sum);
                }
            }

            // Apply changes
            texture = tmp;
        }

        private static void Ellipse(SS_Texture texture, int x, int y, int rW, int rH, int resolution, bool fill, Color colorOutline, Color colorFill)
        {
            SS_Point[] positions = new SS_Point[resolution + 1];

            for (int i = 0; i <= resolution; i++)
            {
                float angle = (float)i / (float)resolution * 2.0f * Mathf.PI;
                positions[i] = new SS_Point(x + ((short)(rW * Mathf.Cos(angle))), y + ((short)(rH * Mathf.Sin(angle))));
            }

            Polygon(texture, positions, colorOutline);

            if (fill)
            {
                FloodFillArea(texture, new SS_Point(x, y), colorFill);
            }
        }

        public static void Ellipse(SS_Texture texture, int aX, int aY, int aW, int aH, int resolution, Color color)
        {

            Ellipse(texture, aX, aY, aW, aH, resolution, false, color, Color.clear);
        }

        public static void EllipseFill(SS_Texture texture, int aX, int aY, int aW, int aH, int resolution, Color color, Color colorFill)
        {
            Ellipse(texture, aX, aY, aW, aH, resolution, true, color, colorFill);
        }

        public static void FloodFillArea(SS_Texture texture, SS_Point pt, Color aColor)
        {

            int w = texture.Width;
            int h = texture.Height;

            Color[] colors = texture.GetPixels();
            Color refCol = colors[pt.x + pt.y * w];

            Queue<SS_Point> nodes = new Queue<SS_Point>();
            nodes.Enqueue(new SS_Point(pt.x, pt.y));

            while (nodes.Count > 0)
            {
                SS_Point current = nodes.Dequeue();

                for (int i = current.x; i < w; i++)
                {
                    Color C = colors[i + current.y * w];

                    if (C != refCol || C == aColor)
                        break;

                    colors[i + current.y * w] = aColor;

                    if (current.y + 1 < h)
                    {
                        C = colors[i + current.y * w + w];
                        if (C == refCol && C != aColor)
                            nodes.Enqueue(new SS_Point(i, current.y + 1));
                    }

                    if (current.y - 1 >= 0)
                    {
                        C = colors[i + current.y * w - w];
                        if (C == refCol && C != aColor)
                            nodes.Enqueue(new SS_Point(i, current.y - 1));
                    }
                }

                for (int i = current.x - 1; i >= 0; i--)
                {
                    Color C = colors[i + current.y * w];

                    if (C != refCol || C == aColor)
                        break;

                    colors[i + current.y * w] = aColor;

                    if (current.y + 1 < h)
                    {
                        C = colors[i + current.y * w + w];

                        if (C == refCol && C != aColor)
                            nodes.Enqueue(new SS_Point(i, current.y + 1));
                    }

                    if (current.y - 1 >= 0)
                    {
                        C = colors[i + current.y * w - w];

                        if (C == refCol && C != aColor)
                            nodes.Enqueue(new SS_Point(i, current.y - 1));
                    }
                }
            }

            texture.SetPixels(colors);
        }

        public static void FloodFill(SS_Texture texture, SS_Point pt, Color targetColor, Color replacementColor)
        {
            targetColor = texture.GetPixel(pt.x, pt.y);
            if (targetColor == replacementColor)
            {
                return;
            }

            Stack<SS_Point> pixels = new Stack<SS_Point>();

            pixels.Push(pt);
            while (pixels.Count != 0)
            {
                SS_Point temp = pixels.Pop();
                int y1 = temp.y;
                while (y1 >= 0 && texture.GetPixel(temp.x, y1) == targetColor)
                {
                    y1--;
                }
                y1++;
                bool spanLeft = false;
                bool spanRight = false;
                while (y1 < texture.Height && texture.GetPixel(temp.x, y1) == targetColor)
                {
                    texture.SetPixel(temp.x, y1, replacementColor);

                    if (!spanLeft && temp.x > 0 && texture.GetPixel(temp.x - 1, y1) == targetColor)
                    {
                        pixels.Push(new SS_Point(temp.x - 1, y1));
                        spanLeft = true;
                    }
                    else if (spanLeft && temp.x - 1 == 0 && texture.GetPixel(temp.x - 1, y1) != targetColor)
                    {
                        spanLeft = false;
                    }
                    if (!spanRight && temp.x < texture.Width - 1 && texture.GetPixel(temp.x + 1, y1) == targetColor)
                    {
                        pixels.Push(new SS_Point(temp.x + 1, y1));
                        spanRight = true;
                    }
                    else if (spanRight && temp.x < texture.Width - 1 && texture.GetPixel(temp.x + 1, y1) != targetColor)
                    {
                        spanRight = false;
                    }
                    y1++;
                }
            }
        }

        public static void Line(SS_Texture texture, int aX, int aY, int aX2, int aY2, Color color)
        {
            int w = aX2 - aX;
            int h = aY2 - aY;

            int dx1 = 0;
            int dy1 = 0;
            int dx2 = 0;
            int dy2 = 0;

            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;

            int longest = Mathf.Abs(w);
            int shortest = Mathf.Abs(h);

            if (!(longest > shortest))
            {
                longest = Mathf.Abs(h);
                shortest = Mathf.Abs(w);

                if (h < 0)
                    dy2 = -1;
                else if (h > 0)
                    dy2 = 1;

                dx2 = 0;
            }

            int numerator = longest >> 1;

            for (int i = 0; i <= longest; i++)
            {
                texture.SetPixel(aX, aY, color);

                numerator += shortest;

                if (!(numerator < longest))
                {
                    numerator -= longest;
                    aX += dx1;
                    aY += dy1;
                }
                else
                {
                    aX += dx2;
                    aY += dy2;
                }
            }
        }

        public static void LineStrip(SS_Texture texture, SS_Point[] SS_Points, Color color)
        {
            SS_Point p, p2;

            // Loop through all SS_Points and draw lines between them except for the last one
            for (int i = 0; i < SS_Points.Length - 1; i++)
            {

                p = new SS_Point((short)SS_Points[i].x, (short)SS_Points[i].y);
                p2 = new SS_Point((short)SS_Points[i + 1].x, (short)SS_Points[i + 1].y);

                Line(texture, p.x, p.y, p2.x, p2.y, color);
            }
        }

        public static void LineThick(SS_Texture texture, SS_Point start, SS_Point end, int width, Color outlineColor, Color fillColor)
        {
            SS_Texture tmpTexture = new SS_Texture(texture.Width, texture.Height, Color.clear);
            SS_Point[] points = new SS_Point[4];

            float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;         

            points[0] = new SS_Point(
                (int)(start.x + Mathf.Cos((angle - 90f) * Mathf.Deg2Rad) * (width / 2)),
                (int)(start.y + Mathf.Sin((angle - 90f) * Mathf.Deg2Rad) * (width / 2)));

            points[1] = new SS_Point(
                (int)(start.x + Mathf.Cos((angle + 90f) * Mathf.Deg2Rad) * (width / 2)),
                (int)(start.y + Mathf.Sin((angle + 90f) * Mathf.Deg2Rad) * (width / 2)));

            points[2] = new SS_Point(
                (int)(end.x + Mathf.Cos((angle + 90f) * Mathf.Deg2Rad) * (width / 2)),
                (int)(end.y + Mathf.Sin((angle + 90f) * Mathf.Deg2Rad) * (width / 2)));

            points[3] = new SS_Point(
                (int)(end.x + Mathf.Cos((angle - 90f) * Mathf.Deg2Rad) * (width / 2)),
                (int)(end.y + Mathf.Sin((angle - 90f) * Mathf.Deg2Rad) * (width / 2)));

            LineStripClosed(tmpTexture, points, outlineColor);
            FloodFillArea(tmpTexture, SS_Utilities.Centroid(points), fillColor);

            MergeColors(texture, tmpTexture, 0, 0);
        }

        public static void LineStripClosed(SS_Texture texture, SS_Point[] SS_Points, Color color)
        {
            SS_Point p, p2;

            // Loop through all SS_Points and draw lines between them except for the last one
            for (int i = 0; i < SS_Points.Length - 1; i++)
            {

                p = new SS_Point((short)SS_Points[i].x, (short)SS_Points[i].y);
                p2 = new SS_Point((short)SS_Points[i + 1].x, (short)SS_Points[i + 1].y);

                Line(texture, p.x, p.y, p2.x, p2.y, color);
            }

            p = new SS_Point((short)SS_Points[0].x, (short)SS_Points[0].y);
            p2 = new SS_Point((short)SS_Points[SS_Points.Length-1].x, (short)SS_Points[SS_Points.Length - 1].y);
            Line(texture, p.x, p.y, p2.x, p2.y, color);
        }

        public static void MergeColors(SS_Texture target, SS_Texture source, int xOffset, int yOffset)
        {
            int x1 = xOffset;
            int y1 = yOffset;

            int x2 = 0;
            int y2 = 0;
            for (int y = y1; y < y1 + source.Height; y++)
            {
                x2 = 0;
                for (int x = x1; x < x1 + source.Width; x++)
                {
                    Color c = source.GetPixel(x2, y2);

                    if (c != Color.clear)
                    {
                        target.SetPixel(x, y, c);
                    }
                    x2++;
                }
                y2++;
            }
        }

        public static void Outline(SS_Texture spriteTexture, Color outlineColor)
        {
            for (int y = 1; y < spriteTexture.Height - 1; y++)
            {
                for (int x = 1; x < spriteTexture.Width - 1; x++)
                {
                    Color c = spriteTexture.GetPixel(x, y);

                    if (c != Color.clear)
                    {
                        Color tL = spriteTexture.GetPixel(x - 1, y - 1);
                        Color tM = spriteTexture.GetPixel(x, y - 1);
                        Color tR = spriteTexture.GetPixel(x + 1, y - 1);

                        Color mL = spriteTexture.GetPixel(x - 1, y);
                        Color mR = spriteTexture.GetPixel(x + 1, y);

                        Color bL = spriteTexture.GetPixel(x - 1, y + 1);
                        Color bM = spriteTexture.GetPixel(x, y + 1);
                        Color bR = spriteTexture.GetPixel(x + 1, y + 1);

                        if (tL == Color.clear || tM == Color.clear || tR == Color.clear ||
                            mL == Color.clear || mR == Color.clear ||
                            bL == Color.clear || bM == Color.clear || bR == Color.clear)
                        {
                            spriteTexture.SetPixel(x, y, outlineColor);
                        }
                    }

                }
            }

            for (int y = 0; y < spriteTexture.Height; y++)
            {
                for (int x = 0; x < spriteTexture.Width; x++)
                {
                    Color c = spriteTexture.GetPixel(x, y);

                    if (c != Color.clear)
                    {
                        if (x == 0 || x == spriteTexture.Width - 1 || y == 0 || y == spriteTexture.Height - 1)
                        {
                            spriteTexture.SetPixel(x, y, outlineColor);
                        }
                    }
                }
            }
        }

        public static void Pixel(SS_Texture texture, int x, int y, Color color)
        {
            texture.SetPixel(x, y, color);
        }

        public static void Polygon(SS_Texture texture, SS_Point[] points, bool fill, Color outlineColor, Color fillColor)
        {
            SS_Point p, p2;

            // Loop through all SS_Points and draw lines between them except for the last one
            for (int i = 0; i < points.Length - 1; i++)
            {
                p = new SS_Point((short)points[i].x, (short)points[i].y);
                p2 = new SS_Point((short)points[i + 1].x, (short)points[i + 1].y);

                Line(texture, p.x, p.y, p2.x, p2.y, outlineColor);
            }

            // Last SS_Point connects to first SS_Point (order is important)
            p = new SS_Point((short)points[0].x, (short)points[0].y);
            p2 = new SS_Point((short)points[points.Length - 1].x, (short)points[points.Length - 1].y);
            
            Line(texture, p.x, p.y, p2.x, p2.y, outlineColor);

            if (fill)
            {
                SS_Point centroid = SS_Utilities.Centroid(points);
                FloodFillArea(texture, centroid, fillColor);
            }
        }

        public static void Polygon(SS_Texture texture, SS_Point[] SS_Points, Color color)
        {
            Polygon(texture, SS_Points, false, color, Color.clear);
        }

        public static void PolygonFill(SS_Texture texture, SS_Point[] SS_Points, Color color, Color fillColor)
        {
            Polygon(texture, SS_Points, true, color, fillColor);
        }

        private static void Rectangle(SS_Texture texture, int aX, int aY, int aW, int aH, bool fill, Color color, Color fillColor)
        {
            Line(texture, aX, aY, aX + aW, aY, color);
            Line(texture, aX + aW, aY, aX + aW, aY + aH, color);
            Line(texture, aX, aY + aH, aX + aW, aY + aH, color);
            Line(texture, aX, aY, aX, aY + aH, color);

            if (fill)
            {
                FloodFillArea(texture, new SS_Point(aX + (aW / 2), aY + (aH / 2)), fillColor);
            }
        }

        public static void Rectangle(SS_Texture texture, int aX, int aY, int aW, int aH, Color color)
        {
            Rectangle(texture, aX, aY, aW, aH, false, color, Color.clear);
        }

        public static void RectangleFill(SS_Texture texture, int aX, int aY, int aW, int aH, Color color, Color fillColor)
        {
            Rectangle(texture, aX, aY, aW, aH, true, color, fillColor);
        }

        public static void Swirl(SS_Texture texture, int aX, int aY, int radius, float twists)
        {
            SS_Texture tmpTexture = new SS_Texture(texture.Width, texture.Height, Color.clear);

            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    // compute the distance and angle from the swirl center:
                    int pixelX = x - aX;
                    int pixelY = y - aY;
                    float pixelDistance = Mathf.Sqrt((pixelX * pixelX) + (pixelY * pixelY));
                    float pixelAngle = Mathf.Atan2(pixelY, pixelX);

                    // work out how much of a swirl to apply (1.0 in the center fading out to 0.0 at the radius):

                    float swirlAmount = 1.0f - (pixelDistance / radius);

                    if (swirlAmount > 0.0f)
                    {
                        float twistAngle = twists * swirlAmount * Mathf.PI * 2.0f;

                        // adjust the pixel angle and compute the adjusted pixel co-ordinates:
                        pixelAngle += twistAngle;
                        pixelX = (int)(Mathf.Cos(pixelAngle) * pixelDistance);
                        pixelY = (int)(Mathf.Sin(pixelAngle) * pixelDistance);

                    }

                    // read and write the pixel
                    tmpTexture.SetPixel(x, y, texture.GetPixel(aX + pixelX, aY + pixelY));
                }
            }

            texture.SetPixels(tmpTexture.GetPixels());
        }
    }
}