using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour
{
    private static CardManager _instance;

    public static CardManager Instance { get { return _instance; } }


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
    }


    //[SerializeField] RectTransform handZone = default;
    //[SerializeField] RectTransform handPanel = default;
    [SerializeField] RectTransform fieldPanel = default;
    [SerializeField] RectTransform resolvingPanel = default;
    [SerializeField] RectTransform widgetsPanel = default;
    [SerializeField] CanvasGroup resolvingPanelUIControlls;

    [SerializeField] GameObject cardPrefab = default;

    private HandZoneWidget handZoneWidget => GameStateManager.Instance.CurrentPlayer.HandZoneWidget;

    private bool resolvingPanelIsHidden;
    private bool cardIsBeingDragged;
    public bool CardIsBeingDragged { get => cardIsBeingDragged; }
    public RectTransform WidgetsPanel { get => widgetsPanel; }

    public void RebuildHandLayout()
    {
        foreach (Card card in activeCards())
        {
            card.Shrink();
            //Todo fix so that cards dont leave screen on scale up
            if (handZoneWidget != null)
            {
                card.SetAnchorAndPivot(handZoneWidget.anchor, handZoneWidget.pivot);
            }
        }
        if (handZoneWidget != null)
        {
            //Debug.Log($"rebuilding {handZoneWidget.name}");
            handZoneWidget.RebuildLayout();
        }

        UpdateResolvingPanelUIControllsState();
    }

    private Card[] activeCards()
    {
        if (handZoneWidget.Rect == null)
            return null;
        var activeCards = handZoneWidget.GetComponentsInChildren<Card>();
        return activeCards;
    }

    internal void CardPickedUp(Card card)
    {
        cardIsBeingDragged = true;
        //card.gameObject.transform.SetParent(resolvingPanel);
    }

    public void CardReleased(Card card)
    {
        cardIsBeingDragged = false;

        Vector2 localMousePosition = handZoneWidget.Rect.InverseTransformPoint(Input.mousePosition);
        if (handZoneWidget.Rect.rect.Contains(localMousePosition) || CardInResolverPanel())
        {
            card.ReleasedInHand();

            return;
        }
        
        //card.gameObject.transform.SetParent(resolvingPanel);
        card.ReleasedOnField(resolvingPanel);

        AbilityResolver.Instance.AddAbilitySteps(card.cardAbility.playedAbilitySteps);
        AbilityResolver.Instance.CheatResolveNext();

        UpdateResolvingPanelUIControllsState();
        //RebuildHandLayout();
    }

    public void MarkerHoldingCardAbilitiesTargeted(Card card)
    {
        //card.gameObject.transform.SetParent(resolvingPanel);
        card.ReleasedOnField(resolvingPanel);

        AbilityResolver.Instance.AddAbilitySteps(card.cardAbility.targetedAbilitySteps);
        AbilityResolver.Instance.CheatResolveNext();

        UpdateResolvingPanelUIControllsState();
    }

    public void DebugDrawCard()
    {
        DrawCards(1);
    }

    internal void DrawCards(int numberOfCardsToDraw)
    {
        for (int i = 0; i < numberOfCardsToDraw; i++)
        {
            var cardData = GameStateManager.Instance.CurrentPlayer.DrawCard();

            if (cardData != null)
            {
                var newCard = Instantiate(cardPrefab, handZoneWidget.Rect).GetComponent<Card>();
                
                cardData.SetupData(newCard);
                newCard.SetupData(cardData);
            }
        }

        RebuildHandLayout();
    }

    public bool CardInResolverPanel()
    {
        if (resolvingPanel.GetComponentsInChildren<Card>().Length > 0)
            return true;
        else
            return false;
    }

    private void UpdateResolvingPanelUIControllsState()
    {
        //resolvingPanelUIControlls.gameObject.SetActive(CardInResolverPanel()); // TODO: Cheating
        resolvingPanelUIControlls.interactable = CardInResolverPanel();
        resolvingPanelUIControlls.alpha = CardInResolverPanel() ? 1f : 0f;
    }

    internal void HideUnhideResolverPanel()
    {
        if (resolvingPanelIsHidden)
            resolvingPanel.transform.Translate(Vector2.left * 500);
        else
            resolvingPanel.transform.Translate(Vector2.left * -500);

        resolvingPanelIsHidden = !resolvingPanelIsHidden;
    }
}
