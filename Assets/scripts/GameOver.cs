using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverMenu : MonoBehaviour
{
    private void Awake()
    {
        TextMeshProUGUI tm = transform.Find("Result").GetComponent<TextMeshProUGUI>();
        if (GameResult.PlayerWon)
        {
            tm.text = "YOU WON";
        }
        else
        {
            tm.text = "YOU DIED";
        }
    }
    public void RestartGame()
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