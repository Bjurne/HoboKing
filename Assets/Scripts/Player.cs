using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerIndex
{
    PlayerOne = 0,
    PlayerTwo = 1,
    PlayerThree = 2,
    PlayerFour = 3,
    PlayerFive = 4,
    PlayerSix = 5,
    PlayerIndexOutOfBounds = 6,
}

public class Player
{
    private string playerName;
    public string Name { get => playerName; }
    private PlayerIndex playerIndex;
    public PlayerIndex PlayerIndex { get => playerIndex; }
    private Deck deck;
    public Deck Deck { get => deck; }
    private GameObject markerPrefab;
    public GameObject MarkerPrefab { get => markerPrefab; }
    private Sprite symbolSprite;
    public Sprite SymbolSprite { get => symbolSprite; }
    private Color mainColor;
    public Color MainColor { get => playerColor.MainColor; }
    public Color SecondaryColor { get => playerColor.SecondaryColor; }
    private HandZoneWidget handZoneWidget;
    public HandZoneWidget HandZoneWidget { get => handZoneWidget; }

    private PlayerColor playerColor;

    internal int cardsDrawnInDrawStep = 1;
    internal int maximumHandSize = 3;
    
    public Player(string playerName, PlayerIndex playerIndex, Deck deck, GameObject markerPrefab, Sprite symbolSprite, PlayerColor playerColor)
    {
        this.playerName = playerName;
        this.playerIndex = playerIndex;
        this.deck = deck;
        this.markerPrefab = markerPrefab;
        this.symbolSprite = symbolSprite;
        this.playerColor = playerColor;

        //var cardCanvas = GameObject.FindObjectOfType<CardManager>().transform; // TODO sluta fuska
        var widgesPanel = CardManager.Instance.WidgetsPanel;
        var widget = GameObject.Instantiate(Resources.Load("Hand Zone [Widget]", typeof(GameObject)), widgesPanel) as GameObject;
        handZoneWidget = widget.GetComponent<HandZoneWidget>();
        handZoneWidget.SetDockedPosition((int)playerIndex);
    }

    internal CardData DrawCard()
    {
        return deck.DrawCard();
    }
}

