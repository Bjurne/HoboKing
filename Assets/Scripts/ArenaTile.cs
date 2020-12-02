using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaTile : MonoBehaviour, ITargetable
{
    [SerializeField] TMPro.TextMeshProUGUI indexField = default;

    private ArenaIndex index = default;
    private TargetableStatus targetableStatus = new TargetableStatus();
    [HideInInspector] public ArenaIndex Index { get { return index; } }
    //[HideInInspector] public bool occupied = false;
    [HideInInspector] public ArenaNode parentArenaNode;
    [HideInInspector] public bool IsHalfMarkedByPlayer(PlayerIndex playerIndex)
    { 
        return halfMarkingPlayer == playerIndex && halfMarkingPlayer != PlayerIndex.PlayerIndexOutOfBounds;
    }
    private PlayerIndex halfMarkingPlayer = PlayerIndex.PlayerIndexOutOfBounds;
    private GameObject occupyingMarker = null;
    public GameObject OccupyingMarker { get => occupyingMarker; }

    private List<ITargetable> neighbourArenaTiles;
    public List<ITargetable> Neighbours { get => neighbourArenaTiles; }

    internal void SetIndex(Vector2 newIndex, ArenaNode arenaNode)
    {
        index = new ArenaIndex(newIndex);
        indexField.text = $"{index.Index}";
        parentArenaNode = arenaNode;
        targetableStatus.isActive = true;
    }

    internal void SetNeighbours(ArenaTile[] neighbours)
    {
        neighbourArenaTiles = new List<ITargetable>(neighbours); ;
    }

    public void DebugIndex()
    {
        StartCoroutine(Mark());
    }

    public IEnumerator Mark()
    {
        var image = GetComponent<Image>();
        Color originalColor = image.color;
        image.color = Color.green;
        yield return new WaitForSeconds(0.2f);
        image.color = originalColor;
    }

    public TargetableStatus TargetableStatus { get => targetableStatus; }

    internal void SetIsHalfMarkedByPlayer(PlayerIndex playerIndex = PlayerIndex.PlayerIndexOutOfBounds)
    {
        halfMarkingPlayer = playerIndex;
    }

    internal void RemoveOccupyingMarker()
    {
        if (occupyingMarker != null)
        {
            if (IsHalfMarkedByPlayer(occupyingMarker.GetComponent<Marker>().Player.PlayerIndex))
                SetIsHalfMarkedByPlayer();
            targetableStatus.isOccupied = false;
            Destroy(occupyingMarker.gameObject);
        }
    }

    internal void AddOccupyingMarker(GameObject marker, Card card = null)
    {
        RemoveOccupyingMarker();
        var newMarker = GameObject.Instantiate(marker, transform, false).GetComponent<Marker>();
        newMarker.SetPlayerProperties(GameStateManager.Instance.CurrentPlayer);
        newMarker.SetNewIndex(Index, this);
        occupyingMarker = newMarker.gameObject;
        if (newMarker.GetType() != typeof(HalfMarker))
        {
            targetableStatus.isOccupied = true;
            newMarker.card = card;
        }
    }
}
