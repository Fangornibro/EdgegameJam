using System;

namespace Features.CharacterModule.Scripts {
    public interface IKillable {
        public float DeathTime { get; }
        public void Kill();
        public event Action OnKilled;
        public bool IsDead { get; }
    }
}