using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    //Chcialbym tu pozniej przeniesc cala logike klawisza esc itd.

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }
}
