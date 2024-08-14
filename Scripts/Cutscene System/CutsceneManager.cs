using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField]
    private CameraAction cameraAction;

    public List<ICutsceneComponent> testScene = new List<ICutsceneComponent>();
    private void Awake() {
        
    }
}
