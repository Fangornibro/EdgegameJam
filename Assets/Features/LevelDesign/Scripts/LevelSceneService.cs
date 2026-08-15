using System.Threading.Tasks;
using Features.GameContextsModule.Scripts;
using Features.MainMenuModule.Scripts;
using UnityEngine;

namespace Features.LevelDesign.Scripts {
    public class LevelSceneService : ILevelSceneService {
        private readonly SceneLoader _sceneLoader;
        private readonly CurrentLevelModel _currentLevelModel;
        private readonly LevelsSequenceConfiguration _levelsSequenceConfiguration;

        public LevelSceneService(SceneLoader sceneLoader, CurrentLevelModel currentLevelModel, LevelsSequenceConfiguration levelsSequenceConfiguration) {
            _sceneLoader = sceneLoader;
            _currentLevelModel = currentLevelModel;
            _levelsSequenceConfiguration = levelsSequenceConfiguration;
        }
        
        public async Task RestartCurrentLevel() {
            if (_currentLevelModel.IsSwitchingScene)
                return;

            _currentLevelModel.IsSwitchingScene = true;
            _currentLevelModel.InvokeOnSceneSwitchStarted();
            await _sceneLoader.SwitchToAsync(SceneNames.Level);
            _currentLevelModel.IsSwitchingScene = false;
        }

        public async Task GoToNextLevel() {
            int index = _levelsSequenceConfiguration.LevelsSequence.IndexOf(_levelsSequenceConfiguration.LevelsSequence.Find(l => l.LevelBehaviour == _currentLevelModel.LevelPrefab));
            if (_levelsSequenceConfiguration.LevelsSequence.Count > index + 1) {
                _currentLevelModel.LevelPrefab = _levelsSequenceConfiguration.LevelsSequence[index + 1].LevelBehaviour;
                await RestartCurrentLevel();
            }
            else
                await GoToMainMenu();
        }

        public async Task GoToMainMenu() {
            if (_currentLevelModel.IsSwitchingScene)
                return;

            _currentLevelModel.IsSwitchingScene = true;
            _currentLevelModel.InvokeOnSceneSwitchStarted();
            await _sceneLoader.SwitchToAsync(SceneNames.MainMenu);
            _currentLevelModel.IsSwitchingScene = false;
        }
    }
}