using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Stellar_Sprites;

public class Ship : MonoBehaviour
{
    // Project specific settings
    private string sortingLayer = "Foreground";

    // Sprite Options
    public int[] AvailableBodyLengths = new int[] { 32, 48, 64 };

    // Configurable field parameters
    public bool CustomSeed = false;
	public int Seed = 0;

	public bool CustomShipType = false;
	public SS_ShipType ShipType = SS_ShipType.Fighter;

	public bool CustomColorBody = true;
    public bool CustomColorHighlight = false;
    public bool CustomColorEngine = true;
    public Color[] Colors = new Color[3] { Color.grey, Color.white, Color.red };

	public bool CustomColorDetail = false;
	public float ColorDetail = 0.125f;  // 0.100 to 0.250

    public bool CustomBodyDetail = false;
    public float BodyDetail = 0.01f;  // 0.01 to 0.1

    public bool CustomBodyLength = false;
    public int BodyLength = 64;

    public bool CustomWingDetail = false;
    public float WingDetail = 0.1f;   // 0.01 to 0.1

    // Sprite generation data
    private SS_Ship stellarSprite;
    private Texture2D texture;
    private bool generationComplete = false;

    // Sprite specific
    public bool AdvancedOptions = true;
    public bool PolygonCollider = true;
    public bool EngineExhaust = true;

    private GameObject ExhaustPrefab;

    public List<Vector2> WeaponPoints;

    // Use this for initialization
    void Start()
    {
        ExhaustPrefab = Resources.Load<GameObject>("Other/EngineExhaust");

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
            //spriteRenderer.sortingLayerID = 1;
            spriteRenderer.sortingLayerName = sortingLayer;
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            if (AdvancedOptions)
            {
                // Create Weapon Points
                foreach (SS_Point p in stellarSprite.WeaponPoint)
                {
                    float x = ((-stellarSprite.Size / 2) + p.x);
                    float y = ((-stellarSprite.Size / 2) + p.y);

                    WeaponPoints.Add(new Vector2(x / 100f, y / 100f));

                    GameObject weapon = new GameObject("Weapon");
                    weapon.name = "Weapon";
                    weapon.transform.parent = transform;
                    weapon.transform.localPosition = new Vector3(x / 100f, y / 100f, 0);
                    weapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }

                if (PolygonCollider)
                {
                    var polygonCollider = GetComponent<PolygonCollider2D>();
                    if (polygonCollider == null)
                    {
                        gameObject.AddComponent<PolygonCollider2D>().isTrigger = true;
                    }
                    else
                    {
                        DestroyImmediate(polygonCollider);
                        gameObject.AddComponent<PolygonCollider2D>().isTrigger = true;
                    }
                }

                if (EngineExhaust)
                {
                    // Create gameObjects at engine points (found in the SS_Ship.cs generation)
                    foreach (SS_Point p in stellarSprite.ExhaustPoint)
                    {
                        float x = ((-stellarSprite.Size / 2) + p.x);
                        float y = ((-stellarSprite.Size / 2) + p.y);
                        // Need to offset this by 1+ if it's on the bottom half of the sprite
                        if (y < stellarSprite.Size / 2)
                            y++;

                        GameObject exhaust = Instantiate(ExhaustPrefab, Vector3.zero, Quaternion.identity);
                        exhaust.name = "EngineExhaust";
                        exhaust.transform.parent = transform;
                        exhaust.transform.localPosition = new Vector3(x / 100f, y / 100f, 0);
                        exhaust.transform.localRotation = Quaternion.Euler(0, 270, 0);
                    }
                }
            }

            // Reset generator flag
            generationComplete = false;
        }
    }

    void GenerateSprite()
    {
        // Generate the sprite object
        stellarSprite = new SS_Ship(Seed, ShipType, BodyDetail, BodyLength, WingDetail, Colors, ColorDetail);

        // Indicate that the generation is complete and the Unity Texture/Sprite can be created
        generationComplete = true;
    }

    // Generate the Stellar Sprite
    public void Generate()
    {
        // Delete any children created during generation
        var children = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "EngineExhaust" ||
                transform.GetChild(i).name == "Weapon")
                children.Add(transform.GetChild(i).gameObject);
        }
        children.ForEach(child => Destroy(child));

        // Setup parameters based on user settings
        if (!CustomSeed)
        {
			Seed = Random.Range (0, 100000000);
        }

        if (!CustomShipType)
        {
            System.Array values = System.Enum.GetValues(typeof(SS_ShipType));
            ShipType = (SS_ShipType)values.GetValue(Random.Range(0, values.Length));
        }

        if (!CustomColorBody)
        {
            Colors[0] = SS_Utilities.GenerateColorWheelColors(Seed, 3)[0];
        }
        if (!CustomColorHighlight)
        {
            Colors[1] = SS_Utilities.GenerateColorWheelColors(Seed, 1)[0];
        }
        if (!CustomColorEngine)
        {
            Colors[2] = SS_Utilities.GenerateColorWheelColors(Seed, 1)[0];
        }

		if (!CustomColorDetail)
		{
			ColorDetail = Random.Range (0.075f, 0.15f);
		}

        if (!CustomBodyDetail)
        {
            BodyDetail = Random.Range(0.01f, 0.1f);
        }

        if (!CustomBodyLength)
        {
            BodyLength = AvailableBodyLengths[Random.Range(0, AvailableBodyLengths.Length)];
        }

        if (!CustomWingDetail)
        {
            WingDetail = Random.Range(0.01f, 0.1f);
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
