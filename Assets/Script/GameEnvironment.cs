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
                // Player - ����
                // Enemy - ���Ϳ��� ��¦ �����

                break;
            case 3:
                // Player - ����
                // Enemy - ���� ������ ���� ����

                break;
            case 4:
                // Player - ����
                // Enemy - ���� ������ ���� ���� + ������

                break;
            case 5:
                // Player - ����
                // Enemy - ���� ������ ���� ���� + ������
                // Obstacle - 4�� ����

                break;
            case 6:
                // Player - 8�� ����Ʈ ���� ����
                // Enemy - ���� ������ ���� ���� + ������
                // Obstacle - 4�� ����

                break;
            case 7:
                // Player - 8�� ����Ʈ ���� ����
                // Enemy - Player �ݴ���
                // Obstacle - 4�� ����

                break;
            case 8:
                // Player - 8�� ����Ʈ ���� ����
                // Enemy - Player �ݴ���
                // Obstacle - 4�� ����

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
