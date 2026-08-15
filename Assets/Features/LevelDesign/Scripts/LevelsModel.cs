using System;
using System.Collections.Generic;
using Features.MainMenuModule.Scripts;

namespace Features.LevelDesign.Scripts {
    public class LevelsModel {
        public LevelBehaviour CurrentLevelPrefab { get; set; }
        public LevelBehaviour CurrentLevelInstance { get; set; }
        public readonly Dictionary<LevelBehaviour, LevelCompletionData> LevelCompletionDatas = new();
        public bool IsSwitchingScene { get; set; }
        public event Action OnSceneSwitchStarted;

        public void SetupLevelCompletionDatas(LevelsSequenceConfiguration levelsSequenceConfiguration) {
            foreach (LevelConfiguration levelConfiguration in levelsSequenceConfiguration.LevelsSequence)
                LevelCompletionDatas[levelConfiguration.LevelBehaviour] = new LevelCompletionData(false, 0);
        }

        public void InvokeOnSceneSwitchStarted() =>
            OnSceneSwitchStarted?.Invoke();

        public void ClearLevelInstance() {
            CurrentLevelInstance = null;
        }
    }
}
