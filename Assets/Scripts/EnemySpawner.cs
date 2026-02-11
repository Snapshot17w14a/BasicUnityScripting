using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, IDamagable, IPausable
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnDistance = 20f;
    [SerializeField] private float spawnRate = 15f;
    [SerializeField] private int maxEnemies = 4;

    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;

    public static int EnemiesAlive = 0;

    public int Health => health;
    public int MaxHealth => maxHealth;

    private bool isPaused = false;
    public bool IsPaused => isPaused;

    private void Start()
    {
        PauseManager.OnPauseStateChanged += SetState;
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            Vector3 spawnPosition = transform.position + Random.onUnitSphere * spawnDistance;
            if (!IsPaused && EnemiesAlive < maxEnemies) Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            EnemiesAlive++;
            yield return new WaitForSeconds(spawnRate);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= (int)damage;
        if (health <= 0) Die();
    }

    public void Heal(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
    }

    public void SetState(bool pause) => isPaused = pause;

    public void Die() => GameManager.Instance.Victory();

    private void OnDestroy()
    {
        PauseManager.OnPauseStateChanged -= SetState;
    }
}
