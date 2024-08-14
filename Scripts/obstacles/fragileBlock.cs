using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fragileBlock : MonoBehaviour
{
    [SerializeField] private GameObject explodePref;
    [SerializeField] private SavedStateMachine _svMachine;

    private StateMachine fragileStates;

    private void Start() {
        fragileStates = new StateMachine("intact", "broken");
        _svMachine.loadLastSavedState(fragileStates);

        gameObject.SetActive(fragileStates.currentState.Equals("intact"));
    }
    public void destroyBlock(Vector2 source)
    {
        Vector2 dir = source - (Vector2)transform.position;

        

        if(dir.x * transform.right.x > 0)
        {
            destroyBlockUnconditional();
        }
        else if (dir.y * transform.right.y > 0)
        {
            destroyBlockUnconditional();
        }

       
    }

    public void destroyBlockUnconditional()
    {
        fragileStates.ChangeState("broken");
        Instantiate(explodePref);
        Destroy(gameObject);
    }
}
