using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImage : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer playerSpriteRenderer;
    private Color color;
    [SerializeField]
    private float activeTime = 0.1f;
    private float timeActive;
    private float alpha;
    [SerializeField]
    private float alphaSet = 0.8f;
    private float alphaMultiplier = 0.85f;
    

    void OnEnable(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        alpha = alphaSet;
        spriteRenderer.sprite = playerSpriteRenderer.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        timeActive = Time.time;
    }

    void Update(){
        alpha *= alphaMultiplier;
        color = new Color(1, 1, 1, alpha);
        spriteRenderer.color = color;
        if(Time.time >= (timeActive + activeTime)){
            PlayerAfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
