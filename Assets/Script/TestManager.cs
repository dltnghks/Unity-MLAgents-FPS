using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct Matchup
{
    [SerializeField]
    public List<GameObject> team1Agents;

    [SerializeField]
    public List<GameObject> team2Agents;
}

public class TestManager : MonoBehaviour
{
    private static TestManager _instance = null;
    public static TestManager Instance
    {
        get { return _instance; }
    }

    private List<float[]> allGameData = new List<float[]>();
    private float _playTime;
    public bool IsEpisodeInit;

    [Header("Test Environment")]
    public bool IsTest;
    public bool IsEnemy;
    public int testIndex = 0;
    [SerializeField]
    public List<Matchup> AgentList = new List<Matchup>();

    [Header("Randomness Control")]
    public bool useFixedSeed = false;
    public int randomSeed = 0;

    public List<GameEnvironment> testEnvironmentList = new List<GameEnvironment>();
    public int GameCount;
    public int EndGameCount;
    public int GameEpisodeCount;
    public int EndGameEpisodeCount;

    private int team1Wins = 0;
    private int team2Wins = 0;
    private int draws = 0;
    private float totalPlayTime = 0;

    private void Awake()
    {
        _instance = this;
        // DontDestroyOnLoad(gameObject); // GameManager가 이미 처리하고 있다면 중복일 수 있음
    }

    private void Start()
    {
        if (IsTest || IsEnemy)
        {
            Init();
        }
    }

    private void Update()
    {
        _playTime += Time.deltaTime;
    }

    public void Init()
    {
        if (testIndex >= AgentList.Count) return;

        string team1Name = (AgentList[testIndex].team1Agents.Count > 0 && AgentList[testIndex].team1Agents[0] != null) ? AgentList[testIndex].team1Agents[0].name : "N/A";
        string team2Name = "Enemy";
        if (!IsEnemy)
        {
            team2Name = (AgentList[testIndex].team2Agents.Count > 0 && AgentList[testIndex].team2Agents[0] != null) ? AgentList[testIndex].team2Agents[0].name : "N/A";
        }

        Debug.Log($"[TestManager] Initializing Test Case {testIndex + 1} / {AgentList.Count}. Matchup: {team1Name} vs {team2Name}");

        if (useFixedSeed)
        {
            Random.InitState(randomSeed);
        }
        else
        {
            Random.InitState(System.Environment.TickCount); // Use system tick count for true randomness
        }

        allGameData.Clear();
        GameManager.ClearCount = 0;
        
        team1Wins = 0;
        team2Wins = 0;
        draws = 0;
        totalPlayTime = 0;
        
        if (IsEnemy)
        {
            GameManager.GamePhase = 5;
            GameCount = 0;
            GameEpisodeCount = 0;
            Time.timeScale = 7;
            testEnvironmentList[0].ClearEnvironment();
            testEnvironmentList[0].initialized = false;
        }
        else if (IsTest)
        {
            GameManager.GamePhase = 8;
            GameCount = 0;
            GameEpisodeCount = 0;
            Time.timeScale = 7;
            testEnvironmentList[0].ClearEnvironment();
            testEnvironmentList[0].initialized = false;
        }
        
        _playTime = 0;
        IsEpisodeInit = true;
    }
    
    private enum EpisodeOutcome { Win, Loss, Draw }

    public void TestGameClear(GameEnvironment environment)
    {
        //Debug.Log($"[TestManager] TestGameClear");
        IsEpisodeInit = false;
        float episodePlayTime = environment.EnvironmentPlayTime;
        // NOTE: We call EndEpisode AFTER determining the winner, because EndEpisode will reset agent states.
        // environment.EndEpisode(); 

        GameCount++;

        var team1Agents = environment.team1Agents;
        var team2Agents = environment.team2Agents;

        // 1. Determine Outcome
        EpisodeOutcome team1Outcome;
        bool isTimeout = environment.EnvironmentPlayTime >= environment.EvironmentMaxTime;

        if (isTimeout)
        {
            team1Outcome = EpisodeOutcome.Draw;
            draws++;
        }
        else
        {
            bool team1HasSurvivors = team1Agents.Exists(a => a != null && a.HP > 0);
            bool team2HasSurvivors = team2Agents.Exists(a => a != null && a.HP > 0);

            if (team1HasSurvivors && !team2HasSurvivors)
            {
                team1Outcome = EpisodeOutcome.Win;
                team1Wins++;
            }
            else if (!team1HasSurvivors && team2HasSurvivors)
            {
                team1Outcome = EpisodeOutcome.Loss;
                team2Wins++;
            }
            else
            {
                team1Outcome = EpisodeOutcome.Draw;
                draws++;
            }
        }
        totalPlayTime += episodePlayTime;
        
        EpisodeOutcome team2Outcome = (team1Outcome == EpisodeOutcome.Win) ? EpisodeOutcome.Loss : (team1Outcome == EpisodeOutcome.Loss) ? EpisodeOutcome.Win : EpisodeOutcome.Draw;

        // Determine names for logging filename
        string team1Name = AgentList[testIndex].team1Agents.Count > 0 ? AgentList[testIndex].team1Agents[0].name : "Team1";
        string team2Name = "Enemy";
        if (!IsEnemy && AgentList[testIndex].team2Agents.Count > 0)
        {
            team2Name = AgentList[testIndex].team2Agents[0].name;
        }

        // Log stats for each agent in Team 1
        foreach (var agent in team1Agents)
        {
            if (agent == null) continue;
            GameLogger.WriteToCSV(new string[] {
                team1Outcome.ToString(),
                GameEpisodeCount.ToString(),
                agent.name,
                agent.TeamID.ToString(),
                agent._saveData.KillCount.ToString(),
                agent._saveData.AttackCount.ToString(),
                agent._saveData.MissCount.ToString(),
                agent._saveData.HitCount.ToString(),
                agent._saveData.DeathCount.ToString(),
                episodePlayTime.ToString()
            }, team1Name, team2Name, IsEnemy);
        }

        // Log stats for each agent in Team 2
        if (!IsEnemy)
        {
            foreach (var agent in team2Agents)
            {
                if (agent == null) continue;
                GameLogger.WriteToCSV(new string[] {
                    team2Outcome.ToString(),
                    GameEpisodeCount.ToString(),
                    agent.name,
                    agent.TeamID.ToString(),
                    agent._saveData.KillCount.ToString(),
                    agent._saveData.AttackCount.ToString(),
                    agent._saveData.MissCount.ToString(),
                    agent._saveData.HitCount.ToString(),
                    agent._saveData.DeathCount.ToString(),
                    episodePlayTime.ToString()
                }, team1Name, team2Name, IsEnemy);
            }
        }
        
        // Now that logging is complete, end the episode to reset the environment.
        environment.EndEpisode();

        if (GameEpisodeCount >= EndGameEpisodeCount)
        {
            // Summary Logic
            float totalEpisodes = (float)team1Wins + team2Wins + draws;
            if (totalEpisodes > 0)
            {
                float team1WinRate = (team1Wins / totalEpisodes) * 100;
                float team2WinRate = (team2Wins / totalEpisodes) * 100;
                float drawRate = (draws / totalEpisodes) * 100;
                float averagePlayTime = totalPlayTime / totalEpisodes;

                GameLogger.WriteToCSV(new string[] { "" }, team1Name, team2Name, IsEnemy);
                GameLogger.WriteToCSV(new string[] { "--- SUMMARY ---" }, team1Name, team2Name, IsEnemy);
                GameLogger.WriteToCSV(new string[] { "Total Episodes", totalEpisodes.ToString() }, team1Name, team2Name, IsEnemy);
                GameLogger.WriteToCSV(new string[] { $"Team1 ({team1Name}) Wins", team1Wins.ToString(), $"{team1WinRate:F2}%" }, team1Name, team2Name, IsEnemy);
                GameLogger.WriteToCSV(new string[] { $"Team2 ({team2Name}) Wins", team2Wins.ToString(), $"{team2WinRate:F2}%" }, team1Name, team2Name, IsEnemy);
                GameLogger.WriteToCSV(new string[] { "Draws", draws.ToString(), $"{drawRate:F2}%" }, team1Name, team2Name, IsEnemy);
                GameLogger.WriteToCSV(new string[] { "Average Play Time", averagePlayTime.ToString("F2") }, team1Name, team2Name, IsEnemy);
                GameLogger.WriteToCSV(new string[] { "--- END SUMMARY ---" }, team1Name, team2Name, IsEnemy);
                GameLogger.WriteToCSV(new string[] { "" }, team1Name, team2Name, IsEnemy);
            }

            testIndex++;
            if (testIndex >= AgentList.Count)
            {
                Debug.Log("[TestManager] All test cases have been completed.");
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            }
            else
            {
                Debug.Log($"[TestManager] Proceeding to the next test case: {testIndex + 1}");
                Init();
            }
        }

        // Reset all agents' save data for the next episode
        // This is now redundant as EndEpisode->ResetEnvironment->StartEpisode destroys and creates new agents
        // But we keep it in case the old agents are accessed before being destroyed.
        foreach (var agent in team1Agents)
        {
            if(agent != null) agent._saveData.ResetCount();
        }
        foreach (var agent in team2Agents)
        {
            if(agent != null) agent._saveData.ResetCount();
        }

        GameEpisodeCount++;
        GameManager.ClearCount = 0;
        IsEpisodeInit = true;
    }

    public static void RestEnvrionment()
    {
        foreach (var gameEnvironment in Instance.testEnvironmentList)
        {
            gameEnvironment.EndEpisode();
        }
    }
}
