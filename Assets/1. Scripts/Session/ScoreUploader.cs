#if !UNITY_WEBGL
using Firebase.Extensions;
using System;
using UnityEngine;

public class ScoreUploader : MonoBehaviour
{
    public static ScoreUploader Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void UploadSession(GameSessionLog session)
    {
        string key = FirebaseInitializer.Instance.DBReference.Child("sessionLogs").Push().Key;
        string json = JsonUtility.ToJson(session);

        FirebaseInitializer.Instance.DBReference.Child("sessionLogs").Child(key)
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    Debug.Log("���� �α� ���ε� �Ϸ�");
                else
                    Debug.LogError("���� �α� ���ε� ����: " + task.Exception);
            });
    }

    public void UploadPlayLog(PlaySessionLog playLog)
    {
        string key = FirebaseInitializer.Instance.DBReference.Child("playLogs").Push().Key;
        string json = JsonUtility.ToJson(playLog);

        FirebaseInitializer.Instance.DBReference.Child("playLogs").Child(key)
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    Debug.Log("�÷��� �α� ���ε� �Ϸ�");
                else
                    Debug.LogError("�÷��� �α� ���ε� ����: " + task.Exception);
            });
    }

    [Serializable]
    public class ScoreData
    {
        public string playerId;
        public int stage;
        public int score;
        public string timestamp;
    }
}
#endif
