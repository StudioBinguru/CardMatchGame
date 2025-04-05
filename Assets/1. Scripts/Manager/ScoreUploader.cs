#if !UNITY_WEBGL
using UnityEngine;
using System;
using Firebase.Database;
using Firebase.Extensions;

public class ScoreUploader : MonoBehaviour
{
    public static ScoreUploader Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UploadScore(string playerId, int stage, int score)
    {
        string key = FirebaseInitializer.Instance.DBReference.Child("scores").Push().Key;

        ScoreData data = new ScoreData
        {
            playerId = playerId,
            stage = stage,
            score = score,
            timestamp = DateTime.UtcNow.ToString("o")
        };

        string json = JsonUtility.ToJson(data);
        FirebaseInitializer.Instance.DBReference.Child("scores").Child(key)
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    Debug.Log("점수 업로드 완료");
                else
                    Debug.LogError("점수 업로드 실패: " + task.Exception);
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
