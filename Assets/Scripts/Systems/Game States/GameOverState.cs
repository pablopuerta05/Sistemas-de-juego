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
        UIManager.Instance.resultScreen.SetActive(true);
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        UIManager.Instance.resultScreen.SetActive(false);
    }

    public void Update()
    {
        
    }
}
