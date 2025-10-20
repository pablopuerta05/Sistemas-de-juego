
public class MoveCommand : ICommand
{
    private PlayerMovement _playerMovement;
    private InputHandler inputHandler;

    public MoveCommand(PlayerMovement playerMovement, InputHandler inputHandler)
    {
        _playerMovement = playerMovement;
        this.inputHandler = inputHandler;
    }

    void ICommand.Execute()
    {
        _playerMovement.Move(inputHandler.moveDir);
    }
}
