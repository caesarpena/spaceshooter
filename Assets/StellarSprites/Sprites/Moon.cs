using System.Threading;
using UnityEngine;
using Stellar_Sprites;

public class Moon : MonoBehaviour
{
    // Project specific settings
    public string sortingLayer = "Moon";

    // Sprite Options
    public int[] AvailableSizes = new int[] { 128 };

    // Configurable field parameters
    public bool CustomSeed = false;
    public int Seed = 0;

    public bool CustomSize = false;
    public int Size = 128;

    public bool CustomFrequency = false;
    public float Frequency = 0.01f;
    public float FrequencyMin = 0.01f;
    public float FrequencyMax = 0.05f;

    public bool CustomLacunarity = false;
    public float Lacunarity = 1.5f;
    public float LacunarityMin = 1f;
    public float LacunarityMax = 2f;

    public bool CustomPersistence = false;
    public float Persistence = 0.75f;
    public float PersistenceMin = 0.75f;
    public float PersistenceMax = 2f;

    public bool CustomOctaves = false;
    public int Octaves = 6;
    public int OctavesMin = 2;
    public int OctavesMax = 12;

    public bool CustomRoughness = false;
	public float Roughness = 0;

    public bool CustomColors = true;
    public Color[] Colors = new Color[] { new Color(0.40f, 0.40f, 0.40f), new Color(0.63f, 0.63f, 0.63f), new Color(0.75f, 0.75f, 0.75f) };

    public bool CustomLighting = false;
    public float LightAngle = 180f;

    // Sprite generation data
    private SS_Moon stellarSprite;
    private Texture2D texture;
    private bool generationComplete = false;

    // Use this for initialization
    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
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

    void GenerateSprite()
    {
        // Generate the sprite object
        stellarSprite = new SS_Moon(Seed, Size, Frequency, Lacunarity, Octaves, Roughness, Colors, LightAngle);

        // Indicate that the generation is complete and the Unity Texture/Sprite can be created
        generationComplete = true;
    }

    // Generate the Stellar Sprite
    public void Generate()
    {
        // Setup parameters based on user settings
        if (!CustomSeed)
        {
			Seed = Random.Range (0, 100000000);
        }
        
        if (!CustomSize)
        {
            Size = AvailableSizes[Random.Range(0, AvailableSizes.Length)];
        }

        if (!CustomFrequency)
            Frequency = Random.Range(FrequencyMin, FrequencyMax);
        if (!CustomLacunarity)
            Lacunarity = Random.Range(LacunarityMin, LacunarityMax);
        if (!CustomPersistence)
            Persistence = Random.Range(PersistenceMin, PersistenceMax);
        if (!CustomOctaves)
            Octaves = Random.Range(OctavesMin, OctavesMax);

        if (!CustomRoughness)
		{
			Roughness = Random.Range (0f, 1f);
		}

        if (!CustomColors)
        {
            Colors = SS_Utilities.GenerateColorWheelColors(Seed, 3);
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

    public void SaveToFile()
    {

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
