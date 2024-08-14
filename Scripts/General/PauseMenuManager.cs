using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
public class PauseMenuManager : MonoBehaviour
{
    private StateMachine pauseStateMachine;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private List<MenuStateScreen> stateScreens = new List<MenuStateScreen>();
    [SerializeField] private List<MenuCutscene> cutscenes;

    private GameObject previousSelected;

    [SerializeField] private Image currentCursor;
    [SerializeField] private float cursorMoveTime = 0.8f;
    private Sequence currentPlayingSequence;

    public static PauseMenuManager Singleton;
    private void Awake() {
        Singleton = this;
        pauseStateMachine = new StateMachine();
        initalizeStates();
        
        previousSelected = eventSystem.firstSelectedGameObject;
    }
    private void Update(){
        if(eventSystem.currentSelectedGameObject == null){
            eventSystem.SetSelectedGameObject(previousSelected);
        }
        else if(previousSelected != eventSystem.currentSelectedGameObject){
            previousSelected = eventSystem.currentSelectedGameObject;
        }

        setCursorVisibility();
        moveCursor();
        setCurrentActiveScreen();
    }

    private void initalizeStates(){
        foreach(MenuStateScreen screen in stateScreens){
            pauseStateMachine.AddState(screen.stateName);
        }
        pauseStateMachine.ChangeState(stateScreens[0].stateName);
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

    public bool isPlayingCutscene(){
        if(currentPlayingSequence != null){
            return currentPlayingSequence.IsPlaying();
        }
        else{
            return false;
        }
    }

    private void moveCursor(){
        Transform cursorAnchor = findCursorAnchor();
        if(cursorAnchor == null){return;}
        currentCursor.transform.DOMove(cursorAnchor.position, cursorMoveTime);
    }

    private void setCursorVisibility(){
        if(currentPlayingSequence == null){return;}
        float a = currentPlayingSequence.IsPlaying() ? 0:255;
        currentCursor.color = new Color(255,255,255,a);

        if(pauseStateMachine.isState("Play")){
            currentCursor.color = new Color(255,255,255,0);
        }
    }


    private MenuCutscene getCutscene(string cutsceneName){
        foreach(MenuCutscene cut in cutscenes){
            if(cut.trainsitionName.Equals(cutsceneName)){
                return cut;
            }
        }
        return null;
    }

    
    private void setCurrentActiveScreen(){
        if(currentPlayingSequence == null) {return;}
        foreach(MenuStateScreen screen in stateScreens){
            screen.screen.interactable = pauseStateMachine.isState(screen.stateName) && !currentPlayingSequence.IsPlaying();
        }
    }


    public void goBack(){
        MenuCutscene cut = getCutscene(pauseStateMachine.currentState);
        cut.getSequence().SetUpdate(true);
        cut.OnPlayReversed += () => {eventSystem.SetSelectedGameObject(cut.previousSelected);};
        cut.playReversed();
        pauseStateMachine.reverseState();
        currentPlayingSequence = cut.getSequence();
    }

    public void playCutscene(string cutsceneName){
        pauseStateMachine.ChangeState(cutsceneName);
        MenuCutscene cut = getCutscene(pauseStateMachine.currentState);
        cut.getSequence().SetUpdate(true);
        cut.OnPlay += () => {eventSystem.SetSelectedGameObject(cut.firstSelected);};
        cut.play();
        currentPlayingSequence = cut.getSequence();
    }

    public void pause(){
        playCutscene("Pause");
        //Time.timeScale = 0;
        GameStateManager.Singleton.setState(GameStateManager.GameState.Paused);
    }

    public void unPause(){
        goBack();
        //Time.timeScale = 1f;
        GameStateManager.Singleton.setState(GameStateManager.GameState.Play);
    }

    public void mainMenu(){
        pauseStateMachine.ChangeState("To_Main");
        SceneTransitioner.Singleton.backToMain();
    }
}
