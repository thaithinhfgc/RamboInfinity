using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    private Rigidbody2D myBody;
    public float throwForce = 10f;
    public float destroyTime = 5f;
    public float fieldOfImpact;
    public float explodeForce;
    public LayerMask layerToHit;
    public GameObject ExplosionEffect;
    public AudioSource ExplodeSound;
    public float radius = 5;
    private string EXPLODE_ANIMATION = "Explode";
    private Animator anim;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (destroyTime > 0)
        {
            destroyTime -= Time.deltaTime;
            ExplodeSound.Play();
        }

        else
        {
            rb.velocity = Vector3.zero;
            anim.SetTrigger(EXPLODE_ANIMATION);
            Destroy(gameObject, 1.3f);
        }
    }
}
