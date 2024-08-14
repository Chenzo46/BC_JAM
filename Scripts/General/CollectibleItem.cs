using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private ScriptableItem itemReference;
    [SerializeField] private SavedStateMachine _svMachine;
    private StateMachine collectibleStates;

    private void Start() {
        collectibleStates = new StateMachine("not_collected", "collected");
        _svMachine.loadLastSavedState(collectibleStates);

        if(collectibleStates.currentState.Equals("collected")){
            gameObject.SetActive(false);
        }
    }
    public void collectItem(){
        InventorySystem.Singleton.collectItem(itemReference);
        collectibleStates.ChangeState("collected");
        gameObject.SetActive(false);
    }
}

