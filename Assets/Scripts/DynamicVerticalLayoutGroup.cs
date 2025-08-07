using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class AutoSizingVerticalLayoutGroup : VerticalLayoutGroup
{
    public override void SetLayoutVertical()
    {
        base.SetLayoutVertical();
        ResizeParentToFitChildren();
    }

    private void ResizeParentToFitChildren()
    {
        float totalHeight = padding.top + padding.bottom;
        int activeChildCount = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            RectTransform child = rectChildren[i];
            totalHeight += child.sizeDelta.y;
            activeChildCount++;
        }

        if (activeChildCount > 1)
            totalHeight += spacing * (activeChildCount - 1);

        // Ustaw nową wysokość RectTransforma
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 size = rt.sizeDelta;
        rt.sizeDelta = new Vector2(size.x, totalHeight);
    }
}