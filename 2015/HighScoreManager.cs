using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighScoreManager : MonoBehaviour
{

    private static HighScoreManager m_instance;
    private const int LeaderboardLength = 10;

    public static HighScoreManager _instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new GameObject("HighScoreManager").AddComponent<HighScoreManager>();
            }
            return m_instance;
        }
    }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void SaveHighScore(string name, string battleSessionTime, int score, int enemiesKilledCount, int bombsUsedCount)
    {
        List<Scores> HighScores = new List<Scores>();

        int i = 1;
        while (i <= LeaderboardLength && PlayerPrefs.HasKey("HighScore" + i + "score"))
        {
            Scores temp = new Scores();
            temp.score = PlayerPrefs.GetInt("HighScore" + i + "score");
            temp.name = PlayerPrefs.GetString("HighScore" + i + "name");
            temp.battleSessionTime = PlayerPrefs.GetString("HighScore" + i + "battleSessionTime");
            temp.enemiesKilledCount = PlayerPrefs.GetInt("HighScore" + i + "enemiesKilledCount");
            temp.bombsUsedCount = PlayerPrefs.GetInt("HighScore" + i + "bombsUsedCount");

            HighScores.Add(temp);
            i++;
        }
        if (HighScores.Count == 0)
        {
            Scores _temp = new Scores();
            _temp.name = name;
            _temp.score = score;
            _temp.battleSessionTime = battleSessionTime;
            _temp.enemiesKilledCount = enemiesKilledCount;
            _temp.bombsUsedCount = bombsUsedCount;

            HighScores.Add(_temp);
        }
        else
        {
            for (i = 1; i <= HighScores.Count && i <= LeaderboardLength; i++)
            {
                if (score > HighScores[i - 1].score)
                {
                    Scores _temp = new Scores();
                    _temp.name = name;
                    _temp.score = score;
                    _temp.battleSessionTime = battleSessionTime;
                    _temp.enemiesKilledCount = enemiesKilledCount;
                    _temp.bombsUsedCount = bombsUsedCount;

                    HighScores.Insert(i - 1, _temp);
                    break;
                }
                if (i == HighScores.Count && i < LeaderboardLength)
                {
                    Scores _temp = new Scores();
                    _temp.name = name;
                    _temp.score = score;
                    _temp.battleSessionTime = battleSessionTime;
                    _temp.enemiesKilledCount = enemiesKilledCount;
                    _temp.bombsUsedCount = bombsUsedCount;

                    HighScores.Add(_temp);
                    break;
                }
            }
        }

        i = 1;
        while (i <= LeaderboardLength && i <= HighScores.Count)
        {
            PlayerPrefs.SetString("HighScore" + i + "name", HighScores[i - 1].name);
            PlayerPrefs.SetInt("HighScore" + i + "score", HighScores[i - 1].score);
            PlayerPrefs.SetString("HighScore" + i + "battleSessionTime", HighScores[i - 1].battleSessionTime);
            PlayerPrefs.SetInt("HighScore" + i + "enemiesKilledCount", HighScores[i - 1].enemiesKilledCount);
            PlayerPrefs.SetInt("HighScore" + i + "bombsUsedCount", HighScores[i - 1].bombsUsedCount);

            i++;
        }

    }

    public List<Scores> GetHighScore()
    {
        List<Scores> HighScores = new List<Scores>();

        int i = 1;
        while (i <= LeaderboardLength && PlayerPrefs.HasKey("HighScore" + i + "score"))
        {
            Scores temp = new Scores();
            temp.score = PlayerPrefs.GetInt("HighScore" + i + "score");
            temp.name = PlayerPrefs.GetString("HighScore" + i + "name");
            temp.battleSessionTime = PlayerPrefs.GetString("HighScore" + i + "battleSessionTime");
            temp.enemiesKilledCount = PlayerPrefs.GetInt("HighScore" + i + "enemiesKilledCount");
            temp.bombsUsedCount = PlayerPrefs.GetInt("HighScore" + i + "bombsUsedCount");

            HighScores.Add(temp);
            i++;
        }

        return HighScores;
    }

    public void ClearLeaderBoard()
    {
        //for(int i=0;i<HighScores.
        List<Scores> HighScores = GetHighScore();

        for (int i = 1; i <= HighScores.Count; i++)
        {
            PlayerPrefs.DeleteKey("HighScore" + i + "name");
            PlayerPrefs.DeleteKey("HighScore" + i + "score");
            PlayerPrefs.DeleteKey("HighScore" + i + "battleSessionTime");
            PlayerPrefs.DeleteKey("HighScore" + i + "enemiesKilledCount");
            PlayerPrefs.DeleteKey("HighScore" + i + "bombsUsedCount");
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}

public class Scores
{
    public string battleSessionTime;
    public int score;
    public int enemiesKilledCount;
    public int bombsUsedCount;
    public string name;
}
