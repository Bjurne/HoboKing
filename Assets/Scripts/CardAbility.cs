using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum AbilityTarget
{
    None,
    Tile,
    Node,
    EnemyMarker,
    OwnMarker,
}

[System.Serializable]
public enum AbilityStep
{
    ChooseTarget,
    AddMarker,
    RemoveMarker,
    CardFinishedResolving,
    BreachBarriers,
    AddHalfMarker,
    Idle,
}

[System.Serializable]
public class CardAbility
{
    public string cardAbilityName;
    public AbilityTarget abilityTarget;
    public List<ICardAbilityStep> playedAbilitySteps;
    public List<ICardAbilityStep> targetedAbilitySteps;

    public CardAbility(List<ICardAbilityStep> playedAbilitySteps, List<ICardAbilityStep> targetedAbilitySteps)
    {
        this.playedAbilitySteps = playedAbilitySteps;
        this.targetedAbilitySteps = targetedAbilitySteps;
    }
}

public interface ICardAbilityStep : IState
{
    AbilityStep step { get; }
    new void Enter();
    new void Execute();
    new void Exit();
}

public class AddMarker : ICardAbilityStep
{
    public AddMarker(AbilityStep step, GameObject markerPrefab, bool reuseTarget = false, Card card = null)
    {
        thisStep = step;
        this.markerPrefab = markerPrefab;
        this.reuseTarget = reuseTarget;
        this.card = card;
    }

    public AbilityStep step { get => thisStep; }
    private AbilityStep thisStep;
    private ITargetable target;
    private GameObject markerPrefab = default;
    private bool reuseTarget;
    private Card card;
    private TargetableStatus allowedTargetableStatus = new TargetableStatus(true, false);

    public void Enter()
    {
        //Debug.Log($"{step} : wait for target tile");
    }

    public void Execute()
    {
        if (Time.frameCount % 5 == 0)
        {
            //Debug.Log($"{step} : waiting");
            //if (AbilityResolver.Instance.CheckForTarget<ArenaTile>(allowedTargetableStatus) != null)
            //{
            //    target = AbilityResolver.Instance.CheckForTarget<ArenaTile>(allowedTargetableStatus) as ArenaTile;
            //    AbilityResolver.Instance.CheatResolveNext();
            //}

            target = AbilityResolver.Instance.Target as ITargetable;

            if (target == null)
            {
                if (AbilityResolver.Instance.CheckForTarget<ArenaTile>(allowedTargetableStatus) != null)
                {
                    target = AbilityResolver.Instance.CheckForTarget<ArenaTile>(allowedTargetableStatus) as ArenaTile;
                    AbilityResolver.Instance.CheatResolveNext();
                }
            }
            else
            {
                if (target.GetType() == typeof(ArenaNode))
                {
                    var node = target as ArenaNode;
                    target = node.OccupyingArenaTile;
                }
                else if (target.GetType() == typeof(Marker))
                {
                    var marker = target as Marker;
                    target = marker.ParentTile;
                }
                //Debug.Log($"Target of {step}: {target}{target.Index.Index}");
                AbilityResolver.Instance.CheatResolveNext();
            }
        }
    }

    public void Exit()
    {
        //if (target.GetType() == typeof(ArenaNode))
        //{
        //    var node = target as ArenaNode;
        //    target = node.OccupyingArenaTile;
        //}

        var targetTile = target as ArenaTile;

        //var newMarker = GameObject.Instantiate(markerPrefab, targetTile.transform, false).GetComponent<Marker>();
        //newMarker.SetPlayerProperties(GameStateManager.Instance.CurrentPlayer);
        //newMarker.SetNewIndex(targetTile.Index, targetTile);
        //Debug.Log($"{step} : tile {target.Index} selected, placing marker and clean up");
        //targetTile.TargetableStatus.isOccupied = true;
        targetTile.AddOccupyingMarker(markerPrefab, card);

        if (card.HasTargetedAbilities)
        {
            targetTile.OccupyingMarker.GetComponent<Marker>().SetIsHoldingAbilities(true);
        }

        if (!reuseTarget)
        {
            AbilityResolver.Instance.ClearTarget();
        }
    }
}

public class AddHalfMarker : ICardAbilityStep
{
    public AddHalfMarker(AbilityStep step, GameObject halfMarkerPrefab, GameObject fullMarkerPrefab, bool reuseTarget = false)
    {
        thisStep = step;
        this.halfMarkerPrefab = halfMarkerPrefab;
        this.fullMarkerPrefab = fullMarkerPrefab;
        this.reuseTarget = reuseTarget;
    }

    public AbilityStep step { get => thisStep; }
    private AbilityStep thisStep;
    private ArenaTile target;
    private GameObject markerPrefab = default;
    private bool reuseTarget;
    private TargetableStatus allowedTargetableStatus = new TargetableStatus(true, false);
    private GameObject fullMarkerPrefab;
    private GameObject halfMarkerPrefab;

    public void Enter()
    {
        //Debug.Log($"{step} : wait for target tile");
    }

    public void Execute()
    {
        if (Time.frameCount % 5 == 0)
        {
            //Debug.Log($"{step} : waiting");
            if (AbilityResolver.Instance.CheckForTarget<ArenaTile>(allowedTargetableStatus) != null)
            {
                target = AbilityResolver.Instance.CheckForTarget<ArenaTile>(allowedTargetableStatus) as ArenaTile;
                AbilityResolver.Instance.CheatResolveNext();
            }
        }
    }

    public void Exit()
    {
        var playerIndex = GameStateManager.Instance.CurrentPlayer.PlayerIndex;
        //Debug.Log($"{target.IsHalfMarkedByPlayer(playerIndex)}");
        markerPrefab = target.IsHalfMarkedByPlayer(playerIndex) ? fullMarkerPrefab : halfMarkerPrefab;

        if (!target.IsHalfMarkedByPlayer(playerIndex))
            target.SetIsHalfMarkedByPlayer(playerIndex);

        target.RemoveOccupyingMarker();
        target.AddOccupyingMarker(markerPrefab);

        //var newMarker = GameObject.Instantiate(markerPrefab, target.transform, false).GetComponent<Marker>();
        //newMarker.SetPlayerProperties(GameStateManager.Instance.CurrentPlayer);
        //newMarker.SetNewIndex(target.Index, target);

        //Debug.Log($"{step} : tile {target.Index} selected, placing marker and clean up");

        if (!reuseTarget)
        {
            AbilityResolver.Instance.ClearTarget();
        }
    }
}

public class BreachBarriers : ICardAbilityStep
{
    public BreachBarriers(AbilityStep step, bool reuseTarget = false)
    {
        thisStep = step;
        this.reuseTarget = reuseTarget;
    }

    public AbilityStep step { get => thisStep; }
    private AbilityStep thisStep;
    private bool reuseTarget;
    private ITargetable target;
    private TargetableStatus allowedTargetableStatus = new TargetableStatus(false, false);


    public void Enter()
    {
        //Debug.Log($"{step} : wait for target tile");
    }

    public void Execute()
    {
        if (Time.frameCount % 5 == 0)
        {
            target = AbilityResolver.Instance.Target as ITargetable;
            
            if (target == null)
            {
                if (AbilityResolver.Instance.CheckForTarget<ArenaNode>(allowedTargetableStatus) != null)
                {
                    target = AbilityResolver.Instance.CheckForTarget<ArenaNode>(allowedTargetableStatus) as ArenaNode;
                    AbilityResolver.Instance.CheatResolveNext();
                }
            }
            else
            {
                if (target.GetType() == typeof(ArenaTile))
                {
                    var tile = target as ArenaTile;
                    target = tile.parentArenaNode;
                }
                //Debug.Log($"Target of {step}: {target}{target.Index.Index}");
                AbilityResolver.Instance.CheatResolveNext();
            }
            //Debug.Log($"{step} : waiting");
        }
    }

    public void Exit()
    {
        var inactiveNeighbourNodes = new List<ITargetable>();
        var arena = GameLocalization.Instance.ActiveArena;
        var NodeItselfAndItsNeighbours = target.Neighbours;
        NodeItselfAndItsNeighbours.Add(target);
        foreach (ITargetable neighbour in NodeItselfAndItsNeighbours)
        {
            //Debug.Log($"Neighbour {Array.IndexOf(target.parentArenaNode.neighbourArenaNodes, neighbour)}{neighbour.Index.Index} isActive: {neighbour.TargetableStatus.isActive}, Neighbour isOccupied: {neighbour.TargetableStatus.isOccupied}");
            if (!neighbour.TargetableStatus.isActive && !neighbour.TargetableStatus.isOccupied)
            {
                inactiveNeighbourNodes.Add(neighbour);
            }
        }
        for (int i = 0; i < inactiveNeighbourNodes.Count; i++)
        {
            arena.AddTile(inactiveNeighbourNodes[i] as ArenaNode);
        }

        if (!reuseTarget)
            AbilityResolver.Instance.ClearTarget();
    }
}

public class RemoveMarker : ICardAbilityStep
{
    public RemoveMarker(AbilityStep step, bool reuseTarget = false)
    {
        thisStep = step;
        this.reuseTarget = reuseTarget;
    }

    public AbilityStep step { get => thisStep; }
    private AbilityStep thisStep;
    private ITargetable target;
    private bool reuseTarget;
    private TargetableStatus allowedTargetableStatus = new TargetableStatus(true, false);

    public void Enter()
    {
        //Debug.Log($"{step} : wait for target tile");
    }

    public void Execute()
    {
        if (Time.frameCount % 5 == 0)
        {
            //Debug.Log($"{step} : waiting");
            //if (AbilityResolver.Instance.CheckForTarget<ArenaTile>(allowedTargetableStatus) != null)
            //{
            //    target = AbilityResolver.Instance.CheckForTarget<ArenaTile>(allowedTargetableStatus) as ArenaTile;
            //    AbilityResolver.Instance.CheatResolveNext();
            //}

            target = AbilityResolver.Instance.Target as ITargetable;

            if (target == null)
            {
                if (AbilityResolver.Instance.CheckForTarget<Marker>(allowedTargetableStatus) != null)
                {
                    target = AbilityResolver.Instance.CheckForTarget<Marker>(allowedTargetableStatus) as Marker;
                    AbilityResolver.Instance.CheatResolveNext();
                }
            }
            else
            {
                if (target.GetType() == typeof(ArenaNode))
                {
                    var node = target as ArenaNode;
                    target = node.OccupyingArenaTile.OccupyingMarker.GetComponent<ITargetable>();
                }
                else if (target.GetType() == typeof(ArenaTile))
                {
                    var tile = target as ArenaTile;
                    target = tile.OccupyingMarker.GetComponent<ITargetable>();
                }
                //Debug.Log($"Target of {step}: {target}{target.Index.Index}");
                AbilityResolver.Instance.CheatResolveNext();
            }
        }
    }

    public void Exit()
    {
        //if (target.GetType() == typeof(ArenaNode))
        //{
        //    var node = target as ArenaNode;
        //    target = node.OccupyingArenaTile;
        //}

        var targetMarker = target as Marker;

        //var newMarker = GameObject.Instantiate(markerPrefab, targetTile.transform, false).GetComponent<Marker>();
        //newMarker.SetPlayerProperties(GameStateManager.Instance.CurrentPlayer);
        //newMarker.SetNewIndex(targetTile.Index, targetTile);
        //Debug.Log($"{step} : tile {target.Index} selected, placing marker and clean up");
        //targetTile.TargetableStatus.isOccupied = true;
        targetMarker.DestroyMarker();

        if (!reuseTarget)
        {
            AbilityResolver.Instance.ClearTarget();
        }
    }
}

public class CardFinishedResolving : ICardAbilityStep
{
    public CardFinishedResolving(AbilityStep step, Card card)
    {
        thisStep = step;
        this.card = card;
    }
    public AbilityStep step { get => thisStep; }
    private AbilityStep thisStep;
    private Card card;


    public void Enter()
    {
        Debug.Log($"{step} : finished resolving, removing card");
        if (card.HasTargetedAbilities && !card.HasBeenPlayed)
            card.gameObject.SetActive(false);
        else
            MonoBehaviour.Destroy(card.gameObject);
        GameStateManager.Instance.TurnPassed();
        CardManager.Instance.RebuildHandLayout();
        AbilityResolver.Instance.ClearTarget();
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
    }
}

public class StepMachineIdle : ICardAbilityStep
{
    public StepMachineIdle(AbilityStep step)
    {
        thisStep = step;
    }

    public AbilityStep step { get => thisStep; }
    private AbilityStep thisStep;

    public void Enter()
    {
        Debug.Log($"stepMachine idle");
    }

    public void Execute()
    {
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"{step} : idling");
        }
    }

    public void Exit()
    {
        Debug.Log($"stepMachine woke up");
    }
}
