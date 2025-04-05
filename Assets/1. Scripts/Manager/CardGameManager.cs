using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class CardGameManager : MonoBehaviour, ICardFlipReceiver, ICardInputBlockProvider
{
    [System.Serializable]
    public class StageData
    {
        public CardOpenMode openMode;
        public int rows;
        public int columns;
        public float timeLimit;
        [Range(0f, 1f)] public float adjacentPairRatio = 0.5f;
    }

    public enum CardOpenMode { AllAtOnce, OneByOne }


    [Header("Time")]
    [SerializeField] protected TimerController timer;
    [SerializeField] protected float gameStartDelayTime;
    [SerializeField] protected float openDelayPerCard;
    [SerializeField] protected float correctMatchDelay = 0.1f;
    [SerializeField] protected float wrongMatchDelay = 0.5f;
    protected float currentStageTimeLimit;

    [Header("Card")]
    [SerializeField] protected Transform cardGrid;
    [SerializeField] protected GameObject cardPrefab;
    [SerializeField] protected Sprite[] cardFrontImages;
    protected List<Card> cardPool = new();

    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    protected ICardShuffler cardShuffler = new CardShuffler();
    protected List<Card> flippedCards = new();
    protected bool isInteractionBlocked = false;
    protected readonly Vector2 baseCellSize = new(80f, 100f);


    protected virtual void Awake()
    {
        foreach (Transform child in cardGrid)
        {
            Card card = child.GetComponent<Card>();
            if (card != null)
            {
                card.gameObject.SetActive(false);
                cardPool.Add(card);
            }
        }

        pauseButton.onClick.AddListener(OnPausePressed);
        resumeButton.onClick.AddListener(OnResumePressed);
        mainMenuButton.onClick.AddListener(OnMainMenuPressed);
    }
    protected abstract void Start();

    protected virtual void OnPausePressed()
    {
        Time.timeScale = 0f;
        isInteractionBlocked = true;
        pausePanel.SetActive(true);
    }

    protected virtual void OnResumePressed()
    {
        Time.timeScale = 1f;
        isInteractionBlocked = false;
        pausePanel.SetActive(false);
    }

    protected virtual void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("0. MainMenu");
    }
    protected void SetupCards(List<(int pairId, Sprite sprite)> pairInfos, int rows)
    {
        SetFixedCardScale(rows);

        for (int i = 0; i < pairInfos.Count; i++)
        {
            Card card = cardPool[i];
            card.gameObject.SetActive(true);
            card.SetCard(pairInfos[i].pairId, pairInfos[i].sprite, this, this);
        }
    }

    protected void ClearBoard()
    {
        foreach (Card card in cardPool)
        {
            card.gameObject.SetActive(false);
        }

        flippedCards.Clear();
    }

    public bool IsInteractionBlocked() => isInteractionBlocked;

    public void OnCardFlipped(Card card)
    {
        flippedCards.Add(card);
        if (flippedCards.Count == 2)
            StartCoroutine(CheckMatch());
    }

    protected virtual IEnumerator CheckMatch()
    {
        isInteractionBlocked = true;

        Card a = flippedCards[0];
        Card b = flippedCards[1];

        bool isMatch = a.PairId == b.PairId;

        //정답이면 짧은 딜레이, 오답이면 긴 딜레이
        float delay = isMatch ? correctMatchDelay : wrongMatchDelay;
        yield return new WaitForSeconds(delay);

        if (isMatch)
        {   //카드 매칭 성공
            a.SetMatched();
            b.SetMatched();

            AddScore(10);
        }
        else
        {   //카드 매칭 실패
            a.FlipBack();
            b.FlipBack();
        }

        flippedCards.Clear();
        isInteractionBlocked = false;

        CheckStageClear();
    }

    protected virtual void OnMatchSuccess() { }
    protected virtual void OnMatchFail() { }

    protected virtual void CheckStageClear()
    {
        //각 모드에서 override
    }

    private void SetFixedCardScale(int rows)
    {
        GridLayoutGroup grid = cardGrid.GetComponent<GridLayoutGroup>();

        float scale = 1f;
        if (rows <= 3) scale = 1.25f;
        else if (rows <= 4) scale = 1f;
        else if (rows <= 5) scale = 0.8f;
        else scale = 0.6f;

        grid.cellSize = baseCellSize * scale;

        foreach (Card card in cardPool)
            card.SetIconScale(scale);
    }
    protected virtual void AddScore(int value)
    {
        //각 모드에서 override
    }
    protected abstract IEnumerator PlayStage(int index);
    protected abstract void UpdateScoreUI(); // 점수 UI 업데이트 (Single / Multi에서 각각 구현)
    public void OnClickExit()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("0. MainMenu");
    }


}