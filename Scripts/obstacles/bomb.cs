using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngineInternal;

public class bomb : MonoBehaviour
{
    [SerializeField] private float explosionTime = 5f;
    [SerializeField] private float explodeRadius = 1f;
    [SerializeField] private LayerMask explodable;
    [SerializeField] private List<string> explodeOnTouch = new List<string>();
    [SerializeField] private GameObject explodeParticles;

    private float explosionRef;

    private void Awake()
    {
        explosionRef = explosionTime;
    }

    private void Update()
    {
        if(explosionRef <= 0)
        {
            //Explode stuff here
            explodeBomb();
        }
        else
        {
            explosionRef -= Time.deltaTime;
        }
    }

    private void explodeNearby()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, explodeRadius, Vector2.zero, 0f, explodable);


        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.tag.Equals("player"))
            {
                hit.transform.GetComponent<CatController>().die(transform.position);
            }
            else if (hit.transform.tag.Equals("fragile"))
            {
                hit.transform.GetComponent<fragileBlock>().destroyBlock(transform.position);
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(explodeOnTouch.Contains(collision.transform.tag))
        {
            explodeBomb();
            
        }

    }

    private void explodeBomb()
    {
        explodeNearby();
        Instantiate(explodeParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }

}
