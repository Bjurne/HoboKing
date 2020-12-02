using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityResolver : MonoBehaviour
{
    private static AbilityResolver _instance;

    public static AbilityResolver Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        abilityStack = new List<ICardAbilityStep>();
        stepMachine = new StepMachine();
    }

    
    private List<ICardAbilityStep> abilityStack;
    private bool isWaitingForTarget;
    private ITargetable selectedTarget;
    internal ITargetable Target { get => selectedTarget; }
    internal StepMachine stepMachine;

    private Type awaitedTargetType;
    private TargetableStatus awaitedTargetableStatus;

    public void Subscribe(GameplayActionButton btn)
    {
        btn.OnGameplayActionEvent += OnGameplayActionEventBroadcast;
    }

    public void Unsubscribe(GameplayActionButton btn)
    {
        btn.OnGameplayActionEvent -= OnGameplayActionEventBroadcast;
    }

    // TODO: Fix interface / inheritance for ActionButtons, instead of using separate functions for GamePlayActions and MenuActions
    public void SubscribeMenuAction(MenuActionButton btn)
    {
        btn.OnMenuActionEvent += OnMenuActionEventBroadcast;
    }

    public void UnsubscribeMenuAction (MenuActionButton btn)
    {
        btn.OnMenuActionEvent -= OnMenuActionEventBroadcast;
    }

    internal void AddAbilitySteps(List<ICardAbilityStep> abilitySteps)
    {
        if (abilitySteps[abilitySteps.Count - 1].GetType() == typeof(CardFinishedResolving))
            abilitySteps.Reverse();
        foreach (ICardAbilityStep step in abilitySteps)
        {
            abilityStack.Add(step);
        }
    }

    internal void ClearAllAbilitySteps()
    {
        abilityStack.Clear();
        ClearTarget();
        stepMachine = new StepMachine(); // TODO: Cheating af here.
    }

    private void ResolveNextStep()
    {
        if (abilityStack.Count > 0)
        {
            stepMachine.ChangeStep(abilityStack[abilityStack.Count - 1]);
            abilityStack.RemoveAt(abilityStack.Count - 1);
        }
        else if (stepMachine.currentlyRunningStep != null)
        {
            stepMachine.currentlyRunningStep.Exit();
        }
    }

    private void Update()
    {
        stepMachine.ExecuteStepUpdate();
    }

    public void CheatResolveNext()
    {
        ResolveNextStep();
    }

    private void OnGameplayActionEventBroadcast(GameplayActions key, GameObject triggeringObject = null)
    {
        if (isWaitingForTarget)
        {
            ITargetable target = triggeringObject.GetComponent<ITargetable>();
            if (target != null && awaitedTargetType == target.GetType() && target.TargetableStatus.IsMatch(awaitedTargetableStatus))
            {
                isWaitingForTarget = false;
                selectedTarget = target;
                //int neighbours = target.Neighbours.Count;
                //Debug.Log($"SELECTED TARGET {selectedTarget}{neighbours} {awaitedTargetType} {awaitedTargetableStatus.isActive} {awaitedTargetableStatus.isOccupied}");
            }
            else
            {
                Debug.LogWarning($"Unexpected Target");
            }
        }
        //else if (key == GameplayActions.MarkerTargeted)
        //{
        //    if (CardManager.Instance.CardInResolverPanel())
        //        return;

        //    var card = triggeringObject.GetComponent<Marker>().card;
        //    if (card != null && GameStateManager.Instance.CurrentPlayer == card.owningPlayer && card.HasTargetedAbilities)
        //    {
        //        card.MarkerTargeted();
        //    }
        //    var marker = triggeringObject.GetComponent<Marker>();
        //    if (marker != null)
        //        marker.SetIsHoldingAbilities(card.HasTargetedAbilities);
        //}
        switch (key)
        {
            case GameplayActions.None:
                break;
            case GameplayActions.TileTargeted:
                break;
            case GameplayActions.NodeTargeted:
                break;
            case GameplayActions.MarkerTargeted:
                if (CardManager.Instance.CardInResolverPanel())
                    return;

                var card = triggeringObject.GetComponent<Marker>().card;
                if (card != null && GameStateManager.Instance.CurrentPlayer == card.owningPlayer && card.HasTargetedAbilities)
                {
                    card.MarkerTargeted();
                }
                var marker = triggeringObject.GetComponent<Marker>();
                if (marker != null)
                    marker.SetIsHoldingAbilities(card.HasTargetedAbilities);
                break;
            case GameplayActions.CancelResolvingCard:
                if (CardManager.Instance.CardInResolverPanel() == false)
                    return;

                Debug.Log($"CANCEL");
                transform.Find("Field Panel").transform.Find("Resolving Panel").GetComponentInChildren<Card>().ReleasedInHand();
                ClearAllAbilitySteps();
                break;
            default:
                break;
        }
    }

    private void OnMenuActionEventBroadcast(MenuActions key)
    {
        switch (key)
        {
            case MenuActions.None:
                break;
            case MenuActions.HideUnhideResolverPanel:
                Debug.Log($"KLICK");
                CardManager.Instance.HideUnhideResolverPanel();
                break;
            case MenuActions.RotateHandZoneWidget:
                Debug.Log($"Rotate");
                var widgetRect = GameStateManager.Instance.CurrentPlayer.HandZoneWidget.Rect;
                //var widgetPreviousPivotPreset = (PivotPresets)(int)RectTransformExtensions.GetAnchor(widgetRect);
                //widgetRect.SetPivot(PivotPresets.MiddleCenter);

                iTween.RotateBy(widgetRect.gameObject, new Vector3(0f, 0f, -0.25f), 0.2f);

                //widgetRect.SetPivot(widgetPreviousPivotPreset);
                break;
            default:
                break;
        }
    }

    internal ITargetable CheckForTarget<T>(TargetableStatus awaitedTargetableStatus)
    {
        awaitedTargetType = typeof(T);
        this.awaitedTargetableStatus = awaitedTargetableStatus;
        isWaitingForTarget = true;
        return selectedTarget;
    }

    internal void ClearTarget()
    {
        isWaitingForTarget = false;
        awaitedTargetableStatus = null;
        awaitedTargetType = null;
        selectedTarget = null;
    }
}


