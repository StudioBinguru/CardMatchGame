using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SinglePlayManager : CardGameManager
{
    [Header("Stage Info")]
    [SerializeField] protected int currentStage = 0;
    [SerializeField] protected int currentScore = 0;
    [SerializeField] protected List<StageData> stages;

    [Header("Stage Result Panel")]
    [SerializeField] private GameObject stageResultPanel;
    [SerializeField] private TextMeshProUGUI clearText;
    [SerializeField] private Button continueButton;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button retryButton;

    [Header("Stage All Clear")]
    [SerializeField] private GameObject stageAllClearPanel;

    [Header("Ready To Start")]
    [SerializeField] private GameObject readyToStart;
    [SerializeField] private GameObject stageInfoObj;
    [SerializeField] private TextMeshProUGUI stageInfoText;
    [SerializeField] private GameObject startTextObj;
    [SerializeField] private float stageIntroDelay = 1.2f;

    [Header("Score Text")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject newRecordStar;

    // 디버그
    [Header("Debug Settings")]
    [SerializeField] protected bool debugMode = false;

    [Header("Debug Usage Tracking")]
    [SerializeField] protected List<string> debugImageNames = new();
    [SerializeField] protected List<int> debugImageUseCounts = new();

    protected Dictionary<string, int> imageUsageStats = new();


    protected override void Start()
    {
        if (debugMode)
        {
            InitImageStats();
            StartCoroutine(DebugShuffleRoutine());
        }
        else
        {
            continueButton.onClick.AddListener(OnClickContinue);
            retryButton.onClick.AddListener(OnClickRetry);

            GameDataManager.HighScoreData best = GameDataManager.Instance.BestRecord;
            highScoreText.text = $"<color=#0E58FF>최고 기록</color>\n Stage{Mathf.Max(1, best.stage)}\n{best.score}점";

            currentScore = 0;
            scoreText.text = $"현재 기록\n Stage{currentStage + 1}\n{currentScore}점";

            StartCoroutine(PlayStage(currentStage));
        }
    }

    protected override IEnumerator PlayStage(int index)
    {
        yield return StartCoroutine(ShowStageIntro(index));

        ClearBoard();
        StageData data = stages[index];
        GenerateCardsFromStage(currentStage);
        currentStageTimeLimit = data.timeLimit;
        timer.SetTime(currentStageTimeLimit);

        if (data.openMode == CardOpenMode.AllAtOnce)
        {
            foreach (Card card in cardPool)
                if (card.gameObject.activeSelf) card.ShowFront();

            yield return new WaitForSeconds(gameStartDelayTime);

            foreach (Card card in cardPool)
                if (card.gameObject.activeSelf) card.FlipBack();

            isInteractionBlocked = false;
        }
        else
        {
            isInteractionBlocked = true;

            foreach (Card card in cardPool)
                if (card.gameObject.activeSelf) card.FlipBackImmediately();

            for (int i = 0; i < cardPool.Count; i++)
            {
                Card card = cardPool[i];
                if (!card.gameObject.activeSelf) continue;

                card.ShowFront();
                yield return new WaitForSeconds(openDelayPerCard);
                card.FlipBack();
            }

            isInteractionBlocked = false;
        }

        timer.StartRunning();
        timer.OnTimeOver = OnTimeOut;
    }
    private void GenerateCardsFromStage(int stageIndex)
    {
        StageData data = stages[stageIndex];

        int totalCards = data.rows * data.columns;
        int pairCount = totalCards / 2;

        if (cardFrontImages.Length < pairCount)
        {
            Debug.LogError($"카드 이미지 부족. 필요한 수: {pairCount}, 보유 수: {cardFrontImages.Length}");
            return;
        }

        var cardInfos = cardShuffler.GenerateShuffledCards(
            data.rows,
            data.columns,
            cardFrontImages,
            data.adjacentPairRatio
        );

        SetupCards(cardInfos, data.rows);
    }
    private IEnumerator ShowStageIntro(int index)
    {
        readyToStart.SetActive(true);
        // 텍스트 세팅
        stageInfoText.text = $"Stage {index + 1}";

        // Stage 텍스트 표시
        DOTween.Restart("StageInfoFade");
        stageInfoObj.SetActive(true);
        yield return new WaitForSeconds(stageIntroDelay);
        stageInfoObj.SetActive(false);

        // Start 텍스트 표시
        DOTween.Restart("StartTextFade");
        startTextObj.SetActive(true);
        yield return new WaitForSeconds(stageIntroDelay);
        startTextObj.SetActive(false);
        readyToStart.SetActive(false);
    }
    protected override void CheckStageClear()
    {
        foreach (Card card in cardPool)
        {
            if (!card.gameObject.activeSelf) continue;
            if (!card.IsMatched) return;
        }

        timer.StopTimer();
        isInteractionBlocked = true;

        if (currentStage + 1 >= stages.Count)
        {
            // 마지막 스테이지 클리어
            stageAllClearPanel.SetActive(true);
            Debug.Log("마지막 스테이지 완료!");
        }
        else
        {
            // 일반 스테이지 클리어
            clearText.text = $"Stage {currentStage + 1} 클리어!";
            StartCoroutine(ShowClearPanelWithDelay(0.2f));
        }
    }

    private IEnumerator ShowClearPanelWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        stageResultPanel.SetActive(true);
    }

    private void OnTimeOut()
    {
        timer.StopTimer();
        isInteractionBlocked = true;

        string playerId = SystemInfo.deviceUniqueIdentifier; // 또는 원하는 방식으로
        ScoreUploader.Instance.UploadScore(playerId, currentStage + 1, currentScore);

        gameOverPanel.SetActive(true);
    }
    private void OnClickRetry()
    {
        gameOverPanel.SetActive(false);

        currentStage = 0;               // 스테이지 초기화
        ClearBoard();                   // 카드 상태 초기화

        StartCoroutine(PlayStage(currentStage)); // 첫 스테이지부터 재시작
    }
    private void OnClickContinue()
    {
        stageResultPanel.SetActive(false);

        foreach (Card card in cardPool)
        {
            card.gameObject.SetActive(false);
        }

        currentStage++;
        UpdateScoreTexts();

        StartCoroutine(PlayStage(currentStage));
    }

    protected override void AddScore(int value)
    {
        if (GameDataManager.Instance == null)
            return;

        currentScore += value;
        UpdateScoreUI();

        // 최고 기록 즉시 저장
        GameDataManager.Instance.SaveBestRecord(currentStage + 1, currentScore);
    }
    protected override void UpdateScoreUI()
    {
        UpdateScoreTexts();
    }
    private void UpdateScoreTexts()
    {
        if (GameDataManager.Instance == null)
            return;

        scoreText.text = $"현재 기록\n Stage{currentStage + 1}\n{currentScore}점";

        var best = GameDataManager.Instance.BestRecord;
        if (currentStage + 1 > best.stage || currentScore > best.score)
        {
            newRecordStar.SetActive(true);
            highScoreText.text = $"<color=#0E58FF>최고 기록</color>\n Stage{currentStage + 1}\n{currentScore}점";
        }
    }

    #region Debug
    protected IEnumerator DebugShuffleRoutine()
    {
        InitImageStats();

        while (true)
        {
            StageData data = stages[currentStage];
            GenerateCardsFromStage(currentStage);
            yield return null;

            ValidateCardPairs();
            UpdateImageUsageStats();

            yield return new WaitForSeconds(1f);
        }
    }

    private void ValidateCardPairs()
    {
        Dictionary<int, int> pairCount = new();

        foreach (Card card in cardPool)
        {
            if (!card.gameObject.activeSelf) continue;

            int id = card.PairId;
            if (!pairCount.ContainsKey(id))
                pairCount[id] = 0;

            pairCount[id]++;
        }

        foreach (var kv in pairCount)
        {
            if (kv.Value != 2)
                Debug.LogWarning($"Pair ID {kv.Key} has {kv.Value} cards (should be 2)");
        }
    }

    private void InitImageStats()
    {
        debugImageNames.Clear();
        debugImageUseCounts.Clear();
        imageUsageStats.Clear();

        foreach (var sprite in cardFrontImages)
        {
            string name = sprite.name;
            debugImageNames.Add(name);
            debugImageUseCounts.Add(0);
            imageUsageStats[name] = 0;
        }
    }

    private void UpdateImageUsageStats()
    {
        foreach (Card card in cardPool)
        {
            if (!card.gameObject.activeSelf) continue;
            string name = card.GetIcon().name;

            if (imageUsageStats.ContainsKey(name))
                imageUsageStats[name]++;
        }

        for (int i = 0; i < debugImageNames.Count; i++)
        {
            string name = debugImageNames[i];
            debugImageUseCounts[i] = imageUsageStats.ContainsKey(name) ? imageUsageStats[name] : 0;
        }
    }
    #endregion

}
