using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lanternSwing : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float swingForce = 3;
    [SerializeField] private float passiveSwing = 200;
    [SerializeField] private float swingFrequency = 5;

    private float RandOffset;

    private void Awake()
    {
        RandOffset = Random.Range(500, 700);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("player") || collision.transform.tag.Equals("boulder"))
        {
            Vector2 dir = collision.transform.position - transform.position;

            rb.AddForce(-dir.normalized * swingForce, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(transform.right * Mathf.Cos((swingFrequency * Time.fixedTime) + RandOffset) * Time.fixedDeltaTime * passiveSwing);
        //rb.AddForce(Vector2.up * Mathf.Sin((swingFrequency * Time.fixedTime) + RandOffset) * Time.fixedDeltaTime * passiveSwing);
    }
}
