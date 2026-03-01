using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private OpenClosePanel pauseMenuCanvas;
    private bool isPaused = false;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
}
