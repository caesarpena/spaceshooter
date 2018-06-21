using UnityEngine;

public class SimpleDemo_PlayerCamera : MonoBehaviour {

    private float speed = 10f;
    private bool smooth = true;

    private GameObject target;
    
	// Use this for initialization
	void Start () {

        target = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate() {

        if (target != null)
        {
            if (smooth)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(target.transform.position.x, target.transform.position.y, -10), speed * Time.deltaTime);
            }
            else
            {
                transform.position = new Vector3(target.transform.position.x, target.transform.position.y, -10);
            }
        }
    }
}
