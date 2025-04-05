using System;
using UnityEngine;

public class GameSessionLogger : MonoBehaviour
{
    public static GameSessionLogger Instance { get; private set; }

    private GameSessionLog gameSessionLog;
    private PlaySessionLog currentPlayLog;

    private void Awake()
    {
        if (Instance != null) { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // [1] 게임 프로그램 실행 시 호출
    public void StartGameSession()
    {
        gameSessionLog = new GameSessionLog
        {
            playerId = SystemInfo.deviceUniqueIdentifier,
            sessionId = Guid.NewGuid().ToString(),
            startTime = DateTime.UtcNow.ToString("o"),
            platform = Application.platform.ToString(),
            deviceModel = SystemInfo.deviceModel,
            deviceOS = SystemInfo.operatingSystem
        };

        Debug.Log("[GameSession] Started: " + gameSessionLog.sessionId);
    }

    // [2] 게임 프로그램 종료 시 호출
    public void EndGameSession()
    {
        if (gameSessionLog == null) return;

        gameSessionLog.endTime = DateTime.UtcNow.ToString("o");
        DateTime start = DateTime.Parse(gameSessionLog.startTime);
        DateTime end = DateTime.Parse(gameSessionLog.endTime);
        gameSessionLog.playDuration = (float)(end - start).TotalSeconds;

        gameSessionLog.highStage = GameDataManager.Instance.BestRecord.stage;
        gameSessionLog.highScore = GameDataManager.Instance.BestRecord.score;

#if UNITY_WEBGL && !UNITY_EDITOR
    string json = JsonUtility.ToJson(gameSessionLog);
    Application.ExternalCall("UploadSessionToFirebase", json);
#elif !UNITY_WEBGL
        ScoreUploader.Instance.UploadSession(gameSessionLog);
#endif

        Debug.Log("[GameSession] Ended");
    }

    // [3] 싱글/멀티 모드 시작 시 호출
    public void StartPlaySession(string playMode)
    {
        if (gameSessionLog == null)
        {
            Debug.LogWarning("Game session has not started.");
            return;
        }

        currentPlayLog = new PlaySessionLog
        {
            sessionId = gameSessionLog.sessionId,
            playMode = playMode,
            playModeStartTime = DateTime.UtcNow.ToString("o")
        };

        Debug.Log("[PlaySession] Started: " + playMode);
    }

    // [4] 싱글/멀티 모드 종료 시 호출
    public void EndPlaySession(int stage, int score)
    {
        if (currentPlayLog == null)
        {
            Debug.LogWarning("Play session not started.");
            return;
        }

        currentPlayLog.playModeEndTime = DateTime.UtcNow.ToString("o");
        DateTime start = DateTime.Parse(currentPlayLog.playModeStartTime);
        DateTime end = DateTime.Parse(currentPlayLog.playModeEndTime);
        currentPlayLog.playModeDuration = (float)(end - start).TotalSeconds;

        currentPlayLog.stage = stage;
        currentPlayLog.score = score;

#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLScoreUploader.Instance.UploadPlayLog(currentPlayLog);
#elif !UNITY_WEBGL
        ScoreUploader.Instance.UploadPlayLog(currentPlayLog);
#endif

        Debug.Log("[PlaySession] Ended");
        currentPlayLog = null;
    }
    public void EndGameSessionFromWeb()
    {
        EndGameSession();
    }
    public GameSessionLog GetGameSessionLog() => gameSessionLog;
    public PlaySessionLog GetCurrentPlaySession() => currentPlayLog;
}
