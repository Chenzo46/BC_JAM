using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;

public class HiddenArea : MonoBehaviour
{
    [SerializeField] private Tilemap tMap;
    [SerializeField] private float fadeDuration = 5f;
    [SerializeField] private SavedStateMachine _svMachine;
    private StateMachine hiddenStates;

    private void Start() {
        hiddenStates = new StateMachine("hidden", "found");
        _svMachine.loadLastSavedState(hiddenStates);

        tMap.color = hiddenStates.isState("found") ? new Color(255,255,255,0) : new Color(255,255,255,255);
    }

    private void Update(){
        //Debug.Log(tMap.color);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("player") && !hiddenStates.isState("found"))
        {
            hiddenStates.ChangeState("found");
            Tween t = DOTween.ToAlpha(() => tMap.color, c => tMap.color = c, 0, fadeDuration);
            t.SetEase(Ease.OutQuint);
            //t.Play();

            
        }
    }
}
