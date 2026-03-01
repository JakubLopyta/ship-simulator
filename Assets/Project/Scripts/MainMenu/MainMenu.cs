using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void GoFreeMode()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
