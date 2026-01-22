using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [Header("RGB (0-255)")]
    [SerializeField, Range(0, 255)] private int r = 255;
    [SerializeField, Range(0, 255)] private int g = 255;
    [SerializeField, Range(0, 255)] private int b = 255;

    public Color CurrentColor => new Color(r / 255f, g / 255f, b / 255f, 1f);

    public void ApplyTo(PlayerColor playerColor)
    {
        if (playerColor == null) return;
        playerColor.SetColor(CurrentColor);
    }
}
