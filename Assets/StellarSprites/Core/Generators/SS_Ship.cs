using UnityEngine;
using System.Collections.Generic;

using LibNoise;
using LibNoise.Generator;

namespace Stellar_Sprites
{
    public enum SS_ShipType
    {
        Fighter = 0,
        Fighter2 = 1,
        Hauler = 2,
		Saucer = 3
    }

    public enum SS_ShipBody
    {
        Human,
        Alien
    }

    public class SS_Ship
	{
        public int Seed { get; set; }
        public int Size { get { return 64; } }

        public SS_Texture Sprite;

        // Body length options (I want the detail steps to always be divisible for even numbers)... this was easier :) 
        //private int[] bodyLengths = { 32, 48, 64 };
        private int[] bodySteps = { 4, 8, 16 };

        private int BodyLength = 0;
        
		private Color ColorBase;
        private Color ColorHighlight;
        private Color ColorEngine;

        private float BodyDetail;
        private float WingDetail;
        private float ColorDetail;

		private SS_Random random;

        public List<SS_Point> ExhaustPoint = new List<SS_Point>();
        public List<SS_Point> WeaponPoint = new List<SS_Point>();

        private bool DebugDrawing = false;

		public SS_Ship(int seed, SS_ShipType shipType, float bodyDetail, int bodyLength, float wingDetail, Color[] colors, float colorDetail)
		{
            Seed = seed;

            ColorBase = colors[0];
            ColorHighlight = colors[1];
            ColorEngine = colors[2];

            BodyDetail = bodyDetail;
            BodyLength = bodyLength;
            WingDetail = wingDetail;
			ColorDetail = colorDetail;

            Sprite = new SS_Texture(Size, Size, Color.clear);

            random = new SS_Random(Seed);

            // generate ship based on type
            if (shipType == SS_ShipType.Fighter)
            {
                CreateWing(Sprite, Color.grey, random.NextBool());
                CreateBody(Sprite, SS_ShipBody.Human, bodyLength, random.Range(0, 4), random.NextBool());
                CreateEngine(Sprite, random.Range(1, 6));
                CreateTank(Sprite, random.Range(1, 6), Color.grey, random.NextBool());
                CreateWing(Sprite, Color.grey, random.NextBool());
                CreateCockpit(Sprite, Color.cyan);
            }
            else if (shipType == SS_ShipType.Fighter2)
            {
                CreateWing(Sprite, Color.grey, random.NextBool());
                CreateWing(Sprite, Color.grey, random.NextBool());
                CreateBody(Sprite, SS_ShipBody.Human, bodyLength, random.Range(0, 4), random.NextBool());
                CreateEngine(Sprite, random.Range(1, 4));
                CreateTank(Sprite, random.Range(1, 4), Color.grey, random.NextBool());
                CreateWing(Sprite, Color.grey, random.NextBool());
                CreateWing(Sprite, Color.grey, random.NextBool());
                CreateCockpit(Sprite, Color.cyan);
            }
            else if (shipType == SS_ShipType.Hauler)
            {
                CreateBody(Sprite, SS_ShipBody.Human, bodyLength, random.Range(0, 4), random.NextBool());
                CreateEngine(Sprite, random.Range(1, 4));
                CreateTank(Sprite, 2, Color.grey, random.NextBool());
                CreateCockpit(Sprite, Color.cyan);
            }
            else if (shipType == SS_ShipType.Saucer)
            {
                BodyLength = 48;

                //CreateWing(Sprite, Color.grey, random.NextBool());
                CreateBody(Sprite, SS_ShipBody.Alien, bodyLength, random.Range(0, 4), random.NextBool());
                //CreateTank(Sprite, 1, Color.grey, random.NextBool());
                //CreateWing(Sprite, Color.grey, random.NextBool());
            }
        }

        // Get a random step (that's not the same as the body length) to calculated the amount of data points that creates
        // the edge of the ship's body
        private int GetRandomBodyStep(int bodyLength)
        {
            int bodyStep = bodyLength;
            while (bodyStep == bodyLength)
            {
                bodyStep = bodySteps[random.Range(0, bodySteps.Length)];
            }

            return bodyStep;
        }

        private void CreateBody(SS_Texture targetTexture, SS_ShipBody body, int length, int smoothCount, bool highlights)
		{
            // Temporary texture
            SS_Texture tmpTexture = new SS_Texture(Size, Size, Color.clear);

            // Determine type of body to generate
            if (body == SS_ShipBody.Human)
            {
                // Data points for body edge
                List<SS_Point> topPoints = new List<SS_Point>();
                List<SS_Point> bottomPoints = new List<SS_Point>();

                // Noise generator
                Perlin perlin = new Perlin(BodyDetail, 2, 0.5, 8, Seed, QualityMode.Medium);

                // Calculated step points
                int step = length / GetRandomBodyStep(length);

                for (int xCnt = 0; xCnt <= length; xCnt += step)
                {
                    // Get some funky noise value
                    float noise = (float)perlin.GetValue(xCnt, 0, 0);
                    noise = (noise + 3.0f) * 0.25f; // Convert to 0 to 1
                    noise = Mathf.Clamp(noise, 0.05f, 1f);

                    int x = Sprite.Center.x - (length / 2) + xCnt;
                    if (x > Size - 1)
                        x = Size - 1;
                    int y = (int)(noise * (Size / 4));

                    topPoints.Add(new SS_Point(x, Sprite.Center.y + y));
                }

                // Fix first and last points so they are less ugly than a random tip and butt.
                topPoints[0] = new SS_Point(topPoints[0].x, Sprite.Center.y + 4);
                topPoints[topPoints.Count - 1] = new SS_Point(topPoints[topPoints.Count - 1].x, Sprite.Center.y + 2);

                // Loop through all the points and smooth them out a bit
                for (int j = 0; j < smoothCount; j++)
                {
                    for (int i = 0; i < topPoints.Count - 1; i++)
                    {
                        float y = (topPoints[i].y + topPoints[i + 1].y) / 2f;
                        y = Mathf.Ceil(y);
                        topPoints[i] = new SS_Point(topPoints[i].x, (int)y);
                    }
                }

                // Duplicate top points to bottom points but inverse the Y position
                for (int i = 0; i < topPoints.Count; i++)
                {
                    SS_Point p = topPoints[i];
                    p.y = Size - p.y - 1;
                    bottomPoints.Add(p);
                }

                // Draw the body outline - use lines since they seem to be more symmetric (pixel placement) than my polygon drawing... not sure why.
                SS_Drawing.LineStrip(tmpTexture, topPoints.ToArray(), SS_StellarSprite.OutlineColor);
                SS_Drawing.LineStrip(tmpTexture, bottomPoints.ToArray(), SS_StellarSprite.OutlineColor);

                // Connect both sizes of lines
                SS_Drawing.Line(tmpTexture, topPoints[0].x, topPoints[0].y, topPoints[0].x, (Size - topPoints[0].y), SS_StellarSprite.OutlineColor);
                SS_Drawing.Line(tmpTexture, topPoints[topPoints.Count - 1].x, topPoints[topPoints.Count - 1].y, topPoints[topPoints.Count - 1].x, (Size - topPoints[topPoints.Count - 1].y), SS_StellarSprite.OutlineColor);

                // Fill with magenta
                SS_Drawing.FloodFillArea(tmpTexture, Sprite.Center, SS_StellarSprite.FillColor);

                // Inner detail (same shape as body, but slightly smaller)
                for (int i = 0; i < topPoints.Count; i++)
                {
                    topPoints[i] = SS_Point.Scale(topPoints[i], Sprite.Center, 0.5f, 0.25f);
                }

                for (int i = 0; i < bottomPoints.Count; i++)
                {
                    bottomPoints[i] = SS_Point.Scale(bottomPoints[i], Sprite.Center, 0.5f, 0.25f);
                }

                // Draw the body outline - use lines since they seem to be more symmetric (pixel placement) than my polygon drawing... not sure why.
                SS_Drawing.LineStrip(tmpTexture, topPoints.ToArray(), SS_StellarSprite.OutlineColor);
                SS_Drawing.LineStrip(tmpTexture, bottomPoints.ToArray(), SS_StellarSprite.OutlineColor);

                // Connect both sizes of lines
                SS_Drawing.Line(tmpTexture, topPoints[0].x, topPoints[0].y, topPoints[0].x, (Size - topPoints[0].y), SS_StellarSprite.OutlineColor);
                SS_Drawing.Line(tmpTexture, topPoints[topPoints.Count - 1].x, topPoints[topPoints.Count - 1].y, topPoints[topPoints.Count - 1].x, (Size - topPoints[topPoints.Count - 1].y), SS_StellarSprite.OutlineColor);

                // Texturize and shade
                if (!DebugDrawing)
                {
                    Texturize(tmpTexture, SS_StellarSprite.FillColor, ColorBase, highlights, true);
                    SS_StellarSprite.ShadeEdge(tmpTexture);
                }
                SS_Drawing.MergeColors(targetTexture, tmpTexture, 0, 0);
            }
            else if (body == SS_ShipBody.Alien)
            {
                float bodyAngleSteps = 360.0f / 32;

                List<SS_Point> bodyPoints = new List<SS_Point>();
                for (float angle = 0; angle < 360f; angle += bodyAngleSteps)
                {
                    int px = (int)((Size / 2) + (Mathf.Cos(angle * Mathf.Deg2Rad) * (BodyLength * 0.5)));
                    int py = (int)((Size / 2) + (Mathf.Sin(angle * Mathf.Deg2Rad) * (BodyLength * 0.5)));

                    bodyPoints.Add(new SS_Point(px, py));
                }

                SS_Drawing.PolygonFill(tmpTexture, bodyPoints.ToArray(), SS_StellarSprite.OutlineColor, SS_StellarSprite.FillColor);

                if (!DebugDrawing)
                {
                    Texturize(tmpTexture, SS_StellarSprite.FillColor, ColorBase, false, true);
                }

                Color ringColor = random.NextColor();
                int ringMin = random.RangeEven(12, 18);
                int ringMax = random.RangeEven(18, 24);

                for (int y = 0; y < Size; y++)
                {
                    for (int x = 0; x < Size; x++)
                    {
                        int dist = SS_Point.Distance(new SS_Point(x, y), Sprite.Center);

                        if (dist >= ringMin && dist <= ringMax)
                        {
                            tmpTexture.SetPixel(x, y, tmpTexture.GetPixel(x, y) * ringColor);
                        }
                    }
                }

                if (random.NextBool()) CreateFlare(tmpTexture, Sprite.Center, 48, true, random.NextColor());
                if (random.NextBool()) CreateFlare(tmpTexture, Sprite.Center, 24, true, Color.white);

                if (!DebugDrawing)
                {
                    SS_StellarSprite.ShadeEdge(tmpTexture);
                }
                SS_Drawing.MergeColors(targetTexture, tmpTexture, 0, 0);
            }
        }

        private void CreateWing(SS_Texture targetTexture, Color baseColor, bool highlights)
        {
            // Temporary texture
            SS_Texture tmpTexture = new SS_Texture(Size, Size, Color.clear);

            // Wing dimensions
            int wingLength = 64;// (int)(random.RangeEven(32, 64));
            int wingSize = (int)(random.Range(4, 12));
            int wingCenterX = wingSize / 2;
            int wingOffsetX = random.Range((Size / 2) - (BodyLength / 2), (Size / 2) + wingSize);
            
            // Data points for body edge
            List<SS_Point> fPoints = new List<SS_Point>();
            List<SS_Point> bPoints = new List<SS_Point>();

            // Noise generators
            RidgedMultifractal fWingNoise = new RidgedMultifractal(WingDetail, 2, 8, Seed, QualityMode.Medium);
            RidgedMultifractal bWingNoise = new RidgedMultifractal(WingDetail, 2, 8, Seed + 1, QualityMode.Medium);

            // Determine if wing has a flat edge
            int fEdgeMod = random.RangeEven(0, 8);
            int bEdgeMod = random.RangeEven(0, 8);

            // Start point of wing (this determinds if the wings are separated or joined
            int startY = 0;
            if (random.NextBool())
                startY = random.RangeEven(2, 8);

            int fEndY = Sprite.Center.y + (wingLength / 2) - fEdgeMod;
            int bEndY = Sprite.Center.y + (wingLength / 2) - bEdgeMod;

            // Calculate steps based on length of modified wing length
            int fStep = (fEndY - Sprite.Center.y) / 4;
            int bStep = (bEndY - Sprite.Center.y) / 4;

            // Front Edge
            for (int y = Sprite.Center.y + startY; y <= fEndY + 1; y += fStep)
            {
                // Get some funky noise value for the back of the wing
                float noise = (float)fWingNoise.GetValue(0, y, 0);
                noise = (noise + 1.0f) * 0.5f; // Convert to 0 to 1
                noise = Mathf.Clamp(noise, 0.05f, 1f);

                int x = (wingOffsetX + wingCenterX) + (int)(noise * wingSize);
                if (x > Size - 1) x = Size - 1;   // Clamp to bounds

                fPoints.Add(new SS_Point(x, y));
            }

            // Back Edge
            for (int y = Sprite.Center.y + startY; y <= bEndY + 1; y += bStep)
            {
                // Get some funky noise value for the front of the wing
                float noise = (float)bWingNoise.GetValue(0, y, 0);
                noise = (noise + 1.0f) * 0.5f; // Convert to 0 to 1
                noise = Mathf.Clamp(noise, 0.05f, 1f);

                int x = (wingOffsetX - wingCenterX) - (int)(noise * wingSize);
                if (x < 0) x = 0;   // Clamp to bounds

                bPoints.Add(new SS_Point(x, y));
            }

            // Smoothing
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < fPoints.Count - 1; i++)
                {
                    float x = (fPoints[i].x + fPoints[i + 1].x) / 2f;
                    fPoints[i] = new SS_Point((int)x, fPoints[i].y);
                }

                for (int i = 0; i < bPoints.Count - 1; i++)
                {
                    float x = (bPoints[i].x + bPoints[i + 1].x) / 2f;
                    bPoints[i] = new SS_Point((int)x, bPoints[i].y);
                }
            }

            // Build polygon using both sets of points (left and right side)
            List<SS_Point> points = new List<SS_Point>();
            for (int i = 0; i < fPoints.Count; i++)
            {
                points.Add(fPoints[i]);
            }
            // Add the back edge points backwards to the point list to keep the vertex ordering correct
            for (int i = bPoints.Count - 1; i >= 0; i--)
            {
                points.Add(bPoints[i]);
            }

            // Create wing weapons before drawing the actual wing so they appear underneigth
            CreateWeapon(targetTexture, new SS_Point(wingOffsetX + wingCenterX, (Size / 2) + (startY + (wingLength / 4))), Color.yellow);
            CreateWeapon(targetTexture, new SS_Point(wingOffsetX + wingCenterX, (Size / 2) - (startY + (wingLength / 4))), Color.yellow);

            // Draw polygon for the wing
            SS_Drawing.PolygonFill(tmpTexture, points.ToArray(), SS_StellarSprite.FillColor, SS_StellarSprite.FillColor);

            // Mirror Vertically for the bottom/right wing
            int cntr = 1;
            for (int y = Sprite.Center.y; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    int newY = y - cntr;
                    tmpTexture.SetPixel(x, newY, tmpTexture.GetPixel(x, y));
                }
                cntr += 2;
            }

            // Draw the wing(s) outline
            SS_Drawing.Outline(tmpTexture, SS_StellarSprite.OutlineColor);

            // Texturize and shade
            if (!DebugDrawing)
            {
                Texturize(tmpTexture, SS_StellarSprite.FillColor, baseColor, highlights, true);
                SS_StellarSprite.ShadeEdge(tmpTexture);
            }
            SS_Drawing.MergeColors(targetTexture, tmpTexture, 0, 0);
        }

        private void CreateWeapon(SS_Texture targetTexture, SS_Point offset, Color baseColor)
		{
            // Temporary texture
            SS_Texture tmpTexture = new SS_Texture(Size, Size, Color.clear);

            // Keeping the weapons very simple for now
            // Just draw 4 layers of lines to create the weapon shape (rectangle)
            for (int y = offset.y - 2; y < offset.y + 2; y++)
            {
                int xEnd = offset.x + 8;
                if (xEnd > Size - 1)
                    xEnd = Size - 1;

                SS_Drawing.Line(tmpTexture, offset.x, y, xEnd, y, baseColor);
            }

            // Create Weapon Points
            WeaponPoint.Add(new SS_Point(offset.x + 8, offset.y));

            SS_Drawing.Outline(tmpTexture, SS_StellarSprite.OutlineColor);

            //Texturize but dont shade (more vibrant)
            if (!DebugDrawing)
            {
                Texturize(tmpTexture, SS_StellarSprite.FillColor, baseColor, false, false);
            }
            SS_Drawing.MergeColors(targetTexture, tmpTexture, 0, 0);
        }

		private void CreateTank(SS_Texture targetTexture, int count, Color baseColor, bool highlights)
		{
            // Temporary texture
            SS_Texture tmpTexture = new SS_Texture(Size, Size, Color.clear);

            // Noise generator
            Perlin perlin = new Perlin(0.1, 2, 0.5, 8, Seed, QualityMode.Medium);

            int length = random.Range((int)(BodyLength * 0.4), (int)(BodyLength * 0.8));// BodyLength / 2;

            int yStep = Size / (1 + count);
            for (int i = 1; i <= count; i++)
            {
                // Data points for body edge
                List<SS_Point> topPoints = new List<SS_Point>();
                List<SS_Point> bottomPoints = new List<SS_Point>();

                // Calculated step points
                int step = 2;
                int xStart = (Size / 2) - length;
                if (xStart < 0) xStart = 0;

                for (int xCnt = 0; xCnt <= length; xCnt += step)
                {
                    // Get some funky noise value
                    float noise = (float)perlin.GetValue(xCnt, 0, 0);
                    noise = (noise + 3.0f) * 0.25f; // Convert to 0 to 1
                    noise = Mathf.Clamp(noise, 0.05f, 1f);

                    int x = xStart + xCnt;
                    int y = (int)(noise * 4);
                    int mod = 0;
                    if (count >= 4) mod = 2;

                    topPoints.Add(new SS_Point(x, i * yStep + y + mod));
                    bottomPoints.Add(new SS_Point(x, i * yStep - y + mod));
                }

                // Draw the body outline (one side only, the other will be mirrored)
                SS_Drawing.LineStrip(tmpTexture, topPoints.ToArray(), SS_StellarSprite.FillColor);
                SS_Drawing.LineStrip(tmpTexture, bottomPoints.ToArray(), SS_StellarSprite.FillColor);

                // Connect both sizes of lines
                SS_Drawing.Line(tmpTexture, topPoints[0].x, topPoints[0].y,
                    bottomPoints[0].x,
                    bottomPoints[0].y,
                SS_StellarSprite.FillColor);
                SS_Drawing.Line(tmpTexture, topPoints[topPoints.Count - 1].x, topPoints[topPoints.Count - 1].y,
                    bottomPoints[bottomPoints.Count - 1].x,
                    bottomPoints[bottomPoints.Count - 1].y,
                SS_StellarSprite.FillColor);

                SS_Point centroid = new SS_Point(
                    xStart + 2,
                    i * yStep + 1);

                // Fill with magenta
                SS_Drawing.FloodFillArea(tmpTexture, centroid, SS_StellarSprite.FillColor);
            }

            // Draw a bar connecting all the engines (no floaters)
            if (count > 1)
            {
                int top = (Size / 2) + ((count * 8) / 2) + 1;
                int bottom = (Size / 2) - ((count * 8) / 2) - 1;
                SS_Drawing.Line(tmpTexture, (Size / 2) - length + 10, bottom, (Size / 2) - length + 10, top, SS_StellarSprite.FillColor);
                SS_Drawing.Line(tmpTexture, (Size / 2) - length + 11, bottom, (Size / 2) - length + 11, top, SS_StellarSprite.FillColor);
                SS_Drawing.Line(tmpTexture, (Size / 2) - length + 12, bottom, (Size / 2) - length + 12, top, SS_StellarSprite.FillColor);
            }

            // Outline engines
            SS_Drawing.Outline(tmpTexture, SS_StellarSprite.OutlineColor);

            // Texturize and shade
            if (!DebugDrawing)
            {
                Texturize(tmpTexture, SS_StellarSprite.FillColor, baseColor, highlights, true);
                SS_StellarSprite.ShadeEdge(tmpTexture);
            }
            SS_Drawing.MergeColors(targetTexture, tmpTexture, 0, 0);
        }

		private void CreateEngine(SS_Texture targetTexture, int count)
        {
            // Temporary texture
            SS_Texture tmpTexture = new SS_Texture(Size, Size, Color.clear);

            // Noise generator
            Perlin perlin = new Perlin(0.1, 2, 0.5, 8, Seed, QualityMode.Medium);

            int yStep = Size / (1 + count);
            for (int i = 1; i <= count; i++)
            {
                // Data points for body edge
                List<SS_Point> topPoints = new List<SS_Point>();
                List<SS_Point> bottomPoints = new List<SS_Point>();

                // Calculated step points
                int step = 2;
                int xStart = (Size / 2) - (BodyLength / 2);

                for (int xCnt = 0; xCnt <= 16; xCnt += step)
                {
                    // Get some funky noise value
                    float noise = (float)perlin.GetValue(xCnt, 0, 0);
                    noise = (noise + 3.0f) * 0.25f; // Convert to 0 to 1
                    noise = Mathf.Clamp(noise, 0.05f, 1f);

                    int x = xStart + xCnt;
                    int y = (int)(noise * 4);
                    int mod = 0;
                    if (count >= 4) mod = 2;

                    topPoints.Add(new SS_Point(x, i * yStep + y + mod));
                    bottomPoints.Add(new SS_Point(x, i * yStep - y - 1 + mod));
                }

                // Draw the body outline (one side only, the other will be mirrored)
                SS_Drawing.LineStrip(tmpTexture, topPoints.ToArray(), SS_StellarSprite.FillColor);
                SS_Drawing.LineStrip(tmpTexture, bottomPoints.ToArray(), SS_StellarSprite.FillColor);

                // Connect both sizes of lines
                SS_Drawing.Line(tmpTexture, topPoints[0].x, topPoints[0].y, 
                    bottomPoints[0].x, 
                    bottomPoints[0].y,
                SS_StellarSprite.FillColor);
                SS_Drawing.Line(tmpTexture, topPoints[topPoints.Count - 1].x, topPoints[topPoints.Count - 1].y, 
                    bottomPoints[bottomPoints.Count - 1].x, 
                    bottomPoints[bottomPoints.Count - 1].y,
                SS_StellarSprite.FillColor);

                SS_Point centroid = new SS_Point(xStart + 2, i * yStep + 1);

                // Fill with magenta
                SS_Drawing.FloodFillArea(tmpTexture, centroid, SS_StellarSprite.FillColor);

                // Create Exhaust Points
                ExhaustPoint.Add(new SS_Point(topPoints[0].x, bottomPoints[0].y + ((topPoints[0].y - bottomPoints[0].y) / 2)));
            }

            // Draw a bar connecting all the engines (no floaters)
            if (count > 1)
            {
                int top = (Size / 2) + ((count * 8) / 2) + 1;
                int bottom = (Size / 2) - ((count * 8) / 2) - 1;
                SS_Drawing.Line(tmpTexture, (Size / 2) - (BodyLength / 2) + 10, bottom, (Size / 2) - (BodyLength / 2) + 10, top, SS_StellarSprite.FillColor);
                SS_Drawing.Line(tmpTexture, (Size / 2) - (BodyLength / 2) + 11, bottom, (Size / 2) - (BodyLength / 2) + 11, top, SS_StellarSprite.FillColor);
                SS_Drawing.Line(tmpTexture, (Size / 2) - (BodyLength / 2) + 12, bottom, (Size / 2) - (BodyLength / 2) + 12, top, SS_StellarSprite.FillColor);
            }

            // Outline engines
            SS_Drawing.Outline(tmpTexture, SS_StellarSprite.OutlineColor);

            // Texturize and shade
            if (!DebugDrawing)
            {
                Texturize(tmpTexture, SS_StellarSprite.FillColor, ColorEngine, false, true);
                SS_StellarSprite.ShadeEdge(tmpTexture);
            }
            SS_Drawing.MergeColors(targetTexture, tmpTexture, 0, 0);
        }

		private void CreateCockpit(SS_Texture targetTexture, Color baseColor)
        {
            // Temporary texture
            SS_Texture tmpTexture = new SS_Texture(Size, Size, Color.clear);

            // Data points for body edge
            List<SS_Point> topPoints = new List<SS_Point>();
            List<SS_Point> bottomPoints = new List<SS_Point>();

            // Noise generator
            Perlin perlin = new Perlin(0.1, 2, 0.5, 8, Seed, QualityMode.Medium);

            // Calculated step points
            int step = 2;
            int xStart = Sprite.Center.x + (BodyLength / 2) - (BodyLength / 4) - 2;

            for (int xCnt = 0; xCnt <= (BodyLength / 4); xCnt += step)
            {
                // Get some funky noise value
                float noise = (float)perlin.GetValue(xCnt, 0, 0);
                noise = (noise + 3.0f) * 0.25f; // Convert to 0 to 1
                noise = Mathf.Clamp(noise, 0.05f, 1f);
                
                int x = xStart + xCnt;
                int y = (int)(noise * 4);

                topPoints.Add(new SS_Point(x, Sprite.Center.y + y));
            }

            // Duplicate top points to bottom points but inverse the Y position
            for (int i = 0; i < topPoints.Count; i++)
            {
                SS_Point p = topPoints[i];
                p.y = Size - p.y - 1;
                bottomPoints.Add(p);
            }

            // Draw the body outline (one side only, the other will be mirrored)
            SS_Drawing.LineStrip(tmpTexture, topPoints.ToArray(), SS_StellarSprite.OutlineColor);
            SS_Drawing.LineStrip(tmpTexture, bottomPoints.ToArray(), SS_StellarSprite.OutlineColor);

            // Connect both sizes of lines
            SS_Drawing.Line(tmpTexture, topPoints[0].x, topPoints[0].y, topPoints[0].x, (Size - topPoints[0].y), SS_StellarSprite.OutlineColor);
            SS_Drawing.Line(tmpTexture, topPoints[topPoints.Count - 1].x, topPoints[topPoints.Count - 1].y, topPoints[topPoints.Count - 1].x, (Size - topPoints[topPoints.Count - 1].y), SS_StellarSprite.OutlineColor);

            // Fill with magenta
            SS_Drawing.FloodFillArea(tmpTexture, new SS_Point(xStart + 1, Sprite.Center.y), SS_StellarSprite.FillColor);

            // Texturize and shade
            if (!DebugDrawing)
            {
                Texturize(tmpTexture, SS_StellarSprite.FillColor, baseColor, false, true);
                SS_StellarSprite.ShadeEdge(tmpTexture);
            }
            SS_Drawing.MergeColors(targetTexture, tmpTexture, 0, 0);
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

        private void Texturize(SS_Texture texture, Color targetColor, Color tint, bool highlights, bool shading)
		{
            Perlin perlin = new Perlin(ColorDetail, 2, 0.5, 8, Seed, QualityMode.High);
			Voronoi hightlightVoronoi = new Voronoi(ColorDetail, 2, Seed, true);

            SS_Texture BaseTexture = SS_StellarSprite.GenerateBaseTexture(Seed, Size, Size, 16);

            for (int y = Size / 2; y < Size; y++)
			{
				for (int x = 0; x < Size; x++)
				{
					if (texture.GetPixel(x, y) == targetColor)
					{
                        Color hullShade = BaseTexture.GetPixel(x, y);

                        //Pixel shade
                        float pixelNoise = (float)perlin.GetValue(x, y, 0);
                        pixelNoise = (pixelNoise + 3.0f) * 0.25f; // 0.5 to 1.0
                        pixelNoise = Mathf.Clamp(pixelNoise, 0.5f, 1f);

                        hullShade *= tint * pixelNoise;

                        if (highlights)
                        {
                            // Pixel shade
                            float hightlightNoise = (float)hightlightVoronoi.GetValue(x, y, 0);
                            hightlightNoise = (hightlightNoise + 1.0f) * 0.5f; // 0.0 to 1.0
                            hightlightNoise = Mathf.Clamp(hightlightNoise, 0.0f, 1f);

                            if (hightlightNoise <= 0.75f)
                            {
                                hullShade = ColorBase * pixelNoise;
                            }
                            else
                            {
                                hullShade = ColorHighlight * pixelNoise;
                            }
                        }

                        // Check the current pixel and find when it hits a solid black outline.  if it does
                        // Make the current pixel a bit darker - do for all 4 dirtections
                        if (shading)
                        {
                            SS_StellarSprite.PixelLighting(texture, x, y, ref hullShade);
                        }

                        hullShade.a = 1.0f;
                        texture.SetPixel(x, y, hullShade);
					}
				}
			}

            // Mirror
            SS_StellarSprite.Mirror(texture, SS_Mirror.Vertical);
		}
    }
}