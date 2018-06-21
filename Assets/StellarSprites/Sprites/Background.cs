using System.Threading;

using UnityEngine;

using Stellar_Sprites;

public class Background : MonoBehaviour
{
    // Project specific settings
    public string sortingLayer = "Background";

    // Sprite Options
    public int[] AvailableSizes = new int[] { 256, 512 };  

    // Configurable field parameters
    public bool CustomSeed = false;
    public int Seed = 0;

    public bool CustomSize = false;
    public int Size = 256;

    public bool CustomNoiseGenerator = false;
    public SS_NoiseGenerator NoiseGenerator = SS_NoiseGenerator.Perlin;

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
    public float BrightnessMin = 0.45f;
    public float BrightnessMax = 0.85f;

    public int TilingCount = 1;

    // Sprite generation data
    private SS_Background stellarSprite;
    private Texture2D texture;
    private bool generationComplete = false;

    // Use this for initialization
    void Start() {

        int width = 1;
        int height = 1;

        Mesh m = new Mesh();
        m.name = "BackgroundPlane";
        m.vertices = new Vector3[] {
            new Vector3(-width, -height, 0),
            new Vector3(width, -height, 0),
            new Vector3(width, height, 0),
            new Vector3(-width, height, 0)
        };
        m.uv = new Vector2[] {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0)
        };
        m.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
        m.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = m;

        Generate();
    }

    // Update is called once per frame
    void Update() {

        // Scale quad to fit camera
        var quadHeight = Camera.main.orthographicSize;
        var quadWidth = quadHeight * Screen.width / Screen.height;
        var scale = quadHeight;
        if (quadWidth > quadHeight)
            scale = quadWidth;
        transform.localScale = new Vector3(scale, scale, 1);

        // Only create the Unity Sprite after the generation thread has completed.  Multithreading cannot happen in Unity's MainThread
        if (generationComplete)
        {
            texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            texture.filterMode = SS_StellarSprite.filterMode;
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.anisoLevel = 0;
            texture.SetPixels(stellarSprite.Sprite.GetPixels());
            texture.Apply();

            MeshRenderer renderer = GetComponent<MeshRenderer>();
            renderer.material = new Material(Shader.Find("Particles/Additive"));
            renderer.material.mainTexture = texture;
            renderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
            renderer.material.mainTextureScale = new Vector2(TilingCount, TilingCount);
            renderer.enabled = true;

            // Reset generator flag
            generationComplete = false;
        }
    }

    void GenerateSprite() {

        // Generate the sprite object
        stellarSprite = new SS_Background(Seed, Size, NoiseGenerator, Frequency, Lacunarity, Persistence, Octaves, Tint, Brightness);

        // Indicate that the generation is complete and the Unity Texture/Sprite can be created
        generationComplete = true;
    }

    public void Generate() {

        // Setup parameters based on user settings
        if (!CustomSeed)
        {
            Seed = Random.Range(0, 100000000);
        }

        if (!CustomNoiseGenerator)
        {
            System.Array values = System.Enum.GetValues(typeof(SS_NoiseGenerator));
            NoiseGenerator = (SS_NoiseGenerator)values.GetValue(Random.Range(0, values.Length));
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
            //Tint = new Color((float)Random.Range(0f, 1f), (float)Random.Range(0f, 1f), (float)Random.Range(0f, 1f));
            double r, g, b = 0;
            SS_Utilities.HSVToRGB(Random.Range(0f / 360f, 360f / 360f), Random.Range(0f, 1f), Random.Range(0f, 1f), out r, out g, out b);
            Tint = new Color((float)r, (float)g, (float)b);
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