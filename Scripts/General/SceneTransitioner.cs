using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SceneTransitioner : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Canvas mainCanvas;
    public static SceneTransitioner Singleton;

    private void Awake()
    {
        Singleton = this;

        StartCoroutine(beginScene());

        mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        mainCanvas.worldCamera = Camera.main;
        mainCanvas.sortingLayerName = "UI";
    }

    public void transitionScene(int sceneIndex)
    {

        StartCoroutine(changeEnum(sceneIndex));
    }

    public void restartScene()
    {
        StartCoroutine(resScene(SceneManager.GetActiveScene().buildIndex));
    }

    public void startGame(int sceneIndex){
        StartCoroutine(fromMain(sceneIndex));
    }

    public void backToMain(){
        StartCoroutine(toMain());
    }


    private IEnumerator changeEnum(int sIndex)
    {
        anim.SetTrigger("in");
        GameStateManager.Singleton.setState(GameStateManager.GameState.Cutscene);
        yield return new WaitForSeconds(1.51f);
        SceneManager.LoadScene($"r{sIndex}");
    }
    private IEnumerator resScene(int sIndex)
    {
        anim.SetTrigger("in");
        GameStateManager.Singleton.setState(GameStateManager.GameState.Cutscene);
        yield return new WaitForSeconds(1.51f);
        SceneManager.LoadScene(sIndex);
    }

    private IEnumerator beginScene()
    {
        anim.SetTrigger("out");
        yield return new WaitForSeconds(1.51f);
        GameStateManager.Singleton.setState(GameStateManager.GameState.Play);
    }

    private IEnumerator fromMain(int sIndex){
        anim.SetTrigger("outMain");
        GameStateManager.Singleton.setState(GameStateManager.GameState.Cutscene);
        yield return new WaitForSeconds(1.51f);
        SceneManager.LoadScene($"r{sIndex}");
    }

    private IEnumerator toMain(){
        anim.SetTrigger("outMain");
        GameStateManager.Singleton.setState(GameStateManager.GameState.Cutscene);
        yield return new WaitForSecondsRealtime(1.51f);
        SceneManager.LoadScene("Main Menu");
    }

}
