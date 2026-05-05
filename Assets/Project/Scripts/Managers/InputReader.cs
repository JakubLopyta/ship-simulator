using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Input Reader", menuName = "Scriptable Objects/Input Reader")]
public class InputReader : ScriptableObject
{
    private InputActions controls;

    public event UnityAction OnPauseEvent;

    public Vector2 LookDelta { get; private set; }
    public float ScrollDelta { get; private set; }
    public bool IsRightMouseButtonPressed { get; private set; }

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
        }
    controls.Enable();
    }

    private void OnDisable()
    {
        controls?.Disable();
    }
}