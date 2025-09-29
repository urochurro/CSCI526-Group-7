using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameEndScreenUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject endScreenPanel;
    public TextMeshProUGUI gameStatusText;
    public TextMeshProUGUI messageText;
    public Button restartButton;
    public Button quitButton;
    
    [Header("Text Content")]
    public string winText = "Congratulations! You Won!";
    public string loseText = "Game Over! You Lost!";
    public string winMessage = "Level Passed! Puzzle Solved!";
    public string loseMessage = "Ghost Caught You!";
    
    [Header("Colors")]
    public Color winColor = Color.green;
    public Color loseColor = Color.red;
    
    private void Start()
    {
        // Initially hide the end screen
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(false);
        }
        
        // Setup button listeners
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        
        // Connect to GameManager
        if (GameManager.I != null)
        {
            GameManager.I.SetEndScreenUI(this);
        }
    }
    
    public void ShowEndScreen(bool playerWon)
    {
        if (endScreenPanel == null) return;
        
        // Show the panel
        endScreenPanel.SetActive(true);
        
        // Update text and color based on win/loss
        if (gameStatusText != null)
        {
            gameStatusText.text = playerWon ? winText : loseText;
            gameStatusText.color = playerWon ? winColor : loseColor;
        }
        
        // Update message text
        if (messageText != null)
        {
            messageText.text = playerWon ? winMessage : loseMessage;
            messageText.color = playerWon ? winColor : loseColor;
        }
        
        // Pause the game
        Time.timeScale = 0f;
    }
    
    public void HideEndScreen()
    {
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(false);
        }
        
        // Resume the game
        Time.timeScale = 1f;
    }
    
    public void RestartGame()
    {
        // Resume time scale before restarting
        Time.timeScale = 1f;
        
        // Restart the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        // Resume time scale before quitting
        Time.timeScale = 1f;
        
        // Quit the application
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    private void OnDestroy()
    {
        // Clean up button listeners
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(QuitGame);
        }
    }
}
