using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class MultiPlayManager : CardGameManager
{
    [Header("Multiplayer Settings")]
    [SerializeField] private StageData multiPlayStage; // 인스펙터에서 설정한 멀티 스테이지 데이터
    [SerializeField] private int playerCount;
    [SerializeField] private int currentPlayerIndex = 0;
    [SerializeField] private List<MultiPlayerProfile> playerProfiles = new();

    [Header("Ready To Start")]
    [SerializeField] private GameObject readyToStart;
    [SerializeField] private GameObject readyTextObj;
    [SerializeField] private GameObject startTextObj;
    [SerializeField] private float stageIntroDelay = 1.2f;


    [Header("Game Result Panel")]
    [SerializeField] private GameObject gameResultPanel;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Button retryButton;
    private bool isGameOver = false;

    private int[] playerScores;
    private int lastWinnerIndex = 0;
    protected override void Start()
    {
        playerCount = GameSettings.SelectedPlayerCount;
        playerScores = new int[playerCount];

        InitializeProfiles(playerCount);
        StartCoroutine(PlayStage(0)); // index는 의미 없지만 CardGameManager 구조 맞추기 위해 유지
    }
    private void InitializeProfiles(int count)
    {
        for (int i = 0; i < playerProfiles.Count; i++)
        {
            if (i < count)
            {
                playerProfiles[i].gameObject.SetActive(true);
                playerProfiles[i].Initialize(i + 1); // 플레이어 번호만 전달
            }
            else
            {
                playerProfiles[i].gameObject.SetActive(false);
            }
        }
    }

    protected override IEnumerator PlayStage(int index)
    {
        ClearBoard();

        yield return StartCoroutine(ShowStageIntro());
        // 카드 생성
        GenerateCards(multiPlayStage.rows, multiPlayStage.columns, multiPlayStage.adjacentPairRatio);

        currentStageTimeLimit = multiPlayStage.timeLimit;
        currentPlayerIndex = index;
        UpdateTurnUI(currentPlayerIndex);
        timer.SetTime(currentStageTimeLimit);

        // 카드 오픈 방식
        if (multiPlayStage.openMode == CardOpenMode.AllAtOnce)
        {
            foreach (Card card in cardPool)
                if (card.gameObject.activeSelf) card.ShowFront();

            yield return new WaitForSeconds(gameStartDelayTime);

            foreach (Card card in cardPool)
                if (card.gameObject.activeSelf) card.FlipBack();
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
        }

        isInteractionBlocked = false;

        yield return new WaitForSeconds(0.2f); // 연출 대기 후

        StartPlayerTurn(index);
    }
    private void GenerateCards(int rows, int cols, float adjacentRatio)
    {
        int totalCards = rows * cols;
        int pairCount = totalCards / 2;

        var cardInfos = cardShuffler.GenerateShuffledCards(
            rows,
            cols,
            cardFrontImages,
            adjacentRatio
        );

        SetupCards(cardInfos, rows);
    }
    private IEnumerator ShowStageIntro()
    {
        readyToStart.SetActive(true);

        // Stage 텍스트 표시
        DOTween.Restart("ReadyTextFade");
        readyTextObj.SetActive(true);
        yield return new WaitForSeconds(stageIntroDelay);
        readyTextObj.SetActive(false);

        // Start 텍스트 표시
        DOTween.Restart("StartTextFade");
        startTextObj.SetActive(true);
        yield return new WaitForSeconds(stageIntroDelay);
        startTextObj.SetActive(false);

        readyToStart.SetActive(false);
    }
    private void StartPlayerTurn(int index)
    {
        currentPlayerIndex = index;

        for (int i = 0; i < playerCount; i++)
        {
            bool isActive = (i == currentPlayerIndex);
            playerProfiles[i].SetTurn(isActive);
        }

        timer.SetTime(currentStageTimeLimit);
        timer.OnTimeOver = HandleTurnTimeout;
        timer.StartRunning();
    }
    private void HandleTurnTimeout()
    {
        int nextPlayer = (currentPlayerIndex + 1) % playerCount;
        StartPlayerTurn(nextPlayer);
    }
    protected override IEnumerator CheckMatch()
    {
        isInteractionBlocked = true;

        Card a = flippedCards[0];
        Card b = flippedCards[1];

        bool isMatch = a.PairId == b.PairId;
        float delay = isMatch ? correctMatchDelay : wrongMatchDelay;

        yield return new WaitForSeconds(delay);

        if (isMatch)
        {
            a.SetMatched();
            b.SetMatched();

            playerScores[currentPlayerIndex] += 10;
            playerProfiles[currentPlayerIndex].UpdateScore(playerScores[currentPlayerIndex]);

            flippedCards.Clear();
            isInteractionBlocked = false;

            CheckStageClear();

            // 게임이 종료되지 않았다면, 턴 유지 (타이머 다시 시작)
            if (!isGameOver)
            {
                timer.SetTime(currentStageTimeLimit);
                timer.StartRunning();
            }
        }
        else
        {
            a.FlipBack();
            b.FlipBack();

            flippedCards.Clear();
            isInteractionBlocked = false;

            NextTurn(); // 오답 시 다음 턴으로 전환
        }
    }
    private void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % playerCount;

        // UI 갱신
        for (int i = 0; i < playerProfiles.Count; i++)
        {
            playerProfiles[i].SetTurn(i == currentPlayerIndex);
        }

        // 타이머 재시작
        timer.SetTime(currentStageTimeLimit);
        timer.StartRunning();
    }
    private void UpdateTurnUI(int playerIndex)
    {
        for (int i = 0; i < playerProfiles.Count; i++)
        {
            playerProfiles[i].SetTurn(i == playerIndex);
        }
    }
    protected override void UpdateScoreUI()
    {
        // 플레이어별 점수 UI 처리 예정
    }

    protected override void CheckStageClear()
    {
        if (isGameOver) return;

        foreach (Card card in cardPool)
        {
            if (!card.gameObject.activeSelf) continue;
            if (!card.IsMatched) return;
        }

        isGameOver = true;           // 중복 호출 방지
        timer.StopTimer();           // 타이머 중단
        isInteractionBlocked = true; // 카드 입력 차단

        int maxScore = -1;
        List<int> topPlayers = new();

        // 최고 점수자 탐색
        for (int i = 0; i < playerCount; i++)
        {
            if (playerScores[i] > maxScore)
            {
                maxScore = playerScores[i];
                topPlayers.Clear();
                topPlayers.Add(i);
            }
            else if (playerScores[i] == maxScore)
            {
                topPlayers.Add(i);
            }
        }

        // 전적 및 결과 텍스트 설정
        string resultText;

        if (topPlayers.Count == 1)
        {
            int winner = topPlayers[0];
            lastWinnerIndex = winner;
            resultText = $"{winner + 1}P 승리!";

            for (int i = 0; i < playerCount; i++)
            {
                if (i == winner)
                    playerProfiles[i].SetRecord(1, 0, 0); // 승
                else
                    playerProfiles[i].SetRecord(0, 0, 1); // 패
            }
        }
        else
        {
            // 무승부 처리
            for (int i = 0; i < playerCount; i++)
            {
                if (topPlayers.Contains(i))
                    playerProfiles[i].SetRecord(0, 1, 0); // 무
                else
                    playerProfiles[i].SetRecord(0, 0, 1); // 패
            }

            // 무승부자 목록 텍스트 생성
            List<string> drawPlayerTexts = new();
            foreach (int index in topPlayers)
                drawPlayerTexts.Add($"{index + 1}P");

            string joinedNames = string.Join(", ", drawPlayerTexts);
            resultText = $"{joinedNames} 무승부!";

            // 무승부자 중 랜덤으로 다음 게임의 시작 플레이어 선택
            int randomIndex = Random.Range(0, topPlayers.Count);
            lastWinnerIndex = topPlayers[randomIndex];
        }


        // UI 처리
        winnerText.text = resultText;
        gameResultPanel.SetActive(true);

        retryButton.onClick.RemoveAllListeners();
        retryButton.onClick.AddListener(RestartGame);
    }
    private void RestartGame()
    {
        gameResultPanel.SetActive(false);
        isGameOver = false;

        for (int i = 0; i < playerCount; i++)
        {
            playerScores[i] = 0;
            playerProfiles[i].ResetScoreOnly();
        }

        ClearBoard();
        StartCoroutine(PlayStage(lastWinnerIndex));
    }

}