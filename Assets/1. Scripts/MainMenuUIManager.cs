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
            SetHighScore();                          // �ؽ�Ʈ ����
            resetRecordPanel.SetActive(false);       // �г� ��Ȱ��ȭ
        });
    }
    public void StartSinglePlay()
    {
        SceneManager.LoadScene("1. SinglePlay");
    }

    public void SetHighScore()
    {
        var best = GameDataManager.Instance.BestRecord;
        highScoreText.text = $"<color=#0E58FF>�ְ� ���</color>\n[Stage {Mathf.Max(1, best.stage)}] {best.score}��";
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