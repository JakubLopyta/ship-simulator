using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Layout/Dynamic Horizontal Layout Group")]
public class DynamicHorizontalLayoutGroup : HorizontalLayoutGroup
{
    public override void SetLayoutHorizontal()
    {
        base.SetLayoutHorizontal(); // zachowaj normalne zachowanie HLG
        UpdateRectWidth();
    }

    public override void SetLayoutVertical()
    {
        base.SetLayoutVertical(); // nie modyfikujemy pionu
    }

    private void UpdateRectWidth()
    {
        float totalWidth = padding.left + padding.right;
        float maxHeight = 0f;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            RectTransform child = rectChildren[i];
            totalWidth += child.sizeDelta.x;

            if (i > 0)
                totalWidth += spacing;

            maxHeight = Mathf.Max(maxHeight, child.sizeDelta.y);
        }

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, padding.top + padding.bottom + maxHeight);
    }
}
