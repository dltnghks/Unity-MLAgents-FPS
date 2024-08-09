using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnvironment : MonoBehaviour
{
    public PlayerSpawner _playerSpawner;
    public EnemySpawner _enemySpawner;
    public ObstacleSpawner _obstacleSpawner;

    public GameAgents _gameAgents;

    private void Start()
    {
        _playerSpawner = GetComponentInChildren<PlayerSpawner>();
        _enemySpawner= GetComponentInChildren<EnemySpawner>();
        _obstacleSpawner = GetComponentInChildren<ObstacleSpawner>();
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
                _gameAgents = _playerSpawner.OnePointRandomSpawn().GetComponent<GameAgents>();
                _enemySpawner.PlayerDirectSpawn(_gameAgents.transform.localPosition, _gameAgents.transform.forward);
                break;
            case 2:
                // Player - 센터
                // Enemy - 센터에서 살짝 벗어나게
                _gameAgents = _playerSpawner.OnePointRandomSpawn().GetComponent<GameAgents>();
                _enemySpawner.PlayerDirectSpawn(_gameAgents.transform.localPosition, _gameAgents.transform.forward);
                break;
            case 3:
                // Player - 센터
                // Enemy - 센터 주위에 랜덤 생성
                _gameAgents = _playerSpawner.OnePointRandomSpawn().GetComponent<GameAgents>();
                _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition);
                break;
            case 4:
                // Player - 센터
                // Enemy - 센터 주위에 랜덤 생성 + 움직임
                _gameAgents = _playerSpawner.OnePointRandomSpawn().GetComponent<GameAgents>();
                _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition);
                break;
            case 5:
                // Player - 센터
                // Enemy - 센터 주위에 랜덤 생성 + 움직임
                // Obstacle - 4개 생성

                break;
            case 6:
                // Player - 8개 포인트 랜덤 생성
                // Enemy - 센터 주위에 랜덤 생성 + 움직임
                // Obstacle - 4개 생성

                break;
            case 7:
                // Player - 8개 포인트 랜덤 생성
                // Enemy - Player 반대편
                // Obstacle - 4개 생성

                break;
            case 8:
                // Player - 8개 포인트 랜덤 생성
                // Enemy - Player 반대편
                // Obstacle - 4개 생성

                break;
            default:
                break;
        }
    }

    public void EndEpisode()
    {

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
