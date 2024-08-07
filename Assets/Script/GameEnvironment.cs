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
        _gameAgents = GetComponentInChildren<GameAgents>();
        StartEpisode();
    }

    public void StartEpisode()
    {
        _playerSpawner.Clear();
        _enemySpawner.Clear();
        _obstacleSpawner.Clear();

        switch (GameManager.GamePhase)
        {
            case 1:
                _playerSpawner.OnePointRandomSpawn();
                _enemySpawner.PlayerDirectSpawn(_gameAgents.transform.position, _gameAgents.transform.forward);
                _obstacleSpawner.AllPointSpawn();
                break;
            case 2:
                // Player - 센터
                // Enemy - 센터에서 살짝 벗어나게

                break;
            case 3:
                // Player - 센터
                // Enemy - 센터 주위에 랜덤 생성

                break;
            case 4:
                // Player - 센터
                // Enemy - 센터 주위에 랜덤 생성 + 움직임

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
