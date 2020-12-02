using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Marker : MonoBehaviour, ITargetable
{
    [SerializeField] internal Image symbolImage = default;
    [SerializeField] GameObject isHoldingAbilitiesIndicator = default;
    private Player player;
    public Player Player { get => player; }
    private Sprite symbolSprite;
    private ArenaIndex arenaIndex;
    private ArenaTile parentTile;
    private TargetableStatus targetableStatus;

    internal Card card = default;

    public ArenaTile ParentTile { get => parentTile; }
    public TargetableStatus TargetableStatus { get => targetableStatus; }

    public List<ITargetable> Neighbours { get => ParentTile.Neighbours; }

    internal virtual void SetPlayerProperties(Player player)
    {
        this.player = player;


        symbolSprite = player.SymbolSprite;
        symbolImage.sprite = symbolSprite;
        symbolImage.color = player.MainColor;

        targetableStatus = new TargetableStatus(true, false);
    }

    internal void SetNewIndex(ArenaIndex newIndex, ArenaTile target)
    {
        arenaIndex = newIndex;
        parentTile = target;
    }

    internal void SetIsHoldingAbilities(bool value)
    {
        isHoldingAbilitiesIndicator.SetActive(value);
    }

    internal void DestroyMarker()
    {
        if (card != null)
            Destroy(card.gameObject);
        
        parentTile.RemoveOccupyingMarker();
        parentTile.SetIsHalfMarkedByPlayer();
    }
}
