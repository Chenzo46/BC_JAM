using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class movingPlatform : MonoBehaviour
{
    [SerializeField] private MovementState selectedState; 
    [SerializeField] private float speed = 10f;

    [Header("Circle Movement")]
    [SerializeField] private float range = 1f;

    [Header("Rectangle Movement")]
    [SerializeField] private Vector2 rectangleRange;
    private List<Vector3> rectanglePoints;
    private int currentPoint = 0;
    private int nextPoint = 1;

    [Header("Free Movement")]
    [SerializeField] private List<Transform> freePoints;

    private Rigidbody2D plrRigid;


    private Vector2 lastPlatPos;



    private enum MovementState
    {
        circle,
        rectangle,
        free,
    }

    private Vector3 orgPos = Vector3.zero;
    void Awake()
    {
        orgPos = transform.position;

        if(selectedState == MovementState.rectangle)
        {
            initalizeRectangle();
        }
        else if (selectedState == MovementState.free)
        {
            initalizeFree();
        }
        else{
            transform.position = orgPos + Vector3.right * range;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (selectedState)
        {
            case MovementState.circle:
                circleMovement();
                break;
            case MovementState.rectangle:
                rectangleMovement();
                break;
            case MovementState.free:
                freeMovement();
                break;
        }   
        

        
    }

    private void LateUpdate() {
        applyCounterForce();
    }

    private void FixedUpdate(){
        //applyCounterForce();
    }

    private void applyCounterForce(){
        if(plrRigid == null){return;}

        //plrRigid.AddForce(cData.getFixedDirection() * cData.speed * forceMultiplier, ForceMode2D.Force);
        Vector2 pChange = (Vector2)transform.position - lastPlatPos;
        plrRigid.transform.position += (Vector3)pChange;
        lastPlatPos = transform.position;
    }

    private void clampPosition(List<Vector3> points){

        float xMin = Mathf.Min(points[currentPoint].x, points[nextPoint].x);
        float xMax = Mathf.Max(points[currentPoint].x, points[nextPoint].x);
        float xClamp = Mathf.Clamp(transform.position.x, xMin, xMax);

        float yMin = Mathf.Min(points[currentPoint].y, points[nextPoint].y);
        float yMax = Mathf.Max(points[currentPoint].y, points[nextPoint].y);
        float yClamp = Mathf.Clamp(transform.position.y, yMin, yMax);

        
        transform.position = new Vector2(xClamp, yClamp);
    }

    private void clampPosition(List<Transform> points){ // Overload method to account for free movement

        float xMin = Mathf.Min(points[currentPoint].position.x, points[nextPoint].position.x);
        float xMax = Mathf.Max(points[currentPoint].position.x, points[nextPoint].position.x);
        float xClamp = Mathf.Clamp(transform.position.x, xMin, xMax);

        float yMin = Mathf.Min(points[currentPoint].position.y, points[nextPoint].position.y);
        float yMax = Mathf.Max(points[currentPoint].position.y, points[nextPoint].position.y);
        float yClamp = Mathf.Clamp(transform.position.y, yMin, yMax);

        
        transform.position = new Vector2(xClamp, yClamp);
    }

    private void circleMovement()
    {
        transform.position = orgPos + new Vector3(Mathf.Cos(Time.time * speed) * range, Mathf.Sin(Time.time * speed) * range);
    }

    private void rectangleMovement()
    {
        Vector3 dir = rectanglePoints[nextPoint] - transform.position;

        transform.position += dir.normalized * speed * Time.deltaTime;

        clampPosition(rectanglePoints);

        if (Vector2.Distance(rectanglePoints[nextPoint], transform.position) <= 0.1f)
        {
            transform.position = rectanglePoints[nextPoint];

            currentPoint = nextPoint;

            nextPoint += 1;

            if(nextPoint >= rectanglePoints.Count)
            {
                nextPoint = 0;
            }

            
        }
    }

    private void freeMovement()
    {
        Vector3 dir = freePoints[nextPoint].position - transform.position;

        transform.position += dir.normalized * speed * Time.deltaTime;


        
        clampPosition(freePoints);

        if (Vector2.Distance(freePoints[nextPoint].position, transform.position) <= 0.05)
        {
            transform.position = freePoints[nextPoint].position;

            currentPoint = nextPoint;

            nextPoint += 1;

            if (nextPoint >= freePoints.Count)
            {
                nextPoint = 0;
            }


        }
    }

    private void initalizeFree()
    {
        if (freePoints.Count == 1)
        {
            GameObject n_Point = new GameObject();
            n_Point.transform.position = transform.position;
            n_Point.transform.SetParent(transform);
            freePoints.Add(n_Point.transform);
        }

        foreach(Transform t in freePoints)
        {
            t.SetParent(null);
        }


        transform.position = freePoints[0].position;
        currentPoint = 0;
        nextPoint = currentPoint + 1;
    }

    private void initalizeRectangle()
    {
        rectanglePoints = new List<Vector3>() 
        {
            transform.position + new Vector3(rectangleRange.x/2,rectangleRange.y/2,0),
            transform.position + new Vector3(rectangleRange.x/2, -rectangleRange.y/2, 0),
            transform.position + new Vector3(-rectangleRange.x/2, -rectangleRange.y/2, 0),
            transform.position + new Vector3(-rectangleRange.x/2, rectangleRange.y/2, 0),
        };

        transform.position = rectanglePoints[currentPoint];
        nextPoint = currentPoint+1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //collision.transform.SetParent(transform);

        if (collision.transform.tag.Equals("player"))
        {
            plrRigid = collision.transform.GetComponent<Rigidbody2D>();
            lastPlatPos = transform.position;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //collision.transform.SetParent(null);

        if (collision.transform.tag.Equals("player"))
        {
            //throwDirection.x *= 2;
            //plrRigid.AddForce(throwDirection, ForceMode2D.Impulse);
            plrRigid = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (selectedState == MovementState.circle && orgPos == Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, range);
        }
        else if(selectedState == MovementState.circle && orgPos != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(orgPos, range);

            //Vector2 reference = (Vector2)orgPos + new Vector2(Mathf.Cos(Time.fixedTime * speed) * range, Mathf.Sin(Time.fixedTime * speed) * range);

            //Vector2 p1 = new Vector2(reference.x - throwDirection.normalized.x*5, reference.y - throwDirection.normalized.y*5);
            //Vector2 p2 = new Vector2(reference.x + throwDirection.normalized.x*5, reference.y + throwDirection.normalized.y*5);
            //Debug.DrawLine(p1, p2);
        }
        else if (selectedState == MovementState.rectangle && orgPos == Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, rectangleRange);
        }
        else if (selectedState == MovementState.rectangle && orgPos != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(orgPos, rectangleRange);
        }
        else if(selectedState == MovementState.free)
        {
            List<Vector3> pointsToDraw = new List<Vector3>();
            
            if(freePoints.Count == 1)
            {
                pointsToDraw.Add(transform.position);
            }

            foreach(Transform t in freePoints)
            {
                pointsToDraw.Add(t.position);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(t.position, 0.1f);
            }
            Gizmos.color = Color.yellow;
            Gizmos.DrawLineStrip(pointsToDraw.ToArray(), true);
        }
    }
}