using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float attackDistance;
    public GameObject coin;
    public GameObject deathAnimation;

    protected Animator anim;
    protected bool facingRight = true;
    protected Transform target;
    protected float targetDistance;
    protected Rigidbody2D rb2d;
    protected SpriteRenderer sprite;
    private string SHOOT_ANIMATION = "Shoot";
    private string DEATH_ANIMATION = "Death";

    public GameObject bulletPrefab;
    public Transform shotSpawner;
    public float fireRate;
    private float nextFire;
    private bool isDead = false;
    private float delayDeath = 1.5f;
    public AudioSource DeathSound;
    public AudioSource YellSound;
    public AudioSource ShootSound;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        target = FindObjectOfType<Player>().transform;
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            targetDistance = transform.position.x - target.position.x;

            if (targetDistance < 0)
            {
                if (!facingRight)
                {
                    Flip();
                }
            }
            else
            {
                if (facingRight)
                {
                    Flip();
                }
            }
            if (Mathf.Abs(targetDistance) < attackDistance && Time.time > nextFire)
            {
                anim.SetTrigger(SHOOT_ANIMATION);
                Shoot();
                nextFire = Time.time + fireRate;
            }
        }
    }

    protected void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void TookDamage(int damage)
    {
        StartCoroutine(TookDamageCoRoutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet") && !isDead)
        {
            isDead = true;
            anim.SetTrigger(DEATH_ANIMATION);
            DeathSound.Play();
            YellSound.Play();
            Destroy(gameObject, delayDeath);
        }
    }

    IEnumerator TookDamageCoRoutine()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
        isDead = true;
    }
    public void Shoot()
    {
        GameObject tempBullet = Instantiate(bulletPrefab, shotSpawner.position, shotSpawner.rotation);
        ShootSound.Play();
        if (!facingRight)
        {
            tempBullet.transform.eulerAngles = new Vector3(0, 0, 180);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet") && !isDead)
        {
            isDead = true;
            anim.SetTrigger(DEATH_ANIMATION);
            DeathSound.Play();
            YellSound.Play();
            Destroy(gameObject, delayDeath);
        }
    }
}
