using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class modularButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onPressed;
    [SerializeField] private Sprite normalSprite, pressedSprite;
    [SerializeField] private SpriteRenderer sprend;
    [SerializeField] private bool stayPressed = true;
    [SerializeField] private List<string> allowedToActivate;
    [SerializeField] private SavedStateMachine _svStateMachine;

    private StateMachine buttonStates;

    private void Start() {
        buttonStates = new StateMachine("idle","pressed");
        _svStateMachine.loadLastSavedState(buttonStates);

        sprend.sprite =  buttonStates.isState("pressed") ? pressedSprite : normalSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (allowedToActivate.Contains(collision.tag) && buttonStates.isState("idle"))
        {
            buttonStates.ChangeState("pressed");
            sprend.sprite = pressedSprite;
            onPressed.Invoke();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (allowedToActivate.Contains(collision.tag) && buttonStates.isState("pressed") && !stayPressed)
        {
            buttonStates.ChangeState("idle");
            sprend.sprite = normalSprite;
        }
    }
}
