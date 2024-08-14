using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bugAI : MonoBehaviour
{
    [SerializeField] private float walkRange = 5f;
    [SerializeField] private float walkSpeed = 100f;
    [SerializeField] private Rigidbody2D rb;

    private bool facingRight = true;
    private Vector2 orgPos;
    private void Awake()
    {
        orgPos = transform.position;
    }

    private void FixedUpdate()
    {
        Vector2 lPoint = (orgPos + Vector2.left * walkRange);
        Vector2 rPoint = (orgPos + Vector2.right * walkRange);

        rb.velocity = new Vector2(walkSpeed * transform.localScale.x * Time.fixedDeltaTime,rb.velocity.y);

        if(facingRight && transform.position.x > rPoint.x)
        {
            facingRight = false;
            transform.localScale = new Vector3(-1,1,1);
        }
        else if(!facingRight && transform.position.x < lPoint.x)
        {
            facingRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(orgPos + Vector2.right*walkRange, 0.1f);

        Gizmos.color = Color.red;

        Gizmos.DrawSphere(orgPos + Vector2.left * walkRange, 0.1f);
    }
}
