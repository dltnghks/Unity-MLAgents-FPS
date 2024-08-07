using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnvironment : MonoBehaviour
{
    public PlayerSpawner _playerSpawner;
    public EnemySpawner _enemySpawner;

    public GameAgents _gameAgents;

    private void Start()
    {
        _playerSpawner = GetComponentInChildren<PlayerSpawner>();
        _enemySpawner= GetComponentInChildren<EnemySpawner>();
        _gameAgents = GetComponentInChildren<GameAgents>();
        StartEpisode();
    }

    public void StartEpisode()
    {
        switch (GameManager.GamePhase)
        {
            case 1:
                _playerSpawner.OnePointRandomSpawn();
                _enemySpawner.PlayerDirectSpawn(_gameAgents.transform.position, _gameAgents.transform.forward);
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
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
