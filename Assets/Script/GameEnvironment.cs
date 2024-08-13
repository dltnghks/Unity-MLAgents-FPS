using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AI;

public class GameEnvironment : MonoBehaviour
{
    public PlayerSpawner _playerSpawner;
    public EnemySpawner _enemySpawner;
    public ObstacleSpawner _obstacleSpawner;

    public GameAgents _gameAgents;

    public float MapSize = 40;

    private void Start()
    {
        MapSize *= transform.localScale.x;
        _playerSpawner = GetComponentInChildren<PlayerSpawner>();
        _enemySpawner= GetComponentInChildren<EnemySpawner>();
        _obstacleSpawner = GetComponentInChildren<ObstacleSpawner>();
        StartEpisode();
    }

    IEnumerator Delay()
    {
        _playerSpawner.Clear();
        _enemySpawner.Clear();
        _obstacleSpawner.Clear();
        // 이동 후 대기 시간만큼 대기
        yield return new WaitForSeconds(1f);
        StartEpisode();
    }

    public void StartEpisode()
    {
        Debug.Log("Phase : " + GameManager.GamePhase);
        _playerSpawner.Clear();
        _enemySpawner.Clear();
        _obstacleSpawner.Clear();

        switch (GameManager.GamePhase)
        {
            case 1:
                // Player - 센터
                // Enemy - 에이전트 정면 생성
                _gameAgents = _playerSpawner.OnePointRandomSpawn(8, 8).GetComponent<GameAgents>();
                _enemySpawner.PlayerDirectSpawn(_gameAgents.transform.localPosition, _gameAgents.transform.forward);
                break;
            case 2:
                // Player - 센터
                // Enemy - 센터에서 살짝 벗어나게
                _gameAgents = _playerSpawner.OnePointRandomSpawn(8, 9).GetComponent<GameAgents>();
                _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition, 10, 0.7f * Mathf.PI, 0.3f*Mathf.PI);
                break;
            case 3:
                // Player - 센터
                // Enemy - 센터 주위에 랜덤 생성
                _gameAgents = _playerSpawner.OnePointRandomSpawn(8, 9).GetComponent<GameAgents>();
                _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition);
                break;
            case 4:
                // Player - 센터
                // Enemy - 센터 주위에 랜덤 생성 + 움직임
                _gameAgents = _playerSpawner.OnePointRandomSpawn(8, 9).GetComponent<GameAgents>();
                _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition);
                _enemySpawner.OnEnemyRandomMove();
                break;
            case 5:
                // Player - 센터
                // Enemy - 센터 주위에 랜덤 생성 + 움직임
                // Obstacle - 4개 생성
                _gameAgents = _playerSpawner.OnePointRandomSpawn(8, 9).GetComponent<GameAgents>();
                _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition);
                _enemySpawner.OnEnemyRandomMove();
                _obstacleSpawner.AllPointSpawn();
                break;
            case 6:
                // Player - 8개 포인트 랜덤 생성
                // Enemy - Player 반대편
                // Obstacle - 4개 생성
                int randomIndex = Random.Range(0, 8);
                Debug.Log("randomIndex : " + randomIndex);
                _gameAgents = _playerSpawner.OnePointRandomSpawn(randomIndex, randomIndex).GetComponent<GameAgents>();
                int npcIndex = (randomIndex + 4) % 8;
                Debug.Log("npcIndex : " + npcIndex);
                _enemySpawner.OnePointRandomSpawn(npcIndex, npcIndex);
                _obstacleSpawner.AllPointSpawn();
                break;
            case 7:
                // Player - 8개 포인트 랜덤 생성
                // Enemy - Player 반대편 + 움직임
                // Obstacle - 4개 생성
                randomIndex = Random.Range(0, 8);
                _gameAgents = _playerSpawner.OnePointRandomSpawn(randomIndex, randomIndex).GetComponent<GameAgents>();
                npcIndex = (randomIndex + 4) % 8;
                _enemySpawner.OnePointRandomSpawn(npcIndex, npcIndex);
                _enemySpawner.OnEnemyRandomMove();
                _obstacleSpawner.AllPointSpawn();
                break;
            case 8:
                // self-play
                _gameAgents = _playerSpawner.OnePointRandomSpawn(1).GetComponent<GameAgents>();
                _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition);
                _obstacleSpawner.AllPointSpawn();
                break;
            default:
                break;
        }

        _gameAgents.Init(this);
        NavMeshBuilder.ClearAllNavMeshes();
        NavMeshBuilder.BuildNavMesh();
    }

    public void EndEpisode()
    {
        StartEpisode();
        //StartCoroutine(Delay());
    }

    private void SpawnPlayer()
    {
        _playerSpawner.OnePointRandomSpawn();
    }

    private void SpawnEnemy()
    {
        _enemySpawner.OnePointRandomSpawn();
    }
}
