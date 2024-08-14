using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flyAI : MonoBehaviour
{
    [SerializeField] private float maxVelocity = 20f;
    [SerializeField] private float flySpeed = 300f;
    [SerializeField] private float agroRange = 5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask playerMask;

    private Vector2 target;

    private bool inPursuit = false;

    private void FixedUpdate()
    {
        findPlayer();

        if (inPursuit)
        {
            Vector2 moveDir = (target - (Vector2)transform.position).normalized;

            rb.velocity = flySpeed * moveDir * Time.fixedDeltaTime;

            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);

            if (target.x > transform.position.x)
            {
                transform.localScale = new Vector3(1,1,1);
            }
            else if(target.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        
        
    }

    private void findPlayer()
    {
        RaycastHit2D playerHit = Physics2D.CircleCast(transform.position, agroRange, Vector2.zero, 0f, playerMask);

        if(playerHit.collider != null)
        {
            inPursuit = true;
            target = playerHit.collider.transform.position;
        }
        else
        {
            inPursuit = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, agroRange);
    }

}
