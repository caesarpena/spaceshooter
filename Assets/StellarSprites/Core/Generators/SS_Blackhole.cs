using UnityEngine;
using Stellar_Sprites;

namespace Stellar_Sprites
{
    public class SS_Blackhole
    {
        public int Seed { get; set; }
        public int Size { get; set; }
        
        public SS_Texture Sprite;
        
        public SS_Blackhole(int seed, int size)
        {
            Seed = seed;
            Size = size;

            Sprite = new SS_Texture(Size, Size, Color.clear);
            SS_Random random = new SS_Random(Seed);
            
            float radius = Size * 0.75f;
            float atmosphereThickness = Size * 0.125f;

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    //float dist = Vector2.Distance(new Vector2(x, y), new Vector2(sprite.Center.x, sprite.Center.y));
                    int dist = SS_Point.Distance(new SS_Point(x, y), Sprite.Center);

                    if (dist <= (radius / 2))
                    {
                        Sprite.SetPixel(x, y, Color.black);
                    }

                    // Create "glow"
                    Color currentPixel = Sprite.GetPixel(x, y);

                    Color atmosphereColor = Color.black;
                    if (currentPixel == Color.clear)
                    {
                        atmosphereColor.a = 1;
                        //float distToEdge = Vector2.Distance(new Vector2(x, y), new Vector2(sprite.Center.x, sprite.Center.y));
                        int distToEdge = SS_Point.Distance(new SS_Point(x, y), Sprite.Center);
                        if (distToEdge < (radius / 2) + atmosphereThickness &&
                            distToEdge > (radius / 2))
                        {
                            float dist2 = dist - (radius / 2);
                            atmosphereColor.a = (atmosphereThickness - dist2) / atmosphereThickness; ;

                            Sprite.SetPixel(x, y, atmosphereColor);
                        }
                    }
                }
            }
            
            // Calculate the number of light points around the even horizon based on the square root of the size halfed.
            int lightSpecCount = (int)(Mathf.Sqrt(Size) / 2) * Size;

            // Create specs of light around event horizon
            for (int i = 0; i < lightSpecCount; i++)
            {
                int a = random.Range(0, 359);
                int dist = (short)random.Range(radius * 0.25f, radius * 0.65f);

                int x = Sprite.Center.x + (int)(Mathf.Cos(a * Mathf.Deg2Rad) * dist);
                int y = Sprite.Center.y + (int)(Mathf.Sin(a * Mathf.Deg2Rad) * dist);

                SS_Point p = new SS_Point(x, y);

                int distToCenter = SS_Point.Distance(p, Sprite.Center);

                float v = 1 - (distToCenter / radius);

                Color c = new Color(v, v, v);

                Sprite.SetPixel(x, y, c);
            }

            SS_Drawing.Swirl(Sprite, Sprite.Width / 2, Sprite.Height / 2, Sprite.Width / 2, 5f);

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(Sprite.Center.x, Sprite.Center.y));

                    if (dist > (radius * 0.25f))
                    {
                        Color c = Sprite.GetPixel(x, y);
                        c.a = 1 - (dist / (Size / 2));
                        Sprite.SetPixel(x, y, c);
                    }
                }
            }

            SS_Drawing.Ellipse(Sprite, Sprite.Center.x, Sprite.Center.y, (int)(radius * 0.25), (int)(radius * 0.25), 32, Color.white);  
        }
    }
}
