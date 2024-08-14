using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class InteractableContext : MonoBehaviour
{
    [SerializeField] private TMP_Text contextAsset;
    [SerializeField] private CanvasGroup cGroup;

    public static InteractableContext Singleton;

    private List<int> priorityList = new List<int>();

    private bool isShowing = false;

    private void Awake() {
        Singleton = this;
    }
    private void Update() {
        
        if(priorityList.Count < 1 && isShowing){
            hide();
        }
        else if (priorityList.Count > 0 && !isShowing){
            show();
        }

        priorityList.Clear();
    }

    public void setContext(string context, int priority){
        priorityList.Add(priority);
        if(priorityList.Max() <= priority){
            contextAsset.text = context;
        }
    }

    public void show(){
        cGroup.alpha = 1;
        isShowing = true;
    }
    public void hide(){
        cGroup.alpha = 0;
        isShowing = false;
    }
}
