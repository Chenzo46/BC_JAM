using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableArea : MonoBehaviour
{
    [SerializeField] private UnityEvent onInteracted;

    [SerializeField] private Vector2 interactableBounds;
    [SerializeField] private Vector2 offset;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private string context;
    [SerializeField] private int priority = 0;

    private bool isInArea = false;

    private void Update() {
        playerInArea();
    }

    private void LateUpdate(){
        if(isInArea && Input.GetButtonDown("Interact")){
            onInteracted.Invoke();
        }
    }

    private void playerInArea(){

        isInArea = Physics2D.OverlapBox((Vector2)transform.position + offset, interactableBounds, 0f, playerMask);        

        if(isInArea){
            InteractableContext.Singleton.setContext(context, priority);
        }   
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + offset, interactableBounds);
    }
}
