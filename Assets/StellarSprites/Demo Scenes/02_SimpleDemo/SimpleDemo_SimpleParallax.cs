using UnityEngine;

public class SimpleDemo_SimpleParallax : MonoBehaviour {

    public float distance = 1f; // greater the distance, the slower the scroll

    private GameObject player;

	// Use this for initialization
	void Start () {

        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void LateUpdate () {

        if (player != null)
        {
            float vX = player.GetComponent<Rigidbody2D>().velocity.y * Time.deltaTime * (0.05f * distance);
            float vY = player.GetComponent<Rigidbody2D>().velocity.x * Time.deltaTime * (0.05f * distance);

            GetComponent<MeshRenderer>().material.mainTextureOffset += new Vector2(vX, vY);
        }
	}
}
