using UnityEngine;

public class PlayerColor : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private string colorProperty = "_BaseColor"; // Standard: "_Color"

    private MaterialPropertyBlock mpb;

    public void SetColor(Color c)
    {
        if (targetRenderer == null) return;

        mpb ??= new MaterialPropertyBlock();

        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(colorProperty, c);
        targetRenderer.SetPropertyBlock(mpb);
    }
}
