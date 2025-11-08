using UnityEngine;

public class GameplayState : IState
{
    [HideInInspector] public string Name { get => "MainMenu State"; }
    public GameManager.GameState gameState { get => GameManager.GameState.Gameplay; }

    private GameManager gameManager;

    // Constructor de la clase
    public GameplayState(GameManager gm)
    {
        gameManager = gm;
    }

    public void Enter()
    {
        Time.timeScale = 1f;
    }

    public void Exit()
    {
        Time.timeScale = 0f;
    }

    public void Update()
    {
        CheckForPause();

        if (gameManager.stopwatch != null)
        {
            gameManager.stopwatch.UpdateStopWatch();
        }
    }

    private void CheckForPause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.SetGameState(GameManager.GameState.Paused);

            if (gameManager.currentState.gameState == GameManager.GameState.Paused)
            {
                Debug.Log("El juego está en pausa.");
            }
        }
    }
}
