using System;
using UnityEngine;

namespace Features.CharacterModule.Scripts {
    public class CharacterEntity : MonoBehaviour, IKillable, IWinnable {
        [field: SerializeField] public float DeathTime { get; private set; } = 1f;
        [field: SerializeField] public float WinTime { get; private set; } = 1f;


        public event Action OnKilled;
        public bool IsDead { get; private set; }
        public event Action OnWin;
        public bool IsWin { get; private set; }

        public void Kill() {
            IsDead = true;
            OnKilled?.Invoke();
        }

        public void Win() {
            IsWin = true;
            OnWin?.Invoke();
        }
    }
}