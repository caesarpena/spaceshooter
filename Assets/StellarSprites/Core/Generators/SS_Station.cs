using UnityEngine;
using System.Collections.Generic;

using LibNoise;
using LibNoise.Generator;

namespace Stellar_Sprites
{
    public enum SS_StationType
    {
        Pod,
        Cool
    }

    public class SS_Station
    {
        public int Seed { get; set; }
        public int Size { get { return 256; } }

        public SS_Texture Sprite;

        List<SS_Point> LightPoints = new List<SS_Point>();

		private SS_Random random;

        public SS_Station(int seed, SS_StationType stationType, Color tint, int numberOfPods)
        {
            Seed = seed;

            random = new SS_Random(Seed);
            Sprite = new SS_Texture(Size, Size, Color.clear);

            if (stationType == SS_StationType.Cool)
            {
                CreateRing(random.Range(0.85f, 1.0f), random.Range(1, 4), true, random.NextColor(), false);
                CreateRing(random.Range(0.5f, 0.75f), random.Range(1, 4), true, random.NextColor(), true);

                // Draw lights
                for (int i = 0; i < LightPoints.Count; i++)
                {
                    CreateFlare(Sprite, LightPoints[i], 16, true, Color.white);
                }

                int podCount = random.RangeEven(2, 8);
                int podWidth = random.RangeEven(24, 32);
                int podHeight = random.RangeEven(12, 24);
                int podDistance = random.RangeEven(64, Size / 2 - 32);
                CreatePods(podCount, podWidth, podHeight, 8, podDistance, 1.0, Color.grey);
                CreatePods(podCount, podWidth, podHeight, 8, podDistance, 0.75, Color.grey);

                Color[] flareColors = SS_Utilities.GenerateColorWheelColors(Seed, 3);
                CreateFlare(Sprite, new SS_Point(Sprite.Center.x, Sprite.Center.y), 128, true, flareColors[0]);
                CreateFlare(Sprite, new SS_Point(Sprite.Center.x, Sprite.Center.y), 64, true, flareColors[1]);
                CreateFlare(Sprite, new SS_Point(Sprite.Center.x, Sprite.Center.y), 32, true, flareColors[2]);

            }
            else if (stationType == SS_StationType.Pod)
            { 
                int podCount = numberOfPods;
                int podSize = 64;
                int step = 360 / podCount;
                int bridgeWidth = random.RangeEven(8, 16);

                // Setup pod positions
                List<SS_Point> podPositions = new List<SS_Point>();
                for (int a = 0; a < 359; a += step)
                {
                    int x = Sprite.Center.x + (int)(Mathf.Cos(a * Mathf.Deg2Rad) * 96);
                    int y = Sprite.Center.y + (int)(Mathf.Sin(a * Mathf.Deg2Rad) * 96);

                    podPositions.Add(new SS_Point(x, y));
                }

                for (int i = 0; i < podPositions.Count; i++)
                {
                    SS_Texture tmpBridgeTexture = new SS_Texture(Size, Size, Color.clear);
                    List<SS_Point> points = new List<SS_Point>();

                    int px1 = podPositions[i].x + (int)(Mathf.Cos((i * step - 90) * Mathf.Deg2Rad) * bridgeWidth);
                    int py1 = podPositions[i].y + (int)(Mathf.Sin((i * step - 90) * Mathf.Deg2Rad) * bridgeWidth);
                    int px2 = podPositions[i].x + (int)(Mathf.Cos((i * step + 90) * Mathf.Deg2Rad) * bridgeWidth);
                    int py2 = podPositions[i].y + (int)(Mathf.Sin((i * step + 90) * Mathf.Deg2Rad) * bridgeWidth);

                    int cx1 = Sprite.Center.x + (int)(Mathf.Cos((i * step - 90) * Mathf.Deg2Rad) * bridgeWidth);
                    int cy1 = Sprite.Center.y + (int)(Mathf.Sin((i * step - 90) * Mathf.Deg2Rad) * bridgeWidth);
                    int cx2 = Sprite.Center.x + (int)(Mathf.Cos((i * step + 90) * Mathf.Deg2Rad) * bridgeWidth);
                    int cy2 = Sprite.Center.y + (int)(Mathf.Sin((i * step + 90) * Mathf.Deg2Rad) * bridgeWidth);

                    points.Add(new SS_Point(cx1, cy1));
                    points.Add(new SS_Point(px1, py1));
                    points.Add(new SS_Point(px2, py2));
                    points.Add(new SS_Point(cx2, cy2));

                    SS_Drawing.PolygonFill(tmpBridgeTexture, points.ToArray(), SS_StellarSprite.FillColor, SS_StellarSprite.FillColor);
                    SS_Drawing.MergeColors(Sprite, tmpBridgeTexture, 0, 0);
                }

                int numPoints = random.RangeEven(6, 10);
                for (int i = 0; i < podPositions.Count; i++)
                {
                    float angleStep = 360.0f / numPoints;

                    List<SS_Point> controlPoints = new List<SS_Point>();
                    for (float angle = 0; angle < 360f; angle += angleStep)
                    {
                        int px = (int)(podPositions[i].x + (Mathf.Cos(angle * Mathf.Deg2Rad) * (podSize * 0.5)));
                        int py = (int)(podPositions[i].y + (Mathf.Sin(angle * Mathf.Deg2Rad) * (podSize * 0.5)));

                        controlPoints.Add(new SS_Point(px, py));
                    }

                    SS_Texture tmpPodTexture = new SS_Texture(Size, Size, Color.clear);
                    SS_Drawing.PolygonFill(tmpPodTexture, controlPoints.ToArray(), SS_StellarSprite.OutlineColor, SS_StellarSprite.FillColor);
                    SS_Drawing.MergeColors(Sprite, tmpPodTexture, 0, 0);

                    List<SS_Point> controlPoints2 = new List<SS_Point>();
                    for (float angle = 0; angle < 360f; angle += angleStep)
                    {
                        int px = (int)(podPositions[i].x + (Mathf.Cos(angle * Mathf.Deg2Rad) * (podSize * 0.4)));
                        int py = (int)(podPositions[i].y + (Mathf.Sin(angle * Mathf.Deg2Rad) * (podSize * 0.4)));

                        controlPoints2.Add(new SS_Point(px, py));
                        LightPoints.Add(new SS_Point(px, py));
                    }

                    SS_Texture tmpPodTexture2 = new SS_Texture(Size, Size, Color.clear);
                    SS_Drawing.PolygonFill(tmpPodTexture2, controlPoints2.ToArray(), SS_StellarSprite.OutlineColor, SS_StellarSprite.FillColor);
                    SS_Drawing.MergeColors(Sprite, tmpPodTexture2, 0, 0);
                }

                int hubSize = random.RangeEven(64, 128);
                int numHubPoints = random.RangeEven(6, 10);

                float hubAngleSteps = 360.0f / numHubPoints;

                List<SS_Point> hubPoints = new List<SS_Point>();
                for (float angle = 0; angle < 360f; angle += hubAngleSteps)
                {
                    int px = (int)(Sprite.Center.x + (Mathf.Cos(angle * Mathf.Deg2Rad) * (hubSize * 0.5)));
                    int py = (int)(Sprite.Center.y + (Mathf.Sin(angle * Mathf.Deg2Rad) * (hubSize * 0.5)));

                    hubPoints.Add(new SS_Point(px, py));
                }

                SS_Texture tmpHub = new SS_Texture(Size, Size, Color.clear);
                SS_Drawing.PolygonFill(tmpHub, hubPoints.ToArray(), SS_StellarSprite.OutlineColor, SS_StellarSprite.FillColor);
                SS_Drawing.MergeColors(Sprite, tmpHub, 0, 0);

                List<SS_Point> hubPoints2 = new List<SS_Point>();
                for (float angle = 0; angle < 360f; angle += hubAngleSteps)
                {
                    int px = (int)(Sprite.Center.x + (Mathf.Cos(angle * Mathf.Deg2Rad) * (hubSize * 0.4)));
                    int py = (int)(Sprite.Center.y + (Mathf.Sin(angle * Mathf.Deg2Rad) * (hubSize * 0.4)));

                    hubPoints2.Add(new SS_Point(px, py));
                }

                SS_Texture tmpHub2 = new SS_Texture(Size, Size, Color.clear);
                SS_Drawing.PolygonFill(tmpHub2, hubPoints2.ToArray(), SS_StellarSprite.OutlineColor, SS_StellarSprite.FillColor);
                SS_Drawing.MergeColors(Sprite, tmpHub2, 0, 0);

                SS_Drawing.Outline(Sprite, Color.black);
                Texturize(Sprite, Color.magenta, tint, false, true);
                SS_StellarSprite.ShadeEdge(Sprite);

                SS_StellarSprite.Mirror(Sprite, SS_Mirror.TopRight);

                foreach (SS_Point p in podPositions)
                {
                    CreateFlare(Sprite, p, 32, false, Color.white);
                }

                foreach (SS_Point p in LightPoints)
                {
                    CreateFlare(Sprite, p, 16, true, Color.white);
                }

                CreateFlare(Sprite, Sprite.Center, hubSize, false, Color.white);
            }
        }

        void CreateRing(float scale, int sectionsPerQuarter, bool details, Color detailColor, bool lights)
        {
            //  Only draws top right quarter, the rest is mirrored
            List<SS_Point> DetailPoints = new List<SS_Point>();

            SS_Texture tmpTexture = new SS_Texture(Size, Size, Color.clear);

            int innerRadius = (int)((Size / 8) * scale);
            int outerRadius = (int)(((Size / 2) - 4) * scale);

            List<SS_Point> innerPoints = new List<SS_Point>();
            List<SS_Point> outerPoints = new List<SS_Point>();

            int step = 90 / sectionsPerQuarter;
            for (int a = 0; a <= 90; a += step)
            {
                int innerX = Sprite.Center.x + (int)(Mathf.Cos(a * Mathf.Deg2Rad) * innerRadius);
                int innerY = Sprite.Center.y + (int)(Mathf.Sin(a * Mathf.Deg2Rad) * innerRadius);

                innerPoints.Add(new SS_Point(innerX, innerY));

                int outerX = Sprite.Center.x + (int)(Mathf.Cos(a * Mathf.Deg2Rad) * outerRadius);
                int outerY = Sprite.Center.y + (int)(Mathf.Sin(a * Mathf.Deg2Rad) * outerRadius);

                outerPoints.Add(new SS_Point(outerX, outerY));

                if (lights)
                {
                    LightPoints.Add(new SS_Point(outerX, outerY));
                    LightPoints.Add(new SS_Point(Size - outerX, outerY));
                    LightPoints.Add(new SS_Point(Size - outerX, Size - outerY));
                    LightPoints.Add(new SS_Point(outerX, Size - outerY));
                }
            }

            // Determine centroids (detail points) for each ring section
            for (int i = 0; i < innerPoints.Count - 1; i++)
            {
                SS_Point[] points = new SS_Point[4];

                int j = i;
                int j2 = i + 1;
                if (i == innerPoints.Count - 1)
                    j2 = 0;

                points[0] = innerPoints[j];
                points[1] = outerPoints[j];
                points[2] = outerPoints[j2];
                points[3] = innerPoints[j2];

                SS_Point centroid = SS_Utilities.Centroid(points);

                if (random.NextBool())
                    DetailPoints.Add(centroid);
            }

            List<SS_Point> ringPoints = new List<SS_Point>();
            for (int i = 0; i < innerPoints.Count; i++)
            {
                ringPoints.Add(innerPoints[i]);
            }

            for (int i = outerPoints.Count - 1; i >= 0; i--)
            {
                ringPoints.Add(outerPoints[i]);
            }

            SS_Drawing.PolygonFill(tmpTexture, ringPoints.ToArray(), SS_StellarSprite.OutlineColor, SS_StellarSprite.FillColor);

            // Add borders between sectons
            float colorVal = 0;// random.Range(0.25f, 1f);
            Color sectionBorder = new Color(colorVal, colorVal, colorVal);
            for (int i = 0; i < innerPoints.Count; i++)
            {
                SS_Drawing.Line(tmpTexture, innerPoints[i].x, innerPoints[i].y, outerPoints[i].x, outerPoints[i].y, sectionBorder);
            }

            if (details)
            {
                for (int i = 0; i < DetailPoints.Count; i++)
                {
                    SS_Drawing.EllipseFill(tmpTexture, DetailPoints[i].x, DetailPoints[i].y, 8, 8, 12, SS_StellarSprite.OutlineColor, detailColor);
                }
            }

            // Texture the section
            Texturize(tmpTexture, SS_StellarSprite.FillColor, Color.grey, false, true);

            SS_StellarSprite.ShadeEdge(tmpTexture);
            SS_StellarSprite.Mirror(tmpTexture, SS_Mirror.TopRight);

            SS_Drawing.MergeColors(Sprite, tmpTexture, 0, 0);
        }

        void CreateFlare(SS_Texture texture, SS_Point lightPoint, int radius, bool customTint, Color tint)
        {
            Color flareColor = tint;
            if (!customTint)
                flareColor = new Color(random.Range(0f, 1f), random.Range(0f, 1f), random.Range(0f, 1f));

            int hRad = radius / 2;

            int xMin = lightPoint.x - hRad;
            int yMin = lightPoint.y - hRad;
            int xMax = lightPoint.x + hRad;
            int yMax = lightPoint.y + hRad;

            for (int y = yMin; y < yMax; y++)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    float distance = SS_Point.Distance(lightPoint, new SS_Point(x, y));

                    if (distance < hRad)
                    {
                        Color c = SS_Utilities.Blend(flareColor, texture.GetPixel(x, y), 1.0f - ((distance - 0) / (hRad - 0)));
                        if (texture.GetPixel(x, y).a == 0f)
                        {
                            c.a = 1f;
                        }
                        texture.SetPixel(x, y, c);
                    }
                }
            }
        }

        void CreatePods(int count, int pW, int pH, int resolution, int distanceFromCenter, double scale, Color baseColor)
        {
            SS_Texture tmpTexture = new SS_Texture(Size, Size, Color.clear);

            int step = 360 / count;

            // Setup pod positions
            List<SS_Point> pods = new List<SS_Point>();
            for (int a = 0; a < 359; a += step)
            {
                int x = Sprite.Center.x + (int)(Mathf.Cos(a * Mathf.Deg2Rad) * distanceFromCenter);
                int y = Sprite.Center.y + (int)(Mathf.Sin(a * Mathf.Deg2Rad) * distanceFromCenter);

                pods.Add(new SS_Point(x, y));
            }

            // Draw pods
            for (int i = 0; i < pods.Count; i++)
            {
                SS_Point[] positions = new SS_Point[resolution + 1];

                // angle from
                float angleToStation = Mathf.Atan2(pods[i].y - Sprite.Center.x, pods[i].x - Sprite.Center.x) * Mathf.Rad2Deg;
                angleToStation += 90;

                for (int j = 0; j <= resolution; j++)
                {
                    // Angle from pod Sprite.Center
                    float angleToPod = (float)j / (float)resolution * 2.0f * Mathf.PI;

                    // Set the original pod point
                    float x = pods[i].x + ((short)((pW * scale) * Mathf.Cos(angleToPod)));
                    float y = pods[i].y + ((short)((pH * scale) * Mathf.Sin(angleToPod)));

                    // rotate the point based on it's angle from the Sprite.Center of the pod
                    float xRotated = pods[i].x + (x - pods[i].x) * Mathf.Cos(angleToStation * Mathf.Deg2Rad) - (y - pods[i].y) * Mathf.Sin(angleToStation * Mathf.Deg2Rad);
                    float yRotated = pods[i].y + (x - pods[i].x) * Mathf.Sin(angleToStation * Mathf.Deg2Rad) + (y - pods[i].y) * Mathf.Cos(angleToStation * Mathf.Deg2Rad);

                    positions[j] = new SS_Point((int)xRotated, (int)yRotated);
                }

                SS_Drawing.PolygonFill(tmpTexture, positions, SS_StellarSprite.OutlineColor, SS_StellarSprite.FillColor);
            }

            Texturize(tmpTexture, SS_StellarSprite.FillColor, baseColor, false, true);
            SS_StellarSprite.ShadeEdge(tmpTexture);
            SS_StellarSprite.Mirror(tmpTexture, SS_Mirror.TopRight);
            SS_Drawing.MergeColors(Sprite, tmpTexture, 0, 0);
        }
       
        void Texturize(SS_Texture texture, Color targetColor, Color tint, bool highlights, bool shading)
        {
            Perlin perlin = new Perlin(0.025, 2, 0.5, 8, Seed, QualityMode.Low);

            SS_Texture BaseTexture = SS_StellarSprite.GenerateBaseTexture(Seed, texture.Width, texture.Height, 8 * (random.Range(1, 2)));

            for (int y = texture.Height / 2; y < texture.Height; y++)
            {
                for (int x = texture.Width / 2; x < texture.Width; x++)
                {
                    if (texture.GetPixel(x, y) == targetColor)
                    {
                        Color hullShade = BaseTexture.GetPixel(x, y);

                        // Pixel shade
                        float pixelNoise = (float)perlin.GetValue(x, y, 0);
                        pixelNoise = (pixelNoise + 3.0f) * 0.25f; // 0.5 to 1.0
                        pixelNoise = Mathf.Clamp(pixelNoise, 0.5f, 1f);

                        hullShade *= tint * pixelNoise;

                        if (shading)
                        {
                            SS_StellarSprite.PixelLighting(texture, x, y, ref hullShade);
                        }

                        hullShade.a = 1.0f;
                        texture.SetPixel(x, y, hullShade);
                    }
                }
                
            }
        }
    }
}