using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatNPC : MonoBehaviour
{
    [SerializeField] private ScriptableNPC npcData;

    private void Start()
    {
        npcData.initalizeConditions();
    }
    public void startNPCDialogue(){
        if(GameStateManager.Singleton.getCurrentGameState() == GameStateManager.GameState.Dialogue){return;}
        DialogueSystem.Singleton.beginDialogueSequence(npcData);
    }

}
