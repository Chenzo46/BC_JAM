using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class growingSpike : MonoBehaviour
{
    [SerializeField] private float cooldown = 5f; 
    [SerializeField] private float offset = 0f;
    [SerializeField] private Animator anim;

    private bool progressing = false;


    private float cooldownRef;

    private void Awake()
    {
        cooldownRef = cooldown + offset;
        cooldownRef = cooldown + offset;
    }

    private void Update()
    {
        if (cooldownRef <= 0 && !progressing)
        {
            anim.SetTrigger("thrust");
            cooldownRef = cooldown;
            progressing = true;
        }
        else if(!progressing)
        {
            cooldownRef -= Time.deltaTime;
        }
    }

    public void completeAnim()
    {
        progressing = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("player_hit_box"))
        {
            collision.GetComponentInParent<CatController>().die(transform.position);
        }
    }
}
