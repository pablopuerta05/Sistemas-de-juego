using UnityEngine;

public class LevelUpState : IState
{
    [HideInInspector] public string Name { get => "Level Up State"; }
    public GameManager.GameState gameState { get => GameManager.GameState.LevelUp; }

    private GameManager gameManager;

    // Constructor de la clase
    public LevelUpState(GameManager gm)
    {
        gameManager = gm;
    }

    public void Enter()
    {
        Time.timeScale = 0f;
        UIManager.Instance.levelUpScreen.SetActive(true);
        //playerObject.SendMessage("RemoveAndApplyUpgrades");
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        UIManager.Instance.levelUpScreen.SetActive(false);
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }
}
