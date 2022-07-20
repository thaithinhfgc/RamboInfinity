using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private string TOUCH_ANIMATION = "Touch";
    Animator anim;
    public AudioSource CheckPointSound;
    private bool isCheck = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            if (!isCheck)
            {
                anim.SetTrigger(TOUCH_ANIMATION);
                CheckPointSound.Play();
            }
            isCheck = true;
        }
    }
}
