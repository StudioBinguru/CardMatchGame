using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiPlayerProfile : MonoBehaviour
{
    [SerializeField] private Image profileImage;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI recordText;
    [SerializeField] private Image turnIndicator;

    private int playerIndex;
    private int score;
    private int wins, draws, losses;

    private readonly Color activeColor = new Color32(0xF6, 0xFF, 0x92, 0xFF); // F6FF92
    private readonly Color inactiveColor = Color.white;

    public void Initialize(int playerNumber)
    {
        // ����, ���� �ʱ�ȭ
        UpdateScore(0);
        SetRecord(0, 0, 0);

        // �÷��̾� �ؽ�Ʈ �ʱ�ȭ
        playerIndex = playerNumber;
        scoreText.text = $"{playerNumber}P: 0��";

        recordText.text = $"0�� 0�� 0��";

        // ���� ��Ȱ��ȭ �� ���� �ʱ�ȭ
        turnIndicator.enabled = false;
        profileImage.color = Color.white;
    }
    public void ResetScoreOnly()
    {
        score = 0;
        scoreText.text = $"{playerIndex}P: 0��";
        turnIndicator.enabled = false;
        profileImage.color = Color.white;
    }
    public void UpdateScore(int newScore)
    {
        score = newScore;
        scoreText.text = $"{playerIndex}P: {score}��";
    }

    public void SetTurn(bool isActive)
    {
        turnIndicator.enabled = isActive;
        profileImage.color = isActive ? activeColor : inactiveColor;
    }

    public void SetRecord(int win, int draw, int loss)
    {
        wins = win;
        draws = draw;
        losses = loss;
        recordText.text = $"{wins}�� {draws}�� {losses}��";
    }
}
