using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombDispenser : MonoBehaviour
{
    [SerializeField] private GameObject bombPref;
    [SerializeField] private bool intervalBased = false;
    [SerializeField] private float dispenseSpeed = 1f;
    [SerializeField] private float dispenseForce = 3f;
    [SerializeField] private Animator anim;

    private float dispenseRef;

    private void Awake()
    {
        dispenseRef = dispenseSpeed;
    }

    private void Update()
    {
        if (!intervalBased) { return; }
        if (dispenseRef <= 0)
        {
            dispense();
            dispenseRef = dispenseSpeed;
        }
        else
        {
            dispenseRef -= Time.deltaTime;
        }
    }

    public void dispense()
    {
        Rigidbody2D brb = Instantiate(bombPref, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();

        brb.AddForce(-transform.up * dispenseForce, ForceMode2D.Impulse);

        anim.SetTrigger("dispense");
    }
}
