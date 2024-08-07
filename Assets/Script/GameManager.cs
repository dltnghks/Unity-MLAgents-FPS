using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    private static int _gamePhase = 1;

    private float _playTime;
    private List<float> _phaseClearTimeList = new List<float>();
    public bool IsTrainning;

    public static GameManager Instance
    {
        get { return _instance; }
    }

    public static int GamePhase
    {
        get { return _gamePhase; }
    }

    private void Start()
    {
        _instance = this;
        Init();
        DontDestroyOnLoad(gameObject);
    }

    private void Init()
    {
        _gamePhase = 0;
        _playTime = 0;
        _phaseClearTimeList.Clear();
    }

    private void Update()
    {
        _playTime += Time.deltaTime;
    }

    public void AddGamePhase()
    {
        _gamePhase++;
        _phaseClearTimeList.Add(_playTime);
    }
}
