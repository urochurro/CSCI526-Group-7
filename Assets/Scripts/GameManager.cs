//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Tilemaps;

//public class GameManager : MonoBehaviour
//{
//    public static GameManager I { get; private set; }
//    public Tilemap targetTilemap;

//    private void Awake()
//    {
//        if (I != null && I != this) Destroy(gameObject);
//        I = this;
//    }

//    public bool CheckWin()
//    {
//        // iterate through all target cells and ensure occupant is a block
//        BoundsInt bounds = targetTilemap.cellBounds;
//        foreach (var pos in bounds.allPositionsWithin)
//        {
//            if (!targetTilemap.HasTile(pos)) continue;
//            var occ = GridManager.I.GetOccupant(pos);
//            if (occ == null || !occ.CompareTag("Block")) return false;
//        }
//        return true;
//    }

//    public void OnBlockMoved()
//    {
//        if (CheckWin())
//        {
//            Debug.Log("You solved the puzzle!");
//            // show UI win
//        }
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement; // for restarting or ending

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    public Tilemap targetTilemap;

    private bool gameEnded = false;

    private void Awake()
    {
        if (I != null && I != this) Destroy(gameObject);
        I = this;
        
        // Reset collectible counts when GameManager is created
        ResetCollectibleCounts();
    }
    
    // Reset collectible tracking (useful for level restarts)
    public void ResetCollectibleCounts()
    {
        totalCollectibles = 0;
        collectedCount = 0;
        UpdateScoreboard();
    }

    #region Win Condition
    public bool CheckWin()
    {
        BoundsInt bounds = targetTilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (!targetTilemap.HasTile(pos)) continue;
            var occ = GridManager.I.GetOccupant(pos);
            if (occ == null || !occ.CompareTag("Block")) return false;
        }
        return true;
    }

    public void OnBlockMoved()
    {
        if (gameEnded) return;

        if (CheckWin())
        {
            gameEnded = true;
            Debug.Log("You solved the puzzle!");
            StopGame();
            ShowEndScreen(true); // Show win screen
        }
    }
    #endregion

    #region Lose Condition
    public void OnPlayerCaught()
    {
        if (gameEnded) return;

        gameEnded = true;
        Debug.Log("Game Over! Ghost caught the player!");
        StopGame();
        ShowEndScreen(false); // Show lose screen
    }
    #endregion

    // Show end screen UI
    private void ShowEndScreen(bool playerWon)
    {
        if (endScreenUI != null)
        {
            endScreenUI.ShowEndScreen(playerWon);
        }
        else
        {
            Debug.LogWarning("End screen UI not found! Make sure GameEndScreenUI is in the scene.");
        }
    }

    private void StopGame()
    {
        // Stop all gameplay coroutines or movement scripts
        // Option 1: Disable player movement
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) player.GetComponent<PlayerController>().enabled = false;

        // Option 2: Disable all ghosts
        var ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (var g in ghosts)
        {
            var ghostAI = g.GetComponent<GhostAI>();
            if (ghostAI != null) ghostAI.enabled = false;
        }

        // Optionally show UI
        // Example: Show win/lose panel
        // WinPanel.SetActive(gameEnded && CheckWin());
        // LosePanel.SetActive(gameEnded && !CheckWin());
    }

    // Inside GameManager.cs
    private int totalCollectibles = 0;
    private int collectedCount = 0;
    
    // UI reference for scoreboard
    private ScoreboardUI scoreboardUI;
    
    // UI reference for end screen
    private GameEndScreenUI endScreenUI;

    // Call this when LevelLoader spawns a collectible
    public void RegisterCollectible()
    {
        totalCollectibles++;
        UpdateScoreboard();
    }

    // Call this when player collects one
    public void OnCollectiblePicked()
    {
        collectedCount++;
        Debug.Log($"Collected {collectedCount} / {totalCollectibles} items");
        UpdateScoreboard();

        if (collectedCount >= totalCollectibles)
        {
            Debug.Log("All collectibles collected!");
            // Optionally, do something like show UI
        }
    }
    
    // Getter methods for UI
    public int GetCollectedCount()
    {
        return collectedCount;
    }
    
    public int GetTotalCollectibles()
    {
        return totalCollectibles;
    }
    
    // Set UI reference
    public void SetScoreboardUI(ScoreboardUI ui)
    {
        scoreboardUI = ui;
    }
    
    // Set end screen UI reference
    public void SetEndScreenUI(GameEndScreenUI ui)
    {
        endScreenUI = ui;
    }
    
    // Update the scoreboard display
    private void UpdateScoreboard()
    {
        if (scoreboardUI != null)
        {
            scoreboardUI.UpdateScoreDisplay();
        }
    }

}
