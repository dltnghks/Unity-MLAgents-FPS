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
        // CSV ÆÄÀÏ °æ·Î ¼³Á¤
        filePath = Path.Combine(Application.dataPath, "game_log.csv");

        // ÆÄÀÏ Çì´õ ÀÛ¼º
        WriteToCSV(new string[] { "Episode", "Agent Name", "Kill Count", "Attack Count", "Miss Count", "Hit Count", "Death Count" });
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
            Time.timeScale = 9;
        }
        
        _playTime = 0;
        _phaseClearTimeList.Clear();

        Debug.Log(AgentList[testIndex].agent1);
        _instance.testEnvironmentList[0].ClearEnvironment();
        _instance.testEnvironmentList[0]._playerSpawner.spawnObject = AgentList[testIndex].agent1;
        _instance.testEnvironmentList[0]._playerSpawner.spawnObject.GetComponent<GameAgents>().TeamID = 0;
        _instance.testEnvironmentList[0]._selfPlaySpawner.spawnObject = AgentList[testIndex].agent2;
        _instance.testEnvironmentList[0]._selfPlaySpawner.spawnObject.GetComponent<GameAgents>().TeamID = 1;
        _init = true;
        _instance.testEnvironmentList[0].initialized = false;
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
                // ì¹´ë©”???„ì¹˜?????„ì¹˜ ? ë‹¹
                MyCamera.transform.position = newPosition;
            }
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            Vector3 newPosition = new Vector3(50, 150, 50);
            // ì¹´ë©”???„ì¹˜?????„ì¹˜ ? ë‹¹
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
        _instance.GameCount++;
        //Debug.Log(_gamePhase + " : " + ClearCount + " , " + RequireClear);

        if (RequireClear <= ClearCount && _gamePhase != 8 && !_instance.IsTest)
        {
            RestEnvrionment();
            AddGamePhase();
            ClearCount = 0;
            if (_gamePhase >= 5)
                RequireClear = 2;
        }

        if (_instance.EndGameCount <= ClearCount && _instance.IsTest)
        {
            var agent1 = _instance.testEnvironmentList[0]._gameAgents;
            Debug.Log(_instance.GameEpisodeCount  + "name : " + agent1.name);
            Debug.Log(_instance.GameEpisodeCount + "KillCount : " + agent1._saveData.KillCount);
            Debug.Log(_instance.GameEpisodeCount + "AttackCount : " + agent1._saveData.AttackCount);
            Debug.Log(_instance.GameEpisodeCount + "MissCount : " + agent1._saveData.MissCount);
            Debug.Log(_instance.GameEpisodeCount + "HitCount : " + agent1._saveData.HitCount);
            Debug.Log(_instance.GameEpisodeCount + "DeathCount : " + agent1._saveData.DeathCount);
            
            var agent2 = _instance.testEnvironmentList[0]._selfPlayAgents;
            Debug.Log(_instance.GameEpisodeCount + "name : " + agent2.name);
            Debug.Log(_instance.GameEpisodeCount + "KillCount : " + agent2._saveData.KillCount);
            Debug.Log(_instance.GameEpisodeCount + "AttackCount : " + agent2._saveData.AttackCount);
            Debug.Log(_instance.GameEpisodeCount + "MissCount : " + agent2._saveData.MissCount);
            Debug.Log(_instance.GameEpisodeCount + "HitCount : " + agent2._saveData.HitCount);
            Debug.Log(_instance.GameEpisodeCount + "DeathCount : " + agent2._saveData.DeathCount);
            Debug.Log(_instance.GameEpisodeCount + "==============================================");


            _instance.WriteToCSV(new string[] {
                _instance.GameEpisodeCount.ToString(),
                agent1.name,
                agent1._saveData.KillCount.ToString(),
                agent1._saveData.AttackCount.ToString(),
                agent1._saveData.MissCount.ToString(),
                agent1._saveData.HitCount.ToString(),
                agent1._saveData.DeathCount.ToString()
            });

            _instance.WriteToCSV(new string[] {
                _instance.GameEpisodeCount.ToString(),
                agent2.name,
                agent2._saveData.KillCount.ToString(),
                agent2._saveData.AttackCount.ToString(),
                agent2._saveData.MissCount.ToString(),
                agent2._saveData.HitCount.ToString(),
                agent2._saveData.DeathCount.ToString()
            });

            agent1._saveData.ResetCount();
            agent2._saveData.ResetCount();

            _instance.GameEpisodeCount++;
            ClearCount = 0;
            if (_instance.GameEpisodeCount >= _instance.EndGameEpisodeCount)
            {
#if UNITY_EDITOR
                _instance.testIndex++;
                if (_instance.testIndex >= 4)
                {
                    UnityEditor.EditorApplication.isPlaying = false;
                }
                else
                {
                    _instance.Init();
                }
#else
        Application.Quit(); // ?´í”Œë¦¬ì??´ì…˜ ì¢…ë£Œ
#endif
            }
        }
    }

    private void WriteToCSV(string[] data)
    {
        // ÆÄÀÏ¿¡ µ¥ÀÌÅÍ¸¦ ¾²±â
        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            string line = string.Join(",", data);
            sw.WriteLine(line);
        }
    }
}
