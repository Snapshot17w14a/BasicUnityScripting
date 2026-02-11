using UnityEngine;

public class EnemyController : SpaceshipController
{
    private int moneyReward = 0;
    private EnemyState state = EnemyState.Chasing;

    private Vector3 rotationDelta;

    enum EnemyState
    {
        Chasing,
        Shooting
    }

    protected override void Start()
    {
        base.Start();
        bullet = Resources.Load<GameObject>("enemyBullet");
        ReadDifficultyData(LoadDifficultyData(GameManager.GameDifficulty));
    }

    protected override void Update()
    {
        base.Update();
        CalculateRotationOffset();
        DetermineState();
        switch(state)
        {
            case EnemyState.Chasing:
                MoveTowardsPlayer();
                break;
            case EnemyState.Shooting:
                if (AllowShooting()) Shoot(PlayerController.Instance.gameObject.transform.position);
                else isShooting = false;
                break;
        }
    }

    private void DetermineState()
    {
        state = Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < 200f ? EnemyState.Shooting : EnemyState.Chasing;
    }

    private void MoveTowardsPlayer()
    {
        float deviation = Mathf.Abs(rotationDelta.x) + Mathf.Abs(rotationDelta.y);
        ApplyForwardForce(1 - deviation / 360f);
    }

    private void CalculateRotationOffset()
    {
        Quaternion targetRotation = Quaternion.LookRotation(PlayerController.Instance.transform.position - transform.position);
        Vector3 eulerDelta = targetRotation.eulerAngles - transform.rotation.eulerAngles;
        eulerDelta.x = (eulerDelta.x > 180 ? eulerDelta.x - 360 : eulerDelta.x < -180 ? eulerDelta.x + 360 : eulerDelta.x) / 180f;
        eulerDelta.y = (eulerDelta.y > 180 ? eulerDelta.y - 360 : eulerDelta.y < -180 ? eulerDelta.y + 360 : eulerDelta.y) / 180f;
        rotationDelta = eulerDelta;
        ApplyHorizontalTorqe(eulerDelta.y < 0.1f ? Mathf.Lerp(0, 1, eulerDelta.y / 0.1f) : 1);
        ApplyVerticalTorqe(eulerDelta.x < 0.1f ? Mathf.Lerp(0, 1, eulerDelta.x / 0.1f) : 1);
    }

    protected override void ReadDifficultyData(DifficultyData difficultyData)
    {
        health = difficultyData.startingEnemyHealth;
        maxHealth = difficultyData.maxEnemyHealth;
        damage = difficultyData.enemyDamage;
        shootCooldown = difficultyData.enemyCooldown;
        moneyReward = difficultyData.enemyMoneyReward;
    }

    public override void Die()
    {
        PlayerController.Instance.AddMoney(moneyReward);
        EnemySpawner.EnemiesAlive--;
        base.Die();
    }
}