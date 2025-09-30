using UnityEngine;

namespace ShipSimulator.CustomCursor
{
    public class CursorController : MonoBehaviour
    {
        [Header("Cursor Textures")]
        [SerializeField] private Texture2D cursorDefault;
        [SerializeField] private Texture2D cursorHover;
        [SerializeField] private Texture2D cursorGrab;
        [SerializeField] private Texture2D cursorText;
        [SerializeField] private Texture2D cursorDot;
        [SerializeField] private Texture2D cursorArrow;
        [SerializeField] private Texture2D cursorCross;
        [SerializeField] private Texture2D cursorCrossfire;

        [Header("Hot Spot Position")]
        [SerializeField] private Vector2 defaultHotSpot = new Vector2(21 ,7);
        [SerializeField] private Vector2 grabHotSpot = new Vector2(22, 18);
        [SerializeField] private Vector2 textHotSpot = new Vector2(31, 30);

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
            Cursor.SetCursor(cursorDefault, defaultHotSpot, CursorMode.Auto);
        }

        public void SetType(CursorType type)
        {
            switch (type)
            {
                case CursorType.Default:
                    Cursor.SetCursor(cursorDefault, defaultHotSpot, CursorMode.Auto); break;
                case CursorType.Hover:
                    Cursor.SetCursor(cursorHover, defaultHotSpot, CursorMode.Auto); break;
                case CursorType.Grab:
                    Cursor.SetCursor(cursorGrab, grabHotSpot, CursorMode.Auto); break;
                case CursorType.Text:
                    Cursor.SetCursor(cursorText, textHotSpot, CursorMode.Auto); break;
                case CursorType.Dot:
                    Cursor.SetCursor(cursorDot, defaultHotSpot, CursorMode.Auto); break;
                case CursorType.Arrow:
                    Cursor.SetCursor(cursorArrow, defaultHotSpot, CursorMode.Auto); break;
                case CursorType.Cross:
                    Cursor.SetCursor(cursorCross, defaultHotSpot, CursorMode.Auto); break;
                case CursorType.Crossfire:
                    Cursor.SetCursor(cursorCrossfire, defaultHotSpot, CursorMode.Auto); break;
                default:
                    Cursor.SetCursor(cursorDefault, defaultHotSpot, CursorMode.Auto); break;
            }
        }
    }
    public enum CursorType
    {
        Default,
        Hover,
        Grab,
        Text,
        Dot,
        Arrow,
        Cross,
        Crossfire
    }

}
