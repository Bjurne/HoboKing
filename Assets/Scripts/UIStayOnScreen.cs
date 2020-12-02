using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class UIStayOnScreen : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    private Vector2 screenBounds;
    //private float objectWidth;
    //private float objectHeight;
    private bool isBeingDragged;
    private RectTransform rect;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        //objectWidth = transform.GetComponent<SpriteRenderer>().bounds.extents.x; //extents = size of width / 2
        //objectHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y; //extents = size of height / 2
    }

    void LateUpdate()
    {
        if (isBeingDragged)
        {
            var width = rect.sizeDelta.x;
            var height = rect.sizeDelta.y;
            Vector3 viewPos = rect.anchoredPosition;
            viewPos.x = Mathf.Clamp(viewPos.x, screenBounds.x * -1 + width, screenBounds.x - width);
            viewPos.y = Mathf.Clamp(viewPos.y, screenBounds.y * -1 + height, screenBounds.y - height);
            rect.anchoredPosition = viewPos;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isBeingDragged = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isBeingDragged = false;
    }
}