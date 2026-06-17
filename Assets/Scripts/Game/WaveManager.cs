using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameUI gameUI;

    private int aliveCount;
    private bool isGameOver;

    public bool IsGameEnded => isGameOver;

    private void Start()
    {
        SpawnWave();

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.OnDeath += HandlePlayerDeath;
        }
    }

    private void OnEnable()
    {
        ZombieHealth.OnAnyZombieDeath += OnZombieKilled;
    }

    private void OnDisable()
    {
        ZombieHealth.OnAnyZombieDeath -= OnZombieKilled;
        Time.timeScale = 1f;
    }

    private void SpawnWave()
    {
        aliveCount = spawnPoints.Length;
        foreach (var spawn in spawnPoints)
        {
            var pos = spawn.position;
            pos.y = 1f;
            Instantiate(zombiePrefab, pos, spawn.rotation);
        }
    }

    private void OnZombieKilled(GameObject zombie)
    {
        if (isGameOver) return;
        aliveCount--;
        if (aliveCount <= 0)
        {
            isGameOver = true;
            if (gameUI != null)
                gameUI.ShowStageClear();
            Time.timeScale = 0f;
        }
    }

    private void HandlePlayerDeath()
    {
        if (isGameOver) return;
        isGameOver = true;
        if (gameUI != null)
            gameUI.ShowGameOver();
        Time.timeScale = 0f;
    }
}