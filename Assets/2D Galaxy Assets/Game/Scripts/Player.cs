using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {



    public bool canTripleShoot = false;
    public bool HasShield = false;

    [SerializeField]
    private int LifeCount = 3;
    private float _fireRate = 0.25f;
    private float _canFire = 0.0f;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _Explosion;

    [SerializeField]
    private GameObject _Shield;

    public float speed = 4.0f;

    private UIManager _uiManager;

	// Use this for initialization
	void Start () {

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager != null)
        {
            _uiManager.UpdateLives(LifeCount);
        }
    }
	
	// Update is called once per frame
	void Update () {

        Movement();
        Shoot();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (!HasShield)
            {
                LifeCount--;
                _uiManager.UpdateLives(LifeCount);

                if (LifeCount < 1)
                {
                    Instantiate(_Explosion, transform.position, Quaternion.identity);
                    Destroy(this.gameObject);
                }
            }
            else
            {
                ShieldOff();
            }          
        }
    }

    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.time > _canFire)
            {
                if (canTripleShoot)
                {
                    TripleShoot();
                }
                else
                {
                    var LaserPos = new Vector2(transform.position.x, transform.position.y + 0.66f);

                    Instantiate(_laserPrefab, LaserPos, Quaternion.identity);
                }

                _canFire = Time.time + _fireRate;
            }
        }
    }

    private void TripleShoot()
    {
     
        Vector3[] LaserPos = { new Vector3(0, 0.93f, 0),
                                    new Vector3(0.2f, 0, 0),
                                        new Vector3(-0.2f, 0, 0) };

        foreach(Vector3 pos in LaserPos)
        {
            Instantiate(_laserPrefab, transform.position + pos, Quaternion.identity);
        }

    }

    private void ShieldOn()
    {
        HasShield = true;
        var shield = Instantiate(_Shield, transform.position, Quaternion.identity, transform);

    }

    private void ShieldOff()
    {
        Transform shield = transform.Find("Player_Shield(Clone)");
        HasShield = false;
        GameObject.Destroy(shield.gameObject);
    }


    public void PowerUp(int id)
    {
        switch (id)
        {
            case 0:
                canTripleShoot = true;
                break;
            case 1:
                speed = 10.0f;
                break;
            case 2:
                ShieldOn();
                break;
        }

        StartCoroutine(PowerUpOff(id));
    }

    IEnumerator PowerUpOff(int id)
    {
        yield return new WaitForSeconds(5.0f);

        switch (id)
        {
            case 0:
                canTripleShoot = false;
                break;
            case 1:
                speed = 5.0f;
                break;
            case 2:
                //HasShield = false;
                break;
        }
       
    }

    private void Movement()
    {     
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

       transform.Translate(Vector3.right * speed * verticalInput * Time.deltaTime);
       transform.Translate(Vector3.down * speed * horizontalInput * Time.deltaTime);

        


        if (transform.position.y > 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }

        if (transform.position.y < -3.8)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }

        if (transform.position.x > 7.0f)
        {
            transform.position = new Vector3(7.0f, transform.position.y, 0);
        }

         if (transform.position.x < -7.0f)
        {
            transform.position = new Vector3(-7.0f, transform.position.y, 0);
        }
    }
}
