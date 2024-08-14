using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedStateMachine : MonoBehaviour
{
    [SerializeField] private int saveID;
    public void loadLastSavedState(StateMachine stateMachineRef){
        if(saveID <= -1){return;}
        
        stateMachineRef.OnStateChanged += (sender, args) => { SceneSaveData.Singleton.updateSaveable(stateMachineRef.asStateData()); };

        stateMachineRef.saveID = saveID;
        stateData savedStateMachine = SceneSaveData.Singleton.getStateMachineData(saveID);
        if(savedStateMachine != null){
            stateMachineRef.ChangeState(savedStateMachine.savedState);
        }
        else{
            SceneSaveData.Singleton.addSaveable(stateMachineRef.asStateData());
        }
    }
}
