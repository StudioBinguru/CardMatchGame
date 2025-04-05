using System;

[Serializable]
public class GameSessionLog
{
    public string playerId;
    public string sessionId;

    public string startTime;  // 앱 실행 시각
    public string endTime;    // 앱 종료 시각
    public float playDuration;

    public int highStage;     // 앱 실행 중 최고 스테이지
    public int highScore;     // 앱 실행 중 최고 점수

    public string platform;   // WebGL, Android, Windows 등
    public string deviceModel;
    public string deviceOS;
}