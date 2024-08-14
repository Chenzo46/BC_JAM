using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics.Tracing;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private CanvasGroup DialogueBox;
    [SerializeField] private TMP_Text dialogueBoxContent;
    [SerializeField] private TMP_Text nameContent;
    [SerializeField] private float textSpeed = 0.01f;
    [SerializeField] private AudioSource audSource;
    [SerializeField] private GameObject nextArrow;
    public static DialogueSystem Singleton;

    private ScriptableNPC.dialogueState currentDialogue;
    private ScriptableNPC currentNPC;

    private AudioClip speakSound;

    private float textSpeedRef = 0;

    private int currentParagraphIndex = 0;
    private int currentCharacterIndex = 0;

    private bool currentParagraphComplete = false;
    private bool itemConsumed = false;

    private void Awake()
    {
        if(Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void beginDialogueSequence(ScriptableNPC npc)
    {
        GameStateManager.Singleton.setState(GameStateManager.GameState.Dialogue);
        currentNPC = npc;
        currentDialogue = npc.getCurrentDialougeState();
        DialogueBox.alpha = 1f;
        dialogueBoxContent.text = "";
        nameContent.text = npc.npcName;
        currentCharacterIndex = 0;
        speakSound = npc.speakSound;
        audSource.clip = speakSound;
        itemConsumed = false;
    }
    private void Update()
    {
        if(GameStateManager.Singleton.getCurrentGameState() == GameStateManager.GameState.Dialogue)
        {
            if (currentParagraphIndex >= currentDialogue.paragraphs.Count)
            {
                completeDialogue();
                return;
            }

            if(currentCharacterIndex == 0 && currentDialogue.consumeItem && !itemConsumed){
                itemConsumed = true;
                string removeStr = currentDialogue.condition.Substring(4,currentDialogue.condition.Length-4);
                InventorySystem.Singleton.removeItem(removeStr);
            }

            if (textSpeedRef <= 0 && currentCharacterIndex < currentDialogue.paragraphs[currentParagraphIndex].Length)
            {
                textSpeedRef = textSpeed;
                dialogueBoxContent.text += currentDialogue.paragraphs[currentParagraphIndex][currentCharacterIndex];
                currentCharacterIndex++;
                audSource.PlayOneShot(speakSound);
            }
            else if (currentCharacterIndex >= currentDialogue.paragraphs[currentParagraphIndex].Length && !currentParagraphComplete)
            {
                currentParagraphComplete = true;
                nextArrow.SetActive(true);
            }
            else
            {
                textSpeedRef -= Time.deltaTime;
            }

            if (Input.GetButtonDown("Interact") && !currentParagraphComplete)
            {
                currentCharacterIndex = currentDialogue.paragraphs[currentParagraphIndex].Length;
                currentParagraphComplete = true;
                dialogueBoxContent.text = currentDialogue.paragraphs[currentParagraphIndex];
                nextArrow.SetActive(true);

            }
            else if (Input.GetButtonDown("Interact") && currentParagraphComplete)
            {
                currentCharacterIndex = 0;
                currentParagraphComplete = false;
                dialogueBoxContent.text = "";
                currentParagraphIndex++;
                nextArrow.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                audSource.Play();
            }
        }
    }

    private void completeDialogue()
    {
        GameStateManager.Singleton.setState(GameStateManager.GameState.Play);
        currentParagraphIndex = 0;
        DialogueBox.alpha = 0f;
        dialogueBoxContent.text = "";
        if (currentDialogue.completeOnEnd)
        {
            GameStateManager.Singleton.setCondition(currentNPC.dialogueStates[currentNPC.dialogueStates.IndexOf(currentDialogue)-1].condition,true);
        }
    }
}
