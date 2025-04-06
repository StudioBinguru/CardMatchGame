using System;

[Serializable]
public class PlaySessionLog
{
    public string sessionId;  // GameSession�� ����
    public string playMode;   // Single or Multi

    public string playModeStartTime;
    public string playModeEndTime;
    public float playModeDuration;

    public int stage;
    public int score;
}
