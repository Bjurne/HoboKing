using UnityEngine;

[System.Serializable]
public class PlayerColor
{
    [SerializeField] Color mainColor;
    [SerializeField] Color secondaryColor;
    internal Color MainColor { get => mainColor; }
    internal Color SecondaryColor { get => secondaryColor; }

    public PlayerColor(Color mainColor, Color secondaryColor)
    {
        this.mainColor = mainColor;
        this.secondaryColor = secondaryColor;
    }
}