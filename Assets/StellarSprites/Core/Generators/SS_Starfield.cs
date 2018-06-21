using UnityEngine;

namespace Stellar_Sprites
{
    public class SS_Starfield {

        public SS_Texture Sprite;
        
        public SS_Starfield(int seed, int size, int starCount)
        {
            // Create sprite texture
            Sprite = new SS_Texture(size, size, Color.clear);
            
            // Random generator
            SS_Random random = new SS_Random(seed);

            // Create point stars
            for (int i = 0; i < starCount; i++)
            {
                int x = random.Range(0, size - 1);
                int y = random.Range(0, size - 1);

                Sprite.SetPixel(x, y, new Color(1f, 1f, 1f, random.Range(0.5f, 1.0f)));
            }
        }
    }
}
