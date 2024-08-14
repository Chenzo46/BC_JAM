using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bouncePad : MonoBehaviour
{
    [SerializeField] private float bounceStrength = 20f;
    [SerializeField] private Animator anim;
    [SerializeField] private List<string> bounceables = new List<string>();
    private bool hasBounced = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (bounceables.Contains(collision.tag) && !hasBounced)
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(bounceStrength * Vector2.up, ForceMode2D.Impulse);
            hasBounced = true;
            anim.SetTrigger("bounce");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (bounceables.Contains(collision.tag) && hasBounced)
        {
            hasBounced = false;
        }
    }
}
