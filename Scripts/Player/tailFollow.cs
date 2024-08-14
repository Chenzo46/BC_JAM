using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tailFollow : MonoBehaviour
{
    [SerializeField] private Transform stem;

    private void Update()
    {
        transform.position = stem.position;
    }
}
