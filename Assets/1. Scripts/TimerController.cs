using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class TimerController : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Image fillImage;

    [Header("Sprite")]
    [SerializeField] private Sprite normalFillSprite;
    [SerializeField] private Sprite warningFillSprite;

    private float totalTime;
    private float timeRemaining;
    private bool isRunning = false;

    public Action OnTimeOver;

    #region Public Methods

    // 타이머 시간 세팅
    public void SetTime(float duration)
    {
        totalTime = duration;
        timeRemaining = duration;
        isRunning = false;

        UpdateVisuals();
    }

    // 타이머 시작
    public void StartRunning()
    {
        isRunning = true;
    }

    // 타이머 강제로 멈춤
    public void StopTimer()
    {
        isRunning = false;
    }

    // 타이머를 0으로 리셋, 시각 요소도 초기화
    public void ResetTimer()
    {
        isRunning = false;
        timeRemaining = 0;
        UpdateVisuals();
    }
    public void ResetAndStart(float duration)
    {
        SetTime(duration);
        StartRunning();
    }

    #endregion

    #region Unity Callbacks

    private void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;
            OnTimeOver?.Invoke();
        }

        UpdateVisuals();
    }

    #endregion

    #region Internal Methods

    // 시간 텍스트, 슬라이더, 색상 등을 업데이트
    private void UpdateVisuals()
    {
        if (timerSlider != null)
            timerSlider.value = timeRemaining / totalTime;

        // 텍스트 (초 : 1/100초)
        if (timeText != null)
        {
            int seconds = Mathf.FloorToInt(timeRemaining);
            int hundredths = Mathf.FloorToInt((timeRemaining - seconds) * 100);
            timeText.text = $"{seconds:00}:{hundredths:00}";
        }

        // 타이머가 10초 이하로 줄어들면 빨간색으로 변경
        if (fillImage != null)
        {
            if (timeRemaining <= 10f)
            {
                fillImage.sprite = warningFillSprite;
            }
            else if (timeRemaining > 10f )
            {
                fillImage.sprite = normalFillSprite;
            }
        }
    }

    #endregion
}
