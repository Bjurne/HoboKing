using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable
{
    TargetableStatus TargetableStatus { get; }
    List<ITargetable> Neighbours { get; }
}

[System.Serializable]
public class TargetableStatus
{
    internal bool isActive;
    internal bool isOccupied;

    public TargetableStatus()
    {

    }

    public TargetableStatus(bool expectedActive, bool expectedOccupied)
    {
        isActive = expectedActive;
        isOccupied = expectedOccupied;
    }
    public bool IsMatch(TargetableStatus wantedStatus)
    {
        return (this.isActive == wantedStatus.isActive && this.isOccupied == wantedStatus.isOccupied);
    }
}
