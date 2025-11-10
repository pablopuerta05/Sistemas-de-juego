using UnityEngine;

public class GameOverState : IState
{
    [HideInInspector] public string Name { get => "MainMenu State"; }
    public GameManager.GameState gameState { get => GameManager.GameState.GameOver; }

    private GameManager gameManager;

    public GameOverState(GameManager gm)
    {
        gameManager = gm;
    }

    public void Enter()
    {
        Time.timeScale = 0f;

        if (UIManager.Instance != null && UIManager.Instance.resultScreen != null)
        {
            UIManager.Instance.resultScreen.SetActive(true);
        }
    }

    public void Exit()
    {
        Time.timeScale = 1f;

        if (UIManager.Instance != null && UIManager.Instance.resultScreen != null)
        {
            UIManager.Instance.resultScreen.SetActive(false);
        }
    }

    public void Update()
    {
        
    }
}
