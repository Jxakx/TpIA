public abstract class State
{
    public abstract void EnterState(PlayerEnemies enemy);
    public abstract void UpdateState(PlayerEnemies enemy);
    public abstract void ExitState(PlayerEnemies enemy);
}

public class StateMachine
{
    private State _currentState;

    public void ChangeState(State newState, PlayerEnemies enemy)
    {
        _currentState?.ExitState(enemy);
        _currentState = newState;
        _currentState.EnterState(enemy);
    }

    public void Update(PlayerEnemies enemy)
    {
        _currentState?.UpdateState(enemy);
    }
}
