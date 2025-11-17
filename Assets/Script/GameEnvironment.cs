using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AI;

public class GameEnvironment : MonoBehaviour
{
    public PlayerSpawner _playerSpawner;
    public EnemySpawner _enemySpawner;
    public ObstacleSpawner _obstacleSpawner;
    public SelfPlaySpawner _selfPlaySpawner;

    public GameAgents _gameAgents;
    public GameAgents _selfPlayAgents;
    public NonPlayerCharacter Enemy;

    [Header("Agents")]
    public List<Controller> ControllerList = new List<Controller>();
    
    public float MapSize = 40;


    public bool initialized;


    void Initialize()
    {
        MapSize *= transform.localScale.x;
        _playerSpawner = GetComponentInChildren<PlayerSpawner>();
        _enemySpawner = GetComponentInChildren<EnemySpawner>();
        _obstacleSpawner = GetComponentInChildren<ObstacleSpawner>();
        _selfPlaySpawner = GetComponentInChildren<SelfPlaySpawner>();
        StartEpisode();
    }

    IEnumerator Delay()
    {
        _playerSpawner.Clear();
        _enemySpawner.Clear();
        _obstacleSpawner.Clear();
        // ?�동 ???��??�간만큼 ?��?
        yield return new WaitForSeconds(1f);
        StartEpisode();
    }

    private int[] testPointList = { 0, 2, 4, 6};
    private int randomIndex = 0;

    public void StartEpisode()
    {
        _playerSpawner.Clear();
        _enemySpawner.Clear();
        _obstacleSpawner.Clear();
        _selfPlaySpawner.Clear();
        ControllerList.Clear();

        //Debug.Log(GameManager.GamePhase);
        switch (GameManager.GamePhase)
        {
            case 1:
                // Player - ?�터
                // Enemy - ?�이?�트 ?�면 ?�성
                _gameAgents = _playerSpawner.OnePointRandomSpawn(8, 8).GetComponent<GameAgents>();
                Enemy = _enemySpawner.PlayerDirectSpawn(_gameAgents.transform.localPosition, _gameAgents.transform.forward).GetComponent<NonPlayerCharacter>();
                break;
            case 2:
                // Player - ?�터
                // Enemy - ?�터?�서 ?�짝 벗어?�게
                _gameAgents = _playerSpawner.OnePointRandomSpawn(8, 9).GetComponent<GameAgents>();
                Enemy = _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition, 10, 0.7f * Mathf.PI, 0.3f*Mathf.PI).GetComponent<NonPlayerCharacter>();
                break;
            case 3:
                // Player - ?�터
                // Enemy - ?�터 주위???�덤 ?�성
                _gameAgents = _playerSpawner.OnePointRandomSpawn(8, 9).GetComponent<GameAgents>();
                Enemy = _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition).GetComponent<NonPlayerCharacter>();
                break;
            case 4:
                // Player - ?�터
                // Enemy - ?�터 주위???�덤 ?�성 + ?�직임
                _gameAgents = _playerSpawner.OnePointRandomSpawn(8, 9).GetComponent<GameAgents>();
                Enemy = _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition).GetComponent<NonPlayerCharacter>();
                _enemySpawner.OnEnemyRandomMove();
                break;
            case 5:
                // Player - ?�터
                // Enemy - ?�터 주위???�덤 ?�성 + ?�직임
                // Obstacle - 4�??�성
                _gameAgents = _playerSpawner.OnePointRandomSpawn(8, 9).GetComponent<GameAgents>();
                Enemy = _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition).GetComponent<NonPlayerCharacter>();
                _enemySpawner.OnEnemyRandomMove();
                _obstacleSpawner.AllPointSpawn();
                break;
            case 6:
                // Player - 8�??�인???�덤 ?�성
                // Enemy - Player 반�??? ?�직임
                // Obstacle - 4�??�성
                randomIndex = Random.Range(0, 8);
                //Debug.Log("randomIndex : " + randomIndex);
                _gameAgents = _playerSpawner.OnePointRandomSpawn(randomIndex, randomIndex).GetComponent<GameAgents>();
                int npcIndex = (randomIndex + 4) % 8;
                //Debug.Log("npcIndex : " + npcIndex);
                Enemy = _enemySpawner.OnePointRandomSpawn(npcIndex, npcIndex).GetComponent<NonPlayerCharacter>();
                _enemySpawner.OnEnemyRandomMove();
                _obstacleSpawner.AllPointSpawn();
                break;
            case 7:
                // Player - 8�??�인???�덤 ?�성
                // Enemy - Player 반�???+ ?�직임
                // Obstacle - 4�??�성
                randomIndex = Random.Range(0, 8);
                _gameAgents = _playerSpawner.OnePointRandomSpawn(randomIndex, randomIndex).GetComponent<GameAgents>();
                npcIndex = (randomIndex + 4) % 8;
                Enemy = _enemySpawner.OnePointRandomSpawn(npcIndex, npcIndex).GetComponent<NonPlayerCharacter>();
                _enemySpawner.OnEnemyRandomMove(0.5f, 40, 20);
                _obstacleSpawner.AllPointSpawn();
                break;
            case 8:
                // self-play
                if (GameManager.Instance.IsTest)
                {
                    randomIndex = Random.Range(0, 8);
                    _gameAgents = _playerSpawner.OnePointRandomSpawn(randomIndex, randomIndex).GetComponent<GameAgents>();
                    npcIndex = (randomIndex + 4) % 8;
                    _selfPlayAgents = _selfPlaySpawner.OnePointRandomSpawn(npcIndex, npcIndex).GetComponent<GameAgents>();
                    _obstacleSpawner.AllPointSpawn();
                    break;
                }

                randomIndex = Random.Range(0, 8);
                _gameAgents = _playerSpawner.OnePointRandomSpawn(randomIndex, randomIndex).GetComponent<GameAgents>();
                npcIndex = (randomIndex + 4) % 8;
                _selfPlayAgents = _selfPlaySpawner.OnePointRandomSpawn(npcIndex, npcIndex).GetComponent<GameAgents>();
                _obstacleSpawner.AllPointSpawn();
                break;
            default:
                break;
        }

        _gameAgents.Init(this);
        if (_selfPlayAgents && !GameManager.Instance.IsEnemy)
        {
            _selfPlayAgents.Init(this);
            var _selfAgentControllerList = _selfPlayAgents.GetComponentsInChildren<Controller>();
            foreach (var controller in _selfAgentControllerList)
            {
                ControllerList.Add(controller);
            }
        }
        
        var controllerList = _gameAgents.GetComponentsInChildren<Controller>();
        foreach (var controller in controllerList)
        {
            ControllerList.Add(controller);
        }

        NavMeshBuilder.ClearAllNavMeshes();
        NavMeshBuilder.BuildNavMesh();
        initialized = true;
    }


    public void EndEpisode()
    {
        //Debug.Log(gameObject.name);
        ResetEnvironment();
    }

    private void ResetEnvironment()
    {
        StopAllCoroutines();
        _environmentPlayTime = 0;
        _playerSpawner.Clear();
        _enemySpawner.Clear();
        _obstacleSpawner.Clear();
        _selfPlaySpawner.Clear();
        ControllerList.Clear();
        StartEpisode();
    }

    public void ClearEnvironment()
    {
        _playerSpawner.SpawnObjectListClear();
        _selfPlaySpawner.SpawnObjectListClear();
     }

    public float _environmentPlayTime = 0.0f;
    public float EvironmentMaxTime = 20.0f;
    void Update()
    {
        if (GameManager.Instance._init)
        {
            if (!initialized)
            {
                Initialize();
            }
            else
            {
                _environmentPlayTime += Time.deltaTime;
                if (_environmentPlayTime >= EvironmentMaxTime)
                {
                    Debug.Log("Time Out");
                    foreach (var controller in ControllerList)
                    {
                        //Debug.Log(controller.GetCumulativeReward());
                        //controller.TimeOutReward();
                        controller.EpisodeInterrupted();
                    }
                    if (GameManager.Instance.IsTest)
                    {
                        GameManager.GameClear(this);
                    }
                    else
                    {
                        ResetEnvironment();
                    }
                }
            }
        }
    }
}
