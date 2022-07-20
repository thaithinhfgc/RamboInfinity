using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10;
    public float destroyTime = 1.5f;
    private string IMPACT_ANIMATION = "Impact";
    private Animator anim;
    private bool shouldMove = true;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            anim.SetTrigger(IMPACT_ANIMATION);
            transform.Translate(new Vector3(0, 0, 0));
            shouldMove = false;
            Destroy(gameObject, 0.5f);
        }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            anim.SetTrigger(IMPACT_ANIMATION);
            transform.Translate(new Vector3(0, 0, 0));
            shouldMove = false;
            Destroy(gameObject, 0.5f);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
