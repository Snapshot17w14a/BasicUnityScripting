using UnityEngine.SceneManagement;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] private EndScreenType endScreenType;

    private enum EndScreenType
    {
        Win,
        Lose
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(endScreenType)
        {
            case EndScreenType.Win:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SceneManager.LoadScene("Main Menu");
                }
                break;
            case EndScreenType.Lose:
                if (Input.GetKeyDown(KeyCode.R))
                {
                    SceneManager.LoadScene("Game");
                }
                break;
        }
    }
}
