using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaNode : MonoBehaviour, ITargetable
{
    [SerializeField] TMPro.TextMeshProUGUI indexField = default;

    private ArenaIndex index = default;
    [HideInInspector] public ArenaIndex Index { get { return index; } }

    //public bool IsActive { get { return isActive; } }
    //public bool IsOccupied { get { return isOccupied; } }
    public bool LeftInactive { get => leftInactive; }
    public ArenaTile OccupyingArenaTile { get => occupyingArenaTile; }

    private bool leftInactive = false;
    //private bool isActive = false;
    //private bool isOccupied = false;
    private ArenaTile occupyingArenaTile;
    private TargetableStatus targetableStatus = new TargetableStatus();

    private List<ITargetable> neighbourArenaNodes;
    public List<ITargetable> Neighbours => neighbourArenaNodes;

    public void SetActive(bool isActive, ArenaTile arenaTile = null)
    {
        targetableStatus.isActive = isActive;

        var color = GetComponent<Image>().color;
        color = isActive ? Color.clear : Color.grey;
        if (arenaTile != null)
        {
            arenaTile.SetIndex(index.Index, this);
            occupyingArenaTile = arenaTile;
            SetIsOccupied(true);
        }
        else
        {
            if (occupyingArenaTile != null)
                Destroy(occupyingArenaTile.gameObject);
            SetIsOccupied(false);
        }
    }

    internal void SetIsOccupied(bool value)
    {
        targetableStatus.isOccupied = value;
    }

    public void SetIndex(Vector2 newIndex)
    {
        index = new ArenaIndex(newIndex);
        indexField.text = $"{index.Index}";
    }

    internal void SetNeighbours(ArenaNode[] neighbours)
    {
        neighbourArenaNodes = new List<ITargetable>(neighbours);
    }

    public void DebugIndex()
    {
        Debug.Log($"{index.Index}");
    }

    internal void SetLeftInactive(bool value)
    {
        leftInactive = true;
    }

    public TargetableStatus TargetableStatus { get => targetableStatus; }

}

public class ArenaIndex
{
    private int xIndex;
    private int yIndex;
    internal Vector2Int Index { get => new Vector2Int(xIndex, yIndex); }

    public ArenaIndex(Vector2 arenaIndex)
    {
        xIndex = (int)arenaIndex.x;
        yIndex = (int)arenaIndex.y;
    }
}
