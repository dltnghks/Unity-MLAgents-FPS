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
                // Player - ����
                // Enemy - ������Ʈ ���� ����
                _gameAgents = _playerSpawner.OnePointRandomSpawn().GetComponent<GameAgents>();
                _enemySpawner.PlayerDirectSpawn(_gameAgents.transform.localPosition, _gameAgents.transform.forward);
                break;
            case 2:
                // Player - ����
                // Enemy - ���Ϳ��� ��¦ �����
                _gameAgents = _playerSpawner.OnePointRandomSpawn().GetComponent<GameAgents>();
                _enemySpawner.PlayerDirectSpawn(_gameAgents.transform.localPosition, _gameAgents.transform.forward);
                break;
            case 3:
                // Player - ����
                // Enemy - ���� ������ ���� ����
                _gameAgents = _playerSpawner.OnePointRandomSpawn().GetComponent<GameAgents>();
                _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition);
                break;
            case 4:
                // Player - ����
                // Enemy - ���� ������ ���� ���� + ������
                _gameAgents = _playerSpawner.OnePointRandomSpawn().GetComponent<GameAgents>();
                _enemySpawner.PlayerCenterRandomSpawn(_gameAgents.transform.localPosition);
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
