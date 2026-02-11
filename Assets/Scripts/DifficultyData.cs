using UnityEngine;

[CreateAssetMenu(fileName = "Player Difficulty Data", menuName = "Difficulty data object", order = 1)]
public class DifficultyData : ScriptableObject
{
    public enum Difficulty
    {
        Easy = 0,
        Normal = 1,
        Hard = 2
    }

    public static string GetDifficultyPath(Difficulty difficulty)
    {
        string path = "PersistentData/Difficulty/";
        switch (difficulty)
        {
            case Difficulty.Easy:
                path += "Easy";
                break;
            case Difficulty.Normal:
                path += "Normal";
                break;
            case Difficulty.Hard:
                path += "Hard";
                break;
            default:
                return "Unknown";
        }
        return path;
    }

    [Header("Player Settings")]
    public int maxHealth;
    public int startingHealth;
    public float initialDamage;
    public float initialCooldown;

    [Header("Enemy Settings")]
    public int maxEnemyHealth;
    public int startingEnemyHealth;
    public float enemyDamage;
    public float enemyCooldown;
    public int enemyMoneyReward;

    public override string ToString()
    {
        return GetType().Name + " - " + GetDifficultyPath(GameManager.GameDifficulty);
    }
}
