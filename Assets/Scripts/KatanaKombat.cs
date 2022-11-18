using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaKombat : MonoBehaviour
{
    // Start is called before the first frame update

    private bool gotInput;
    public bool isAttack;
    [SerializeField]
    private float inputTimer, attackRadius, attackDamage;
    private Animator anim;
    [SerializeField]
    private Transform attackHitBoxPos;
    [SerializeField]
    private LayerMask damageLayer;
    private float lastInputTime = Mathf.NegativeInfinity;
    private AudioSource attackSound;
    public AudioSource[] attackSounds;
    private AudioSource yellSound;
    public AudioSource[] yellSounds;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckKombatInput();
        CheckAttack();
    }

    void CheckKombatInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            gotInput = true;
            lastInputTime = Time.time;
        }
    }

    void CheckAttack()
    {
        if (gotInput)
        {
            if (!isAttack)
            {
                gotInput = false;
                isAttack = true;
                anim.SetBool("IsAttack", true);
                attackSound = attackSounds[Random.Range(0, attackSounds.Length)];
                attackSound.Play();
                yellSound = yellSounds[Random.Range(0, yellSounds.Length)];
                yellSound.Play();
            }
        }
        if (Time.time >= lastInputTime + inputTimer)
        {
            gotInput = false;
        }
    }

    public void FinishAttack()
    {
        isAttack = false;
        anim.SetBool("IsAttack", false);
    }


    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackHitBoxPos.position, attackRadius);
    }
}
