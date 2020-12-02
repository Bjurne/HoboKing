using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuActionButton : MonoBehaviour, IPointerClickHandler, ITargetable
{
    public MenuActions menuAction;
    //public bool indexedGameplayAction = false;
    //public Vector2 btnIndex;
    public Action<MenuActions> OnMenuActionEvent;
    private TargetableStatus targetableStatus = new TargetableStatus();

    public void OnPointerClick(PointerEventData eventData)
    {
        MenuActionEventTriggered();
    }

    private void MenuActionEventTriggered()
    {
        OnMenuActionEvent.Invoke(menuAction);
    }

    private void OnEnable()
    {
        //SetInteractable(true);
        StartCoroutine(WaitForSetupCompletion(true));
    }

    private void OnDisable()
    {
        SetInteractable(false);
        //StartCoroutine(WaitForSetupCompletion(false));
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
            AbilityResolver.Instance.SubscribeMenuAction(this);
        else
            AbilityResolver.Instance.UnsubscribeMenuAction(this);
    }

    public TargetableStatus TargetableStatus { get => targetableStatus; }

    public List<ITargetable> Neighbours => throw new NotImplementedException(); // TODO: Do MenuActionButton need neighbours?
}

public enum MenuActions
{
    None,
    HideUnhideResolverPanel,
    RotateHandZoneWidget,
}