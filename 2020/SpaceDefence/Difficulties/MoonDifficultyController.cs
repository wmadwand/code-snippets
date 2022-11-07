using System.Linq;
using MiniGames.Games.SpaceDefence.Data;
using UnityEngine;

namespace MiniGames.Games.SpaceDefence.Difficulties
{
    public class SpaceDefenceDifficultyController : MonoBehaviour
    {
        public SpaceDefenceUserObject userObject;
        public SpaceDefenceDifficultyModel defaultDifficulty;
        public SpaceDefenceDifficultyModel[] difficulties;

        public SpaceDefenceDifficultyModel CurrentDifficulty { get; private set; }

        public void SetDifficulty()
        {
            CurrentDifficulty = difficulties.FirstOrDefault(d => d.Check(userObject.model.gamesComplete)) ?? defaultDifficulty;
        }

        public void CompleteDifficulty()
        {
            userObject.model.gamesComplete++;
            userObject.Save();
        }
    }
}