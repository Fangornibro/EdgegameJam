using System;

namespace Features.CharacterModule.Scripts {
    public interface IWinnable {
        public float WinTime { get; }
        public void Win();
        public event Action OnWin;
        public bool IsWin { get; }
    }
}