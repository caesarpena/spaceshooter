using UnityEngine;

using Stellar_Sprites;

public class SimpleDemo_PlayerController : MonoBehaviour {

    private float thrust = 250f;
    private float turnSpeed = 250f;

    private Ship ship;
    private SS_ShipType shipType;

    // Use this for initialization
    void Start () {

        ship = GetComponent<Ship>();
        shipType = GetComponent<Ship>().ShipType;
    }
	
	// Update is called once per frame
    void Update () {
    }

    void FixedUpdate()
    {
        if (shipType == SS_ShipType.Saucer)
        {
            float xVelocity = Input.GetAxis("Horizontal") * thrust * Time.deltaTime;
            float yVelocity = Input.GetAxis("Vertical") * thrust * Time.deltaTime;

            GetComponent<Rigidbody2D>().AddForce(transform.up * yVelocity);
            GetComponent<Rigidbody2D>().AddForce(transform.right * xVelocity);
        }
        else
        {
            if (Input.GetMouseButton(1))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Turning
                float angle = Mathf.Atan2(mousePosition.y - transform.position.y, mousePosition.x - transform.position.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), turnSpeed * Time.deltaTime);

                // Movement
                float distToCenter = Vector2.Distance(Input.mousePosition, new Vector2(Screen.width / 2, Screen.height / 2));
                float maxDist = Mathf.Sqrt((Screen.width ^ 2) + (Screen.height ^ 2));
                GetComponent<Rigidbody2D>().AddForce(transform.right * (distToCenter / maxDist));

                // Engine exhaust when accelerating
                foreach (ParticleSystem exhaust in ship.GetComponentsInChildren<ParticleSystem>())
                {
                    exhaust.Play();
                }
            }
            else
            {
                // Stop engine exhausts when not accelerating
                foreach (ParticleSystem exhaust in ship.GetComponentsInChildren<ParticleSystem>())
                {
                    exhaust.Stop();
                }
            }
        }
    }
}

