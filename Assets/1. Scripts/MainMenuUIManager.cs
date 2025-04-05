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
        highScoreText.text = $"�ְ� ���\n Stage{Mathf.Max(1, best.stage)}\n{best.score}��";
    }
    public void SelectPlayerCount(int count)
    {
        GameSettings.SelectedPlayerCount = count;
        SceneManager.LoadScene("2. MultiPlay");
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("���� ����");
    }
}