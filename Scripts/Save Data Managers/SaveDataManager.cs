using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening.Plugins;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    //public List<ISaveData> saveDataInterfaces = new List<ISaveData>();
    public static bool showDebugMessages = false;

    private List<SaveFile> saveFiles = new List<SaveFile>();
    private SaveFile currentSaveFile;

    public static string saveDataPath;

    public static SaveDataManager Singleton;

    private void OnEnable() {
        if(Singleton == null){
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
        createSaveDataPath();
        initalizeSaveFiles();
    }

    public void loadAll(){
        
    }

    public void saveAll(){
        
    }

    private void initalizeSaveFiles(){
        for(int idx = 0; idx < 3; idx++){
            SaveFile sFile = new SaveFile();
            sFile.rootPath = Path.Combine(saveDataPath, $"file_{idx}");

            try{
                Directory.CreateDirectory(sFile.rootPath);
                sFile.noData = true;
            } catch{
                sFile.noData = false;
            }

            for(int jdx = 0; jdx < 3; jdx++){
                string path = Path.Combine(sFile.rootPath, ((SaveDataType)jdx).ToString());

                try{
                    Directory.CreateDirectory(path);
                }
                catch{
                    // f
                }
            }

            saveFiles.Add(sFile);
        }
    }

    public void createSaveDataPath(){

        string path = Path.Combine(Application.persistentDataPath, "BC_SAVE_DATA");

        saveDataPath = path;

        try{
            Directory.CreateDirectory(path);
        } 
        catch{
            debugMessage("Save data path directory has already been created", DebugType.Warning);
        }
    }
    public void getSaveDataTypeDirectory(ISaveData sData){

        string typeName = sData.getSaveDataType().ToString();
        string _path = "";
        try{
            _path = Path.Combine(currentSaveFile.rootPath, typeName);
        } catch{ // If no current save file exists then set it to the first save file
            currentSaveFile = saveFiles[0];
            _path = Path.Combine(currentSaveFile.rootPath, typeName);
        }
        
        sData.saveDataDirectory = _path;
        //Debug.Log(_path);
    }

    public void setSaveFile(int fileIndex){
        currentSaveFile = saveFiles[fileIndex];
        
        currentSaveFile.rootPath = Path.Combine(saveDataPath, $"file_{fileIndex}");
    }

    public string getRootFilePath(){

        debugMessage(currentSaveFile.rootPath);

        if(currentSaveFile.rootPath == null){
            currentSaveFile = saveFiles[0];
        }

        return currentSaveFile.rootPath;
    }

    public void clearAllSaveData(){
        for(int idx = 0; idx < 3; idx++){
            string savePath = Path.Combine(getRootFilePath(), ((SaveDataType)idx).ToString());
            string[] jsonPaths = Directory.GetFiles(savePath);

            foreach(string s in jsonPaths){
                File.Delete(s);
            }
        }
    }

    public bool saveFileHasData(int saveFileIndex){
        return saveFiles[saveFileIndex].noData;
    }

    public static void debugMessage(string message, DebugType debugType){
        if(!showDebugMessages){ return; }
        switch(debugType){
            case DebugType.Message:
                Debug.Log(message);
                break;
            case DebugType.Warning:
                Debug.LogWarning(message);
                break;
        }
    }

    public static void debugMessage(string message){
        if(!showDebugMessages){ return; }
        Debug.Log(message);
    }

    public enum DebugType{
        Warning,
        Message
    }

    

}

public interface ISaveData{
    public string saveDataDirectory 
    {
        get;
        set;
    }
    public void load();
    public void save();

    public SaveDataType getSaveDataType();

    public void setDirectoryRoot();
}

public enum SaveDataType{
    SceneData,
    InventoryData,
    ProgressionData,
}

public struct SaveFile{
    public string rootPath;
    public bool noData;
}

