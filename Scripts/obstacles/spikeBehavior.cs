using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spikeBehavior : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("player_hit_box"))
        {
            collision.GetComponentInParent<CatController>().die(transform.position);
        }
    }
}
