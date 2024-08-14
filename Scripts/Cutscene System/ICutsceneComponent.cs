using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public interface ICutsceneComponent
{
    void _playComponent(EventHandler playEvent);
   
    public enum ActionType{
        ChangeCameraTarget,
        CameraPath,

    }
    [System.Serializable]
    public struct ComponentData{
        public bool isCompleted {get; set;}
        public bool hasStarted {get;set;}
        public bool isPlaying {get;set;}
        public float duration;
        public Ease easeType;
        public ActionType actionType;
    }

}
