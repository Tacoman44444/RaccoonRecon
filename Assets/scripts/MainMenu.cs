using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Quits the game
    public void QuitGame()
    {
        Debug.Log("Quit Game!");
        Application.Quit();
    }
}