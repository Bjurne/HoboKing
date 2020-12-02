using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T : IState
{
    public T currentlyRunningState;
    public T previouslyRunningState;


    public void ChangeState(T newState)
    {
        if (currentlyRunningState != null)
        {
            currentlyRunningState.Exit();
        }

        previouslyRunningState = currentlyRunningState;

        currentlyRunningState = newState;
        currentlyRunningState.Enter();
    }

    public void ExecuteStateUpdate()
    {
        IState runningState = currentlyRunningState;
        if (runningState != null)
        {
            runningState.Execute();
        }
    }

    public void SwitchToPreviousState()
    {
        currentlyRunningState.Exit();
        currentlyRunningState = previouslyRunningState;
        currentlyRunningState.Enter();
    }
}
