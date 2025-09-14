using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private List<IState> states = new List<IState>();

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        states.Add(new MainMenuState());
        states.Add(new GameplayState());
        states.Add(new PausedState());
        states.Add(new GameOverState());

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
    [HideInInspector] public float StopwatchTime => stopwatchTime;

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
        //          En States BUSCA el state, EN DONDE el state.gameState sea igual a newState    
        currentState = states.Find(state => state.gameState == newState);
        // el de arriba y abajo son iguales, elegir uno.
        foreach (IState state in states)
        {
            if (state.gameState == newState)
            {
                currentState = state;
                break;
            }
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
