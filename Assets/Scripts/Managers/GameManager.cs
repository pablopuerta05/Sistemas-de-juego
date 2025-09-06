using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    #endregion

    public enum GameState
    {
        MainMenu,
        Gameplay,
        Paused,
        GameOver
    }

    // Store the current state of the game
    public GameState currentState;
    // Store the previous state of the game
    public GameState previousState;

    private void Update()
    {
        switch (currentState)
        {
            case GameState.MainMenu:
                break;
            case GameState.Gameplay:
                break;
            case GameState.Paused:
                break;
            case GameState.GameOver:
                break;
            default:
                break;
        }
    }
}
