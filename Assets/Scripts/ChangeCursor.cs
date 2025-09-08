using UnityEngine;
using UnityEngine.EventSystems;

namespace ShipSimulator.CustomCursor
{
    public class ChangeCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private CursorType cursorType;

        public void OnPointerEnter(PointerEventData eventData)
        {
            CursorController.instance.SetType(cursorType);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CursorController.instance.SetType(CursorType.Default);
        }
    }
}
