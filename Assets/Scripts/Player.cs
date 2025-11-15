using UnityEngine;

public class Player
{
    private readonly PlayerView _view;
    private readonly CameraBounds _bounds;
    private readonly GameplaySettings _settings;
    private readonly Transform _transform;
    private Vector2 _currentVelocity;

    public Vector2 Position { get; private set; }
    private readonly Vector2 _initialPosition;

    public Player(PlayerView view, CameraBounds bounds, GameplaySettings settings)
    {
        _view = view;
        _bounds = bounds;
        _settings = settings;
        _transform = view.Transform;
        _initialPosition = _transform.position;
        Position = _initialPosition;
    }

    public void Tick(float deltaTime, Vector2 movementInput)
    {
        Position = _transform.position;

        _currentVelocity = movementInput * _settings.PlayerSpeed;

        var next = Position + _currentVelocity * deltaTime;
        next = _bounds.Clamp(next, _view.Radius);

        Position = next;
        _transform.position = next;
    }

    public void ResetState()
    {
        Position = _initialPosition;
        _transform.position = _initialPosition;
        _currentVelocity = Vector2.zero;
    }

}
