using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLocalization : MonoBehaviour
{
    private static GameLocalization _instance;

    public static GameLocalization Instance { get { return _instance; } }


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

    [SerializeField] PlayerColorLibrary colorLibrary = default;
    [SerializeField] CardDataLibrary cardDataLibrary = default;
    [SerializeField] SymbolLibrary symbolLibrary = default;
    [SerializeField] GameObject fullMarkerPrefab = default;
    [SerializeField] GameObject halfMarkerPrefab = default;
    [SerializeField] Arena activeArena = default;

    public Arena ActiveArena { get => activeArena; }

    public PlayerColor GetPlayerColor(PlayerIndex playerIndex)
    {
        if (colorLibrary.KeyFound(playerIndex))
            return colorLibrary.GetValue(playerIndex);
        else
        {
            Debug.LogWarning($"{playerIndex} not found in colorLibrary");
            return new PlayerColor(Color.white, Color.grey);
        }
    }

    public GameObject GetMarkerPrefab()
    {
        return GameStateManager.Instance.CurrentPlayer.MarkerPrefab;
    }

    public GameObject CheatGetPrefab(bool fullMarker)
    {
        if (fullMarker)
            return fullMarkerPrefab;
        else
            return halfMarkerPrefab;
    }

    internal Sprite GetPlayerSymbol(PlayerIndex symbolKey)
    {
        return symbolLibrary.GetValue(symbolKey);
    }

    public CardData GetCardData(string cardName)
    {
        return cardDataLibrary.GetValue(cardName);
    }
}
