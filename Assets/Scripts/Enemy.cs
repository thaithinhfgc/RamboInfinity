using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float attackDistance;
    public GameObject coin;
    public GameObject deathAnimation;
    public Transform leftWayPoint;
    public Transform rightWayPoint;
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
    private bool isDead = false;
    private float delayDeath = 1.5f;
    public AudioSource DeathSound;
    public AudioSource YellSound;
    public AudioSource ShootSound;
    private RaycastHit2D hit;
    private Vector3 raycastDir;
    public LayerMask ignoreLayer;
    public bool isAggro;
    public bool canMove;
    private float currentCooldown;
    public float reloadCooldown;
    private bool readyToFire;
    private AudioSource hitSound;

    public AudioSource[] hitSounds;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        target = FindObjectOfType<Katana>().transform;
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        raycastDir = Vector3.right;
        canMove = true;
        readyToFire = true;
        currentCooldown = reloadCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            targetDistance = transform.position.x - target.position.x;
            // if (Mathf.Abs(targetDistance) < attackDistance && Time.time > nextFire)
            // {
            //     anim.SetTrigger(SHOOT_ANIMATION);
            //     Shoot();
            //     nextFire = Time.time + fireRate;
            // }
            EnemyMove();
            EnemyAnimation();
            EnemyAwareness();
        }
        else
        {
            rb2d.velocity = Vector2.zero;
        }
    }

    void EnemyAnimation()
    {
        anim.SetBool("Run", rb2d.velocity != Vector2.zero);
    }

    void EnemyAwareness()
    {
        hit = Physics2D.Raycast(shotSpawner.transform.position, raycastDir, attackDistance, ignoreLayer);
        if (hit.collider != null && readyToFire)
        {
            isAggro = true;
            canMove = false;
            GameObject tempBullet = Instantiate(bulletPrefab, shotSpawner.position, shotSpawner.rotation);
            ShootSound.Play();
            if (!facingRight)
            {
                tempBullet.transform.eulerAngles = new Vector3(0, 0, 180);
            }

            readyToFire = false;
            Invoke(nameof(ResetFire), reloadCooldown);
        }
        if (isAggro)
        {
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
        }
    }

    void EnemyMove()
    {
        Vector3 v = rb2d.velocity;
        if (canMove)
        {
            if (facingRight)
            {
                v.x = speed;
            }
            else
            {
                v.x = -speed;
            }
        }
        else
        {
            v.x = 0;
        }
        rb2d.velocity = v;
    }

    protected void Flip()
    {
        facingRight = !facingRight;
        raycastDir = -raycastDir;
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
        if (collision.CompareTag("Bullet") && !isDead && collision.GetComponent<Bullet>().isPlayerBullet)
        {
            isDead = true;
            anim.SetTrigger(DEATH_ANIMATION);
            DeathSound.Play();
            YellSound.Play();
            Destroy(gameObject, delayDeath);
        }
        if (collision.CompareTag("Weapon") && !isDead && collision.GetComponentInParent<KatanaKombat>().isAttack)
        {
            Debug.Log("DEAD");
            isDead = true;
            anim.SetTrigger(DEATH_ANIMATION);
            Destroy(gameObject, delayDeath);
            hitSound = hitSounds[Random.Range(0, hitSounds.Length)];
            hitSound.Play();
            DeathSound.Play();
            YellSound.Play();
        }
        if (collision.CompareTag("Collector"))
        {
            Flip();
        }
        Debug.Log(collision.gameObject.name);
    }

    IEnumerator TookDamageCoRoutine()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
        isDead = true;
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

    void ResetFire()
    {
        readyToFire = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(shotSpawner.transform.position, shotSpawner.transform.position + raycastDir * attackDistance);
    }
}
