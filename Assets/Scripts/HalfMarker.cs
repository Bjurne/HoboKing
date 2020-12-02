using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HalfMarker : Marker
{
    [SerializeField] PlayerSymbolWidget symbolWidget = default;

    internal override void SetPlayerProperties(Player player)
    {
        base.SetPlayerProperties(player);

        symbolWidget.SetFillAmount(0.5f);
    }
}
