using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Katana : MonoBehaviour
{
    private Rigidbody2D rb;
    private float movementInputDirection;
    public bool isFacingRight = true;
    public float speed;
    public float jumpForce;
    private Animator anim;
    private bool isRun;
    private bool isGround;
    private bool isTouchWall;
    public bool isWallSliding;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    public float wallCheckDistance;
    private bool canJump;
    public Transform wallCheck;
    public float wallSlideSpeed;
    public bool isDash;
    public float dashTime;
    public float dashSpeed;
    public float dashCoolDown;
    private float dashTimeLeft;
    private bool isDead = false;
    private float lastImageXpos;
    public float distanceBetweenImages;
    private bool tookDamage = false;
    private float lastDash = -100f;
    public bool canMove;
    public float damageTime = 1f;
    private bool canFlip;
    private int facingDirection = 1;
    private AudioSource jumpSound;
    private int maxHealth = 5;
    private int maxLife = 3;
    public int health;
    public int numOfLife;
    public int numOfHeart;
    public Image[] hearts;
    public Image[] grenades;
    public Sprite life;
    public RectTransform fader;
    public Image[] lifes;
    public Sprite emptyHeart;
    private Vector3 respawnPoint;
    public Sprite fullHeart;
    public AudioSource[] jumpSounds;
    public AudioSource DeathSound;
    public AudioSource HurtSound;
    public AudioSource FoodSound;
    public AudioSource ExtraLifeSound;
    public AudioSource ClearSound;
    public AudioSource dashSound;
    [Range(0, 1)] public float timeScale;
    // Start is called before the first frame update

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        canMove = true;
        canFlip = true;
        respawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            CheckInput();
            CheckMovementDirection();
            UpdateAnimations();
            CheckIfCanJump();
            CheckIfWallSliding();
            CheckDash();
            PlayerHealth();
            PlayerLife();
            Time.timeScale = timeScale;
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();
        CheckSurround();
    }

    void CheckInput()
    {
        movementInputDirection = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (Time.time >= (lastDash + dashCoolDown) && isGround)
            {
                Dash();
            }
        }
    }

    void Dash()
    {
        isDash = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }

    void CheckDash()
    {
        if (isDash)
        {
            if (dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;
                dashSound.Play();
                rb.velocity = new Vector2(dashSpeed * facingDirection, rb.velocity.y);
                dashTimeLeft -= Time.deltaTime;
                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }
            if (dashTimeLeft <= 0 || isTouchWall)
            {
                isDash = false;
                canMove = true;
                canFlip = true;
            }
        }
    }

    void ApplyMovement()
    {
        if (canMove)
        {
            rb.velocity = new Vector2(movementInputDirection * speed, rb.velocity.y);
        }
        if (isWallSliding)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }
        if (rb.velocity.x != 0)
        {
            isRun = true;
        }
        else
        {
            isRun = false;
        }
    }

    void CheckIfCanJump()
    {
        if ((isGround || isWallSliding) && rb.velocity.y <= 0f)
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }
    }

    void CheckIfWallSliding()
    {
        if (isTouchWall && !isGround && rb.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }
    void Flip()
    {
        if (!isWallSliding && canFlip && !GetComponent<KatanaKombat>().isAttack)
        {
            isFacingRight = !isFacingRight;
            facingDirection *= -1;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    void Jump()
    {
        if (canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpSound = jumpSounds[Random.Range(0, jumpSounds.Length)];
            jumpSound.Play();
        }
    }

    void UpdateAnimations()
    {
        anim.SetBool("IsRun", isRun);
        anim.SetBool("IsGround", isGround);
        anim.SetFloat("YVelocity", rb.velocity.y);
        anim.SetBool("IsWallSlide", isWallSliding);
        anim.SetBool("IsDash", isDash);
    }
    void CheckSurround()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isTouchWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(transform.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }

    public void DiasbleFlip()
    {
        if (canJump && !isWallSliding)
        {
            canFlip = false;
        }
    }

    public void EnableFlip()
    {
        canFlip = true;
    }

    public void DisableMovement()
    {
        canMove = false;
        if (isGround)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    IEnumerator TookDamage(int damage)
    {
        tookDamage = true;
        health -= damage;
        if (health < 0)
        {
            rb.AddForce(new Vector2(0, 10f), ForceMode2D.Impulse);
            numOfLife--;
            if (numOfLife < 0)
            {
                Invoke("LoseScene", 3f);
                anim.SetTrigger("IsDead");
                isDead = true;
                DeathSound.Play();
            }
            else
            {
                transform.position = respawnPoint;
                health = maxHealth;
                tookDamage = false;
            }

        }
        else
        {
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet") && !tookDamage && !GetComponent<KatanaKombat>().isAttack && !isDash)
        {
            StartCoroutine(TookDamage(1));
        }

        if (other.CompareTag("Check Point"))
        {
            respawnPoint = transform.position;
            Debug.Log(respawnPoint);
        }
        if (other.CompareTag("Food"))
        {
            health = maxHealth;
            FoodSound.Play();
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
}
