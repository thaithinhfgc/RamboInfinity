using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public static AudioClip shootSound, runSound, jumpSound, deathSound;
    static AudioSource audioSource;
    void Start()
    {
        shootSound = Resources.Load<AudioClip>("shoot");
        runSound = Resources.Load<AudioClip>("run");
        jumpSound = Resources.Load<AudioClip>("jump");
        deathSound = Resources.Load<AudioClip>("death");
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "shoot":
                audioSource.PlayOneShot(shootSound);
                break;
            case "run":
                audioSource.PlayOneShot(runSound);
                break;
            case "jump":
                audioSource.PlayOneShot(jumpSound);
                break;
            case "death":
                audioSource.PlayOneShot(deathSound);
                break;
        }
    }
}
