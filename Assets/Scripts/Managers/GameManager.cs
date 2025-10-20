using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private List<IState> states = new List<IState>();

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        states.Add(new MainMenuState(this));
        states.Add(new GameplayState(this));
        states.Add(new LevelUpState(this));
        states.Add(new PausedState(this));
        states.Add(new GameOverState(this));

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
        LevelUp,
        Paused,
        GameOver
    }

    // Store the current state of the game
    public IState currentState;

    // flag to check if the game is over
    public bool isGameOver = false;

    // reference to the stopwatch
    public Stopwatch stopwatch;

    // Level up event delegate to start the level up (better than SendMessage())
    public event System.Action OnLevelUpApplied;

    private void Start()
    {
        SetGameState(GameState.MainMenu); // Establecer el estado inicial
        stopwatch = new Stopwatch();
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
        // En States BUSCA el state, EN DONDE el state.gameState sea igual a newState    
        currentState = states.Find(state => state.gameState == newState);

        if (currentState == null)
        {
            Debug.LogError("Estado no encontrado: " + newState);
            return;
        }

        // Llamamos al método Enter del nuevo estado
        currentState.Enter();
    }

    public void TriggerLevelUp()
    {
        OnLevelUpApplied?.Invoke();
    }
}
