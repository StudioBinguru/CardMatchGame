using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class TimerController : MonoBehaviour
{
    #region UI Elements

    [Header("UI")]
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Image fillImage;

    #endregion

    #region Timer Sprites

    [Header("Sprite")]
    [SerializeField] private Sprite normalFillSprite;
    [SerializeField] private Sprite warningFillSprite;

    #endregion

    #region Timer State

    private float totalTime;
    private float timeRemaining;
    private bool isRunning = false;

    #endregion

    #region Events

    public Action OnTimeOver;

    #endregion

    #region Public Methods

    // 타이머를 설정합니다. 이 시점에는 흐르지 않고, 시각 요소만 보여줍니다.
    public void SetTime(float duration)
    {
        totalTime = duration;
        timeRemaining = duration;
        isRunning = false;

        UpdateVisuals();
    }

    // 타이머 흐름을 시작합니다. (SetTime 이후에 호출)
    public void StartRunning()
    {
        isRunning = true;
    }

    // 타이머를 강제로 멈춥니다.
    public void StopTimer()
    {
        isRunning = false;
    }

    // 타이머를 0으로 리셋하고 시각 요소도 초기화합니다.
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

    // 시간 텍스트, 슬라이더, 색상 등을 업데이트합니다.
    private void UpdateVisuals()
    {
        // 슬라이더 갱신
        if (timerSlider != null)
            timerSlider.value = timeRemaining / totalTime;

        // 텍스트 (초 : 1/100초)
        if (timeText != null)
        {
            int seconds = Mathf.FloorToInt(timeRemaining);
            int hundredths = Mathf.FloorToInt((timeRemaining - seconds) * 100);
            timeText.text = $"{seconds:00}:{hundredths:00}";
        }

        // Fill 이미지 경고 스프라이트 처리
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
