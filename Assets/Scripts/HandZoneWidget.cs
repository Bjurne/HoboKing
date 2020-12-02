using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandZoneWidget : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] RectTransform rect = default;
    [SerializeField] CanvasGroup canvasGroup = default;
    public RectTransform Rect { get => rect; }
    private Canvas canvas;
    private Vector2 dockedToScreenPosition;
    private Vector2 dockedPosition;
    private bool manuallyPositioned = false;
    internal AnchorPresets anchor;
    internal PivotPresets pivot;
    internal HorizontalLayoutGroup horizontalLayoutGroup = default;
    private Vector2[] viewportDockingPositionsArray;
    private int originalSpacing = 88;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();

        viewportDockingPositionsArray = new Vector2[4]
        {
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(0,0),
            new Vector2(1,0)
        };
    }
    public void OnDrag(PointerEventData eventData)
    {
        //if (manuallyPositioned == false)
        //    manuallyPositioned = true;
        
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    internal void SetActive(bool value)
    {
        horizontalLayoutGroup.enabled = value;
        RebuildLayout();

        canvasGroup.interactable = value;
        canvasGroup.blocksRaycasts = value;
        canvasGroup.alpha = value ? 1f : 0f;

        if (value)
            iTween.ScaleFrom(gameObject, Vector2.one * 0.6f, 1f);

        if (dockedToScreenPosition != null && dockedPosition != null && value && !manuallyPositioned)
        {
            transform.position = dockedPosition;
            iTween.MoveFrom(gameObject, dockedToScreenPosition, 1f);
        }
    }

    internal void RebuildLayout()
    {
        horizontalLayoutGroup.spacing = originalSpacing - (rect.childCount * 20);
        //horizontalLayoutGroup.childControlWidth = true;
        //horizontalLayoutGroup.childControlHeight = true;
        horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        LayoutRebuilder.MarkLayoutForRebuild(rect);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    internal void SetDockedPosition(int dockPositionIndex, int offsetX = 0, int offsetY = 0)
    {
        var previousAnchor = RectTransformExtensions.GetAnchor(rect);

        anchor = AnchorPresets.BottomCenter;
        pivot = PivotPresets.MiddleCenter;
        switch (dockPositionIndex)
        {
            case 0: 
                anchor = AnchorPresets.TopLeft;
                pivot = PivotPresets.TopLeft;
                break;
            case 1: 
                anchor = AnchorPresets.TopRight;
                pivot = PivotPresets.TopRight;
                break;
            case 2:
                anchor = AnchorPresets.BottomLeft;
                pivot = PivotPresets.BottomLeft;
                break;
            case 3:
                anchor = AnchorPresets.BottomRight;
                pivot = PivotPresets.BottomRight;
                break;
            default:
                break;
        }

        if (previousAnchor != anchor)
        {
            offsetX = 0;
            offsetY = 0;
        }
        
        rect.SetPivot(pivot);
        rect.SetAnchor(anchor, (offsetX), (offsetY));

        dockedToScreenPosition = rect.transform.position;
        //if (offsetX == 0 && offsetY == 0)
        //    manuallyPositioned = false;
        manuallyPositioned = !(offsetX == 0f && offsetY == 0f);

        if (!manuallyPositioned)
        {
            var centerOfScreen = new Vector2(Screen.width / 2, Screen.height / 2);
            var direction = (centerOfScreen - (Vector2)rect.transform.position).normalized;
            dockedPosition = dockedToScreenPosition + (direction * 50f);
        }
        else
            dockedPosition = dockedToScreenPosition;

        iTween.MoveTo(gameObject, dockedPosition, 1f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var closestDockingPositionDistance = new Vector2(5000f, 5000f);
        var closestDockingPositionIndex = 0;

        // TODO: 2 calls to Camera.main in this function alone, fix a better solution
        Vector2 viewportPosition = Camera.main.ScreenToViewportPoint(rect.position);
        foreach (Vector2 dockingPosition in viewportDockingPositionsArray)
        {
            var distance = viewportPosition - dockingPosition;
            if (distance.sqrMagnitude < closestDockingPositionDistance.sqrMagnitude)
            {
                closestDockingPositionDistance = distance;
                closestDockingPositionIndex = Array.IndexOf(viewportDockingPositionsArray, dockingPosition);
                Debug.Log($"Closest corner = {closestDockingPositionIndex}, at distance = {closestDockingPositionDistance}");
            }
        }

        var offset = closestDockingPositionDistance.sqrMagnitude > 0.2f * 0.2f ? viewportPosition - viewportDockingPositionsArray[closestDockingPositionIndex] : Vector2.zero;
        if (offset != Vector2.zero)
        {
            Debug.Log($"Offset is = {offset}");
            offset = Camera.main.ViewportToScreenPoint(offset);
        }

        // TODO: stop resetting offset to Vector2.zero in order to be able to manually position widget. The offset gets bad values when anchoring
        // the widget to a new corner, other than the previously anchored corner.
        //offset = Vector2.zero;

        SetDockedPosition(closestDockingPositionIndex, (int)offset.x, (int)offset.y);
        CardManager.Instance.RebuildHandLayout();
    }
}
