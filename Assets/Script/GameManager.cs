using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using System.IO;

[System.Serializable]
public struct BattleAgent
{
    [SerializeField]
    public GameObject agent1;

    [SerializeField]
    public GameObject agent2;
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    private List<float[]> allGameData = new List<float[]>();  // ���Ǽҵ� �����͸� ������ ����Ʈ

    private static int _gamePhase = 1;

    private static float _playTime;
    private static List<float> _phaseClearTimeList = new List<float>();

    public static int ClearCount = 0;
    public static int RequireClear = 1;

    public Camera MyCamera;
    public List<GameEnvironment> gameEnvironmentList = new List<GameEnvironment>();
    
    [Header("Test Environment")]
    public bool IsTest;
    int testIndex = 0;
    [SerializeField]
    public List<BattleAgent> AgentList= new List<BattleAgent>();

    public List<GameEnvironment> testEnvironmentList = new List<GameEnvironment>();
    public int GameCount;
    public int EndGameCount;
    public int GameEpisodeCount;
    public int EndGameEpisodeCount;

    public static GameManager Instance
    {
        get { return _instance; }
    }

    public static int GamePhase
    {
        get { return _gamePhase; }
        set { _gamePhase = value; }
    }

    private void Awake()
    {
        _instance = this;
        Init();
        DontDestroyOnLoad(gameObject);
        Random.InitState(0);
    }

    private string filePath;
    private string fileName;
    private void Start()
    {
        // CSV ���� ��� ����
        filePath = Path.Combine(Application.dataPath, "game_log.csv");
        // ���� ��� �ۼ�
        //WriteToCSV(new string[] { "Episode", "Agent Name", "Kill Count", "Attack Count", "Miss Count", "Hit Count", "Death Count" });
    }

    public bool _init = false;

    private void Init()
    {
        _init = false;
        ClearCount = 0;
        _gamePhase = 1;
        if (IsTest)
        {
            _gamePhase = 8;
            GameCount = 0;
            GameEpisodeCount = 0;
            Time.timeScale = 4;
            //Debug.Log(AgentList[testIndex].agent1);
            _instance.testEnvironmentList[0].ClearEnvironment();
            _instance.testEnvironmentList[0]._playerSpawner.spawnObject = AgentList[testIndex].agent1;
            _instance.testEnvironmentList[0]._playerSpawner.spawnObject.GetComponent<GameAgents>().TeamID = 0;
            _instance.testEnvironmentList[0]._selfPlaySpawner.spawnObject = AgentList[testIndex].agent2;
            _instance.testEnvironmentList[0]._selfPlaySpawner.spawnObject.GetComponent<GameAgents>().TeamID = 1;
            _instance.testEnvironmentList[0].initialized = false;
        }
        
        _playTime = 0;
        _phaseClearTimeList.Clear();

        _init = true;
    }

    private void Update()
    {
        _playTime += Time.deltaTime;
   
        for (int i = 0; i < gameEnvironmentList.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.F1 + i))
            {
                Vector3 newPosition = gameEnvironmentList[i].transform.position;
                newPosition.y = 50.0f;
                // 카메???�치?????�치 ?�당
                MyCamera.transform.position = newPosition;
            }
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            Vector3 newPosition = new Vector3(50, 150, 50);
            // 카메???�치?????�치 ?�당
            MyCamera.transform.position = newPosition;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            AddGamePhase(-1);
            RestEnvrionment();
        }
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            AddGamePhase();
            RestEnvrionment();
        }


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = 1f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = 2f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Time.timeScale = 3f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Time.timeScale = 4f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Time.timeScale = 5f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Time.timeScale = 6f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Time.timeScale = 7f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Time.timeScale = 8f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Time.timeScale = 9f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Time.timeScale *= 2f;
        }
    }

    public static void RestEnvrionment()
    {
        if (!_instance.IsTest)
        {
            foreach (var gameEnvironment in _instance.gameEnvironmentList)
            {
                gameEnvironment.EndEpisode();
            }
        }
        else
        {
            foreach (var gameEnvironment in _instance.testEnvironmentList)
            {
                gameEnvironment.EndEpisode();
            }
        }
    }

    public static void AddGamePhase(int value)
    {
        _gamePhase = (_gamePhase + value) % 8;
        if (_gamePhase == 0) _gamePhase = 8;
    }

    public static void AddGamePhase()
    {
        if(_gamePhase <= 7)
            _gamePhase++;
        Debug.Log("AddGamePhase : " + _gamePhase + ", ClearTime : " + _playTime);
        _phaseClearTimeList.Add(_playTime);
        _playTime = 0;
    }

    public static void GameClear(GameEnvironment environment)
    {
        environment.EndEpisode();
        ClearCount++;
      
        Debug.Log("Phase : " + _gamePhase + ", " + " / " + RequireClear);
        if (RequireClear <= ClearCount && _gamePhase != 8)
        {
            if (_gamePhase >= 4)
            {
                RequireClear = 2;
            }
            ClearCount = 0;
            AddGamePhase();
            RestEnvrionment();
        }

        if (_instance.IsTest) {

            _instance.GameCount++;

            var agent1 = _instance.testEnvironmentList[0]._gameAgents;
            var agent2 = _instance.testEnvironmentList[0]._selfPlayAgents;
            
            // ���Ǽҵ帶�� �����͸� �����Ͽ� ����Ʈ�� �߰�
            _instance.allGameData.Add(new float[] {
            agent1._saveData.KillCount,
            agent1._saveData.AttackCount,
            agent1._saveData.MissCount,
            agent1._saveData.HitCount,
            agent1._saveData.DeathCount,
            agent2._saveData.KillCount,
            agent2._saveData.AttackCount,
            agent2._saveData.MissCount,
            agent2._saveData.HitCount,
            agent2._saveData.DeathCount
            });

            // ���Ǽҵ� �����͸� ���
            // (���Ǽҵ� ��ȣ, ������Ʈ �̸�, �¸�, ����, �̽�, �ǰ�, �й�)
            _instance.WriteToCSV(new string[] {
            _instance.GameEpisodeCount.ToString(),
            agent1.name,
            agent1._saveData.KillCount.ToString(),
            agent1._saveData.AttackCount.ToString(),
            agent1._saveData.MissCount.ToString(),
            agent1._saveData.HitCount.ToString(),
            agent1._saveData.DeathCount.ToString()
            }, agent1.name, agent2.name);

            _instance.WriteToCSV(new string[] {
            _instance.GameEpisodeCount.ToString(),
            agent2.name,
            agent2._saveData.KillCount.ToString(),
            agent2._saveData.AttackCount.ToString(),
            agent2._saveData.MissCount.ToString(),
            agent2._saveData.HitCount.ToString(),
            agent2._saveData.DeathCount.ToString()
            }, agent1.name, agent2.name);


            // ��ü ��� ��� (���� ���Ǽҵ尡 ����Ǹ� ��� ���)
            if (_instance.GameEpisodeCount >= _instance.EndGameEpisodeCount)
            {
                float[] totalSums = new float[10]; // agent1�� agent2 ������ �����͸� 5���� ����ϱ� ������ �� 10���� �׸�
                foreach (var data in _instance.allGameData)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        totalSums[i] += data[i];
                    }
                }

                // ��� �� ���
                float[] averages = new float[10];
                for (int i = 0; i < totalSums.Length; i++)
                {
                    averages[i] = totalSums[i] / _instance.allGameData.Count;
                }

                // ��� ���
                _instance.WriteToCSV(new string[] {
                "Average",
                agent1.name,
                averages[0].ToString(), // KillCount
                averages[1].ToString(), // AttackCount
                averages[2].ToString(), // MissCount
                averages[3].ToString(), // HitCount
                averages[4].ToString()  // DeathCount
            }, agent1.name, agent2.name);

                _instance.WriteToCSV(new string[] {
                "Average",
                agent2.name,
                averages[5].ToString(), // KillCount
                averages[6].ToString(), // AttackCount
                averages[7].ToString(), // MissCount
                averages[8].ToString(), // HitCount
                averages[9].ToString()  // DeathCount
            }, agent1.name, agent2.name);

#if UNITY_EDITOR
                _instance.testIndex++;
                if (_instance.testIndex >= _instance.AgentList.Count)
                {
                    UnityEditor.EditorApplication.isPlaying = false;
                }
                else
                {
                    _instance.Init();
                }
#else
            Application.Quit();
#endif
            }


            // ������Ʈ ������ �ʱ�ȭ
            agent1._saveData.ResetCount();
            agent2._saveData.ResetCount();

            _instance.GameEpisodeCount++;
            ClearCount = 0;
        }
    }

    private void WriteToCSV(string[] data, string agent1Name, string agent2Name)
    {
        // ���ϸ��� "{agent1�̸�}_vs_{agent2�̸�}.csv"�� ����
        string filename = $"결과/{agent1Name}1_vs_{agent2Name}2.csv";
        string filePath = Path.Combine(Application.dataPath, filename);

        // ���Ͽ� �����͸� ����
        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            string line = string.Join(",", data);
            sw.WriteLine(line);
        }
    }
}
