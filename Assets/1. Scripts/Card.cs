using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("Card Info")]
    [SerializeField] private int pairId;

    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private RectTransform iconRect;
    [SerializeField] private GameObject front;
    [SerializeField] private GameObject back;

    [Header("Flip Settings")]
    [SerializeField] private float flipDuration = 0.2f;

    private Button button;
    private Coroutine flipRoutine;

    private ICardFlipReceiver receiver;
    private ICardInputBlockProvider inputBlockProvider;

    private bool isFlipped = false;
   [SerializeField] private bool isMatched = false;

    public int PairId => pairId;
    public bool IsMatched => isMatched;

    private static readonly Color normalColor = Color.white;
    private static readonly Color flippingColor = new Color(0.776f, 0.776f, 0.776f); // C6C6C6
    private readonly Vector2 baseIconSize = new Vector2(60f, 60f);

    // 카드 정보 초기화
    public void SetCard(int index, Sprite image, ICardFlipReceiver receiver, ICardInputBlockProvider blockProvider)
    {
        this.pairId = index;
        this.icon.sprite = image;
        this.receiver = receiver;
        this.inputBlockProvider = blockProvider;

        isMatched = false;
        isFlipped = false;

        button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(Flip);

        ShowFront(); // 기본적으로 앞면 보여주기
    }

    // 카드 앞면 보여주기 (즉시)
    public void ShowFront()
    {
        front.SetActive(true);
        back.SetActive(false);
        isFlipped = true;
    }

    // 카드 뒷면 보여주기 (즉시)
    public void FlipBackImmediately()
    {
        isFlipped = false;
        front.SetActive(false);
        back.SetActive(true);
        transform.localScale = Vector3.one;
        SetCardColor(normalColor);
    }

    // 유저 입력으로 카드 뒤집기
    public void Flip()
    {
        if (isFlipped || isMatched) return;
        if (inputBlockProvider?.IsInteractionBlocked() == true) return;
        if (flipRoutine != null) return;

        isFlipped = true;
        flipRoutine = StartCoroutine(FlipRoutine(true));
        receiver?.OnCardFlipped(this);
    }

    // 카드 뒷면으로 뒤집기
    public void FlipBack()
    {
        if (!isFlipped || isMatched) return;
        if (flipRoutine != null) return;

        isFlipped = false;
        flipRoutine = StartCoroutine(FlipRoutine(false));
    }

    // 정답 처리
    public void SetMatched()
    {
        isMatched = true;
    }

    // 카드 애니메이션 처리
    private IEnumerator FlipRoutine(bool toFront)
    {
        float time = 0f;
        Vector3 originalScale = transform.localScale;

        SetCardColor(flippingColor);

        while (time < flipDuration)
        {
            float progress = time / flipDuration;
            float scale = Mathf.Lerp(1f, 0f, progress);
            transform.localScale = new Vector3(scale, 1f, 1f);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = new Vector3(0f, 1f, 1f);
        front.SetActive(toFront);
        back.SetActive(!toFront);

        time = 0f;

        while (time < flipDuration)
        {
            float progress = time / flipDuration;
            float scale = Mathf.Lerp(0f, 1f, progress);
            transform.localScale = new Vector3(scale, 1f, 1f);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        SetCardColor(normalColor);
        flipRoutine = null; // 종료 처리
    }

    private void SetCardColor(Color color)
    {
        if (icon != null)
            icon.color = color;

        if (front.TryGetComponent(out Image frontImg))
            frontImg.color = color;

        if (back.TryGetComponent(out Image backImg))
            backImg.color = color;
    }

    public void SetIconScale(float scale)
    {
        iconRect.sizeDelta = baseIconSize * scale;
    }

    public Sprite GetIcon()
    {
        return icon.sprite;
    }
}
