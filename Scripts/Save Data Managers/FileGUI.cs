using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FileGUI : MonoBehaviour
{
    [SerializeField] private int fileIndex;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button clearButton;
    [SerializeField] private TMP_Text timePlayed;
    [SerializeField] private TMP_Text lastAreaVisited;
    [SerializeField] private TMP_Text percentPlayed;
    [SerializeField] CanvasGroup SavePresent;
    [SerializeField] CanvasGroup noSavePresent;

    private string filePath;

    private void Start() {
        setVisibility();
    }

    public void setVisibility(){
        filePath = Path.Combine(SaveDataManager.saveDataPath, $"file_{fileIndex}", SavedValueManager.Singleton.getSaveDataType().ToString());

        SavePresent.alpha =  isSaveDataPresent()? 1:0;
        noSavePresent.alpha = isSaveDataPresent() ? 0:1;

        lastAreaVisited.text = $"Area: {SavedValueManager.Singleton.getDirectString(filePath, "last_area")}";
    }

    private bool isSaveDataPresent(){
        return SavedValueManager.Singleton.getDirectData(filePath, "game_started");
    }

    public void setContext(){
        continueButton.onClick.RemoveAllListeners();
        newGameButton.onClick.RemoveAllListeners();

        continueButton.onClick.AddListener(() => { GameStateManager.Singleton.continueGameFile(fileIndex);});
        newGameButton.onClick.AddListener(() => { GameStateManager.Singleton.startNewGameFile(fileIndex); setVisibility(); setContext();});

        clearButton.interactable = isSaveDataPresent();
        
    }
}