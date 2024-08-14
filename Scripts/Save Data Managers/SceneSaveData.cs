using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.IO;
public class SceneSaveData : MonoBehaviour, ISaveData
{
    public static SceneSaveData Singleton;
    public SceneData sData = new SceneData();

    [SerializeField] private string areaName = "The Depths";

    private string mainSaveDataDirectory;

    public string saveDataDirectory{
        get{
            return mainSaveDataDirectory;
        }
        set{
            mainSaveDataDirectory = value;
        }
    }

    private void Awake() {
        sData.sceneStates = new List<stateData>();
        Singleton = this;
        saveDataDirectory = Path.Combine(SaveDataManager.Singleton.getRootFilePath(), SaveDataType.SceneData.ToString());

        load();
    }

    public void load(){
        string savedFilePath = Path.Combine(mainSaveDataDirectory, $"scene_{SceneManager.GetActiveScene().buildIndex}_state_data.json");
        
        string savedJson;
        try{
            savedJson = File.ReadAllText(savedFilePath);
            sData = JsonConvert.DeserializeObject<SceneData>(savedJson);
            SaveDataManager.debugMessage("Save Data Loaded");
        }
        catch{
            SaveDataManager.debugMessage("This scene has no previous save data, creating a new save data...", SaveDataManager.DebugType.Warning);
            save();
        }
        
        GameStateManager.Singleton.setString("last_area", areaName);
    }

    public void save(){
        setDirectoryRoot();
        string sceneStateData = JsonConvert.SerializeObject(sData);
        string savedFilePath = Path.Combine(mainSaveDataDirectory, $"scene_{SceneManager.GetActiveScene().buildIndex}_state_data.json");
        File.WriteAllText(savedFilePath, sceneStateData);
        SaveDataManager.debugMessage(savedFilePath);
    }

    public void clear(){
        sData = new SceneData();
        save();
    }

    public static void clearSceneDataDirectory(){
        string[] sceneStates = Directory.GetFiles(Path.Combine(SaveDataManager.Singleton.getRootFilePath(), SaveDataType.SceneData.ToString()));
        
        foreach(string s in sceneStates){
            File.Delete(s);
        }
    }

    public void setDirectoryRoot(){
        SaveDataManager.Singleton.getSaveDataTypeDirectory(this);
    }

    public stateData getStateMachineData(int saveID){
        return sData.getSavedState(saveID);
    }

    public SaveDataType getSaveDataType(){
        return SaveDataType.SceneData;
    }

    public void addSaveable(stateData _stateMachine){

        bool containsStateData = false;

        foreach(stateData st in sData.sceneStates){
            if(st.sID == _stateMachine.sID){
                containsStateData = true;
                break;
            }
        }

        if(!containsStateData){
            sData.sceneStates.Add(_stateMachine);
        }

        save();
    }

    public void updateSaveable(stateData state){
        foreach(stateData st in sData.sceneStates){
            if(st.sID == state.sID){
                st.savedState = state.savedState;
            }
        }
        save();
    }

    [System.Serializable]
    public class SceneData{
        public List<stateData> sceneStates;

        public stateData getSavedState(int savedID){

            foreach(stateData sd in sceneStates){
                if(sd.sID == savedID){
                    return sd;
                }
            }

            return null;
        }
    }
}

[System.Serializable]
public class stateData{
    public string savedState;
    public int sID;

    public stateData(string savedState, int sID){
        this.savedState = savedState;
        this.sID = sID;
    }
}
