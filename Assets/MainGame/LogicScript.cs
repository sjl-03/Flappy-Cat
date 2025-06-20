using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    public int playerScore;
    public Text scoreText;
    public GameObject gameOverScreen;
    public bool isGameOver = false;

    [ContextMenu("Increase Score")]
    public void addScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();
    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void titleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void gameOver()
    {
        isGameOver = true;
        gameOverScreen.SetActive(true);
    }

    public void SavePlayerScore(string playerName)
    {
        if (string.IsNullOrEmpty(playerName)) return;

        LeaderboardTransferScript.nameToSubmit = playerName;
        LeaderboardTransferScript.scoreToSubmit = playerScore;

        Debug.Log($"Saved {playerName} with score {playerScore} — loading TitleScreen...");
        SceneManager.LoadScene("TitleScreen");
    }
}
