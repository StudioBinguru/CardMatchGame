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
                    Debug.Log("세션 로그 업로드 완료");
                else
                    Debug.LogError("세션 로그 업로드 실패: " + task.Exception);
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
                    Debug.Log("플레이 로그 업로드 완료");
                else
                    Debug.LogError("플레이 로그 업로드 실패: " + task.Exception);
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
