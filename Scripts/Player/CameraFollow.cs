using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private Vector2 deadzone = new Vector2(1,1);
    [SerializeField] private float smoothFactor = 0.1f;
    [SerializeField] private bool hasBounds = false;
    [SerializeField] private Vector2 cameraBounds = Vector2.zero;
    [SerializeField] private Vector2 boundsCenter = Vector2.zero;

    private Vector2 dist => target.position - transform.position;

    private Vector3 temp = new Vector3(0,0,0);

    private Vector3 refVec;

    private Transform orgTarget;

    public static CameraFollow Singleton;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
        }

        target = GameObject.FindGameObjectWithTag("player").transform;

        temp = target.position;
        orgTarget = target;

        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }

    private void Update()
    {
        if(target == null) { return; }
        if(Mathf.Abs(dist.x) > deadzone.x)
        {
            temp.x = (target.position.x + offset.x) - deadzone.x * Mathf.Sign(dist.x);
        }

        if(Mathf.Abs(dist.y) > deadzone.y)
        {
            temp.y = (target.position.y + offset.y) - deadzone.y * Mathf.Sign(dist.y);
        }

        temp.z = offset.z;
        transform.position = Vector3.SmoothDamp(transform.position, temp, ref refVec, smoothFactor);

        if (hasBounds)
        {
            confineCamera();
        }
    }

    private void confineCamera()
    {
        float x = Mathf.Clamp(transform.position.x, boundsCenter.x - (cameraBounds.x / 2), boundsCenter.x + (cameraBounds.x/2));
        float y = Mathf.Clamp(transform.position.y, boundsCenter.y - (cameraBounds.y / 2), boundsCenter.y + (cameraBounds.y / 2));
        transform.position = new Vector3(x,y,transform.position.z);
    }

    private void OnDrawGizmosSelected()
    {
        if (hasBounds)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(boundsCenter, 0.5f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boundsCenter, cameraBounds);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(boundsCenter, new Vector2(cameraBounds.x + Camera.main.orthographicSize * 4, cameraBounds.y + Camera.main.orthographicSize * 2));
        }
        
    }
}
