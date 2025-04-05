using Unity.Collections;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    [System.Serializable]
    public class HighScoreData
    {
        public int stage;
        public int score;
    }
    public static GameDataManager Instance { get; private set; }

    [SerializeField, ReadOnly] private HighScoreData bestRecord = new HighScoreData();

    public HighScoreData BestRecord => bestRecord;

    private const string Key_BestRecord = "BestRecord";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadData();
    }
    public void SaveBestRecord(int stage, int score)
    {
        if (score > bestRecord.score)
        {
            bestRecord.stage = stage;
            bestRecord.score = score;

            string json = JsonUtility.ToJson(bestRecord);
            PlayerPrefs.SetString(Key_BestRecord, json);
            PlayerPrefs.Save();
        }
    }


    private void LoadData()
    {
        string json = PlayerPrefs.GetString(Key_BestRecord, "");
        if (!string.IsNullOrEmpty(json))
        {
            bestRecord = JsonUtility.FromJson<HighScoreData>(json);
        }
    }
    public void ResetRecord()
    {
        PlayerPrefs.DeleteKey(Key_BestRecord);
        bestRecord = new HighScoreData { stage = 0, score = 0 };
        PlayerPrefs.Save();

        LoadData();
    }
}
