using System;

[Serializable]
public class GameSessionLog
{
    public string playerId;
    public string sessionId;

    public string startTime;  // �� ���� �ð�
    public string endTime;    // �� ���� �ð�
    public float playDuration;

    public int highStage;     // �� ���� �� �ְ� ��������
    public int highScore;     // �� ���� �� �ְ� ����

    public string platform;   // WebGL, Android, Windows ��
    public string deviceModel;
    public string deviceOS;
}