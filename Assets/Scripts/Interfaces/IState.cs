
public interface IState 
{
    string Name { get; }

    GameManager.GameState gameState { get; }

    void Enter();

    void Update();

    void Exit();
}
