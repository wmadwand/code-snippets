using System;
using System.Linq;
using MiniGames.Games.SpaceDefence.Game;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MiniGames.Games.SpaceDefence.Data
{
    [CreateAssetMenu(menuName = "MiniGames/SpaceDefence/SpaceDefenceDifficultyModel", fileName = "SpaceDefenceDifficultyModel")]
    public class SpaceDefenceDifficultyModel : ScriptableObject
    {
        public GameSettings gameSettings;
        [Required(InfoMessageType.Warning)]
        public GameRound[] rounds;
        public bool tutorial;

        //-------------------------------------------------

        public bool Check(int gamesComplete)
        {
            if (gameSettings.maxGamesCount == -1)
            {
                return gameSettings.minGamesCount <= gamesComplete;
            }

            return gameSettings.minGamesCount <= gamesComplete && gamesComplete <= gameSettings.maxGamesCount;
        }

        public GameRound GetCurrentRound(int index)
        {
            return rounds.ElementAt(index);
        }

        public int GetTotalGameHazardCount()
        {
            int count = 0;
            Array.ForEach(rounds, round => count += round.hazardCount);

            return count;
        }
    }

    //-------------------------------------------------

    [Serializable]
    public class GameRound
    {
        [ValidateInput("ValidateInput", "Item mustn't be None!", InfoMessageType.Warning)]
        [ValidateInput("ValidateInput2", "HazardSpawns list mustn't be empty!", InfoMessageType.Warning)]
        public HazardSpawnModel[] HazardSpawns;
        public int hazardCount;
        public bool shuffle;

        private bool ValidateInput(HazardSpawnModel[] HazardSpawns)
        {
            return !HazardSpawns.Contains(null);
        }

        private bool ValidateInput2(HazardSpawnModel[] HazardSpawns)
        {
            return HazardSpawns.Length > 0;
        }
    }
}