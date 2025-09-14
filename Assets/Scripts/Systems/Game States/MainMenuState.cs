using UnityEngine;

public class MainMenuState : IState
{
    public string Name { get => "MainMenu State"; }
    public GameManager.GameState gameState { get => GameManager.GameState.MainMenu; }

    private GameManager gameManager;

    // Constructor de la clase
    public MainMenuState()
    {
        gameManager = GameManager.Instance;
    }


    public void Enter()
    {
        Time.timeScale = 0f; // Pausar el tiempo mientras estamos en el men�
    }

    public void Exit()
    {
        Time.timeScale = 1f;
    }

    public void Update()
    {
        //if (Time.timeScale == 1f)
        //{
        //    gameManager.SetGameState(GameManager.GameState.Gameplay);
        //}
    }
}
