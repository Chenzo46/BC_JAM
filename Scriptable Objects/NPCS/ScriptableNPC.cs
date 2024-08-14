using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/New NPC", fileName = "Jango")]
public class ScriptableNPC : ScriptableObject
{
    public AudioClip speakSound;
    public string npcName;
    public List<dialogueState> dialogueStates = new List<dialogueState>();


    [System.Serializable]
    public class dialogueState
    {
        public string condition = "none";
        public int priority = 0;
        [TextArea(5,10)] public List<string> paragraphs = new List<string>();
        public bool completeOnEnd = false;
        public bool consumeItem = false;
    }

    public void sortDialogueStates()
    {
        dialogueStates = dialogueStates.OrderBy(state => state.priority).ToList();
        dialogueStates.Reverse();
        /*
        foreach (dialogueState d in dialogueStates)
        {
            Debug.Log(d.priority);
        }
        */
        
    }

    public dialogueState getCurrentDialougeState()
    {
        sortDialogueStates();

        foreach (dialogueState d in dialogueStates)
        {
            if (GameStateManager.Singleton.getConditionState(d.condition))
            {
                return d;
            }
            else if (d.condition.Equals("none"))
            {
                return d;
            }
        }

        return null;
    }

    public void initalizeConditions()
    {
        foreach (dialogueState d in dialogueStates)
        {
            GameStateManager.Singleton.addCondition(d.condition);
        }
    }
}
