using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSymbolWidget : MonoBehaviour
{
    [SerializeField] Image symbolImage = default;
    [SerializeField] Image backgroundImage = default;

    private void Awake()
    {
        symbolImage.sprite = GameStateManager.Instance.CurrentPlayer.SymbolSprite;
        symbolImage.color = GameStateManager.Instance.CurrentPlayer.MainColor;
        backgroundImage.color = GameStateManager.Instance.CurrentPlayer.SecondaryColor;
    }

    internal void SetFillAmount(float value)
    {
        symbolImage.fillAmount = value;
        backgroundImage.fillAmount = value;
    }
}
