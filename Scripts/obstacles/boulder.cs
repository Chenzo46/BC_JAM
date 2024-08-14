using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class boulder : MonoBehaviour
{
    public float speed { get; set; } = 0;
    [SerializeField] private float rotationalSpeed = 60f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private CircleCollider2D cCol;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask spikeMask;
    [SerializeField] private GameObject particlePref;

    public int direction { get; set; } = 1;

    private void Start()
    {
        rb.AddForce(Vector2.right * direction * speed, ForceMode2D.Impulse);
    }

    private void Update()
    {
        //transform.rotation = Quaternion.Euler(0,0,transform.eulerAngles.z+(rotationalSpeed * Time.deltaTime));

        //rb.angularVelocity = rotationalSpeed * (direction*-1);

        rb.angularVelocity = -rb.velocity.x * rotationalSpeed;

        checkForPlayer();
        checkForSpike();
    }

    private void checkForPlayer()
    {
        RaycastHit2D rt = Physics2D.CircleCast(transform.position, cCol.radius, Vector2.zero, 0f, playerMask);

        if(rt.collider != null)
        {
            rt.transform.GetComponent<CatController>().die(transform.position);
        }
    }

    private void checkForSpike()
    {
        RaycastHit2D rt = Physics2D.CircleCast(transform.position, cCol.radius, Vector2.zero, 0f, spikeMask);

        if (rt.collider != null)
        {
            explode();
        }
    }

    private void explode()
    {
        Instantiate(particlePref, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
