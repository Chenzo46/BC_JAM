using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using DG.Tweening;
using System;

public class CameraAction : MonoBehaviour, ICutsceneComponent
{
    
    public ICutsceneComponent.ComponentData actionData;
    
    public void _playComponent(EventHandler eventHandler){
        actionData.hasStarted = true;
        //eventHandler.
    }

    public void followPath(){
        Debug.Log("Test");
    }

    public void changeTarget(){

    }
}
