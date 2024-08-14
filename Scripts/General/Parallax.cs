using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float parallaxFactor = 1f;
    [SerializeField] private Sprite referenceSprite;
    private Vector3 offset;
    [SerializeField] private int renderDistance = 2;
    [SerializeField] private GameObject mainBack;
    [SerializeField] private int orderInSortingLayer = 0;
    [SerializeField] private bool repeat = false;
    [SerializeField] private bool maintainY = false;
    private List<List<Transform>> backgroundList = new List<List<Transform>>();
    private Vector2 orgPos;
    private Vector2 dist;

    private float currentMidX = 0;
    private float currentMidY = 0;

    private float leftX;
    private float rightX;
    private float topY;
    private float bottomY;

    private float maintainYRef;


    private void Awake()
    {
        orgPos = transform.position;

        offset = referenceSprite.bounds.extents*2;

        leftX = transform.position.x - (offset.x*1.5f);
        rightX = transform.position.x + (offset.x*1.5f);

        topY = transform.position.y + (offset.y * 1.5f);
        bottomY = transform.position.y - (offset.y * 1.5f);

        maintainYRef = transform.GetChild(0).position.y;

        initalizeBackgroundList();
    }

    private void LateUpdate()
    {
        dist = (Vector2)Camera.main.transform.position - orgPos;

        if (maintainY && Camera.main.transform.position.y <= maintainYRef)
        {
            transform.position = new Vector3(orgPos.x + (dist.x * parallaxFactor), transform.position.y, transform.position.z);
            
        }
        else if(maintainY)
        {
            transform.position = new Vector3(orgPos.x + (dist.x * parallaxFactor), orgPos.y, transform.position.z);
        }
        else
        {
            transform.position = orgPos + (dist * parallaxFactor);
        }

        

        if (repeat)
        {
            spriteShift();
        }
        
    }

    private void initalizeBackgroundList()
    {
        float x = 0;
        float y = 0;
        for(int i = renderDistance; i >= -renderDistance; i--)
        {
            List<Transform> row = new List<Transform>();
            y = i * offset.y;

            if(i != 0 && maintainY)
            {
                continue;
            }

            for(int j = -renderDistance; j <= renderDistance; j++)
            {
                x = j * offset.x;
                Transform g = Instantiate(mainBack, transform).GetComponent<Transform>();

                float relY = transform.GetChild(0).transform.localPosition.y;

                g.localPosition = new Vector3(x, y + relY, 0);
                SpriteRenderer s = g.GetComponent<SpriteRenderer>();
                s.sortingOrder = orderInSortingLayer;
                s.sprite = referenceSprite;
                row.Add(g);
            }
            backgroundList.Add(row);
        }
        
    }

    private void spriteShift()
    {
        //horizontal
        if(Camera.main.transform.position.x <= leftX)
        {
            hShift(-1);
        }
        else if(Camera.main.transform.position.x >= rightX)
        {
            hShift(1);
        }
        //vertical
        if(Camera.main.transform.position.y <= bottomY && !maintainY)
        {
            vShift(-1);
        }
        else if(Camera.main.transform.position.y >= topY && !maintainY)
        {
            vShift(1);
        }
    }

    private void hShift(int dir)
    {
        if (dir < 0)
        {
            for(int i = 0; i < backgroundList.Count; i++)
            {
                List<Transform> ls = backgroundList[i];

                Transform rightmost = ls[ls.Count-1];

                rightmost.localPosition = new Vector3(currentMidX-((renderDistance+1) * offset.x),rightmost.localPosition.y,0);

                ls.Remove(rightmost);

                ls.Insert(0,rightmost);
            }

            currentMidX -= offset.x;
            leftX -= offset.x * 1.5f;
            rightX -= offset.x * 1.5f;
        }
        else if (dir > 0)
        {
            for (int i = 0; i < backgroundList.Count; i++)
            {
                List<Transform> ls = backgroundList[i];

                Transform leftmost = ls[0];

                leftmost.localPosition = new Vector3(currentMidX + ((renderDistance + 1) * offset.x), leftmost.localPosition.y, 0);

                ls.Remove(leftmost);

                ls.Add(leftmost);
            }

            currentMidX += offset.x;
            leftX += offset.x * 1.5f;
            rightX += offset.x * 1.5f;
        }
    }
    private void vShift(int dir)
    {
        if(dir < 0)
        {
            List<Transform> top = backgroundList[0];

            foreach(Transform t in top)
            {
                t.localPosition = new Vector3(t.localPosition.x, currentMidY - ((renderDistance + 1) * offset.y), 0);
            }

            backgroundList.Remove(top);
            backgroundList.Add(top);

            currentMidY -= offset.y;
            topY -= offset.y * 1.5f;
            bottomY -= offset.y * 1.5f;
        }
        else if(dir > 0)
        {
            List<Transform> bottom = backgroundList[backgroundList.Count-1];

            foreach (Transform t in bottom)
            {
                t.localPosition = new Vector3(t.localPosition.x, currentMidY + ((renderDistance + 1) * offset.y), 0);
            }

            backgroundList.Remove(bottom);
            backgroundList.Insert(0,bottom);

            currentMidY += offset.y;
            topY += offset.y * 1.5f;
            bottomY += offset.y * 1.5f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector2(rightX,transform.position.y), 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector2(leftX, transform.position.y), 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector2(transform.position.x, topY), 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector2(transform.position.x, bottomY), 0.5f);

    }
}
