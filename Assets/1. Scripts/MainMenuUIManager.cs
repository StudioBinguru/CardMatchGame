using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button resetButton;
    [SerializeField] private GameObject resetRecordPanel;

    private void Start()
    {
        SetHighScore();
        resetButton.onClick.AddListener(() =>
        {
            GameDataManager.Instance.ResetRecord();
            SetHighScore();                          // 텍스트 갱신
            resetRecordPanel.SetActive(false);       // 패널 비활성화
        });
    }
    public void StartSinglePlay()
    {
        SceneManager.LoadScene("1. SinglePlay");
    }

    public void SetHighScore()
    {
        var best = GameDataManager.Instance.BestRecord;
        highScoreText.text = $"<color=#0E58FF>최고 기록</color>\n[Stage {Mathf.Max(1, best.stage)}] {best.score}점";
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