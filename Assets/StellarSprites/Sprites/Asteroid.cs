using System.Threading;

using UnityEngine;

using Stellar_Sprites;

public class Asteroid : MonoBehaviour
{
    // Project specific settings
    public string sortingLayer = "Asteroid";

    // Sprite Options
    public int[] AvailableSizes = new int[] { 64 };
    public Color[] AvailableMineralColors = new Color[] { Color.green, Color.yellow, Color.red, Color.cyan, Color.magenta };

    // Configurable field parameters
    public bool CustomSeed = false;
    public int Seed = 0;

    public bool CustomSize = false;
    public int Size = 64;

    public bool CustomColors = true;
    public Color[] Colors = new Color[] { new Color(0.40f, 0.40f, 0.40f), new Color(0.63f, 0.63f, 0.63f), new Color(0.75f, 0.75f, 0.75f) };

    public bool CustomMinerals = false;
    public bool Minerals = false;

    public bool CustomMineralColor = false;
    public bool MineralRandomize = false;
    public Color MineralColor = Color.green;

    public bool CustomLighting = false;
    public float LightAngle = 180f;

    // Sprite generation data
    private SS_Asteroid stellarSprite;
    private Texture2D texture;
    private bool generationComplete = false;

    // Use this for initialization
    void Start() {

        Generate();
    }

    // Update is called once per frame
    void Update() {

        // Only create the Unity Sprite after the generation thread has completed.  Multithreading cannot happen in Unity's MainThread
        if (generationComplete)
        {
            // Create the texture object and get pixel data from the sprite object
            texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            texture.filterMode = SS_StellarSprite.filterMode;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.anisoLevel = 0;
            texture.SetPixels(stellarSprite.Sprite.GetPixels());
            texture.Apply();

            // Create the unity sprite from the texture object
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            spriteRenderer.sortingLayerName = sortingLayer;
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // Reset generator flag
            generationComplete = false;
        }
    }

    void GenerateSprite() {

        // Generate the sprite object
        stellarSprite = new SS_Asteroid(Seed, Size, Colors, Minerals, MineralColor, LightAngle);

        // Indicate that the generation is complete and the Unity Texture/Sprite can be created
        generationComplete = true;
    }

    public void Generate() {

        // Setup parameters based on user settings
        if (!CustomSeed)
        {
            Seed = Random.Range(0, 100000000);
        }

        if (!CustomSize)
        {
            Size = AvailableSizes[Random.Range(0, AvailableSizes.Length)];
        }

        if (!CustomColors)
        {
            Colors = SS_Utilities.GenerateColorWheelColors(Seed, 3);
        }

        if (!CustomMinerals)
        {
            Minerals = Random.Range(0, 2) > 0;
        }

        if (!CustomMineralColor)
        {
            MineralColor = AvailableMineralColors[Random.Range(0, AvailableMineralColors.Length)];
        }

        if (SS_StellarSprite.Threaded)
        {
            // Create the Stellar Sprite Object in it's own thread
            Thread generatorThread = new Thread(new ThreadStart(GenerateSprite));
            generatorThread.Start();
        }
        else
        {
            GenerateSprite();
        }
    }

    public void SaveToFile() {

#if UNITY_STANDALONE_WIN

        // Check if path exists, if not created it
        if (!System.IO.Directory.Exists(Application.dataPath + @"\StellarSprites\Saved Sprites\"))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + @"\StellarSprites\Saved Sprites\");
        }

        // Save the texture data to a PNG file in the Saved Sprites directory
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + @"\StellarSprites\Saved Sprites\" + stellarSprite.GetType().Name + "_" + Seed.ToString() + ".png", bytes);

#endif
    }
}
