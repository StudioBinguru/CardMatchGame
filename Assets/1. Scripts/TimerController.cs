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

    // Ÿ�̸Ӹ� �����մϴ�. �� �������� �帣�� �ʰ�, �ð� ��Ҹ� �����ݴϴ�.
    public void SetTime(float duration)
    {
        totalTime = duration;
        timeRemaining = duration;
        isRunning = false;

        UpdateVisuals();
    }

    // Ÿ�̸� �帧�� �����մϴ�. (SetTime ���Ŀ� ȣ��)
    public void StartRunning()
    {
        isRunning = true;
    }

    // Ÿ�̸Ӹ� ������ ����ϴ�.
    public void StopTimer()
    {
        isRunning = false;
    }

    // Ÿ�̸Ӹ� 0���� �����ϰ� �ð� ��ҵ� �ʱ�ȭ�մϴ�.
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

    // �ð� �ؽ�Ʈ, �����̴�, ���� ���� ������Ʈ�մϴ�.
    private void UpdateVisuals()
    {
        // �����̴� ����
        if (timerSlider != null)
            timerSlider.value = timeRemaining / totalTime;

        // �ؽ�Ʈ (�� : 1/100��)
        if (timeText != null)
        {
            int seconds = Mathf.FloorToInt(timeRemaining);
            int hundredths = Mathf.FloorToInt((timeRemaining - seconds) * 100);
            timeText.text = $"{seconds:00}:{hundredths:00}";
        }

        // Fill �̹��� ��� ��������Ʈ ó��
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
