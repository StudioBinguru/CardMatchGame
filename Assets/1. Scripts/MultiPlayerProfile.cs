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
        // 점수, 전적 초기화
        UpdateScore(0);
        SetRecord(0, 0, 0);

        // 플레이어 텍스트 초기화
        playerIndex = playerNumber;
        scoreText.text = $"{playerNumber}P: 0점";

        recordText.text = $"0승 0무 0패";

        // 전구 비활성화 및 색상 초기화
        turnIndicator.enabled = false;
        profileImage.color = Color.white;
    }
    public void ResetScoreOnly()
    {
        score = 0;
        scoreText.text = $"{playerIndex}P: 0점";
        turnIndicator.enabled = false;
        profileImage.color = Color.white;
    }
    public void UpdateScore(int newScore)
    {
        score = newScore;
        scoreText.text = $"{playerIndex}P: {score}점";
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
        recordText.text = $"{wins}승 {draws}무 {losses}패";
    }
}
