using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GameplayActionButton : MonoBehaviour, IPointerClickHandler, ITargetable
{
    public GameplayActions gameplayAction;
    //public bool indexedGameplayAction = false;
    //public Vector2 btnIndex;
    public Action<GameplayActions, GameObject> OnGameplayActionEvent;
    private TargetableStatus targetableStatus = new TargetableStatus();

    public void OnPointerClick(PointerEventData eventData)
    {
        GameplayActionEventTriggered();
    }

    private void GameplayActionEventTriggered()
    {
        OnGameplayActionEvent.Invoke(gameplayAction, gameObject);
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForSetupCompletion(true));
    }

    private void OnDisable()
    {
        SetInteractable(false);
    }

    private IEnumerator WaitForSetupCompletion(bool value)
    {
        while (AbilityResolver.Instance == null)
        {
            yield return new WaitForEndOfFrame();
        }

        SetInteractable(value);
    }

    internal void SetInteractable(bool value)
    {
        if (value)
            AbilityResolver.Instance.Subscribe(this);
        else
            AbilityResolver.Instance.Unsubscribe(this);
    }

    public TargetableStatus TargetableStatus { get => targetableStatus; }

    public List<ITargetable> Neighbours => throw new NotImplementedException(); // TODO: Do GameplayActionButton need neighbours?
}

public enum GameplayActions
{
    None,
    TileTargeted,
    NodeTargeted,
    MarkerTargeted,
    CancelResolvingCard,
}
