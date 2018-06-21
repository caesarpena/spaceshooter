using UnityEngine;

using LibNoise;
using LibNoise.Generator;

namespace Stellar_Sprites
{
    public enum SS_PlanetType
    {
        Gas_Giant,      // Like Jupiter/Neptune or Uranus :P
        Terrestrial,    // Like Earth
        Barren
    }

    public class SS_Planet
    {
        public int Seed { get; set; }
        public int Width { get; set; }  // x2 Height if planet has ring
        public int Height { get; set; } // Actual Size of planet

        public SS_Texture Sprite;

        private Color[] gradientColors;
        private SS_Random random;

        public SS_Planet(int seed, int size, Color[] colors, SS_PlanetType planetType, double frequency, double lacunarity, double persistence, int octaves, bool oceans, bool clouds, float cloudDensity, float cloudTransparency, bool atmosphere, bool city, float cityDensity, bool ring, float ringDetail, float lightAngle)
        {
            Seed = seed;

            if (ring)
                Width = size * 2;
            else
                Width = size;
            Height = size;

            // randomize based on seed
            random = new SS_Random(seed);

            // Create the sprite texture
            Sprite = new SS_Texture(Width, Height, Color.clear);

            // Create a gradient using the supplid colors and between 16 and 32 steps
            gradientColors = SS_Utilities.CreateGradient(colors, 16, 32);

            // Generate Perlin Noise
            //Perlin noise = new Perlin(frequency, 2, 0.5, 8, seed, QualityMode.High);
            Perlin noise = new Perlin(frequency, lacunarity, persistence, octaves, seed, QualityMode.High);
            Perlin cloudNoise = new Perlin(0.02, 2, 0.5, 12, seed + 1, QualityMode.Low);

			// Radius of planet - only use 90% to make room for the atmosphere
            float radius = Height * 0.9f;

			// Oceans levels - determines how much water/terrain is visible
			float settleLevel = 0.4f;	//random.Range(0.25f, 0.75f);

			// Thickeness of atmosphere - between 8-16 pixels
            int atmosphereThickness = random.Range((int)(Height * 0.01f), (int)(Height * 0.05f));
            atmosphereThickness = Mathf.Clamp(atmosphereThickness, 8, 16);

			// Calculate light position based on supplied lightAngle (degrees)
            Vector2 lightPosition = new Vector2(
                Sprite.Center.x + (Mathf.Cos(lightAngle * Mathf.Deg2Rad) * (radius * 0.8f)),
                Sprite.Center.y + (Mathf.Sin(lightAngle * Mathf.Deg2Rad) * (radius * 0.8f)));

            if (lightPosition.y < 0) lightPosition.y = 0;
            if (lightPosition.y > Height - 1) lightPosition.y = Height - 1;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    // Get distance of current point to the center of the sprite
                    float dist = Vector2.Distance(new Vector2(x, y), Sprite.Center.ToVector2);

                    // Check to see if this point is within the planets radius
                    if (dist <= (radius / 2))
                    {
                        // Get noise value for current point and clamp it between 0 and 1
                        float planetNoise = (float)noise.GetValue(x, y, 0);
                        planetNoise = (planetNoise + 1.0f) * 0.5f;
                        planetNoise = Mathf.Clamp(planetNoise, 0f, 1f);

                        // Gas Giant
                        if (planetType == SS_PlanetType.Gas_Giant)
                        {
                            // Get distance of the current point to the top-center of the sprite
                            float distNorthPole = Vector2.Distance(new Vector2(x, Height - 1), Sprite.Center.ToVector2);

                            // Generate gassy noise
                            float n = (float)noise.GetValue(dist / 10 + (planetNoise * 10f), y - (distNorthPole / 5) + (planetNoise * 10f), 0);
                            n = (n + 1.0f) * 0.5f;
                            n = Mathf.Clamp(n, 0f, 1f);
                            n *= (gradientColors.Length - 1);
                            Sprite.SetPixel(x, y, gradientColors[(int)n]);
                        }
                        // Terrestrial
                        else if (planetType == SS_PlanetType.Terrestrial)
                        {
                            Color pixelColor = new Color();

                            if (oceans)
                            {
                                if (planetNoise > settleLevel)
                                {
                                    float n = planetNoise * (gradientColors.Length - 1);

                                    // Generate color and noise so land doesn't look to smooth
                                    pixelColor = gradientColors[(int)n];
                                    pixelColor *= planetNoise;
                                    pixelColor.a = 1.0f;
                                }
                                else
                                {
                                    float n = planetNoise * ((gradientColors.Length - 1) / colors.Length);

                                    // solid ocean color
                                    pixelColor = gradientColors[(int)n];
                                }
                            }
                            else
                            {
                                float n = planetNoise * (gradientColors.Length - 1);

                                // Generate color and noise so land doesn't look to smooth
                                pixelColor = gradientColors[(int)n];
                            }

                            pixelColor.a = 1.0f;
                            Sprite.SetPixel(x, y, pixelColor);

                            if (clouds)
                            {
                                float cloud = (float)cloudNoise.GetValue(x, y, 0);
                                cloud = (cloud + 1.0f) * 0.5f;
                                cloud = Mathf.Clamp(cloud, 0f, 1f);

                                if (cloud >= cloudDensity)
                                {
                                    Color cloudColor = Color.white;
                                    Color planetColor = Sprite.GetPixel(x, y);

                                    float alpha = cloudTransparency * cloud;
                                    Color newColor = new Color();
                                    newColor.r = alpha * cloudColor.r + (1 - alpha) * planetColor.r;
                                    newColor.g = alpha * cloudColor.g + (1 - alpha) * planetColor.g;
                                    newColor.b = alpha * cloudColor.b + (1 - alpha) * planetColor.b;
                                    newColor.a = 1f;
                                    Sprite.SetPixel(x, y, newColor);
                                }
                            }
                        }
                        else if (planetType == SS_PlanetType.Barren)
                        {
                            // Generate gassy noise
                            float n = planetNoise;
                            n = (n + 1.0f) * 0.5f;
                            n = Mathf.Clamp(n, 0f, 1f);
                            n *= (gradientColors.Length - 1);
                            Sprite.SetPixel(x, y, gradientColors[(int)n]);
                        }
                    }

                    // Create inner atmosphere
                    if (atmosphere)
                    {
                        Color atmosphereColor = gradientColors[0];
                        atmosphereColor.a = 1f;

                        if (dist < (radius / 2) && dist > (radius / 2) - atmosphereThickness)
                        {
                            float d = Mathf.Abs(dist - (radius / 2));
                            float a = (atmosphereThickness - d) / atmosphereThickness;

                            Color newColor = SS_Utilities.Blend(atmosphereColor, Sprite.GetPixel(x, y), a);
                            Sprite.SetPixel(x, y, newColor);
                        }
                    }
                }
            }

            // Ring
            SS_Texture tmpRingRotated = new SS_Texture(Width, Height, Color.clear);
            SS_Texture tmpRing = new SS_Texture(Width, Height, Color.clear);
            if (ring)
            {
                Perlin perlinRing = new Perlin(ringDetail, 2, 0.5, 8, seed, QualityMode.High);

                // Create a gradient using the supplid colors and between 16 and 32 steps
                Color[] ringColors = SS_Utilities.GenerateColorWheelColors(seed, 6);
                ringColors[1] = Color.black;
                ringColors[4] = Color.black;
                Color[] ringGradient = SS_Utilities.CreateGradient(ringColors, 8, 16);

                float ringW = (int)(radius * 0.6);
                float ringH = (int)(radius * random.Range(0.05f, 0.2f));
                int resolution = 360;

                // Basically we are drawing a bunch of ellipses that get bigger
                for (int i = 0; i < (radius / 2) - 16; i++)
                {
                    // I'm spicy and confusing because my programmer is a douche.
                    // I'll explain
                    // get some noise and normalize it from 0-1
                    float ringNoise = (float)perlinRing.GetValue(i, 0, 0);
                    ringNoise = (ringNoise + 1.0f) * 0.5f;
                    ringNoise = Mathf.Clamp(ringNoise, 0.0f, 1f);

                    // multiple said 0.0-1 value by the number of colors available for the ring to get an int value for what color to use of the array
                    Color c = ringGradient[(int)(ringNoise * (ringGradient.Length - 1))];

                    // The darkness of the color value also sets the alpha value (darker values are more transparent)
                    c.a = (c.r + c.g + c.b) / 3f;

                    SS_Drawing.Ellipse(tmpRing, Sprite.Center.x, Sprite.Center.y, (int)ringW, (int)ringH, resolution * 4, c);

                    ringW += 1f;
                    ringH += 0.5f;
                }

                // rotate ring
                float ringAngle = random.Range(-3f, 3f);
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        SS_Point rotatedPoint = SS_Point.Rotate(new SS_Point(x, y), Sprite.Center.x, Sprite.Center.y, ringAngle);

                        tmpRingRotated.SetPixel(x, y, tmpRing.GetPixel(rotatedPoint.x, rotatedPoint.y));
                    }
                }

                //SS_Drawing.Blur(ref tmpRingRotated);

                // Copy Ring data to Planet Sprite ignoring the parts "behind" the planet.
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        // Bottom (in front of planet)
                        if (y <= (Height / 2))
                        {
                            // Make sure we have a ring pixel
                            if (tmpRingRotated.GetPixel(x, y) != Color.clear)
                            {
                                // if the pixel behind the ring pixel is clear, then just copy the data as is
                                if (Sprite.GetPixel(x, y) == Color.clear)
                                {
                                    Sprite.SetPixel(x, y, tmpRingRotated.GetPixel(x, y));
                                }
                                // if the pixel behind the ring pixel IS NOT clear, then we have to blend the two pixels
                                // using the ring's alpha for the blend factor
                                else
                                {
                                    Color newColor = SS_Utilities.Blend(tmpRingRotated.GetPixel(x, y), Sprite.GetPixel(x, y), tmpRingRotated.GetPixel(x, y).a);
                                    Sprite.SetPixel(x, y, newColor);
                                }
                            }

                        }
                        // Top (behind planet)
                        else
                        {
                            // no blending here, so just copy the pixel (ignoring pixels that already have a value)
                            if (Sprite.GetPixel(x, y) == Color.clear)
                                Sprite.SetPixel(x, y, tmpRingRotated.GetPixel(x, y));
                        }
                    }
                }
            }

            // Atmosphere and Shadows depend on ring
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    // Get distance of current point to the center of the sprite
                    float dist = Vector2.Distance(new Vector2(x, y), Sprite.Center.ToVector2);

                    // Create outer atmosphere
                    if (atmosphere)
                    {
                        Color currentPixel = Sprite.GetPixel(x, y);
                        Color atmosphereColor = gradientColors[0];
                        atmosphereColor.a = 1f;

                        if (dist < (radius / 2) + atmosphereThickness && dist > (radius / 2))
                        {
                            float d = Mathf.Abs(dist - (radius / 2));
                            atmosphereColor.a = (atmosphereThickness - d) / atmosphereThickness;

                            if (currentPixel == Color.clear)
                            {
                                Sprite.SetPixel(x, y, atmosphereColor);
                            }
                            else
                            {
                                Color newColor = SS_Utilities.Blend(atmosphereColor, Sprite.GetPixel(x, y), atmosphereColor.a);
                                Sprite.SetPixel(x, y, newColor);
                            }
                        }
                    }

                    // Shadow
                    float lightDistance = Vector2.Distance(new Vector2(x, y), lightPosition);

                    lightDistance = 1.15f - SS_Utilities.Normalize(lightDistance, 0, Height);
                    if (lightDistance < 0.025f)
                        lightDistance = 0.025f;

                    Color lightingColor = Sprite.GetPixel(x, y);
                    lightingColor.r *= lightDistance;
                    lightingColor.g *= lightDistance;
                    lightingColor.b *= lightDistance;
                    Sprite.SetPixel(x, y, lightingColor);

                    // City lights
                    if (city)
                    {
                        if (dist <= (radius / 2))
                        {
                            float pixelNoise = (float)noise.GetValue(x, y, 0);
                            pixelNoise = (pixelNoise + 1.0f) * 0.5f;
                            pixelNoise = Mathf.Clamp(pixelNoise, 0f, 1f);
                            
                            if (Sprite.GetPixel(x, y).grayscale < 0.025f)
                            {
                                // Find land edges
                                if (pixelNoise > settleLevel && pixelNoise < settleLevel + 0.05f)
                                {
                                    if (random.Range(0f, 1f) > cityDensity)
                                    {
                                        // I don't know - i just wrotes numbers beside some colors and hoped for the best.
                                        // Hurray for laziness!
                                        Color newColor = (Color.white * 0.65f + Color.yellow * 0.85f) * 0.8f;
                                        newColor.a = 1;

                                        // Blend the city light with the ring if there is one
                                        Color ringColor = tmpRingRotated.GetPixel(x, y);
                                        if (ring && ringColor != Color.clear)
                                            newColor = SS_Utilities.Blend(newColor, ringColor, ringColor.a);

                                        Sprite.SetPixel(x, y, newColor);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
