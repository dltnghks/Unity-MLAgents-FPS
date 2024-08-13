using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    private static int _gamePhase = 1;

    private static float _playTime;
    private static List<float> _phaseClearTimeList = new List<float>();

    public static int ClearCount = 0;
    public static int RequireClear = 1;

    public bool IsTest;

    public Camera MyCamera;
    public List<GameEnvironment> gameEnvironmentList = new List<GameEnvironment>();

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
    }

    private void Init()
    {
        ClearCount = 0;
        _gamePhase = 1;
        _playTime = 0;
        _phaseClearTimeList.Clear();
        var environmentList = GetComponentsInChildren<GameEnvironment>();
        foreach(var environment in environmentList)
        {
            gameEnvironmentList.Add(environment);
        }
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
                // 카메라 위치에 새 위치 할당
                MyCamera.transform.position = newPosition;
            }
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            Vector3 newPosition = new Vector3(50, 150, 50);
            // 카메라 위치에 새 위치 할당
            MyCamera.transform.position = newPosition;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && IsTest)
        {
            AddGamePhase(-1);
            RestEnvrionment();
        }
        if (Input.GetKeyDown(KeyCode.RightShift) && IsTest)
        {
            AddGamePhase();
            RestEnvrionment();
        }
    }

    public static void RestEnvrionment()
    {
        foreach (var gameEnvironment in _instance.gameEnvironmentList)
        {
            gameEnvironment.EndEpisode();
        }
    }

    public static void AddGamePhase(int value)
    {
        _gamePhase += value;
    }

    public static void AddGamePhase()
    {
        if(_gamePhase <= 7)
            _gamePhase++;
        _phaseClearTimeList.Add(_playTime);
    }

    public static void GameClear(GameEnvironment environment)
    {
        environment.EndEpisode();
        ClearCount++;
        if (RequireClear <= ClearCount)
        {
            RestEnvrionment();
            ClearCount = 0;
            AddGamePhase();
        }
    }
}
