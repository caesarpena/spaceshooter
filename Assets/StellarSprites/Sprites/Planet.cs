using System.Threading;
using UnityEngine;
using Stellar_Sprites;

public class Planet : MonoBehaviour
{
    // Project specific settings
    public string sortingLayer = "Planet";

    // Sprite Options
    public int[] AvailableSizes = new int[] { 256, 512 };

    // Configurable field parameters
    public bool CustomSeed = false;
    public int Seed = 0;

    public bool CustomSize = false;
    public int Size = 256;

    public bool CustomColors = false;
    public Color[] Colors = new Color[] { Color.blue, Color.green, Color.red };

    public bool CustomPlanetType = false;
    public SS_PlanetType PlanetType = SS_PlanetType.Gas_Giant;

    public bool CustomFrequency = true;
    public float Frequency = 0.01f;
    public float FrequencyMin = 0.005f;
    public float FrequencyMax = 0.02f;

    public bool CustomLacunarity = true;
    public float Lacunarity = 2;
    public float LacunarityMin = 1f;
    public float LacunarityMax = 4f;

    public bool CustomPersistence = true;
    public float Persistence = 0.5f;
    public float PersistenceMin = 0.25f;
    public float PersistenceMax = 0.75f;

    public bool CustomOctaves = true;
    public int Octaves = 8;
    public int OctavesMin = 8;
    public int OctavesMax = 10;

    public bool CustomAtmosphere = false;
    public bool Atmosphere = false;

    public bool CustomOceans = false;
    public bool Oceans = false;
    public Color OceanColor = new Color(0.11f, 0.42f, 0.63f);

    public bool CustomClouds = false;
    public bool Clouds = false;
    public bool CloudsRandomize = false;
    public float CloudsDensity = 0.55f;
    public float CloudsTransparency = 0.25f;

    public bool CustomRing = false;
    public bool Ring = false;
    public bool RingRandomize = false;
    public float RingDetail = 0.01f;
    public float RingDetailMin = 0.001f;
    public float RingDetailMax = 0.05f;

	public bool CustomCity = false;
	public bool City = false;
    public bool CityRandomoize = false;
    public float CityDensity = 0.95f;
    public float CityDensityMin = 0.85f;
    public float CityDensityMax = 0.975f;

    public bool CustomLighting = false;
    public float LightAngle = 180f;

    // Sprite generation data
    private SS_Planet stellarSprite;
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
            texture = new Texture2D(stellarSprite.Width, stellarSprite.Height, TextureFormat.RGBA32, false);
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
        stellarSprite = new SS_Planet(Seed, Size, Colors, PlanetType, Frequency, Lacunarity, Persistence, Octaves, Oceans, Clouds, CloudsDensity, CloudsTransparency, Atmosphere, City, CityDensity, Ring, RingDetail, LightAngle);

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
        
        if (!CustomSize)
        {
            Size = AvailableSizes[Random.Range(0, AvailableSizes.Length)];
        }
        
        if (!CustomColors)
        {
            //Colors = SS_Utilities.GenerateColorWheelColors(Seed, 3);
            Colors = new Color[]
            {
                new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)),
                new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)),
                new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f))
            };
        }
        
        if (!CustomPlanetType)
        {
            System.Array values = System.Enum.GetValues(typeof(SS_PlanetType));
            PlanetType = (SS_PlanetType)values.GetValue(Random.Range(0, values.Length));
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

        if (!CustomOceans)
        {
            Oceans = Random.Range(0, 2) > 0;
        }
        if (Oceans)
        {
            Colors[0] = OceanColor;
        }

        if (!CustomClouds)
        {
            Clouds = false;

            // Only for terrestrial planets
            if (PlanetType == SS_PlanetType.Terrestrial)
            {
                Clouds = Random.Range(0, 2) > 0;
                CloudsDensity = Random.Range(0.25f, 0.75f);
                CloudsTransparency = Random.Range(0.25f, 0.75f);
            }
        }
        else
        {
            if (CloudsRandomize)
            {
                CloudsDensity = Random.Range(0.25f, 0.75f);
                CloudsTransparency = Random.Range(0.25f, 0.75f);
            }
        }
        
        if (!CustomAtmosphere)
        {
            Atmosphere = Random.Range(0, 2) > 0;

			//// If the planet always meets the following conditions, give it an atmosphere
			//if (PlanetType == SS_PlanetType.Terrestrial && Oceans)
			//{
			//	Atmosphere = true;
			//}
        }
        
		if (!CustomCity)
        {
			City = Random.Range(0, 2) > 0;

			if (City)
			{
            	CityDensity = Random.Range(CityDensityMin, CityDensityMax);
			}
        }
		if (PlanetType == SS_PlanetType.Gas_Giant)
		{
			// No city lights on gas giants
			City = false;
		}

        if (!CustomRing)
        {
            Ring = Random.Range(0, 2) > 0;

            if (Ring)
            {
                RingDetail = Random.Range(RingDetailMin, RingDetailMax);
            }
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
