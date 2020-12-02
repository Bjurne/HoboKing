using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void Enter();
    void Execute();
    void Exit();
}

public interface IGameState : IState
{
    new void Enter();
    new void Execute();
    new void Exit();
}

public class DrawState : IGameState
{
    CardManager cardManager;
    int numberOfCardsToDraw;

    public DrawState(CardManager cardManager, int numberOfCardsToDraw)
    {
        this.cardManager = cardManager;
        this.numberOfCardsToDraw = numberOfCardsToDraw;
    }
    public void Enter()
    {
        cardManager.DrawCards(numberOfCardsToDraw);
    }

    public void Execute()
    {

    }

    public void Exit()
    {
    }
}

