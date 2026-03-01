using UnityEngine;
using UnityEngine.EventSystems;

namespace ShipSimulator.CustomCursor
{
    public class ChangeCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Cursor Type")]
        [SerializeField] private CursorType cursorType;
        [Header("Unique Settings")]
        [SerializeField] private bool isGrabbable = false;
        private bool isHovered;

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
            CursorController.instance.SetType(cursorType);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            CursorController.instance.SetType(CursorType.Default);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isGrabbable)
            {
                CursorController.instance.SetType(CursorType.Grab);
                CursorController.instance.LockCursor();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            CursorController.instance.UnlockCursor();
            if (isHovered)
            {
                CursorController.instance.SetType(cursorType);
            }
            else CursorController.instance.SetType(CursorType.Default);
        }
    }
}
