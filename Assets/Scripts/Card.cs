using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerEnterHandler, IPointerExitHandler, IEndDragHandler
{
    [SerializeField] RectTransform cardRect = default;
    [SerializeField] float smallScale = default;
    [SerializeField] float bigScale = default;
    [SerializeField] AnchorPresets anchoredEdge;
    [SerializeField] TMPro.TextMeshProUGUI fieldDescriptions = default;
    [SerializeField] TMPro.TextMeshProUGUI fieldCardName = default;
    [SerializeField] Image cardImage = default;
    [SerializeField] Image playerSymbolImage = default;

    private Canvas canvas = default;
    private Camera mainCamera;
    private Vector2 positionInHand;
    private CardManager cardManager => CardManager.Instance;
    private bool cardHasBeenPickedUp = false;
    private int childIndexInWidget;

    [SerializeField] CardData cardData = default;

    internal CardAbility cardAbility;
    private string[] cardDescriptions;
    private Sprite cardSprite;
    internal Player owningPlayer { get; private set; }

    private bool hasTargetedAbilities;
    private bool hasBeenPlayed;
    private Vector2 velocity;

    public bool HasTargetedAbilities { get => hasTargetedAbilities; }
    public bool HasBeenPlayed { get => hasBeenPlayed; }

    internal void Start()
    {
        //SetupData();
        //cardData.SetupData(this);
        mainCamera = Camera.main;
        //cardManager = GetComponentInParent<CardManager>();
        canvas = GetComponentInParent<Canvas>().rootCanvas;
        Shrink();
        //cardRect.SetAnchor(AnchorPresets.MiddleCenter);
        cardRect.SetPivot(PivotPresets.TopLeft);
        //playerSymbolImage.sprite = owningPlayer.SymbolSprite;
        //playerSymbolImage.color = owningPlayer.MainColor;
    }

    public void SetupData(CardData newCardData = null)
    {
        if (newCardData != null)
            cardData = newCardData;

        cardDescriptions = cardData.cardDescriptions;
        cardSprite = cardData.cardSprite;
        
        cardImage.sprite = cardSprite;
        fieldCardName.text = cardData.cardName;
        cardAbility = cardData.cardAbility;
        owningPlayer = GameStateManager.Instance.CurrentPlayer;
        hasTargetedAbilities = cardAbility.targetedAbilitySteps.Count > 0;

        UpdateCardDescriptions(cardDescriptions);
    }

    private void UpdateCardDescriptions(string[] newDescriptions = null)
    {
        string fullDescription = string.Empty;

        // TODO: Go back to using strings, or a library to find appropriate descriptions based on ability steps
        //foreach (string str in newDescriptions)
        //    fullDescription = ($"{fullDescription} \n {str}");

        foreach (ICardAbilityStep step in cardAbility.playedAbilitySteps)
        {
            if (step.step == AbilityStep.CardFinishedResolving)
                fullDescription = ($"{fullDescription} \n");
            else
                fullDescription = ($"{fullDescription} \n {step.step}");
        }

        if (hasTargetedAbilities)
        {
            if (hasBeenPlayed)
                fullDescription = "";
            else
                fullDescription += $" \n FLIPSIDE:";

            foreach (ICardAbilityStep step in cardAbility.targetedAbilitySteps)
            {
                if (step.step == AbilityStep.CardFinishedResolving)
                    fullDescription = ($"{fullDescription} \n");
                else
                    fullDescription = ($"{fullDescription} \n {step.step}");
            }
        }

        fieldDescriptions.text = fullDescription;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        cardRect.SetPivot(PivotPresets.MiddleCenter);

        cardRect.localScale = Vector3.one * bigScale;
        cardHasBeenPickedUp = true;
        cardManager.CardPickedUp(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //cardRect.anchoredPosition += eventData.delta / canvas.scaleFactor;
        cardRect.position = KeepOnScreen((Vector2)cardRect.position + eventData.delta);
        velocity = eventData.delta * 2f;
        if (velocity.magnitude >= 5f)
        {
            var lookTarget = new Vector3(cardRect.position.x + velocity.x, cardRect.position.y + velocity.y, 75f);
            //cardRect.LookAt(lookTarget);
            iTween.LookUpdate(gameObject, iTween.Hash("looktarget", lookTarget, "time", 5f));
        }
        //else
        //{
        //    var lookTarget = new Vector3(cardRect.position.x, cardRect.position.y, 10000f);
        //    //cardRect.LookAt(lookTarget);
        //    iTween.LookUpdate(gameObject, iTween.Hash("looktarget", lookTarget, "time", 1f));
        //}
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Shrink();
        //cardRect.SetAnchor(anchoredEdge);
        //cardRect.SetPivot(PivotPresets.MiddleCenter);

        var lookTarget = new Vector3(cardRect.position.x, cardRect.position.y, 10000f);
        iTween.LookTo(gameObject, lookTarget, 2f);

        if (cardManager != null)
        {
            cardManager.CardReleased(this);
            cardManager.RebuildHandLayout();
        }
    }

    private void Update()
    {
        if (cardHasBeenPickedUp && velocity.magnitude < 5f)
        {
            var lookTarget = new Vector3(cardRect.position.x, cardRect.position.y, 100f);
            //cardRect.LookAt(lookTarget);
            iTween.LookUpdate(gameObject, iTween.Hash("looktarget", lookTarget, "time", 10f));
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //cardRect.SetAnchor(AnchorPresets.BottomCenter);
        //cardRect.SetPivot(PivotPresets.BottomCenter);
        if (cardHasBeenPickedUp == false && cardManager.CardIsBeingDragged == false)
        {
            positionInHand = cardRect.anchoredPosition;
            childIndexInWidget = cardRect.GetSiblingIndex();
            owningPlayer.HandZoneWidget.horizontalLayoutGroup.enabled = false;
            cardRect.SetParent(canvas.transform);
            iTween.ScaleTo(gameObject, Vector3.one * bigScale, 0.2f);
            Debug.Log($"{cardRect.GetAnchor()}");

            //cardRect.localScale = Vector3.one * bigScale * 0.7f;
            StartCoroutine(DelayedRebuild());
        }
    }

    private IEnumerator DelayedRebuild()
    {
        yield return new WaitForEndOfFrame();

        if (cardManager != null)
            cardManager.RebuildHandLayout();

        yield return null;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cardHasBeenPickedUp == false && cardManager.CardIsBeingDragged == false)
        {
            cardRect.SetParent(owningPlayer.HandZoneWidget.transform);
            cardRect.SetSiblingIndex(childIndexInWidget);
            owningPlayer.HandZoneWidget.horizontalLayoutGroup.enabled = true;

            iTween.Stop(gameObject);
            Shrink();
            if (cardManager != null)
                cardManager.RebuildHandLayout();
        }

        //cardRect.SetAnchor(anchoredEdge);
        //cardRect.SetPivot(PivotPresets.MiddleCenter);
    }

    internal void Shrink()
    {
        cardRect.localScale = Vector3.one * smallScale;
    }

    internal void ReleasedOnField(RectTransform resolvingPanel)
    {
        transform.SetParent(resolvingPanel, true);
        Debug.Log($"Move to resolving area");
        Vector3 releasePosition = cardRect.position;
        //cardRect.SetPivot(PivotPresets.MiddleCenter);
        //cardRect.SetAnchor(AnchorPresets.MiddleCenter);
        //iTween.MoveFrom(gameObject, releasePosition , 1f);
        var targetPosition = new Vector2(resolvingPanel.position.x - (resolvingPanel.sizeDelta.x / 2f), resolvingPanel.position.y);
        //cardRect.anchoredPosition = releasePosition;
        //transform.SetParent(canvas.rootCanvas.transform);
        //iTween.MoveAdd(gameObject, velocity * 20f, 7f);
        //iTween.MoveTo(gameObject, targetPosition, 8f);
        Vector3[] path = new Vector3[2]
        {
            //releasePosition - (targetPosition * 0.2f) + ((Vector3)velocity * 60f),
            //releasePosition - (targetPosition * 0.5f) + ((Vector3)velocity * 100f),
            //releasePosition - (targetPosition * 0.8f) + ((Vector3)velocity * 60f)
            //(Vector3)velocity * 5f,
            //(Vector3)velocity * 10f,
            //(Vector3)velocity * 5f
            targetPosition + (velocity * 5f),
            targetPosition
        };

        //foreach (Vector3 pathPoint in path)
        //{
        //    Debug.DrawLine(transform.position, pathPoint, Color.red, 10f);
        //}
        //Debug.Break();

        var velocityTimeFactor = (Mathf.FloorToInt((Mathf.Abs(velocity.magnitude)) / 100f)) * 0.1f;
        //var velocityTimeFactor = (Mathf.FloorToInt((Mathf.Abs(velocity.x) + Mathf.Abs(velocity.y)) / 50f)) * 0.5f;
        var punchPower = 100f + (100f * velocityTimeFactor);
        iTween.PunchRotation(gameObject, iTween.Hash("x", punchPower / 4f, "y", punchPower / 4f, "z", punchPower, "delay", 0.1f + (0.75f * velocityTimeFactor), "time", 1f + (0.5f * velocityTimeFactor))); //, Vector3.one * 75f, 1f + velocityTimeFactor);
        iTween.MoveTo(gameObject, iTween.Hash("position", targetPosition, "time", 1f + velocityTimeFactor, "path", path, "movetopath", true, "easetype", iTween.EaseType.easeOutBack));
    }

    internal void SetAnchorAndPivot(AnchorPresets newAnchor, PivotPresets newPivot)
    {
        //cardRect.SetAnchor(newAnchor);
        cardRect.SetPivot(newPivot);

        //var centerOfScreen = new Vector2(Screen.width / 2, Screen.height / 2);
        //var direction = (centerOfScreen - (Vector2)cardRect.transform.position).normalized;

        //var roundedDirectionX = Mathf.RoundToInt(direction.x);
        //var roundedDirectionY = Mathf.RoundToInt(direction.y);

        //var roundedDirection = new Vector2(roundedDirectionX, roundedDirectionY);

        //Debug.Log($"{roundedDirection}");
    }

    internal void ReleasedInHand()
    {
        cardRect.SetParent(owningPlayer.HandZoneWidget.transform);
        cardRect.SetSiblingIndex(childIndexInWidget);
        owningPlayer.HandZoneWidget.horizontalLayoutGroup.enabled = true;

        //transform.SetParent(owningPlayer.HandZoneWidget.transform);
        cardRect.anchoredPosition = positionInHand;
        iTween.ShakePosition(gameObject, Vector3.one * 15f, 0.1f);
        cardHasBeenPickedUp = false;
        Shrink();
    }

    internal void MarkerTargeted()
    {
        if (cardManager != null && hasTargetedAbilities)
        {
            if (!hasBeenPlayed)
            {
                hasBeenPlayed = true;
                UpdateCardDescriptions();
            }
            hasTargetedAbilities = false;
            gameObject.SetActive(true);
            cardManager.MarkerHoldingCardAbilitiesTargeted(this);
            cardManager.RebuildHandLayout();
        }
    }

    private Vector3 KeepOnScreen(Vector3 wantedPosition)
    {
        var outPosition = wantedPosition;
        var cornersArray = new Vector3[4];
        cardRect.GetWorldCorners(cornersArray);

        foreach (Vector3 corner in cornersArray)
        {
            if (!canvas.pixelRect.Contains(corner))
            {
                //Vector3 centerOfScreen = mainCamera.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
                //var direction = (wantedPosition - (centerOfScreen + (Vector3)cardRect.rect.size / 2f)).normalized;
                var direction = canvas.pixelRect.center - (Vector2)corner;
                outPosition += (Vector3)direction / 20f;
                iTween.ShakeRotation(gameObject, Vector3.one * 5f, 0.1f);
            }
        }

        return outPosition;
    }
}
