#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;

public class WebGLScoreUploader : MonoBehaviour
{
    //유니티에서 JavaScript 함수 UploadSessionToFirebase() 등을 직접 호출하겠다는 의미
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void UploadSessionToFirebase(string jsonSessionLog);

    [DllImport("__Internal")]
    private static extern void UploadPlayLogToFirebase(string jsonPlayLog);
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
    public void UploadSession(GameSessionLog session)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string json = JsonUtility.ToJson(session);
        UploadSessionToFirebase(json);
#else
        Debug.Log($"[에디터 모드] 세션 업로드 생략: {session.sessionId}");
#endif
    }

    public void UploadPlayLog(PlaySessionLog playLog)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string json = JsonUtility.ToJson(playLog);
        UploadPlayLogToFirebase(json);
#else
        Debug.Log($"[에디터 모드] 플레이 로그 업로드 생략: {playLog.sessionId} / {playLog.playMode}");
#endif
    }
}
