using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    [SerializeField] private List<ICutsceneComponent.ComponentData> cutsceneActions = new List<ICutsceneComponent.ComponentData>();
    private int cutsceneActionIndex = 0;
    private ICutsceneComponent currentCutsceneAction;

    private bool cutsceneStarted = false;

    private void Start() {
        initalizeCutsceneActions();
    }

    private EventHandler playEvent;

    public void startCutscene(){
        cutsceneStarted = true;
        //playEvent.Invoke(this, EventArgs.Empty);
    }

    private void initalizeCutsceneActions(){
        foreach(ICutsceneComponent.ComponentData cData in cutsceneActions){

            switch(cData.actionType){
                //Camera Action linking
                case ICutsceneComponent.ActionType.ChangeCameraTarget:
                case ICutsceneComponent.ActionType.CameraPath:
                    CameraAction nCamAction = new GameObject().AddComponent<CameraAction>();
                    nCamAction.transform.parent = transform;
                    nCamAction.actionData = cData;
                    break;
                //Player Action Linking
                //

            }

        }
    }

}
