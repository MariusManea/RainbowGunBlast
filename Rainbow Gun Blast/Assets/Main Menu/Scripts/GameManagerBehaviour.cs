using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManagerBehaviour : NetworkBehaviour
{
    public GameObject[] players;
    public GameObject[] oldPlayers;
    public List<GameObject> deadPlayers;
    public Dictionary<GameObject, int> teams;

    public float findTimer;
    public int gameCode;

    public int maxTeams;
    public int currentIndex;
    public bool isDeathMatch;
    public bool startMatch;
    public bool gameOver;
    public float matchTimer;
    public float warmUpTimer;
    public float overTimer;

    public Vector3[] soloSpawnPositions;
    public Vector3[] soloSpawnRotations;
    public Vector3[] teamSpawnPositions;
    public Vector3[] teamSpawnRotations;

    private int respawnIndex;

    // Start is called before the first frame update
    void Start()
    {

        findTimer = 0;
        if (isServer)
        {
            gameCode = GameObject.Find("NetworkManager").GetComponent<HostGame>().gameCode;
            teams = new Dictionary<GameObject, int>(10);
            deadPlayers = new List<GameObject>(10);
            maxTeams = (gameCode / 10 % 10 == 0) ? 10 : 2;
            isDeathMatch = gameCode % 10 == 0;
            respawnIndex = 0;
            oldPlayers = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            FindPlayers();
            if (gameOver)
            {
                GameOver();
                return;
            }
            StartCoroutine(CheckDeads());
            StartCoroutine(RemovePlayer());
            if (!startMatch)
            {
                warmUpTimer -= Time.deltaTime;
                if (warmUpTimer < 0)
                {
                    startMatch = true;
                    StartCoroutine(WarmUpOver());
                }
            }
            else
            {
                if (isDeathMatch)
                {
                    DeathMatchPlay();
                }
                else
                {
                    LastManPlay();
                }
            }
        }
    }

    public void FindPlayers()
    {
        findTimer += Time.deltaTime;
        if (findTimer > 1)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            findTimer = 0;
            foreach (GameObject player in players)
            {
                if (!teams.ContainsKey(player))
                {
                    teams.Add(player, currentIndex);
                    if (maxTeams == 2)
                    {
                        foreach (GameObject teamMate in teams.Keys)
                        {
                            if (teams[teamMate] == 1)
                            {
                                teamMate.GetComponent<walk>().RpcChangeMaterial(1);
                            }
                        }
                    }
                    if (maxTeams == 2)
                    {
                        currentIndex = (currentIndex + 1) % maxTeams;
                    } 
                    else
                    {
                        currentIndex++;
                    }
                }
            }
        }
    }

    public void DeathMatchPlay()
    {
        matchTimer -= Time.deltaTime;
        if (matchTimer < 0)
        {
            gameOver = true;
        }
    }

    public void LastManPlay()
    {
        StartCoroutine(CheckLastTeamRoutine());
    }

    IEnumerator CheckLastTeamRoutine()
    {
        List<int> teamsList = new List<int>();
        foreach (GameObject player in players)
        {
            if (player != null && !teamsList.Contains(teams[player]) && !deadPlayers.Contains(player))
            {
                teamsList.Add(teams[player]);
            }
            if (teamsList.Count > 1)
            { 
                break;
            }
            yield return false;
        }
        if (teamsList.Count == 1)
        {
            gameOver = true;
        }
    }

    public void GameOver()
    {
        overTimer -= Time.deltaTime;
        if (overTimer < 0)
        {
            NetworkServer.Shutdown();
            NetworkManager.singleton.StopHost();
        }
        else
        {

            if (maxTeams == 10)
            {
                GameObject winner;
                if (isDeathMatch)
                {
                    winner = GetSoloDeathWinner();
                }
                else
                {
                    winner = GetSoloManWinner();
                }
                foreach (GameObject player in players)
                {
                    player.GetComponent<walk>().RpcFinalResult(player == winner);
                }
            }
            else
            {
                int winnerTeam;
                if (isDeathMatch)
                {
                    winnerTeam = GetTeamDeathWinner();
                }
                else
                {
                    winnerTeam = GetTeamManWinner();
                }
                foreach (GameObject player in players)
                {
                    player.GetComponent<walk>().RpcFinalResult(teams[player] == winnerTeam);
                }
            }
        }
    }

    IEnumerator CheckDeads()
    {
        foreach (GameObject player in players)
        {
            if (player != null && player.GetComponent<walk>().isDead && !deadPlayers.Contains(player))
            {
                deadPlayers.Add(player);
            }
        }
        for (int idx = 0; idx < deadPlayers.Count; idx++)
        {
            if (deadPlayers[idx] != null && !deadPlayers[idx].GetComponent<walk>().isDead)
            {
                deadPlayers.RemoveAt(idx);
                idx--;
            } else
            {
                if (isDeathMatch)
                {
                    GameObject respawnPoint = GameObject.Find("StartPos" + (respawnIndex + 1));
                    respawnIndex = (respawnIndex + 1) % 10;
                    deadPlayers[idx].GetComponent<walk>().CmdRespawn(this.gameObject, false, respawnPoint.transform.position);
                }
            }
        }
        yield return true;
    }

    IEnumerator WarmUpOver()
    {
        int idx = 0;
        foreach (GameObject player in players)
        {
            if (player != null)
            {
                GameObject respawnPoint = GameObject.Find("StartPos" + (idx + 1));
                idx = (idx + 1) % 10;
                //player.GetComponent<walk>().RpcSetRespawnPoint(respawnPoint.transform.position);
                player.GetComponent<walk>().CmdRespawn(this.gameObject, true, respawnPoint.transform.position);
            }
        }
        yield return true;
    }

    GameObject GetSoloDeathWinner()
    {
        GameObject bestPlayer = null;

        foreach (GameObject player in players)
        {
            if (player != null)
            {
                if (bestPlayer == null)
                {
                    bestPlayer = player;
                }
                else
                {
                    if (player.GetComponent<walk>().score > bestPlayer.GetComponent<walk>().score)
                    {
                        bestPlayer = player;
                    }
                    else
                    {
                        if (player.GetComponent<walk>().score == bestPlayer.GetComponent<walk>().score)
                        {
                            if (player.GetComponent<walk>().deaths < bestPlayer.GetComponent<walk>().deaths)
                            {
                                bestPlayer = player;
                            }
                        }
                    }
                }
            }
        }

        return bestPlayer;
    }

    GameObject GetSoloManWinner()
    {
        foreach (GameObject player in players)
        {
            if (player != null && !player.GetComponent<walk>().isDead)
            {
                return player;
            }
        }

        return null;
    }

    int GetTeamDeathWinner()
    {
        int greenTeam = 0;
        int redTeam = 0;
        int greenDeaths = 0;
        int redDeaths = 0;
        foreach (GameObject player in teams.Keys)
        {
            if (player != null)
            {
                if (teams[player] == 0)
                {
                    greenTeam += player.GetComponent<walk>().score;
                    greenDeaths += player.GetComponent<walk>().deaths;
                }
                else
                {
                    redTeam += player.GetComponent<walk>().score;
                    redDeaths += player.GetComponent<walk>().deaths;
                }
            }
        }
        if (greenTeam > redTeam)
        {
            return 0;
        }
        else
        {
            if (greenTeam == redTeam)
            {
                if (greenDeaths < redDeaths)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 1;
            }
        }
    }

    int GetTeamManWinner()
    {
        foreach (GameObject player in players)
        {
            if (!player.GetComponent<walk>().isDead)
            {
                return teams[player];
            }
        }
        return 0;
    }

    IEnumerator RemovePlayer()
    {
        if (oldPlayers != null)
        {
            foreach (GameObject player in oldPlayers)
            {
                bool found = false;
                foreach (GameObject newPlayer in players)
                {
                    if (player == newPlayer)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    if (deadPlayers.Contains(player))
                    {
                        deadPlayers.Remove(player);
                    }
                    teams.Remove(player);
                }
                yield return null;
            }
        }
        oldPlayers = players;
    }
}
