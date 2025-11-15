using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private bool autoSpawnCharacters = true;
    [SerializeField] private PlayerView playerPrefab;
    [SerializeField] private EnemyView enemyPrefab;
    [SerializeField] private GameplaySettings gameplaySettings;

    private InputService _inputService;
    private Player _player;
    private EnemyManager _enemyManager;
    private CameraBounds _bounds;
    private bool _isPaused;
    private PlayerView _playerView;
    private EnemyView[] _enemyViews;
    private Transform _spawnRoot;

    private void Awake()
    {
        if (gameplaySettings == null)
        {
            Debug.LogError("GameplaySettings asset is missing.");
            enabled = false;
            return;
        }

        var camera = Camera.main;
        if (camera == null)
        {
            Debug.LogError("Main camera is required for CameraBounds.");
            enabled = false;
            return;
        }

        _bounds = new CameraBounds(camera);
        EnsureUISceneLoaded();

        _inputService = new InputService();
        _inputService.SetState(GameState.Gameplay);
    }

    private void Start()
    {
        TryRegisterWithGUI();
    }

    private void Update()
    {
        _inputService.Update();

        if (_inputService.EscapePressed && _inputService.CurrentState == GameState.Gameplay)
        {
            GUIController.Instance?.ToggleMenu();
            _inputService.ConsumeEscape();
        }

        if (_isPaused || _player == null || _enemyManager == null)
        {
            return;
        }

        _player.Tick(Time.deltaTime, _inputService.Movement);
        _enemyManager.Tick(Time.deltaTime, _player.Position);
    }

    private void EnsureUISceneLoaded()
    {
        var uiScene = gameplaySettings.UISceneName;
        if (string.IsNullOrEmpty(uiScene))
        {
            return;
        }

        if (GUIController.Instance != null)
        {
            return;
        }

        var scene = SceneManager.GetSceneByName(uiScene);
        if (scene.isLoaded)
        {
            return;
        }

        SceneManager.LoadSceneAsync(uiScene, LoadSceneMode.Additive);
    }

    private void TryRegisterWithGUI()
    {
        var gui = GUIController.Instance;
        if (gui == null)
        {
            gui = FindFirstObjectByType<GUIController>();
        }

        if (gui != null)
        {
            gui.RegisterBootstrapper(this, _inputService);
        }
        else
        {
            StartCoroutine(WaitForGUI());
        }
    }

    private IEnumerator WaitForGUI()
    {
        var elapsed = 0f;
        while (elapsed < gameplaySettings.GuiWaitTimeout)
        {
            var gui = GUIController.Instance;
            if (gui == null)
            {
                gui = FindFirstObjectByType<GUIController>();
            }

            if (gui != null)
            {
                gui.RegisterBootstrapper(this, _inputService);
                yield break;
            }

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Debug.LogWarning($"GUIController not found within {gameplaySettings.GuiWaitTimeout} seconds.");
    }

    public void SetPaused(bool paused)
    {
        _isPaused = paused;
    }

    public void ResetGame()
    {
        if (_player == null || _enemyManager == null)
        {
            return;
        }

        _player.ResetState();
        _enemyManager.ResetState();
    }

    public void StartNewGame()
    {
        if (_player == null || _enemyManager == null)
        {
            EnsureCharacters(_bounds);
            ApplyViewSettings();

            if (_playerView == null || _enemyViews == null || _enemyViews.Length == 0)
            {
                Debug.LogError("Cannot start game without player/enemy views.");
                return;
            }

            _player = new Player(_playerView, _bounds, gameplaySettings);
            _enemyManager = new EnemyManager(_enemyViews, _bounds, gameplaySettings);
        }

        ResetGame();
    }

    private void ApplyViewSettings()
    {
        if (_playerView != null)
        {
            _playerView.Init(gameplaySettings.PlayerRadius);
        }

        if (_enemyViews != null)
        {
            foreach (var enemy in _enemyViews)
            {
                enemy?.Init(gameplaySettings.EnemyRadius);
            }
        }
    }

    private void EnsureCharacters(CameraBounds bounds)
    {
        if (!autoSpawnCharacters)
        {
            return;
        }

        var root = _spawnRoot != null ? _spawnRoot : transform;
        var rect = bounds.GetWorldRect();

        var playerRadius = gameplaySettings.PlayerRadius;
        var enemyRadius = gameplaySettings.EnemyRadius;
        var margin = Mathf.Max(playerRadius, enemyRadius);

        float minX = rect.xMin + margin;
        float maxX = rect.xMax - margin;
        float minY = rect.yMin + margin;
        float maxY = rect.yMax - margin;

        if (minX > maxX)
        {
            minX = maxX = rect.center.x;
        }

        if (minY > maxY)
        {
            minY = maxY = rect.center.y;
        }

        var desiredEnemyCount = Mathf.Max(1, gameplaySettings.EnemyCount);
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(desiredEnemyCount + 1));
        if (gridSize % 2 == 0)
        {
            gridSize += 1;
        }

        float width = Mathf.Max(0.01f, maxX - minX);
        float height = Mathf.Max(0.01f, maxY - minY);
        float stepX = gridSize > 1 ? width / (gridSize - 1) : width;
        float stepY = gridSize > 1 ? height / (gridSize - 1) : height;
        int centerIndex = gridSize / 2;

        Vector2 playerPosition = new Vector2(minX + centerIndex * stepX, minY + centerIndex * stepY);

        if (playerPrefab == null && _playerView == null)
        {
            Debug.LogError("Player prefab is required for auto spawn.");
            return;
        }

        if (enemyPrefab == null && (_enemyViews == null || _enemyViews.Length == 0))
        {
            Debug.LogError("Enemy prefab is required for auto spawn.");
            return;
        }

        if (_playerView == null)
        {
            _playerView = Instantiate(playerPrefab, playerPosition, Quaternion.identity, root);
        }
        else
        {
            _playerView.transform.position = playerPosition;
        }

        if (_enemyViews == null || _enemyViews.Length == 0)
        {
            _enemyViews = SpawnEnemies(root, desiredEnemyCount, gridSize, centerIndex, minX, minY, stepX, stepY);
        }
        else
        {
            PositionExistingEnemies(gridSize, centerIndex, minX, minY, stepX, stepY);
        }
    }

    private EnemyView[] SpawnEnemies(Transform root, int desiredEnemyCount, int gridSize, int centerIndex, float minX, float minY, float stepX, float stepY)
    {
        var pool = new EnemyPool(enemyPrefab, root, desiredEnemyCount);
        var spawned = new List<EnemyView>(desiredEnemyCount);

        for (int row = 0; row < gridSize && spawned.Count < desiredEnemyCount; row++)
        {
            for (int col = 0; col < gridSize && spawned.Count < desiredEnemyCount; col++)
            {
                if (row == centerIndex && col == centerIndex)
                {
                    continue;
                }

                var position = new Vector3(minX + col * stepX, minY + row * stepY, 0f);
                var enemy = pool.Get(position);
                spawned.Add(enemy);
            }
        }

        return spawned.ToArray();
    }

    private void PositionExistingEnemies(int gridSize, int centerIndex, float minX, float minY, float stepX, float stepY)
    {
        int placed = 0;
        for (int row = 0; row < gridSize && placed < _enemyViews.Length; row++)
        {
            for (int col = 0; col < gridSize && placed < _enemyViews.Length; col++)
            {
                if (row == centerIndex && col == centerIndex)
                {
                    continue;
                }

                var position = new Vector3(minX + col * stepX, minY + row * stepY, 0f);
                _enemyViews[placed].transform.position = position;
                placed++;
            }
        }
    }
}
