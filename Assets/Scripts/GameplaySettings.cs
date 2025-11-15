using UnityEngine;

[CreateAssetMenu(fileName = "GameplaySettings", menuName = "Game/Gameplay Settings")]
public class GameplaySettings : ScriptableObject
{
    [Header("Movement")]
    [SerializeField] private float playerSpeed = 6f;
    [SerializeField] private float enemySpeed = 3f;
    [SerializeField] private float enemyStopDistance = 0.25f;
    [SerializeField] private int enemyCount = 1000;

    [Header("Visuals")]
    [SerializeField] private float playerRadius = 0.2f;
    [SerializeField] private float enemyRadius = 0.2f;

    [Header("Scenes")]
    [SerializeField] private string uiSceneName = "UIOverlay";
    [SerializeField] private float guiWaitTimeout = 5f;

    public float PlayerSpeed => playerSpeed;
    public float EnemySpeed => enemySpeed;
    public float EnemyStopDistance => enemyStopDistance;
    public int EnemyCount => enemyCount;
    public float PlayerRadius => playerRadius;
    public float EnemyRadius => enemyRadius;
    public string UISceneName => uiSceneName;
    public float GuiWaitTimeout => guiWaitTimeout;
}
