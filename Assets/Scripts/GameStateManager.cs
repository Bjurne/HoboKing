using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    #region Singleton
    private static GameStateManager _instance;

    public static GameStateManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            MonoBehaviour.Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    [SerializeField] CardManager cardManager = default;
    [SerializeField] int numberOfPlayers = default;
    
    private StateMachine<IGameState> stateMachine;
    private int currentRound;

    private Player[] players;
    private Player currentPlayer;
    public Player CurrentPlayer { get => currentPlayer; }

    private int currentTurn;

    public Deck deck;

    private void Start()
    {
        players = new Player[numberOfPlayers];
        for (int i = 0; i < numberOfPlayers; i++)
        {
            currentPlayer = players[i];
            var playerIndex = (PlayerIndex)Mathf.Clamp(i, 0, 6);
            var playerName = $"{playerIndex.ToString()}";
            var prefab = GameLocalization.Instance.CheatGetPrefab(true);
            var symbolSprite = GameLocalization.Instance.GetPlayerSymbol(playerIndex);
            var mainColor = GameLocalization.Instance.GetPlayerColor(playerIndex);
            var cardDeck = ScriptableObject.CreateInstance("Deck") as Deck;
            cardDeck.SetupDeck(deck.CardDatas);
            Player player = new Player(playerName, playerIndex, cardDeck, prefab, symbolSprite, mainColor);
            players[i] = player;
            player.HandZoneWidget.SetActive(false);
            //Debug.Log($"{player.Name} - {playerIndex} - {prefab.name} - {player.HandZoneWidget}");

            //stateMachine.ChangeState(new DrawState(cardManager, 3));
        }

        stateMachine = new StateMachine<IGameState>();

        //for (int i = 0; i < numberOfPlayers; i++)
        //{
        //    currentPlayer = players[i];
        //    stateMachine.ChangeState(new DrawState(cardManager, 3));
        //    currentPlayer.HandZoneWidget.SetActive(false);
        //}
        StartCoroutine(InitialDrawSequence());
    }

    private IEnumerator InitialDrawSequence()
    {
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < numberOfPlayers; i++)
        {
            currentPlayer = players[i];
            stateMachine.ChangeState(new DrawState(cardManager, 3));
            currentPlayer.HandZoneWidget.SetActive(false);
        }

        currentPlayer = players[0];
        currentPlayer.HandZoneWidget.SetActive(true);
    }

    public void TurnPassed()
    {
        if (currentTurn % numberOfPlayers == 0)
            NewRound();
        else
            NewTurn();
    }

    private void NewRound()
    {
        currentRound++;
        NewTurn();
        GameLocalization.Instance.ActiveArena.DebugScoreCount();
    }

    private void NewTurn()
    {
        currentTurn++;
        SetCurrentPlayer();
        stateMachine.ChangeState(new DrawState(cardManager, currentPlayer.cardsDrawnInDrawStep));
    }

    private void SetCurrentPlayer()
    {
        currentPlayer.HandZoneWidget.SetActive(false);

        var turn = currentTurn % numberOfPlayers;
        currentPlayer = (players[turn]);

        currentPlayer.HandZoneWidget.SetActive(true);
    }

    internal Player[] GetAllPlayers()
    {
        return players;
    }
}
