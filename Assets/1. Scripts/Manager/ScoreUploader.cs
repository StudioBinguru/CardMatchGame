using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ScoreUploader : MonoBehaviour
{
    public static ScoreUploader Instance { get; private set; }

    // Google Apps Script에서 발급받은 배포 URL
    [SerializeField] private string googleScriptUrl = "https://script.google.com/macros/s/AKfycbyEc7nN3GWJhyK6Fm7bDRpFQz1f4yQTxAp_GgI6XqngbLrab-OawlCwuywpq41TJoQ/exec";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 점수 전송 호출 함수
    public void UploadScore(string playerId, int stage, int score)
    {
        StartCoroutine(PostScore(playerId, stage, score));
    }

    private IEnumerator PostScore(string playerId, int stage, int score)
    {
        // 데이터 클래스 생성 후 JSON 변환
        var json = JsonUtility.ToJson(new ScoreData
        {
            playerId = playerId,
            stage = stage,
            score = score
        });

        // HTTP POST 요청 설정
        var request = new UnityWebRequest(googleScriptUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 보내기
        yield return request.SendWebRequest();

        // 결과 확인
        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log("점수 업로드 성공");
        else
            Debug.LogError("점수 업로드 실패: " + request.error);
    }

    // 내부에서 사용할 데이터 구조
    [System.Serializable]
    public class ScoreData
    {
        public string playerId;
        public int stage;
        public int score;
    }
}
