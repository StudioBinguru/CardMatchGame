using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ScoreUploader : MonoBehaviour
{
    public static ScoreUploader Instance { get; private set; }

    // Google Apps Script���� �߱޹��� ���� URL
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

    // ���� ���� ȣ�� �Լ�
    public void UploadScore(string playerId, int stage, int score)
    {
        StartCoroutine(PostScore(playerId, stage, score));
    }

    private IEnumerator PostScore(string playerId, int stage, int score)
    {
        // ������ Ŭ���� ���� �� JSON ��ȯ
        var json = JsonUtility.ToJson(new ScoreData
        {
            playerId = playerId,
            stage = stage,
            score = score
        });

        // HTTP POST ��û ����
        var request = new UnityWebRequest(googleScriptUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // ��û ������
        yield return request.SendWebRequest();

        // ��� Ȯ��
        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log("���� ���ε� ����");
        else
            Debug.LogError("���� ���ε� ����: " + request.error);
    }

    // ���ο��� ����� ������ ����
    [System.Serializable]
    public class ScoreData
    {
        public string playerId;
        public int stage;
        public int score;
    }
}
