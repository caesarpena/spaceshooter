using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    [SerializeField]
    private float _speed = 2.0f;

    [SerializeField]
    private int PowerId;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5.84)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            var player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.PowerUp(PowerId);
                GameObject.Destroy(gameObject);
            }          
        }   
    }
}
