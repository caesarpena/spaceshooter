using UnityEngine;

public class SimpleDemo_ShipWander : MonoBehaviour {

    enum ShipState
    {
        Flying,
        Waiting
    };

    private float Thrust = 250f;
    private float TurnSpeed = 5f;

    private Vector2 homePosition;
    private Vector2 targetPosition;

    private float distanceToHome = 0;
    private float maxTravel = 10;

    private Ship ship;
    private ShipState state = ShipState.Waiting;
    private float timeToWait = 0f;
    private float timeWaiting = 0f;
    
	// Use this for initialization
	void Start () {

        timeToWait = 0f;

        homePosition = transform.position;
        targetPosition = homePosition;

        ship = GetComponent<Ship>();
	}
	
	// Update is called once per frame
    void Update () {

        //Debug.DrawLine(transform.position, targetPosition, Color.red);
        //Debug.DrawLine(transform.position, homePosition, Color.green);
    }

	void FixedUpdate () {

        if (state == ShipState.Waiting)
        {
            timeWaiting += Time.deltaTime;
            if (timeWaiting > timeToWait)
            {
                state = ShipState.Flying;
                timeWaiting = 0f;
            }
        }
        else if (state == ShipState.Flying)
        {
            var offset = targetPosition - new Vector2(transform.position.x, transform.position.y);
            var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), TurnSpeed * Time.deltaTime);

            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            if (rb2D != null)
            {
                rb2D.AddForce(transform.right * Thrust * Time.deltaTime);
                    
                // Engine exhaust when accelerating
                foreach (ParticleSystem exhaust in ship.GetComponentsInChildren<ParticleSystem>())
                {
                    exhaust.Play();
                }
            }

            distanceToHome = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), targetPosition);
            if (distanceToHome < 2f)
            {
                // Find a new home
                float x = homePosition.x + (Random.insideUnitCircle.x * maxTravel);
                float y = homePosition.y + (Random.insideUnitCircle.y * maxTravel);

                targetPosition = new Vector2(x, y);

                state = ShipState.Waiting;
                timeToWait = Random.Range(1f, 4f);

                // Engine exhaust when accelerating
                foreach (ParticleSystem exhaust in ship.GetComponentsInChildren<ParticleSystem>())
                {
                    exhaust.Stop();
                }
            }
        }
	}
}
