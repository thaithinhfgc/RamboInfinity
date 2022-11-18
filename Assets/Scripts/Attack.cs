using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    // Start is called before the first frame update
    private KatanaKombat katanaKombat;
    private Vector3 forceDirection;

    private AudioSource reflectSound;

    public AudioSource[] reflectSounds;
    void Start()
    {
        katanaKombat = GetComponentInParent<KatanaKombat>();
    }

    // Update is called once per frame
    void Update()
    {
        var point = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(1);
        point.z = transform.position.z;
        forceDirection = (point - transform.position);

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (katanaKombat.isAttack)
        {
            if (col.gameObject.CompareTag("Bullet"))
            {
                col.gameObject.GetComponent<Bullet>().isPlayerBullet = true;
                float rotationZ = Mathf.Atan2(forceDirection.y, forceDirection.x) * Mathf.Rad2Deg;
                if (FindObjectOfType<Katana>().isFacingRight)
                {
                    rotationZ += 180f;
                }
                CameraShake.Instance.ShakeCamera(1f, 0.2f);
                col.transform.Rotate(0, 0, rotationZ);

                reflectSound = reflectSounds[Random.Range(0, reflectSounds.Length)];
                reflectSound.Play();
            }
        }
    }
}
