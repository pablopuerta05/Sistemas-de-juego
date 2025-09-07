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
    public IState currentState;

    // flag to check if the game is over
    public bool isGameOver = false;

    // flag to check if the player is choosing their upgrades
    public bool choosingUpgrade;

    // reference to the player's game object
    public GameObject playerObject;

    [Header("Stopwatch")]
    public float timeLimit; // the time limit in seconds
    private float stopwatchTime; // the current time elapsed since the stopwatch started

    private void Start()
    {
        SetGameState(GameState.MainMenu); // Establecer el estado inicial
        UIManager.Instance.DisableScreens();
    }

    private void Update()
    {
        // Delegamos el comportamiento del estado actual
        currentState?.Update();
    }

    // Define the method to change the state of the game
    public void SetGameState(GameState newState)
    {
        // Si hay un estado anterior, llamamos al Exit
        currentState?.Exit();

        // Establecemos el nuevo estado y llamamos al Enter
        switch (newState)
        {
            case GameState.MainMenu:
                currentState = new MainMenuState();
                break;
            case GameState.Gameplay:
                currentState = new GameplayState();
                break;
            case GameState.Paused:
                currentState = new PausedState();
                break;
            case GameState.GameOver:
                currentState = new GameOverState();
                break;
            default:
                Debug.LogWarning("Current state does not exist");
                break;
        }

        // Llamamos al método Enter del nuevo estado
        currentState.Enter();
    }

    private void UpdateStopWatch()
    {
        stopwatchTime += Time.deltaTime;

        UIManager.Instance.UpdateStopWatchDisplay();

        if (stopwatchTime >= timeLimit)
        {
            playerObject.SendMessage("Kill");
        }
    }

    public void StartLevelUp()
    {
        //ChangeState(GameState.LevelUp);
        playerObject.SendMessage("RemoveAndApplyUpgrades");
    }

    public void EndLevelUp()
    {
        choosingUpgrade = false;
        Time.timeScale = 1;
        //levelUpScreen.SetActive(false);
        //ChangeState(GameState.Gameplay);
    }

    // gives us the time since the level has started
    public float GetElapsedTime()
    {
        return stopwatchTime;
    }
}
