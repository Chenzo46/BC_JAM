using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;

public class SavedValueManager : MonoBehaviour, ISaveData
{
    private SaveableCollection progressionData = new SaveableCollection();
    private string mainSaveDataDirectory;

    public string saveDataDirectory{
        get{
            return mainSaveDataDirectory;
        }
        set{
            mainSaveDataDirectory = value;
        }
    }

    public static SavedValueManager Singleton;

    private void Awake() {
        Singleton = this;
    }

    public void save(){
        saveDataDirectory = getDirectoryRoot();
        string path = Path.Combine(mainSaveDataDirectory, "progression.json");
        string jsonData = JsonUtility.ToJson(progressionData, true);
        File.WriteAllText(path,jsonData);
    }

    public void load(){
        saveDataDirectory = getDirectoryRoot();
        string path = Path.Combine(mainSaveDataDirectory, "progression.json");
        //Debug.Log(path);

        try{
            string collectionJsonData = File.ReadAllText(path);
            SaveableCollection sc =  JsonUtility.FromJson<SaveableCollection>(collectionJsonData);
            progressionData = sc;
        } catch{
            SaveDataManager.debugMessage("No Saveable Data manager found, creating a new one...");
            save();
        }

        
    }

    public void setDirectoryRoot(){}

    public string getDirectoryRoot(){
        return Path.Combine(SaveDataManager.Singleton.getRootFilePath(), getSaveDataType().ToString());
    }


    public void parseSaveableData(Dictionary<string,bool> bools, Dictionary<string,float> floats, Dictionary<string,string> strings){
        progressionData = new SaveableCollection();
        //Add Bools
        foreach(KeyValuePair<string, bool> ab in bools){
            SaveableProgression<bool> prog = new SaveableProgression<bool>
            {
                Value1 = ab.Key,
                Value2 = ab.Value
            };
            progressionData.addBool(prog);
        }
        //Add floats
        foreach(KeyValuePair<string, float> ab in floats){
            SaveableProgression<float> prog = new SaveableProgression<float>
            {
                Value1 = ab.Key,
                Value2 = ab.Value
            };
            progressionData.addFloat(prog);
        }
        //Add strings
        foreach(KeyValuePair<string, string> ab in strings){
            SaveableProgression<string> prog = new SaveableProgression<string>
            {
                Value1 = ab.Key,
                Value2 = ab.Value
            };
            progressionData.addString(prog);
        }
        save();
    }

    public Dictionary<string, bool> getProgressionDict(){
        Dictionary<string, bool> _nProgDict = new Dictionary<string, bool>();

        foreach(SaveableProgression<bool> ab in progressionData.savedBools){
            _nProgDict.Add(ab.Value1, ab.Value2);
        }

        return _nProgDict;
    }

    public Dictionary<string, float> getFloatDict(){
        Dictionary<string, float> _nProgDict = new Dictionary<string, float>();

        foreach(SaveableProgression<float> ab in progressionData.savedFloats){
            _nProgDict.Add(ab.Value1, ab.Value2);
        }

        return _nProgDict;
    }
    public Dictionary<string, string> getStringDict(){
        Dictionary<string, string> _nProgDict = new Dictionary<string, string>();

        foreach(SaveableProgression<string> ab in progressionData.savedStrings){
            _nProgDict.Add(ab.Value1, ab.Value2);
        }

        return _nProgDict;
    }

    public SaveDataType getSaveDataType(){
        return SaveDataType.ProgressionData;
    }

    public bool getDirectData(string path, string key){
        try{
            string fullPath = Path.Combine(path, "progression.json");
            //Debug.Log(fullPath);
            string collectionJsonData = File.ReadAllText(fullPath);
            SaveableCollection sc =  JsonUtility.FromJson<SaveableCollection>(collectionJsonData);

            //Search in Bools
            foreach(SaveableProgression<bool> sp in sc.savedBools){
                if(sp.Value1.Equals(key)){
                    return sp.Value2;
                }
            }
        } catch{
            return false;
        }

        return false;
    }

    public float? getDirectFloat(string path, string key){
        try{
            string fullPath = Path.Combine(path, "progression.json");
            //Debug.Log(fullPath);
            string collectionJsonData = File.ReadAllText(fullPath);
            SaveableCollection sc =  JsonUtility.FromJson<SaveableCollection>(collectionJsonData);
            //Search in Floats
            foreach(SaveableProgression<float> sp in sc.savedFloats){
                if(sp.Value1.Equals(key)){
                    return sp.Value2;
                }
            }
        } catch{
            return null;
        }

        return null;
    }

    public string getDirectString(string path, string key){
        try{
            string fullPath = Path.Combine(path, "progression.json");
            //Debug.Log(fullPath);
            string collectionJsonData = File.ReadAllText(fullPath);
            SaveableCollection sc =  JsonUtility.FromJson<SaveableCollection>(collectionJsonData);

            //Search in Strings
            foreach(SaveableProgression<string> sp in sc.savedStrings){
                if(sp.Value1.Equals(key)){
                    return sp.Value2;
                }
            }
        } catch{
            return null;
        }

        return null;
    }
}

[Serializable]
public class SaveableCollection{
    public List<SaveableProgression<bool>> savedBools = new List<SaveableProgression<bool>>();
    public List<SaveableProgression<float>> savedFloats = new List<SaveableProgression<float>>();
    public List<SaveableProgression<string>> savedStrings = new List<SaveableProgression<string>>();
    
    public SaveableCollection(){
        
    }

    public void addBool(SaveableProgression<bool> dataToAdd){
        savedBools.Add(dataToAdd);
    }
    public void addFloat(SaveableProgression<float> dataToAdd){
        savedFloats.Add(dataToAdd);
    }

    public void addString(SaveableProgression<string> dataToAdd){
        savedStrings.Add(dataToAdd);
    }
}

[Serializable]
public struct SaveableProgression<T>{

    public string Value1;
    public T Value2;

}

[Serializable]
public class SaveableData<A>{
    public A Value;

    public SaveableData(A Value){
        this.Value = Value;
    }
}
