using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    private float _speed = 4.0f;

    [SerializeField]
    private GameObject _Explosion;

    private UIManager uIManager;
	// Use this for initialization
	void Start () {

        uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }
	
	// Update is called once per frame
	void Update () {

        transform.Translate(Vector3.right * _speed * Time.deltaTime);

        Ship ship = transform.GetComponent<Ship>();

        foreach (ParticleSystem exhaust in ship.GetComponentsInChildren<ParticleSystem>())
        {
            exhaust.Play();
        }

        if (transform.position.y < -4.2f)
        {
            var ramdomX = Random.Range(-7f, 7f);
            transform.position = new Vector3(ramdomX, 4.2f, 0);
        } 
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Instantiate(_Explosion, transform.position, Quaternion.identity);
        GameObject.Destroy(this.gameObject);
        uIManager.UPdateScore();
    }
}
