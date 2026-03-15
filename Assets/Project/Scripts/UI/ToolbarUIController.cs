using UnityEngine;
using UnityEngine.UI;
using System;

public class ToolbarUIController : MonoBehaviour
{
    private Color translucentButtonColor = new Color32(0, 0, 0, 0);
    private Color selectedButtonColor = new Color32(78, 101, 192, 190);

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button zoomInButton;
    [SerializeField] private Button zoomOutButton;
    [SerializeField] private Button moveButton;
    [SerializeField] private Button lineButton;

    private bool isPlaying;
    private bool isMoving;
    private bool isLineActive;

    public void OnLineButtonPressed()
    {
        isLineActive = !isLineActive;
        SetButtonColor(lineButton, isLineActive);
        OnLine?.Invoke(isLineActive);
    }

    private void Start()
    {
        OnPauseButtonPressed();
    }

    public static event Action<bool> OnPlay;
    public static event Action<bool> OnPause;
    public static event Action<bool> OnStop;
    public static event Action<bool> OnRestart;
    public static event Action<bool> OnZoomIn;
    public static event Action<bool> OnZoomOut;
    public static event Action<bool> OnMove;
    public static event Action<bool> OnLine;

    public void OnPlayButtonPressed()
    {
        if (isPlaying)
        {
            OnPauseButtonPressed();
            return;
        }

        isPlaying = true;
        SetButtonColor(playButton, true);
        SetButtonColor(pauseButton, false);
        SetButtonColor(stopButton, false);
        OnPlay?.Invoke(true);
    }

    public void OnPauseButtonPressed()
    {
        isPlaying = false;
        SetButtonColor(pauseButton, true);
        SetButtonColor(playButton, false);
        SetButtonColor(stopButton, false);
        OnPause?.Invoke(true);
    }

    public void OnStopButtonPressed()
    {
        isPlaying = false;
        SetButtonColor(stopButton, true);
        SetButtonColor(playButton, false);
        SetButtonColor(pauseButton, false);
        OnStop?.Invoke(true);
    }

    public void OnRestartButtonPressed()
    {
        isPlaying = false;
        SetButtonColor(playButton, false);
        SetButtonColor(pauseButton, false);
        SetButtonColor(stopButton, false);
        OnRestart?.Invoke(true);
    }

    public void OnZoomInPressed()
    {
        OnZoomIn?.Invoke(true);
    }

    public void OnZoomOutPressed()
    {
        OnZoomOut?.Invoke(true);
    }

    public void OnMovePressed()
    {
        isMoving = !isMoving;
        SetButtonColor(moveButton, isMoving);
        OnMove?.Invoke(isMoving);
    }

    private void SetButtonColor(Button button, bool selected)
    {
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = selected ? selectedButtonColor : translucentButtonColor;
        button.colors = colorBlock;
    }
}
