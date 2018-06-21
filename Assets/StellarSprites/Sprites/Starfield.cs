using System.Threading;

using UnityEngine;

using Stellar_Sprites;

public class Starfield : MonoBehaviour
{
    // Project specific settings
    public string sortingLayer = "Starfield";

    // Sprite Options
    public int[] AvailableSizes = new int[] { 512, 1024 };  

    // Configurable field parameters
    public bool CustomSeed = false;
    public int Seed = 0;

    public bool CustomSize = false;
    public int Size = 512;

    public bool CustomStarCount = false;
    public int StarCount = 1000;
    public int StarCountMin = 500;
    public int StarCountMax = 2000;

    // Sprite generation data
    private SS_Starfield stellarSprite;
    private Texture2D texture;
    private bool generationComplete = false;

    // Use this for initialization
    void Start() {

        int width = 1;
        int height = 1;

        Mesh m = new Mesh();
        m.name = "StarfieldPlane";
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
            renderer.material.mainTextureScale = new Vector2(1, 1);
            renderer.enabled = true;

            // Reset generator flag
            generationComplete = false;
        }
    }

    void GenerateSprite() {

        // Generate the sprite object
        stellarSprite = new SS_Starfield(Seed, Size, StarCount);

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

        if (!CustomStarCount)
        {
            StarCount = Random.Range(StarCountMin, StarCountMax);
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