using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private OpenClosePanel pauseMenuCanvas;
    private bool isPaused = false;

    [SerializeField] private InputReader inputReader;

    void Start()
    {
        if (inputReader != null)
        {
            inputReader.OnPauseEvent += TogglePause;
        }
    }
    private void OnDestroy()
    {
        if (inputReader != null)
        {
            inputReader.OnPauseEvent -= TogglePause;
        }
    }
    public void Pause()
    {
        isPaused = true;
        pauseMenuCanvas.OpenWindow();
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenuCanvas.CloseWindow();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }

    private void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
}
