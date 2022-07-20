using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveForce = 10f;
    [SerializeField]
    private float jumpForce = 11f;
    private float movementX;
    private bool facingRight = true;
    private bool lookingUp;
    private bool lookingDown;
    private bool lookingRight;
    private bool lookingLeft;
    private Rigidbody2D myBody;
    private SpriteRenderer sr;
    public Rigidbody2D bomb;
    private Animator anim;
    private string RUN_ANIMATION = "Run";
    private string SHOOT_ANIMATION = "Shoot";
    private string SHOOT_UP_ANIMATION = "Shoot Up";
    private string SHOOT_DOWN_ANIMATION = "Shoot Down";
    private string SHOOT_DOWN_FORWARD_ANIMATION = "Shoot Down Forward";
    private string SHOOT_UP_FORWARD_ANIMATION = "Shoot Up Forward";
    private string DEFEATED_ANIMATION = "Defeated";
    private string GROUND_TAG = "Ground";
    private bool isGround;
    private bool isDead = false;
    public GameObject bulletPrefab;
    public GameObject grenadePrefab;
    public Transform shotSpawner;
    private bool tookDamage = false;
    private int maxHealth = 5;
    private int maxNade = 3;
    private int maxLife = 3;
    public float damageTime = 1f;
    public AudioSource RunSound;
    public AudioSource ShootSound;
    public AudioSource DeathSound;
    public AudioSource HurtSound;
    public AudioSource JumpSound;
    public AudioSource ThrowSound;
    public AudioSource FoodSound;
    public AudioSource GrenadeSound;
    public AudioSource ExtraLifeSound;
    public AudioSource ClearSound;
    public int health;
    public int numOfHeart;
    public int nade;
    public int numOfNade;
    public Image[] hearts;
    public Image[] grenades;
    public Sprite fullHeart;
    public Sprite grenade;
    public Sprite emptyHeart;
    private Vector3 respawnPoint;
    public int numOfLife;
    public Image[] lifes;
    public Sprite life;
    public RectTransform fader;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        respawnPoint = transform.position;
    }
    void Start()
    {
        respawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            PlayerMoveKeyboard();
            Animate();
            PlayerJump();
            PlayerShoot();
            PlayerHealth();
            PlayerGrenade();
            PlayerLife();
            PlayerQuit();
        }

    }

    void PlayerMoveKeyboard()
    {
        movementX = Input.GetAxis("Horizontal");

        lookingUp = Input.GetKey(KeyCode.W);
        lookingDown = Input.GetKey(KeyCode.S);
        lookingRight = Input.GetKey(KeyCode.D);
        lookingLeft = Input.GetKey(KeyCode.A);
        transform.position += new Vector3(movementX, 0, 0) * Time.deltaTime * moveForce;
    }
    void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            myBody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            isGround = false;
            JumpSound.Play();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GROUND_TAG))
        {
            isGround = true;
        };
    }
    void Animate()
    {
        if (movementX != 0 && !RunSound.isPlaying && isGround)
        {
            RunSound.Play();
        }
        if (lookingUp && movementX == 0)
        {
            anim.SetBool(SHOOT_UP_ANIMATION, true);
        }
        else
        {
            anim.SetBool(SHOOT_UP_ANIMATION, false);
        }
        if (movementX != 0 && lookingUp)
        {
            anim.SetBool(SHOOT_UP_FORWARD_ANIMATION, true);
        }
        else
        {
            anim.SetBool(SHOOT_UP_FORWARD_ANIMATION, false);
        }
        if (movementX != 0 && lookingDown)
        {
            anim.SetBool(SHOOT_DOWN_FORWARD_ANIMATION, true);
        }
        else
        {
            anim.SetBool(SHOOT_DOWN_FORWARD_ANIMATION, false);
        }
        if (movementX != 0 && !lookingUp && !lookingDown)
        {
            anim.SetBool(RUN_ANIMATION, true);
        }
        else
        {
            anim.SetBool(RUN_ANIMATION, false);
        }
        if (!lookingLeft && !lookingRight && !isGround && lookingDown)
        {
            anim.SetBool(SHOOT_DOWN_ANIMATION, true);
        }
        else
        {
            anim.SetBool(SHOOT_DOWN_ANIMATION, false);
        }
        if (movementX == 0 && Input.GetKeyDown(KeyCode.J))
        {
            anim.SetBool(SHOOT_ANIMATION, true);
        }
        else
        {
            anim.SetBool(SHOOT_ANIMATION, false);
        }
        if (movementX < 0 && facingRight)
        {
            Flip();
        }

        //



        if (movementX > 0 && !facingRight)
        {
            Flip();
        }

        anim.SetBool(RUN_ANIMATION, movementX != 0);
        anim.SetBool(SHOOT_UP_ANIMATION, lookingUp);

    }
    void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    IEnumerator TookDamage(int damage)
    {
        tookDamage = true;
        health -= damage;
        if (health < 0)
        {
            myBody.AddForce(new Vector2(0, 10f), ForceMode2D.Impulse);
            numOfLife--;
            if (numOfLife < 0)
            {
                Invoke("LoseScene", 3f);
                anim.SetTrigger(DEFEATED_ANIMATION);
                isDead = true;
                DeathSound.Play();
            }
            else
            {
                transform.position = respawnPoint;
                health = maxHealth;
                numOfNade = maxNade;
                tookDamage = false;
            }

        }
        else
        {
            HurtSound.Play();
            Physics2D.IgnoreLayerCollision(9, 10);
            for (float i = 0; i < damageTime; i += 0.2f)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                yield return new WaitForSeconds(0.1f);
                GetComponent<SpriteRenderer>().enabled = true;
                yield return new WaitForSeconds(0.1f);
            }
            Physics2D.IgnoreLayerCollision(9, 10, false);
            tookDamage = false;
        }
    }

    void LoseScene()
    {
        fader.gameObject.SetActive(true);
        LeanTween.scale(fader, Vector3.zero, 0);
        LeanTween.scale(fader, new Vector3(1, 1, 1), 0.5f).setOnComplete(() =>
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1));

    }

    void WinScene()
    {
        fader.gameObject.SetActive(true);
        LeanTween.scale(fader, Vector3.zero, 0);
        LeanTween.scale(fader, new Vector3(1, 1, 1), 0.5f).setOnComplete(() =>
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 2)
);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet") && !tookDamage)
        {
            StartCoroutine(TookDamage(1));
        }

        if (other.CompareTag("Check Point"))
        {
            respawnPoint = transform.position;
            Debug.Log(respawnPoint);
        }

        if (other.CompareTag("Collector"))
        {
            StartCoroutine(TookDamage(6));
        }
        if (other.CompareTag("Food"))
        {
            health = maxHealth;
            FoodSound.Play();
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Grenade"))
        {
            numOfNade += 1;
            if (numOfNade > maxNade)
            {
                numOfNade = maxNade;
            }
            GrenadeSound.Play();
            Destroy(other.gameObject);
        }
        if (other.CompareTag("End"))
        {
            if (!ClearSound.isPlaying)
            {
                ClearSound.Play();
            }
            Invoke("WinScene", 7f);
        }
        if (other.CompareTag("ExtraLife"))
        {
            numOfLife++;
            if (numOfLife > maxLife)
            {
                numOfLife = maxLife;
            }
            ExtraLifeSound.Play();
            Destroy(other.gameObject);
        }

    }

    void PlayerHealth()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
            if (i < numOfHeart)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    void PlayerGrenade()
    {
        for (int i = 0; i < grenades.Length; i++)
        {
            if (i < numOfNade)
            {
                grenades[i].enabled = true;
            }
            else
            {
                grenades[i].enabled = false;
            }
        }
    }

    void PlayerLife()
    {
        for (int i = 0; i < lifes.Length; i++)
        {
            if (i < numOfLife)
            {
                lifes[i].enabled = true;
            }
            else
            {
                lifes[i].enabled = false;
            }
        }
    }

    void PlayerShoot()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ShootSound.Play();
            GameObject bullet = Instantiate(bulletPrefab, shotSpawner.position, shotSpawner.rotation);
            if (!facingRight)
            {
                if (lookingUp && movementX < 0)
                {
                    bullet.transform.eulerAngles = new Vector3(0, 0, 150);
                    bullet.transform.position += bullet.transform.rotation * new Vector3(1, -1, 0);
                }
                else if (lookingDown && movementX < 0)
                {
                    bullet.transform.eulerAngles = new Vector3(0, 0, 205);
                    bullet.transform.position += bullet.transform.rotation * new Vector3(1, 1, 0);
                }
                else if (lookingDown && !lookingLeft && !lookingRight && !isGround)
                {
                    bullet.transform.eulerAngles = new Vector3(0, 0, 270);
                    bullet.transform.position += bullet.transform.rotation * new Vector3(2, 1, 0);
                }
                else if (lookingUp && !lookingLeft && !lookingRight)
                {
                    bullet.transform.eulerAngles = new Vector3(0, 0, 90);
                    bullet.transform.position += bullet.transform.rotation * new Vector3(2, -0.7f, 0);
                }
                else
                {
                    bullet.transform.eulerAngles = new Vector3(0, 0, 180);
                }
            }

            if (facingRight)
            {
                if (lookingUp && movementX > 0)
                {
                    bullet.transform.eulerAngles = new Vector3(0, 0, 30);
                    bullet.transform.position += bullet.transform.rotation * new Vector3(1, 1, 0);
                }
                else if (lookingDown && movementX > 0)
                {
                    bullet.transform.eulerAngles = new Vector3(0, 0, 335);
                    bullet.transform.position += bullet.transform.rotation * new Vector3(1, -1, 0);
                }
                else if (lookingDown && !lookingLeft && !lookingRight && !isGround)
                {
                    bullet.transform.eulerAngles = new Vector3(0, 0, 270);
                    bullet.transform.position += bullet.transform.rotation * new Vector3(2, -1, 0);
                }
                else if (lookingUp && !lookingLeft && !lookingRight)
                {
                    bullet.transform.eulerAngles = new Vector3(0, 0, 90);
                    bullet.transform.position += bullet.transform.rotation * new Vector3(2, 0.7f, 0);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.K) && numOfNade > 0)
        {
            ThrowSound.Play();
            Rigidbody2D grenade = Instantiate(bomb, transform.position, transform.rotation);
            if (facingRight)
            {
                grenade.AddForce(new Vector2(12, 10), ForceMode2D.Impulse);
            }
            else
            {
                grenade.AddForce(new Vector2(-12, 10), ForceMode2D.Impulse);
            }

            numOfNade--;
        }
    }

    void PlayerQuit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            fader.gameObject.SetActive(true);
            LeanTween.scale(fader, Vector3.zero, 0);
            LeanTween.scale(fader, new Vector3(1, 1, 1), 0.5f).setOnComplete(() =>
                    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1));
        }
    }
}