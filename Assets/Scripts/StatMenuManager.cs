using UnityEngine;
using UnityEngine.SceneManagement;

public class StatMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject difficultyPanel;

    public void ToggleDifficultyButtons()
    {
        difficultyPanel.SetActive(!difficultyPanel.activeSelf);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SetDifficulty(int difficulty)
    {
        GameManager.GameDifficulty = (DifficultyData.Difficulty)difficulty;
        SceneManager.LoadScene("Game");
    }
}