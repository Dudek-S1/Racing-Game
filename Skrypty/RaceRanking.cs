using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceRanking : MonoBehaviour
{
    private List<Player> players;

    public RaceRanking()
    {
        players = new List<Player>();
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
        SortPlayersByRaceTime();
    }

    private void SortPlayersByRaceTime()
    {
        players.Sort((p1, p2) => p1.RaceTime.CompareTo(p2.RaceTime));
    }

    public List<Player> GetPlayers()
    {
        return players;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(new PlayerListWrapper { Players = players });
    }

    public void FromJson(string json)
    {
        PlayerListWrapper wrapper = JsonUtility.FromJson<PlayerListWrapper>(json);
        players = wrapper.Players;
    }

    [System.Serializable]
    private class PlayerListWrapper
    {
        public List<Player> Players;
    }

    [System.Serializable]
    public class Player
    {
        public string Name;
        public float RaceTime;
        public string GearboxType;
        public string DriveType;

        public Player(string name, float raceTime, string gearboxType, string driveType)
        {
            Name = name;
            RaceTime = raceTime;
            GearboxType = gearboxType;
            DriveType = driveType;
        }
    }
}
