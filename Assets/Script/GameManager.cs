using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using System.IO;
using Unity.MLAgents;
using UnityEditor.MPE;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    
    [SerializeField] private int _gamePhase = 1;

    private static float _playTime;
    private static List<float> _phaseClearTimeList = new List<float>();

    public static int ClearCount = 0;

    public Camera MyCamera;
    public List<GameEnvironment> gameEnvironmentList = new List<GameEnvironment>();
    
    public bool IsEpisodeInit;

    public static GameManager Instance
    {
        get { return _instance; }
    }

    public static int GamePhase
    {
        get { return Instance._gamePhase; }
        set { Instance._gamePhase = value; }
    }

    private void Awake()
    {
        _instance = this;
        Init();
        DontDestroyOnLoad(gameObject);
        Random.InitState(0);
    }

    public bool _init = false;

    private void Init()
    {
        _init = false;
        ClearCount = 0;
        Instance._gamePhase = 1;
        
        _playTime = 0;
        _phaseClearTimeList.Clear();

        _init = true;
        _instance.IsEpisodeInit = true;
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
                // 카메라 위치를 해당 위치로 할당
                MyCamera.transform.position = newPosition;
            }
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            Vector3 newPosition = new Vector3(50, 150, 50);
            // 카메라 위치를 해당 위치로 할당
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
        // In Test Mode, TestManager will handle this.
        if (TestManager.Instance != null && (TestManager.Instance.IsTest || TestManager.Instance.IsEnemy))
        {
            TestManager.RestEnvrionment();
            return;
        }

        foreach (var gameEnvironment in _instance.gameEnvironmentList)
        {
            gameEnvironment.EndEpisode();
        }
    }

    public static void AddGamePhase(int value)
    {
        Instance._gamePhase = (Instance._gamePhase + value) % 8;
        if (Instance._gamePhase == 0) Instance._gamePhase = 8;
    }

    public static void AddGamePhase()
    {
        if(Instance._gamePhase <= 7)
            Instance._gamePhase++;
        Debug.Log("AddGamePhase : " + Instance._gamePhase + ", ClearTime : " + _playTime);
        _phaseClearTimeList.Add(_playTime);
        _playTime = 0;
    }

    public static void GameClear(GameEnvironment environment)
    {
        // If TestManager exists and is in test mode, delegate to it.
        if (TestManager.Instance != null && (TestManager.Instance.IsTest || TestManager.Instance.IsEnemy))
        {
            TestManager.Instance.TestGameClear(environment);
            return;
        }

        _instance.IsEpisodeInit = false;
        environment.EndEpisode();
        
        float mapLevelFloat = Academy.Instance.EnvironmentParameters.GetWithDefault("map_level", 0.0f);
        int mapLevel = Mathf.RoundToInt(mapLevelFloat) + 1;

        // 레벨이 변경되었을 때만 맵을 교체 (성능 최적화)
        if (Instance._gamePhase != mapLevel && Instance._gamePhase != 8)
        {
            Instance._gamePhase = mapLevel;
            ClearCount = 0;
            RestEnvrionment();
        }

        _instance.IsEpisodeInit = true;
    }
}