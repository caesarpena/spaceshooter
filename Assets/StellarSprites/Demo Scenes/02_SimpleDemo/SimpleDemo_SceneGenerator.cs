using UnityEngine;

public class SimpleDemo_SceneGenerator : MonoBehaviour {

    public enum Prefabs
    {
        Asteroid,
        Background, 
        Blackhole,
        Cloud,
        Moon,
        Planet,
        Ship,
        Starfield,
        Station,
        Sun
    }

    private GameObject[] prefabs;
    public GameObject prefabShipWanderer;

    public Camera SceneCamera;

    private int minPlanets = 5;
    private int maxPlanets = 5;

    private int minMoonsPerPlanet = 0;
    private int maxMoonsPerPlanet = 3;

    private int minAsteroidClusters = 5;
    private int maxAsteroidClusters = 5;

    private int minStations = 5;
    private int maxStations = 15;

    private int minShips = 5;
    private int maxShips = 5;

    // Use this for initialization
    void Start() {

        prefabs = new GameObject[10];
        prefabs[0] = Resources.Load<GameObject>("Asteroid");
        prefabs[1] = Resources.Load<GameObject>("Background");
        prefabs[2] = Resources.Load<GameObject>("Blackhole");
        prefabs[3] = Resources.Load<GameObject>("Cloud");
        prefabs[4] = Resources.Load<GameObject>("Moon");
        prefabs[5] = Resources.Load<GameObject>("Planet");
        prefabs[6] = Resources.Load<GameObject>("Ship");
        prefabs[7] = Resources.Load<GameObject>("Starfield");
        prefabs[8] = Resources.Load<GameObject>("Station");
        prefabs[9] = Resources.Load<GameObject>("Sun");

        // Create Background
        GameObject bg1 = Instantiate(prefabs[(int)Prefabs.Background], new Vector3(0, 0, 0), Quaternion.identity);
        bg1.AddComponent<SimpleDemo_SimpleParallax>();
        bg1.GetComponent<SimpleDemo_SimpleParallax>().distance = 0.7f;
        bg1.transform.parent = SceneCamera.transform;

        GameObject bg2 = Instantiate(prefabs[(int)Prefabs.Background], new Vector3(0, 0, 0), Quaternion.identity);
        bg2.AddComponent<SimpleDemo_SimpleParallax>();
        bg2.GetComponent<SimpleDemo_SimpleParallax>().distance = 0.85f;
        bg2.transform.parent = SceneCamera.transform;

        // Create Starfields
        GameObject sf1 = Instantiate(prefabs[(int)Prefabs.Starfield], new Vector3(0, 0, 0), Quaternion.identity);
        sf1.AddComponent<SimpleDemo_SimpleParallax>();
        sf1.GetComponent<SimpleDemo_SimpleParallax>().distance = 0.55f;
        sf1.transform.parent = SceneCamera.transform;

        GameObject sf2 = Instantiate(prefabs[(int)Prefabs.Starfield], new Vector3(0, 0, 0), Quaternion.identity);
        sf2.AddComponent<SimpleDemo_SimpleParallax>();
        sf2.GetComponent<SimpleDemo_SimpleParallax>().distance = 0.65f;
        sf2.transform.parent = SceneCamera.transform;

        GameObject sf3 = Instantiate(prefabs[(int)Prefabs.Starfield], new Vector3(0, 0, 0), Quaternion.identity);
        sf3.AddComponent<SimpleDemo_SimpleParallax>();
        sf3.GetComponent<SimpleDemo_SimpleParallax>().distance = 0.75f;
        sf3.transform.parent = SceneCamera.transform;

        GameObject sf4 = Instantiate(prefabs[(int)Prefabs.Starfield], new Vector3(0, 0, 0), Quaternion.identity);
        sf4.AddComponent<SimpleDemo_SimpleParallax>();
        sf4.GetComponent<SimpleDemo_SimpleParallax>().distance = 0.85f;
        sf4.transform.parent = SceneCamera.transform;

        float angle, dist, x, y;
        Vector3 position;

        // Add a star
        GameObject sun = (GameObject)Instantiate(prefabs[(int)Prefabs.Sun], Vector2.zero, Quaternion.identity);
        sun.transform.parent = transform.Find("Suns");
        sun.transform.position = new Vector3(sun.transform.position.x, sun.transform.position.y, sun.transform.parent.position.z);

        // Add planets
        int numPlanets = Random.Range(minPlanets, maxPlanets);
        for (int i = 0; i < numPlanets; i++)
        {
            angle = Random.Range(0f, 360f);
            dist = (i + 2) * 15;

            x = Mathf.Cos(angle) * dist;
            y = Mathf.Sin(angle) * dist;

            position = new Vector3(x, y);

            GameObject planet = (GameObject)Instantiate(prefabs[(int)Prefabs.Planet], position, Quaternion.identity);
            planet.transform.parent = transform.Find("Planets");
            planet.transform.position = new Vector3(planet.transform.position.x, planet.transform.position.y, planet.transform.parent.position.z);

            // Add moon(s)
            int numMoons = Random.Range(minMoonsPerPlanet, maxMoonsPerPlanet);
            for (int j = 0; j < numMoons; j++)
            {
                angle = Random.Range(0f, 360f);
                dist = Random.Range(1.5f, 3.0f);

                x = Mathf.Cos(angle) * dist;
                y = Mathf.Sin(angle) * dist;

                position = new Vector3(planet.transform.position.x + x, planet.transform.position.y, 0);

                GameObject moon = (GameObject)Instantiate(prefabs[(int)Prefabs.Moon], position, Quaternion.identity);
                moon.transform.parent = transform.Find("Moons");
                moon.transform.position = new Vector3(moon.transform.position.x, moon.transform.position.y, moon.transform.parent.position.z);
            }

            // Add ship(s)
            int numShips = Random.Range(minShips, maxShips);
            for (int j = 0; j < numShips; j++)
            {
                angle = Random.Range(0f, 360f);
                dist = (j + 1) * 1;

                x = Mathf.Cos(angle) * dist;
                y = Mathf.Sin(angle) * dist;

                position = new Vector3(planet.transform.position.x + x, planet.transform.position.y, 0);

                GameObject ship = (GameObject)Instantiate(prefabShipWanderer, position, Quaternion.identity);
                ship.transform.parent = transform.Find("Ships");
                ship.transform.position = new Vector3(ship.transform.position.x, ship.transform.position.y, ship.transform.parent.position.z);
            }
        }

        // Add stations
        int numStations = Random.Range(minStations, maxStations);
        for (int i = 0; i < numStations; i++)
        {
            angle = Random.Range(0f, 360f);
            dist = (i + 2) * 12;

            x = Mathf.Cos(angle) * dist;
            y = Mathf.Sin(angle) * dist;

            position = new Vector3(x, y);

            GameObject station = (GameObject)Instantiate(prefabs[(int)Prefabs.Station], position, Quaternion.identity);
            station.transform.parent = transform.Find("Planets");
            station.transform.position = new Vector3(station.transform.position.x, station.transform.position.y, station.transform.parent.position.z);
        }

        // Add asteroids
        int numAsteroidClusters = Random.Range(minAsteroidClusters, maxAsteroidClusters);
        for (int i = 0; i < numAsteroidClusters; i++)
        {
            angle = Random.Range(0f, 360f);
            dist = (i + 2) * 6;

            x = Mathf.Cos(angle) * dist;
            y = Mathf.Sin(angle) * dist;

            position = new Vector3(x, y);

            GameObject asteroidCluster = (GameObject)Instantiate(prefabs[(int)Prefabs.Asteroid], position, Quaternion.identity);
            asteroidCluster.transform.parent = transform.Find("Asteroids");
            asteroidCluster.transform.position = new Vector3(asteroidCluster.transform.position.x, asteroidCluster.transform.position.y, asteroidCluster.transform.parent.position.z);
        }


    }

    // Update is called once per frame
    void Update () {

 
	}
}
