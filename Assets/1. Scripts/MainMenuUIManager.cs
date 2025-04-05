using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void Start()
    {
        SetHighScore();
    }
    public void StartSinglePlay()
    {
        SceneManager.LoadScene("1. SinglePlay");
    }

    public void SetHighScore()
    {
        var best = GameDataManager.Instance.BestRecord;
        highScoreText.text = $"최고 기록\n Stage{Mathf.Max(1, best.stage)}\n{best.score}점";
    }
    public void SelectPlayerCount(int count)
    {
        GameSettings.SelectedPlayerCount = count;
        SceneManager.LoadScene("2. MultiPlay");
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("게임 종료");
    }
}