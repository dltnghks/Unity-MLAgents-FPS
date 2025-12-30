using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AI;
using Unity.MLAgents;

public class GameEnvironment : MonoBehaviour
{
    public PlayerSpawner _playerSpawner;
    public EnemySpawner _enemySpawner;
    public ObstacleSpawner _obstacleSpawner;
    public SelfPlaySpawner _selfPlaySpawner;

    public List<GameAgents> team1Agents = new List<GameAgents>();
    public List<GameAgents> team2Agents = new List<GameAgents>();
    public NonPlayerCharacter Enemy;

    [Header("Agents")]
    public List<Controller> ControllerList = new List<Controller>();
    
    public float MapSize = 40;
    public bool initialized;
    public int testIndex = 0;

    private bool isGameOver = false; // Flag to defer GameClear call by one frame

    void Initialize()
    {
        MapSize *= transform.localScale.x;
        _playerSpawner = GetComponentInChildren<PlayerSpawner>();
        _enemySpawner = GetComponentInChildren<EnemySpawner>();
        _obstacleSpawner = GetComponentInChildren<ObstacleSpawner>();
        _selfPlaySpawner = GetComponentInChildren<SelfPlaySpawner>();
        StartEpisode();
    }
    
    public void StartEpisode()
    {
        isGameOver = false; // Reset game over flag at the start of an episode
        if(_playerSpawner) _playerSpawner.SpawnObjectListClear();
        if(_enemySpawner) _enemySpawner.SpawnObjectListClear();
        if(_selfPlaySpawner) _selfPlaySpawner.SpawnObjectListClear();
        if(_obstacleSpawner) _obstacleSpawner.SpawnObjectListClear();

        ControllerList.Clear();
        team1Agents.Clear();
        team2Agents.Clear();
        
        switch (GameManager.GamePhase)
        {
            // Cases 1-7 remain the same
            case 1:
                team1Agents.Add(_playerSpawner.OnePointRandomSpawn(8, 8).GetComponent<GameAgents>());
                Enemy = _enemySpawner.PlayerDirectSpawn(team1Agents[0].transform.localPosition, team1Agents[0].transform.forward).GetComponent<NonPlayerCharacter>();
                break;
            case 2:
                team1Agents.Add(_playerSpawner.OnePointRandomSpawn(8, 9).GetComponent<GameAgents>());
                Enemy = _enemySpawner.PlayerCenterRandomSpawn(team1Agents[0].transform.localPosition, 10, 0.7f * Mathf.PI, 0.3f*Mathf.PI).GetComponent<NonPlayerCharacter>();
                break;
            case 3:
                team1Agents.Add(_playerSpawner.OnePointRandomSpawn(8, 9).GetComponent<GameAgents>());
                Enemy = _enemySpawner.PlayerCenterRandomSpawn(team1Agents[0].transform.localPosition).GetComponent<NonPlayerCharacter>();
                break;
            case 4:
                team1Agents.Add(_playerSpawner.OnePointRandomSpawn(8, 9).GetComponent<GameAgents>());
                Enemy = _enemySpawner.PlayerCenterRandomSpawn(team1Agents[0].transform.localPosition).GetComponent<NonPlayerCharacter>();
                _enemySpawner.OnEnemyRandomMove();
                break;
            case 5:
                if (TestManager.Instance != null && TestManager.Instance.IsEnemy)
                {
                    var currentMatchup = TestManager.Instance.AgentList[TestManager.Instance.testIndex];
                    team1Agents = _playerSpawner.SpawnTeam(currentMatchup.team1Agents, 0);
                    Enemy = _enemySpawner.PlayerCenterRandomSpawn(Vector3.zero, 0, 0, 0).GetComponent<NonPlayerCharacter>();
                    _enemySpawner.OnEnemyMovePointSetting();
                }
                else
                {
                    team1Agents.Add(_playerSpawner.OnePointRandomSpawn(8, 9).GetComponent<GameAgents>());
                    Enemy = _enemySpawner.PlayerCenterRandomSpawn(team1Agents[0].transform.localPosition).GetComponent<NonPlayerCharacter>();
                    _enemySpawner.OnEnemyRandomMove();
                }
                _obstacleSpawner.AllPointSpawn();
                break;
            case 6:
                testIndex = Random.Range(0, 8);
                team1Agents.Add(_playerSpawner.OnePointRandomSpawn(testIndex, testIndex).GetComponent<GameAgents>());
                int npcIndex = (testIndex + 4) % 8;
                Enemy = _enemySpawner.OnePointRandomSpawn(npcIndex, npcIndex).GetComponent<NonPlayerCharacter>();
                _enemySpawner.OnEnemyRandomMove();
                _obstacleSpawner.AllPointSpawn();
                break;
            case 7:
                testIndex = Random.Range(0, 8);
                team1Agents.Add(_playerSpawner.OnePointRandomSpawn(testIndex, testIndex).GetComponent<GameAgents>());
                npcIndex = (testIndex + 4) % 8;
                Enemy = _enemySpawner.OnePointRandomSpawn(npcIndex, npcIndex).GetComponent<NonPlayerCharacter>();
                _enemySpawner.OnEnemyRandomMove(0.5f, 40, 20);
                _obstacleSpawner.AllPointSpawn();
                break;
            case 8:
                if (TestManager.Instance != null && TestManager.Instance.IsTest)
                {
                    var currentMatchup = TestManager.Instance.AgentList[TestManager.Instance.testIndex];
                    team1Agents = _playerSpawner.SpawnTeam(currentMatchup.team1Agents, 0);
                    team2Agents = _selfPlaySpawner.SpawnTeam(currentMatchup.team2Agents, 1);
                }
                else
                {
                    testIndex = Random.Range(0, 8);
                    team1Agents.Add(_playerSpawner.OnePointRandomSpawn(testIndex, testIndex).GetComponent<GameAgents>());
                    npcIndex = (testIndex + 4) % 8;
                    team2Agents.Add(_selfPlaySpawner.OnePointRandomSpawn(npcIndex, npcIndex).GetComponent<GameAgents>());
                }
                _obstacleSpawner.AllPointSpawn();
                break;
            default:
                break;
        }

        foreach(var agent in team1Agents)
        {
            if (agent == null) continue;
            agent.Init(this);
            var controllers = agent.GetComponentsInChildren<Controller>();
            foreach (var controller in controllers) ControllerList.Add(controller);
        }
        foreach(var agent in team2Agents)
        {
            if (agent == null) continue;
            agent.Init(this);
            var controllers = agent.GetComponentsInChildren<Controller>();
            foreach (var controller in controllers) ControllerList.Add(controller);
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
        EnvironmentPlayTime = 0;
        StartEpisode();
    }

    public void ClearEnvironment()
    {
        if(_playerSpawner) _playerSpawner.SpawnObjectListClear();
        if(_selfPlaySpawner) _selfPlaySpawner.SpawnObjectListClear();
    }

    public float EnvironmentPlayTime = 0.0f;
    public float EvironmentMaxTime = 20.0f;
    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance._init)
        {
            if (!initialized)
            {
                Initialize();
            }
            else
            {
                if (isGameOver)
                {
                    isGameOver = false; // Reset flag
                    GameManager.GameClear(this);
                    return; // End update for this frame
                }

                if (GameManager.Instance.IsEpisodeInit == false && (TestManager.Instance == null || TestManager.Instance.IsEpisodeInit == false))
                {
                    return;
                }

                EnvironmentPlayTime += Time.deltaTime;
                if (EnvironmentPlayTime >= EvironmentMaxTime)
                {
                    foreach (var controller in ControllerList)
                    {
                        controller.EpisodeInterrupted();
                    }

                    if (TestManager.Instance != null && (TestManager.Instance.IsTest || TestManager.Instance.IsEnemy))
                    {
                        isGameOver = true; // Set flag to end game on next frame
                    }
                    else
                    {
                        ResetEnvironment();
                    }
                }
            }
        }
    }

    public void OnAgentDied(GameAgents deadAgent)
    {
        // For training vs NPC, a single death ends the game
        if (GameManager.GamePhase < 8)
        {
            isGameOver = true;
            return;
        }

        // For multi-agent scenarios, check for team wipe
        List<GameAgents> teamToCheck = (deadAgent.TeamID == 0) ? team1Agents : team2Agents;
        bool teamEliminated = true;
        foreach (var agent in teamToCheck)
        {
            if (agent != null && agent.HP > 0)
            {
                teamEliminated = false;
                break;
            }
        }

        if (teamEliminated)
        {
            Debug.Log($"Team {deadAgent.TeamID} has been eliminated.");
            isGameOver = true; // Set flag to end game on next frame
        }
    }
}
