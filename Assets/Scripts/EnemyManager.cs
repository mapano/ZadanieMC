using UnityEngine;

public class EnemyManager
{
    private readonly Transform[] _transforms;
    private readonly Vector2[] _positions;
    private readonly bool[] _isFrozen;
    private readonly float[] _radii;
    private readonly Vector2[] _initialPositions;
    private readonly CameraBounds _bounds;
    private readonly GameplaySettings _settings;
    private readonly float _stopDistanceSqr;

    public EnemyManager(EnemyView[] views, CameraBounds bounds, GameplaySettings settings)
    {
        _bounds = bounds;
        _settings = settings;
        _stopDistanceSqr = settings.EnemyStopDistance * settings.EnemyStopDistance;

        var count = views.Length;
        _transforms = new Transform[count];
        _positions = new Vector2[count];
        _isFrozen = new bool[count];
        _radii = new float[count];
        _initialPositions = new Vector2[count];

        for (int i = 0; i < count; i++)
        {
            _transforms[i] = views[i].Transform;
            _positions[i] = _transforms[i].position;
            _radii[i] = views[i].Radius;
            _initialPositions[i] = _positions[i];
        }
    }

    public void Tick(float deltaTime, Vector2 playerPosition)
    {
        var travel = Mathf.Max(_settings.EnemySpeed, 0f) * deltaTime;

        for (int i = 0; i < _transforms.Length; i++)
        {
            if (_isFrozen[i])
            {
                continue;
            }

            var position = _positions[i];
            var toEnemy = position - playerPosition;
            var sqrMagnitude = toEnemy.sqrMagnitude;

            if (sqrMagnitude <= _stopDistanceSqr)
            {
                _isFrozen[i] = true;
                continue;
            }

            if (sqrMagnitude > 0.0001f)
            {
                var direction = toEnemy / Mathf.Sqrt(sqrMagnitude);
                position += direction * travel;
            }

            position = _bounds.Clamp(position, _radii[i]);
            _positions[i] = position;
            _transforms[i].position = position;
        }
    }

    public void ResetState()
    {
        for (int i = 0; i < _transforms.Length; i++)
        {
            var start = _initialPositions[i];
            _isFrozen[i] = false;
            _positions[i] = start;
            _transforms[i].position = start;
        }
    }

}
