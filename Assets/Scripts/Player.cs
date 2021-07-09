 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
    
{
    // configuration parametres

    [Header("Player")]
    [SerializeField] float  movementSpeed = 10f;
    [SerializeField] float padding = 1f; // boundary mei add aur subract karenge taaki player thoda sa bhi bahaar na jaaye play area ke
    [SerializeField] int health = 200;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0, 1)] float deathSoundVolume = 0.7f;
    [SerializeField] AudioClip shootSound;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.25f;

    [Header("Projectiles")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = 0.1f;

    Coroutine firingCoroutine;

    float xMin;
    float xMax;
    float yMin;
    float yMax;
    // Start is called before the first frame update
    void Start()
    {
        setUpMovementBoundaries();  
    }

   

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    // 2 methods below are exactly the same for the enemy and player

    private void OnTriggerEnter2D(Collider2D other)
    {

        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; } // this will protect against null references and exceptions by exiting out of the method if damage dealer does not exist
        ProcessHit(damageDealer);

    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.hit();

        if (health <= 0)
        {
            Die();
            

        }
    }

    private void Die()
    {
        FindObjectOfType<Level>().LoadGameOver();
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSoundVolume);

    }

    public int GetHealth()
    {
        return health;

    }
    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());

        }

        if (Input.GetButtonUp("Fire1")) {

            StopCoroutine(firingCoroutine);
        }

    }

    IEnumerator FireContinuously() {

       while(true)  {

            GameObject laser = Instantiate(laserPrefab,
                      transform.position,
                      Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);

            yield return new WaitForSeconds(0.05f); 

       }

    }
    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * movementSpeed;  // Here we multiply by delta time to make the game frame rate independent that means that the movement speed would be constant in all computers no matter their performance, what delta time represents is the time taken by each frame, on slower computers time taken per frame is more and the frame rate is less and on faster computers time taken per frame is less and frame rate is more. Multiplying the 2 stablizes the frame rate and makes the game frame rate independent
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * movementSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);

    }
    private void setUpMovementBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding; // isme sirf x chahiye to y,z kuch bhi ho farak nahi padhta
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 1, 0)).y - padding;
    }
}
