using UnityEngine;

public class PausedState : IState
{
    [HideInInspector] public string Name { get => "MainMenu State"; }
    public GameManager.GameState gameState { get => GameManager.GameState.Paused; }

    private GameManager gameManager;

    public PausedState(GameManager gm)
    {
        gameManager = gm;
    }
    public void Enter()
    {
        Time.timeScale = 0f;
        UIManager.Instance.pauseScreen.SetActive(true);
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        UIManager.Instance.pauseScreen.SetActive(false);
    }

    public void Update()
    {
        CheckForResume();
    }

    private void CheckForResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.SetGameState(GameManager.GameState.Gameplay);

            if (gameManager.currentState.gameState == GameManager.GameState.Gameplay)
            {
                Debug.Log("Saliendo de la pausa.");
            }
        }
    }
}
