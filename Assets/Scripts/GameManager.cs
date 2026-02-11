using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Fill bars")]
    [SerializeField] private FillBar hullIntegrity;
    [SerializeField] private FillBar gunOverheat;
    [SerializeField] private FillBar throttleBar;

    [Header("UI elements")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject pauseCanvas;

    private static DifficultyData.Difficulty gameDifficulty = DifficultyData.Difficulty.Normal;
    public static DifficultyData.Difficulty GameDifficulty
    {
        get => gameDifficulty;
        set => gameDifficulty = value;
    }

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.visible = false;
        PauseManager.SetGameState(true);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    public void EndTutorial()
    {
        tutorialCanvas.SetActive(false);
        mainCanvas.SetActive(true);
        PauseManager.SetGameState(false);
    }

    private void UpdateUI()
    {
        PlayerController player = PlayerController.Instance;
        hullIntegrity.SetFill((float)player.Health / (float)player.MaxHealth);
        gunOverheat.SetFill(player.CurrentOverheat);
        throttleBar.SetFill(player.Throttle);
        moneyText.text = player.CurrentMoney.ToString();
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }

    public void GameOver()
    {
        SceneManager.LoadScene("Game Over");
    }

    public void Victory()
    {
        SceneManager.LoadScene("Victory");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Exit()
    {
       Application.Quit();
    }
}
