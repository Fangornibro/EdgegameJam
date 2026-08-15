using System.Threading.Tasks;
using Features.GameContextsModule.Scripts;
using Features.MainMenuModule.Scripts;
using UnityEngine;

namespace Features.LevelDesign.Scripts {
    public class LevelSceneService : ILevelSceneService {
        private readonly SceneLoader _sceneLoader;
        private readonly LevelsModel _levelsModel;
        private readonly LevelsSequenceConfiguration _levelsSequenceConfiguration;

        public LevelSceneService(SceneLoader sceneLoader, LevelsModel levelsModel, LevelsSequenceConfiguration levelsSequenceConfiguration) {
            _sceneLoader = sceneLoader;
            _levelsModel = levelsModel;
            _levelsSequenceConfiguration = levelsSequenceConfiguration;
        }
        
        public async Task RestartCurrentLevel(bool withScoreCleanup = true) {
            if (_levelsModel.IsSwitchingScene)
                return;

            if(withScoreCleanup) 
                _levelsModel.LevelCompletionDatas[_levelsModel.CurrentLevelPrefab].Score = 0;
            _levelsModel.IsSwitchingScene = true;
            _levelsModel.InvokeOnSceneSwitchStarted();
            await _sceneLoader.SwitchToAsync(SceneNames.Level);
            _levelsModel.IsSwitchingScene = false;
        }

        public async Task GoToNextLevel() {
            int index = _levelsSequenceConfiguration.LevelsSequence.IndexOf(_levelsSequenceConfiguration.LevelsSequence.Find(l => l.LevelBehaviour == _levelsModel.CurrentLevelPrefab));
            _levelsModel.LevelCompletionDatas[_levelsModel.CurrentLevelPrefab].IsCompleted = true;
            if (_levelsSequenceConfiguration.LevelsSequence.Count > index + 1) {
                _levelsModel.CurrentLevelPrefab = _levelsSequenceConfiguration.LevelsSequence[index + 1].LevelBehaviour;
                await RestartCurrentLevel(false);
            }
            else
                await GoToMainMenu(false);
        }

        public async Task GoToMainMenu(bool withScoreCleanup = true) {
            if (_levelsModel.IsSwitchingScene)
                return;

            if(withScoreCleanup) 
                _levelsModel.LevelCompletionDatas[_levelsModel.CurrentLevelPrefab].Score = 0;
            _levelsModel.IsSwitchingScene = true;
            _levelsModel.InvokeOnSceneSwitchStarted();
            await _sceneLoader.SwitchToAsync(SceneNames.MainMenu);
            _levelsModel.IsSwitchingScene = false;
        }
    }
}