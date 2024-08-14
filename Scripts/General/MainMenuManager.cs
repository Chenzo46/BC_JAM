using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainMenuManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private List<MenuStateScreen> stateScreens = new List<MenuStateScreen>();
    private StateMachine menuStateMachine;

    public EventHandler onSelectedObjectChanged;

    private GameObject previousSelected;

    public static MainMenuManager Singleton;

    [Header("Cursor Settings")]
    [SerializeField] private Image currentCursor;
    [SerializeField] private float cursorMoveTime = 0.8f;

    [Header("Selected Text Settings")]
    [SerializeField] private TextAnimator.DisplacementType textAnimateType;
    [SerializeField] private float animationMagintude = 0;
    [SerializeField] private float animationOffset = 0;
    [SerializeField] private float animationFrequency = 1;

    [Header("Cutscene Settings")]
    [SerializeField] private List<MenuCutscene> cutscenes;

    private Sequence currentPlayingSequence;

    private TextAnimator buttonTextAnimator;

    private void Awake() {
        Time.timeScale = 1f;
        Singleton = this;
        menuStateMachine = new StateMachine();
        initalizeStates();
        previousSelected = eventSystem.firstSelectedGameObject;

        animateCurrentSelectedText();

        //onSelectedObjectChanged += (sender, args) => { moveCursor(); };
        onSelectedObjectChanged += (sender, args) => { animateCurrentSelectedText(); };
    }

    private void Update() {
        if(eventSystem.currentSelectedGameObject == null){
            eventSystem.SetSelectedGameObject(previousSelected);
        }
        else if(previousSelected != eventSystem.currentSelectedGameObject){
            previousSelected = eventSystem.currentSelectedGameObject;
            onSelectedObjectChanged.Invoke(this, EventArgs.Empty);
        }

        setCurrentActiveScreen();
        setCursorVisibility();
        moveCursor();

        if(buttonTextAnimator == null){return;}
        buttonTextAnimator.moveText(textAnimateType, animationMagintude, animationFrequency, animationOffset);
    }

    private void initalizeStates(){
        foreach(MenuStateScreen screen in stateScreens){
            menuStateMachine.AddState(screen.stateName);
        }
        menuStateMachine.ChangeState(stateScreens[0].stateName);
    }

    private void moveCursor(){
        Transform cursorAnchor = findCursorAnchor();
        if(cursorAnchor == null){return;}
        currentCursor.transform.DOMove(cursorAnchor.position, cursorMoveTime);
    }


    private Transform findCursorAnchor(){
        Transform refTransform = eventSystem.currentSelectedGameObject == null ? previousSelected.transform : eventSystem.currentSelectedGameObject.transform;
        for(int i = 0; i < refTransform.childCount; i++){
            Transform child = refTransform.GetChild(i);
            if(child.tag.Equals("cursorAnchor")){
                return child;
            }
        }
        return null;
    }

    private void animateCurrentSelectedText(){
        try{
            if(!(menuStateMachine.isState("Main") || menuStateMachine.isState("File_Selected") || menuStateMachine.isState("Clear"))){return;}

            Button currentButton = eventSystem.currentSelectedGameObject == null ? previousSelected.GetComponent<Button>() : eventSystem.currentSelectedGameObject.GetComponent<Button>();
            TMP_Text currentText = currentButton.transform.GetChild(0).GetComponent<TMP_Text>();

            if(buttonTextAnimator == null)
            { 
                buttonTextAnimator = new TextAnimator(currentText); 
            }
            else{
                buttonTextAnimator.changeTextToAnimate(currentText);
            }

           
        }catch{
            // No Button selected BUB!
        }
    }


    private void setCurrentActiveScreen(){
        if(currentPlayingSequence == null) {return;}
        foreach(MenuStateScreen screen in stateScreens){
            screen.screen.interactable = menuStateMachine.isState(screen.stateName) && !currentPlayingSequence.IsPlaying();
        }
    }

    private void setCursorVisibility(){
        if(currentPlayingSequence == null){return;}
        float a = currentPlayingSequence.IsPlaying() ? 0:255;
        currentCursor.color = new Color(255,255,255,a);
    }

    private MenuCutscene getCutscene(string cutsceneName){
        foreach(MenuCutscene cut in cutscenes){
            if(cut.trainsitionName.Equals(cutsceneName)){
                return cut;
            }
        }
        return null;
    }
    public void goToGame(){
        menuStateMachine.ChangeState("To_Game");
    }


    public void goBack(){
        MenuCutscene cut = getCutscene(menuStateMachine.currentState);
        cut.OnPlayReversed += () => {eventSystem.SetSelectedGameObject(cut.previousSelected);};
        cut.playReversed();
        menuStateMachine.reverseState();
        currentPlayingSequence = cut.getSequence();
    }

    public void playCutscene(string cutsceneName){
        menuStateMachine.ChangeState(cutsceneName);
        MenuCutscene cut = getCutscene(menuStateMachine.currentState);
        cut.OnPlay += () => {eventSystem.SetSelectedGameObject(cut.firstSelected);};
        cut.play();
        currentPlayingSequence = cut.getSequence();
    }

    public void goToRoom(int num){
        SceneTransitioner.Singleton.transitionScene(num);
    }
    
}

[Serializable]
public struct MenuStateScreen{
    public CanvasGroup screen;
    public string stateName;
}

[Serializable]
public class MenuCutscene{

    public string trainsitionName;
    public GameObject firstSelected;
    public GameObject previousSelected;

    public delegate void transitionEvent();
    public event transitionEvent OnPlay;
    public event transitionEvent OnPlayReversed;
    public event transitionEvent OnEnd;

    private Sequence cutsceneSequence;

    [SerializeField] private List<TweenData> tweensToAdd;

    public void play(){
        // Invoke play event
        OnPlay?.Invoke();
        
        // Initalize Sequence

        cutsceneSequence = DOTween.Sequence();
        cutsceneSequence.SetAutoKill(false);

        // Append Tweens
        foreach(TweenData tData in tweensToAdd){
            Tweener toAdd = null;
            switch(tData.tweenType){
                case TweenData.TweenType.move:
                    toAdd = tData.objectToTween.DOAnchorPos(tData.endValue, tData.duration).SetEase(tData.easeType);
                    break;
                case TweenData.TweenType.fade:
                    CanvasGroup canvasGroup = tData.objectToTween.GetComponent<CanvasGroup>();
                    toAdd = DOTween.To(x => canvasGroup.alpha = x, tData.endValue.x, tData.endValue.y, tData.duration).SetEase(tData.easeType);
                    break;
                case TweenData.TweenType.scaleX:
                    toAdd = tData.objectToTween.DOScaleX(tData.endValue.x, tData.duration);
                    break;
                case TweenData.TweenType.scaleY:
                    toAdd = tData.objectToTween.DOScaleY(tData.endValue.y, tData.duration);
                    break;
            }
            toAdd.SetUpdate(true);
            

            if(tData.isSimultaneous){
                cutsceneSequence.Join(toAdd);
            }
            else{
                cutsceneSequence.Append(toAdd);
            }
        }
        cutsceneSequence.onComplete += () => {OnEnd?.Invoke();};
        cutsceneSequence.Play();
    }

    public void playReversed(){
        OnPlayReversed?.Invoke();
        cutsceneSequence.PlayBackwards();

    }

    public Sequence getSequence(){
        return cutsceneSequence;
    }
    //
    [Serializable]
    private struct TweenData{
        public TweenType tweenType;
        public Ease easeType;
        public RectTransform objectToTween;
        public Vector3 endValue;
        public float duration;
        public bool isSimultaneous;
        public enum TweenType{
            fade,
            move,
            scaleX,
            scaleY,
        }
    }
}

public class TextAnimator{
    private TMP_Text referenceText;
    private TMP_MeshInfo[] originalMeshInfo;

    //TMP_MeshInfo.mesh.verticies = Working
    //TMP_MeshInfo.vertices = Draft

    public TextAnimator(TMP_Text referenceText){
        this.referenceText = referenceText;
        originalMeshInfo = new TMP_MeshInfo[referenceText.textInfo.meshInfo.Length];
        referenceText.textInfo.meshInfo.CopyTo(originalMeshInfo, 0);
    }

    public void moveText(DisplacementType dType, float amplitude, float frequency, float offset){
        referenceText.ForceMeshUpdate();

        foreach(TMP_CharacterInfo charInfo in referenceText.textInfo.characterInfo){
            TMP_MeshInfo mInfo = referenceText.textInfo.meshInfo[charInfo.materialReferenceIndex];

            for(int idx = 0; idx < 4; ++idx){
                int vertIndex = charInfo.vertexIndex + idx;
                Vector3 orgPos = mInfo.vertices[vertIndex];
                if(vertIndex-idx == 0 && referenceText.text.Length <= 6){
                    mInfo.vertices[vertIndex] = orgPos + generateDisplacement(dType, amplitude * 0.1f /** ((mInfo.vertices.Length - vertIndex - idx) *0.1f)*/, frequency, mInfo.vertices[vertIndex-idx].x*0.1f);//offset : orgPos.x*0.1f
                }
                else{
                    mInfo.vertices[vertIndex] = orgPos + generateDisplacement(dType, amplitude /** ((mInfo.vertices.Length - vertIndex - idx) *0.1f)*/, frequency, mInfo.vertices[vertIndex-idx].x*0.1f);//offset : orgPos.x*0.1f
                }
                
                //referenceText.color = rainbowColor(frequency);
                //mInfo.colors32[vertIndex] = rainbowColor(frequency);
            }
        }

        updateMesh();
    }

    public void changeTextToAnimate(TMP_Text referenceText){
        revertMesh();
        this.referenceText = referenceText;
        originalMeshInfo = new TMP_MeshInfo[referenceText.textInfo.meshInfo.Length];
        referenceText.textInfo.meshInfo.CopyTo(originalMeshInfo, 0);
    }

    private void updateMesh(){
        int idx = 0;
        foreach(TMP_MeshInfo mInfo in referenceText.textInfo.meshInfo){
            mInfo.mesh.vertices = mInfo.vertices;
            //mInfo.mesh.colors32 = mInfo.colors32;
            referenceText.UpdateGeometry(mInfo.mesh, idx);
            idx++;
        }
    }

    private void revertMesh(){
       
        referenceText.ForceMeshUpdate();
        int idx = 0;
        foreach(TMP_MeshInfo mInfo in referenceText.textInfo.meshInfo){
            mInfo.mesh.vertices = originalMeshInfo[idx].vertices;
            //mInfo.mesh.colors32 = originalMeshInfo[idx].colors32;
            referenceText.UpdateGeometry(mInfo.mesh, idx);
            idx++;
        }
    }

    private Color rainbowColor(float frequency){ // Stupid dumb code that doesn't work (It's dumb and stupid)
        float pi = Mathf.PI;
        float r = 0;
        float g = 0;
        float b = 0;
        float normTime = getNormalizedTime() * 1f;
        // r behavior
        if(normTime <= pi/3 || normTime >= 5*pi/3){
            r = 255;
        }
        else if (normTime >= pi/3 && normTime <= 2*pi/3){
            float t = (normTime - pi/3) / (2*pi/3);
            r = 255 - ((1 - t) * 255);
        }
        //g behavior
        if(normTime >= pi/3 && normTime <= pi){
            g = 255;
        }
        else if(normTime < pi/3 && normTime >= 0){
            float t = normTime / (pi/3);
            g = (1 - t) * 255;
        }

        return new Color(r,g,b);
    }

    private float getNormalizedTime(){
        float refTime = (Time.time) % (2*Mathf.PI);

        return refTime;
    }

    private Vector3 generateDisplacement(DisplacementType dType, float amplitude, float frequency, float offset){
        switch(dType){
            case DisplacementType.sine:
                return new Vector3(
                    0, 
                    amplitude * Mathf.Sin(Time.time * frequency + offset), 
                    0);
            case DisplacementType.noise:
                return new Vector3(
                    Random.Range(-1,1)*amplitude * Mathf.PerlinNoise1D(Time.time * frequency + offset), 
                    Random.Range(-1,1)*amplitude * Mathf.PerlinNoise1D(Time.time * frequency + offset), 
                    0);
        }
        return Vector3.zero;
    }

    public enum DisplacementType{
        sine,
        noise,
    }

}
