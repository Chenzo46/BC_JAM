using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateMachine
{
    public List<string> states = new List<string>();
    public string currentState{get; private set;} = null;

    private List<string> stateHistory = new List<string>();

    public EventHandler OnStateChanged;

    public int saveID;

    public StateMachine(){
         //OnStateChanged = new EventHandler(this, EventArgs.Empty);
    }

    public StateMachine(params string[] defaultStates){
        AddStates(defaultStates);
    }

    public void AddState(string stateName){
        if(!states.Contains(stateName)){
            states.Add(stateName);
        }
        if(currentState == null && states.Count > 0){
            currentState = stateName;
            stateHistory.Add(currentState);
        }
    }

    public void AddStates(string[] stateNames){
        foreach(string stateName in stateNames){
            if(!states.Contains(stateName)){
                states.Add(stateName);
            }

            if(currentState == null && states.Count > 0){
                currentState = stateName;
            }

        }

    }

    public void ChangeState(string stateName){
        if(states.Contains(stateName)){
            stateHistory.Add(currentState);
            currentState = stateName;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void reverseState(){
        currentState = stateHistory[stateHistory.Count-1];
        stateHistory.Remove(stateHistory[stateHistory.Count-1]);
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool isState(string comparedState){
        return comparedState.Equals(currentState);
    }

    public stateData asStateData(){
        return new stateData(currentState, saveID);
    }
}
