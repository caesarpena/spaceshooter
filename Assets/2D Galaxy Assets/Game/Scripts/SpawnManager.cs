using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    [SerializeField]
    private GameObject EnemyBase;

    //[SerializeField]
    //private GameObject EnemyShip;

    [SerializeField]
    private GameObject[] powerups;

    private bool _switch = false;

    float timer = 0.0f;
    // Use this for initialization
    void Start () {
        _switch = true;

        SpawnPlayer();

        StartCoroutine(SpawnPrefabs("EnemyShip"));
        //StartCoroutine(SpawnPrefabs("Asteroid"));
        StartCoroutine(SpawnPowerUps());
        StartCoroutine(Timer());
    }

    //Number of seconds that to spawn enemies and powerups
    private void SpawnPlayer()
    {   
        GameObject prefab = Resources.Load<GameObject>("Ship");
        GameObject newGO = Instantiate(prefab, transform, true);
              
        //GameObject newGO = Instantiate(prefab, new Vector3(0,0,0), Quaternion.Euler(new Vector3(0,0,90)), transform);;
    }

    private IEnumerator Timer()
    {
        float timer = 0.0f;

        while (_switch)
        {
            //Adds 1 every second
            timer++;
            //Spawning time
            if (timer >= 50)
            {
                _switch = false;
                StopAllCoroutines();
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    //Spawn enemies
    private IEnumerator SpawnPrefabs(string prefabName)
    {
        int i=0;

        while (_switch)
        {
            //Check the number of enemies that are currently in the scene.
            i = EnemyBase.transform.childCount;
            //Spawn up to 5 enemies, the non destroy enemies will be reused after the get off the screen.
            if (i <= 7)
            {
                GameObject prefab = Resources.Load<GameObject>(prefabName);
                //GameObject newGO = Instantiate(prefab, transform, true);
                GameObject newGO = Instantiate(prefab, new Vector3((Random.Range(-7f, 7f)), 7, 0), Quaternion.Euler(new Vector3(0, 0, -90)), EnemyBase.transform);
                newGO.tag = "Enemy";
            }          
            yield return new WaitForSeconds(1.0f);
        }
    }

    //Spawn PowerUps
    private IEnumerator SpawnPowerUps()
    {
        while (true)
        {
            //takes a random number between 0 and 2 
            //and Assigned to the array to randomly instantiate a different PowerUp
            var ramdomX = Random.Range(0, 3);
            Instantiate(powerups[ramdomX], new Vector3(Random.Range(-7f, 7f), 7, 0), Quaternion.identity);
            yield return new WaitForSeconds(5.0f);
        }
    }
}
