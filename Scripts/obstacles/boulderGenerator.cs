using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boulderGenerator : MonoBehaviour
{
    [SerializeField] private float rate = 3f;
    [SerializeField] private GameObject boulderPrefab;
    [SerializeField] private float speed = 20f;
    [SerializeField] private bool intervalBased = true;
    [SerializeField] private SavedStateMachine _svMachine;
    private StateMachine generatorStates;

    private float rateRef;

    private bool disabled = false;
    private void Start()
    {
        rateRef = rate;

        generatorStates = new StateMachine("on", "off");
        _svMachine.loadLastSavedState(generatorStates);
    }
    public void spawnBoulder()
    {
        boulder b = Instantiate(boulderPrefab, transform.position, Quaternion.identity).GetComponent<boulder>();
        b.direction = (int)Mathf.Sign(transform.right.x);
        b.speed = speed;
    }

    private void Update()
    {
        if (generatorStates.currentState.Equals("off") || !intervalBased) { return; }
        if(rateRef <= 0)
        {
            spawnBoulder();
            rateRef = rate;
        }
        else
        {
            rateRef -= Time.deltaTime;
        }
    }

    public void toggleSpawner()
    {
        generatorStates.ChangeState(generatorStates.currentState.Equals("on") ? "off" : "on");
    }
}
