using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Input Reader", menuName = "Scriptable Objects/Input Reader")]
public class InputReader : ScriptableObject
{
    private InputActions controls;

    //[Header("General Shortcuts")]
    public event UnityAction OnPauseEvent;

    //[Header("Map Controll Input")]
    public event Action<Vector2> OnMapLeftClick;

    [Header("Sailing Camera Input")]
    public Vector2 LookDelta { get; private set; }
    public float ScrollDelta { get; private set; }
    public bool IsRightMouseButtonPressed { get; private set; }

    [Header("Map Camera Input")]
    public bool IsMapDragPressed { get; private set; }
    public Vector2 MapDragDelta { get; private set; }
    public float MapZoomDelta { get; private set; }

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new InputActions();
        
            controls.GlobalSettings.Pause.performed += context => OnPauseEvent?.Invoke();

            controls.LockedCamera.RightClick.performed += context => IsRightMouseButtonPressed = true;
            controls.LockedCamera.RightClick.canceled += context => IsRightMouseButtonPressed = false;

            controls.LockedCamera.Look.performed += context => LookDelta = context.ReadValue<Vector2>();
            controls.LockedCamera.Look.canceled += context => LookDelta = Vector2.zero;

            controls.LockedCamera.Scroll.performed += context => ScrollDelta = context.ReadValue<Vector2>().y * 0.01f;
            controls.LockedCamera.Scroll.canceled += context => ScrollDelta = 0f;

            controls.Map.LeftClick.performed += context =>
                {
                    if (Mouse.current != null)
                    {
                        Vector2 mousePosition = Mouse.current.position.ReadValue();
                        OnMapLeftClick?.Invoke(mousePosition);
                    }
                };

            controls.Map.RightClick.performed += context => IsMapDragPressed = true;
            controls.Map.RightClick.canceled += context => IsMapDragPressed = false;

            controls.Map.PointerDelta.performed += context => MapDragDelta = context.ReadValue<Vector2>();
            controls.Map.PointerDelta.canceled += context => MapDragDelta = Vector2.zero;

            controls.Map.Scroll.performed += context => MapZoomDelta = context.ReadValue<Vector2>().y * 0.1f;
            controls.Map.Scroll.canceled += context => MapZoomDelta = 0f;
        }
        controls.GlobalSettings.Enable();
        controls.LockedCamera.Enable();
        controls.Map.Disable();
    }

    private void OnDisable()
    {
        controls?.Disable();
    }

    public void SetMapInputActive(bool isMapActive)
    {
        if (isMapActive)
        {
            controls.LockedCamera.Disable();
            controls.Map.Enable();
        }
        else
        {
            // Zamkniêto mapê
            controls.Map.Disable();
            controls.LockedCamera.Enable();
        }
    }
}