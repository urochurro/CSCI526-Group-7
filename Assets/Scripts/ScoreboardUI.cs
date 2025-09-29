using UnityEngine;
using TMPro;

public class ScoreboardUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    
    [Header("Positioning")]
    public Vector2 offset = new Vector2(20, -20); // Offset from top-left corner
    
    private void Start()
    {
        // Auto-find score text if not assigned
        if (scoreText == null)
        {
            scoreText = GetComponentInChildren<TextMeshProUGUI>();
        }
        
        // Position the scoreboard in top-left corner
        PositionScoreboard();
        
        // Connect to GameManager
        if (GameManager.I != null)
        {
            GameManager.I.SetScoreboardUI(this);
            UpdateScoreDisplay();
        }
    }
    
    private void PositionScoreboard()
    {
        if (scoreText != null)
        {
            // Get the Canvas component
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                // If no canvas found, try to find one in the scene
                canvas = FindObjectOfType<Canvas>();
            }
            
            if (canvas != null)
            {
                // Get the RectTransform of the score text
                RectTransform rectTransform = scoreText.GetComponent<RectTransform>();
                
                // Set anchor to top-left
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.pivot = new Vector2(0, 1);
                
                // Set position with offset
                rectTransform.anchoredPosition = offset;
                
                // Set text alignment to top-left
                scoreText.alignment = TextAlignmentOptions.TopLeft;
            }
        }
    }
    
    private void OnEnable()
    {
        // Subscribe to GameManager events when this object is enabled
        if (GameManager.I != null)
        {
            UpdateScoreDisplay();
        }
    }
    
    public void UpdateScoreDisplay()
    {
        if (scoreText != null && GameManager.I != null)
        {
            int collected = GameManager.I.GetCollectedCount();
            int total = GameManager.I.GetTotalCollectibles();
            scoreText.text = $"Collectibles: {collected} / {total}";
        }
    }
}
