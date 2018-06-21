using System.Threading;
using UnityEngine;
using Stellar_Sprites;

public class Station : MonoBehaviour
{
    // Project specific settings
    public string sortingLayer = "Station";

    // Configurable field parameters
    public bool CustomSeed = false;
    public int Seed = 0;

    public bool CustomStationType = false;
    public SS_StationType StationType = SS_StationType.Cool;

    public bool CustomTint = false;
    public Color Tint = Color.blue;

    public bool CustomPods = false;
    public int NumberOfPods = 6;

    public Color OutlineColor = Color.white;

    // Sprite generation data
    private SS_Station stellarSprite;
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
            texture = new Texture2D(stellarSprite.Size, stellarSprite.Size, TextureFormat.RGBA32, false);
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
        stellarSprite = new SS_Station(Seed, StationType, Tint, NumberOfPods);

        // Indicate that the generation is complete and the Unity Texture/Sprite can be created
        generationComplete = true;
    }

    /// <summary>
    /// Added this code to a method so that I could call it from the Editor script
    /// </summary>
    public void Generate()
    {
        // Setup parameters based on user settings
        if (!CustomSeed)
        {
			Seed = Random.Range (0, 100000000);
        }

        if (!CustomStationType)
        {
            System.Array values = System.Enum.GetValues(typeof(SS_StationType));
            StationType = (SS_StationType)values.GetValue(Random.Range(0, values.Length));
        }

        if (!CustomTint)
        {
            Tint = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

        if (!CustomPods)
        {
            SS_Random random = new SS_Random(Seed);
            NumberOfPods = random.RangeEven(2, 10);
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
        // Windows only
#if UNITY_STANDALONE_WIN

        // Check if path exists, if not created it
        if (!System.IO.Directory.Exists(Application.dataPath + @"\StellarSprites\Saved Sprites\"))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + @"\StellarSprites\Saved Sprites\");
        }

        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + @"\StellarSprites\Saved Sprites\" + stellarSprite.GetType().Name + "_" + Seed.ToString() + ".png", bytes);

#endif
    }
}
