using System.Threading;

using UnityEngine;

using Stellar_Sprites;

public class Cloud : MonoBehaviour
{
    // Project specific settings
    public string sortingLayer = "Cloud";

    // Sprite Options
    public int[] AvailableSizes = new int[] { 256, 512 };  

    // Configurable field parameters
    public bool CustomSeed = false;
    public int Seed = 0;

    public bool CustomSize = false;
    public int Size = 256;

    public bool CustomFrequency = true;
    public float Frequency = 0.01f;
    public float FrequencyMin = 0.0075f;
    public float FrequencyMax = 0.0125f;

    public bool CustomLacunarity = true;
    public float Lacunarity = 1.5f;
    public float LacunarityMin = 1.25f;
    public float LacunarityMax = 1.75f;

    public bool CustomPersistence = true;
    public float Persistence = 0.75f;
    public float PersistenceMin = 0.75f;
    public float PersistenceMax = 0.80f;

    public bool CustomOctaves = true;
    public int Octaves = 8;
    public int OctavesMin = 8;
    public int OctavesMax = 10;

    public bool CustomTint = false;
    public Color Tint = Color.blue;

    public bool CustomBrightness = false;
    public float Brightness = 0.5f;
    public float BrightnessMin = 0.4f;
    public float BrightnessMax = 1f;

    // Sprite generation data
    private SS_Cloud stellarSprite;
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
            texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            texture.filterMode = SS_StellarSprite.filterMode;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.anisoLevel = 0;
            texture.SetPixels(stellarSprite.Sprite.GetPixels());
            texture.Apply();

            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            spriteRenderer.sortingLayerName = sortingLayer;
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            transform.Rotate(new Vector3(0, 0, Random.Range(0f, 359f)));
            transform.localScale = new Vector3(Random.Range(1f, 3f), Random.Range(1f, 3f), 1);

            // Reset generator flag
            generationComplete = false;
        }
    }

    void GenerateSprite() {

        // Generate the sprite object
        stellarSprite = new SS_Cloud(Seed, Size, Frequency, Lacunarity, Octaves, Tint, Brightness);

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

        if (!CustomFrequency)
        {
            Frequency = Random.Range(FrequencyMin, FrequencyMax);
        }
        if (!CustomLacunarity)
        {
            Lacunarity = Random.Range(LacunarityMin, LacunarityMax);
        }
        if (!CustomPersistence)
        {
            Persistence = Random.Range(PersistenceMin, PersistenceMax);
        }
        if (!CustomOctaves)
        {
            Octaves = Random.Range(OctavesMin, OctavesMax);
        }

        if (!CustomTint)
        {
            Tint = new Color((float)Random.Range(0f, 1f), (float)Random.Range(0f, 1f), (float)Random.Range(0f, 1f));
        }

        if (!CustomBrightness)
        {
            Brightness = Random.Range(BrightnessMin, BrightnessMax);
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