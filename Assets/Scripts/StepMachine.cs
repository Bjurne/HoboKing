using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepMachine
{
    public ICardAbilityStep currentlyRunningStep;
    public ICardAbilityStep previouslyRunningStep;


    public void ChangeStep(ICardAbilityStep newStep)
    {
        if (currentlyRunningStep != null)
        {
            currentlyRunningStep.Exit();
        }

        previouslyRunningStep = currentlyRunningStep;

        currentlyRunningStep = newStep;
        currentlyRunningStep.Enter();
    }

    public void ExecuteStepUpdate()
    {
        ICardAbilityStep runningStep = currentlyRunningStep;
        if (runningStep != null)
        {
            runningStep.Execute();
        }
    }

    public void SwitchToPreviousStep()
    {
        currentlyRunningStep.Exit();
        currentlyRunningStep = previouslyRunningStep;
        currentlyRunningStep.Enter();
    }
}
