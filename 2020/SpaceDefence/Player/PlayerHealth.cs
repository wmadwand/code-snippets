using UnityEngine;

namespace MiniGames.Games.SpaceDefence.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        public int Value => _value;
        public bool IsAlive => _value > 0;

        [SerializeField] protected int _value = 3;

        //-------------------------------------------------

        public void GetDamage(int value)
        {
            Remove(value);
            OnGetDamage();
        }

        public void AddHealth(int value)
        {
            Add(value);
        }

        public virtual void OnGetDamage() { }

        //-------------------------------------------------

        private void Add(int value)
        {
            _value += value;
        }

        private void Remove(int value)
        {
            if (value <= 0)
            {
                return;
            }

            _value -= value;
        }
    }
}