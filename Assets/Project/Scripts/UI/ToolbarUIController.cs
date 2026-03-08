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

    private bool isPlaying;
    private bool isPaused;
    private bool isStopped;
    private bool isZoomingIn;
    private bool isZoomingOut;
    private bool isMoving;

    public static event Action<bool> OnPlay;
    public static event Action<bool> OnPause;
    public static event Action<bool> OnStop;
    public static event Action<bool> OnRestart;
    public static event Action<bool> OnZoomIn;
    public static event Action<bool> OnZoomOut;
    public static event Action<bool> OnMove;

    public void OnPlayButtonPressed()
    {
        isPlaying = !isPlaying;
        SetButtonColor(playButton, isPlaying);
        OnPlay?.Invoke(isPlaying);
    }

    public void OnPauseButtonPressed()
    {
        isPaused = !isPaused;
        SetButtonColor(pauseButton, isPaused);
        OnPause?.Invoke(isPaused);
    }

    public void OnStopButtonPressed()
    {
        isStopped = !isStopped;
        SetButtonColor(stopButton, isStopped);
        OnStop?.Invoke(isStopped);
    }

    public void OnRestartButtonPressed()
    {
        isPlaying = false;
        isPaused = false;
        isStopped = false;
        SetButtonColor(playButton, false);
        SetButtonColor(pauseButton, false);
        SetButtonColor(stopButton, false);
        OnRestart?.Invoke(true);
    }

    public void OnZoomInPressed()
    {
        isZoomingIn = !isZoomingIn;
        SetButtonColor(zoomInButton, isZoomingIn);
        OnZoomIn?.Invoke(isZoomingIn);
    }

    public void OnZoomOutPressed()
    {
        isZoomingOut = !isZoomingOut;
        SetButtonColor(zoomOutButton, isZoomingOut);
        OnZoomOut?.Invoke(isZoomingOut);
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
