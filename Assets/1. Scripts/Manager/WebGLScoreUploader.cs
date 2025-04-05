#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;

public class WebGLScoreUploader : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void UploadScoreToFirebase(string playerId, int stage, int score);
#endif

    public static WebGLScoreUploader Instance { get; private set; }

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

    public void UploadScore(string playerId, int stage, int score)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        UploadScoreToFirebase(playerId, stage, score);
#else
        Debug.Log($"[에디터 모드] 업로드 생략: {playerId} / Stage {stage} / {score}점");
#endif
    }
}
