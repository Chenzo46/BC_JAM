using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerKiller : MonoBehaviour
{
    [SerializeField] private Vector2 killBounds;
    [SerializeField] private Vector2 offset;
    [SerializeField] private LayerMask playerLayer;

    private void Update()
    {
        checkForPlayer();
    }

    private void checkForPlayer()
    {
        RaycastHit2D playerInHitBox = Physics2D.BoxCast((Vector2)transform.position + offset, killBounds, 0f, Vector2.zero, 0f, playerLayer);

        if (playerInHitBox.collider != null) 
        {
            playerInHitBox.transform.GetComponent<CatController>().die(transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube((Vector2)transform.position + offset, killBounds);
    }


}
