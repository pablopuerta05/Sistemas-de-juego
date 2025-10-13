using UnityEngine;

public class MainMenuState : IState
{
    [HideInInspector] public string Name { get => "MainMenu State"; }
    public GameManager.GameState gameState { get => GameManager.GameState.MainMenu; }

    private GameManager gameManager;

    // Constructor de la clase
    public MainMenuState(GameManager gm)
    {
        gameManager = gm;
    }

    public void Enter()
    {
        Time.timeScale = 0f; // Pausar el tiempo mientras estamos en el menú
    }

    public void Exit()
    {
        Time.timeScale = 1f;
    }

    public void Update()
    {
        
    }
}
