using System;

namespace Features.LevelDesign.Scripts {
    public class CurrentLevelModel {
        public LevelBehaviour LevelPrefab { get; set; }
        public LevelBehaviour LevelInstance { get; set; }
        public bool IsSwitchingScene { get; set; }
        public event Action OnSceneSwitchStarted;

        public void InvokeOnSceneSwitchStarted() =>
            OnSceneSwitchStarted?.Invoke();

        public void ClearLevelInstance() {
            LevelInstance = null;
        }
    }
}
