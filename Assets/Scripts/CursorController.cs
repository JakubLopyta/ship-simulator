using UnityEngine;

namespace ShipSimulator.CustomCursor
{
    public class CursorController : MonoBehaviour
    {
        [Header("Cursor Textures")]
        [SerializeField] private Texture2D cursorDefault;
        [SerializeField] private Texture2D cursorHover;
        [SerializeField] private Texture2D cursorDot;
        [SerializeField] private Texture2D cursorArrow;
        [SerializeField] private Texture2D cursorCross;

        [Header("Hot Spot")]
        [SerializeField] private Vector2 hotSpotPosition = Vector2.zero;

        public static CursorController instance { get; private set; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }

        private void Start()
        {
            Cursor.SetCursor(cursorDefault, hotSpotPosition, CursorMode.Auto);
        }

        public void SetType(CursorType type)
        {
            switch (type)
            {
                case CursorType.Default:
                    Cursor.SetCursor(cursorDefault, hotSpotPosition, CursorMode.Auto); break;
                case CursorType.Hover:
                    Cursor.SetCursor(cursorHover, hotSpotPosition, CursorMode.Auto); break;
                case CursorType.Dot:
                    Cursor.SetCursor(cursorDot, hotSpotPosition, CursorMode.Auto); break;
                case CursorType.Arrow:
                    Cursor.SetCursor(cursorArrow, hotSpotPosition, CursorMode.Auto); break;
                case CursorType.Cross:
                    Cursor.SetCursor(cursorCross, hotSpotPosition, CursorMode.Auto); break;
                default:
                    Cursor.SetCursor(cursorDefault, hotSpotPosition, CursorMode.Auto); break;
            }
        }
    }
    public enum CursorType
    {
        Default,
        Hover,
        Dot,
        Arrow,
        Cross
    }

}
