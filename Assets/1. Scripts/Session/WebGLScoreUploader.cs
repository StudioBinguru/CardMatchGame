#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;

public class WebGLScoreUploader : MonoBehaviour
{
    //����Ƽ���� JavaScript �Լ� UploadSessionToFirebase() ���� ���� ȣ���ϰڴٴ� �ǹ�
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
        Debug.Log($"[������ ���] ���� ���ε� ����: {session.sessionId}");
#endif
    }

    public void UploadPlayLog(PlaySessionLog playLog)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string json = JsonUtility.ToJson(playLog);
        UploadPlayLogToFirebase(json);
#else
        Debug.Log($"[������ ���] �÷��� �α� ���ε� ����: {playLog.sessionId} / {playLog.playMode}");
#endif
    }
}
