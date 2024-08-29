using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AI;

public class GameEnvironment : MonoBehaviour
{
    public PlayerSpawner _playerSpawner;
    public EnemySpawner _enemySpawner;
    public ObstacleSpawner _obstacleSpawner;
    public Spawner _selfPlaySpawner;

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
        // ?¥Îèô ???ÄÍ∏??úÍ∞ÑÎßåÌÅº ?ÄÍ∏?
        yield return new WaitForSeconds(1f);
        StartEpisode();
    }

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
                // Player - ?ºÌÑ∞
                // Enemy - ?êÏù¥?ÑÌä∏ ?ïÎ©¥ ?ùÏÑ±
                _gameAgents = _playerSpawner.OnePointRandomSpawn(8, 8).GetComponent<GameAgents>();
                Enemy = _enemySpawner.PlayerDirectSpawn(_gameAgents.transform.localPosition, _gameAgents.transform.forward).GetComponent<NonPlayerCharacter>();
                break;
            case 2:
                // Player - ?ºÌÑ∞
                // Enemy - ?ºÌÑ∞?êÏÑú ?¥Ïßù Î≤óÏñ¥?òÍ≤å
                _gameAgents = _playerSpawner.OnePointRandomSpawn(8, 9).GetComponent<GameAgents>();
                Enemy = _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition, 10, 0.7f * Mathf.PI, 0.3f*Mathf.PI).GetComponent<NonPlayerCharacter>();
                break;
            case 3:
                // Player - ?ºÌÑ∞
                // Enemy - ?ºÌÑ∞ Ï£ºÏúÑ???úÎç§ ?ùÏÑ±
                _gameAgents = _playerSpawner.OnePointRandomSpawn(8, 9).GetComponent<GameAgents>();
                Enemy = _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition).GetComponent<NonPlayerCharacter>();
                break;
            case 4:
                // Player - ?ºÌÑ∞
                // Enemy - ?ºÌÑ∞ Ï£ºÏúÑ???úÎç§ ?ùÏÑ± + ?ÄÏßÅÏûÑ
                _gameAgents = _playerSpawner.OnePointRandomSpawn(8, 9).GetComponent<GameAgents>();
                Enemy = _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition).GetComponent<NonPlayerCharacter>();
                _enemySpawner.OnEnemyRandomMove();
                break;
            case 5:
                // Player - ?ºÌÑ∞
                // Enemy - ?ºÌÑ∞ Ï£ºÏúÑ???úÎç§ ?ùÏÑ± + ?ÄÏßÅÏûÑ
                // Obstacle - 4Í∞??ùÏÑ±
                _gameAgents = _playerSpawner.OnePointRandomSpawn(8, 9).GetComponent<GameAgents>();
                Enemy = _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition).GetComponent<NonPlayerCharacter>();
                _enemySpawner.OnEnemyRandomMove();
                _obstacleSpawner.AllPointSpawn();
                break;
            case 6:
                // Player - 8Í∞??¨Ïù∏???úÎç§ ?ùÏÑ±
                // Enemy - Player Î∞òÎ??? ?ÄÏßÅÏûÑ
                // Obstacle - 4Í∞??ùÏÑ±
                int randomIndex = Random.Range(0, 8);
                //Debug.Log("randomIndex : " + randomIndex);
                _gameAgents = _playerSpawner.OnePointRandomSpawn(randomIndex, randomIndex).GetComponent<GameAgents>();
                int npcIndex = (randomIndex + 4) % 8;
                //Debug.Log("npcIndex : " + npcIndex);
                Enemy = _enemySpawner.OnePointRandomSpawn(npcIndex, npcIndex).GetComponent<NonPlayerCharacter>();
                _enemySpawner.OnEnemyRandomMove();
                _obstacleSpawner.AllPointSpawn();
                break;
            case 7:
                // Player - 8Í∞??¨Ïù∏???úÎç§ ?ùÏÑ±
                // Enemy - Player Î∞òÎ???+ ?ÄÏßÅÏûÑ
                // Obstacle - 4Í∞??ùÏÑ±
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
                    randomIndex = GameManager.Instance.GameCount % 8;
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
        if (_selfPlayAgents)
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
                        controller.EpisodeInterrupted();
                    }
                    GameManager.GameClear(this);
                }
            }
        }
    }
}
