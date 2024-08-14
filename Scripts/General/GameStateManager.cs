using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [TextArea(2,4)][SerializeField] private List<string> gameConditions = new List<string>();
    private Dictionary<string, float>  gameFloats = new Dictionary<string, float>();
    private Dictionary<string, string>  gameStrings = new Dictionary<string, string>();

    private Dictionary<string, bool> gameConditionsDict = new Dictionary<string, bool>();

    private float secondsPlayed;

    private bool game_started = false;

    public enum GameState
    {
        Paused,
        Play,
        Cutscene,
        Dialogue,
    }

    private GameState currentState = GameState.Play;

    public static GameStateManager Singleton;


    public int currentRoom { get; set; } = 0;
    public int previousRoom { get; set; } = 0;

    private void Awake()
    {
        if(Singleton == null)
        {
            Singleton = this;
            initalizeConditionDict();
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update(){
        if(game_started && currentState == GameState.Play){
            secondsPlayed += Time.deltaTime;
        }

        checkForPauseInput();
        //Debug.Log($"Previous Room {previousRoom}");
        //Debug.Log($"Current Room {currentRoom}");
    }

    private void checkForPauseInput(){
        if(PauseMenuManager.Singleton == null) {return;}
        if(Input.GetButtonDown("pause") && currentState != GameState.Paused && !PauseMenuManager.Singleton.isPlayingCutscene()){
            PauseMenuManager.Singleton.pause();
        }
        else if (Input.GetButtonDown("pause") && currentState == GameState.Paused && !PauseMenuManager.Singleton.isPlayingCutscene()){
            PauseMenuManager.Singleton.unPause();
        }
    }

    private void initalizeConditionDict()
    {
        foreach(string cond in gameConditions)
        {
            gameConditionsDict.Add(cond, false);
        }
    }

    public GameState getCurrentGameState()
    {
        return currentState;
    }

    public void setState(GameState state)
    {
        currentState = state;
    }

    public void addCondition(string condition)
    {
        if (!gameConditionsDict.ContainsKey(condition))
        {
            gameConditionsDict.Add(condition, false);
        }
    }

    public void addCondition(string condition, bool defaultState)
    {
        if (!gameConditionsDict.ContainsKey(condition))
        {
            gameConditionsDict.Add(condition, defaultState);
        }
        else{
            setCondition(condition, defaultState);
        }
    }

    public bool getConditionState(string condition)
    {
        foreach(KeyValuePair<string, bool> cnd in gameConditionsDict)
        {
            if (cnd.Key.Equals(condition))
            {
                return cnd.Value;
            }
        }

        return false;
    }

    public void saveTimePlayed(){
        game_started = false;
        setFloat("time_played", secondsPlayed);
        SavedValueManager.Singleton.parseSaveableData(gameConditionsDict,gameFloats,gameStrings);
    }

    public void setCondition(string condition, bool state)
    {
        gameConditionsDict[condition] = state;
        SavedValueManager.Singleton.parseSaveableData(gameConditionsDict,gameFloats,gameStrings);        
    }

    public void setFloat(string name, float value){
        gameFloats[name] = value; 
        SavedValueManager.Singleton.parseSaveableData(gameConditionsDict,gameFloats,gameStrings);
    }

    public float getFloat(string name, float defaultValue){
        try{
            return gameFloats[name];
        } catch{
            setFloat(name, defaultValue);
            return defaultValue;
        }
    }

    public void setString(string name, string value){
        gameStrings[name] = value;
        SavedValueManager.Singleton.parseSaveableData(gameConditionsDict,gameFloats,gameStrings);
    }

    public void setOnLeaveVariables(){
        setFloat("current_scene", currentRoom);
        setFloat("previous_scene", previousRoom);
        setFloat("time_played", secondsPlayed);

        currentRoom = 0;
        previousRoom = 0;
    }

    public void startNewGameFile(int fileIndex){
        SaveDataManager.Singleton.setSaveFile(fileIndex);
        SaveDataManager.Singleton.clearAllSaveData();
    }

    public void continueGameFile(int fileIndex){

        SaveDataManager.Singleton.setSaveFile(fileIndex);
        SavedValueManager.Singleton.load();
        gameConditionsDict = SavedValueManager.Singleton.getProgressionDict();
        gameFloats = SavedValueManager.Singleton.getFloatDict();
        gameStrings = SavedValueManager.Singleton.getStringDict();

        if(!gameConditionsDict.ContainsKey("game_started")){
            gameConditionsDict.Add("game_started",true);
            SavedValueManager.Singleton.parseSaveableData(gameConditionsDict,gameFloats,gameStrings);
        }

        game_started = true;
        
        secondsPlayed = getFloat("time_played", 0);
        currentRoom = (int)getFloat("current_scene", 0);
        previousRoom = (int)getFloat("previous_scene", 0);

        SceneTransitioner.Singleton.startGame(currentRoom);
    }
}
