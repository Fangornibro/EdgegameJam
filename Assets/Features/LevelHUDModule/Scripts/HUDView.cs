using System.Collections.Generic;
using Features.GameContextsModule.Scripts;
using Features.LevelDesign.Scripts;
using Features.MainMenuModule.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Features.LevelHUDModule.Scripts {
    public class HUDView : MonoBehaviour {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _goToMainMenuButton;
        [SerializeField] private GameObject _tipsRoot;

        private ILevelSceneService _levelSceneService;
        private LevelsModel _levelsModel;
        private LevelsSequenceConfiguration _levelsSequenceConfiguration;

        private void OnEnable() {
            _levelSceneService ??= ServiceLocator.Get<ILevelSceneService>();
            _levelsModel ??= ServiceLocator.Get<LevelsModel>();
            _levelsSequenceConfiguration ??= ServiceLocator.Get<LevelsSequenceConfiguration>();

            UpdateTipsVisibility();

            _restartButton.onClick.AddListener(Restart);
            _goToMainMenuButton.onClick.AddListener(GoToMainMenu);
            _levelsModel.OnSceneSwitchStarted += SetButtonsNotInteractable;

        }

        private void OnDisable() {
            _restartButton.onClick.RemoveListener(Restart);
            _goToMainMenuButton.onClick.RemoveListener(GoToMainMenu);
            _levelsModel.OnSceneSwitchStarted -= SetButtonsNotInteractable;
        }

        private async void Restart() =>
            await _levelSceneService.RestartCurrentLevel();

        private async void GoToMainMenu() =>
            await _levelSceneService.GoToMainMenu();

        // Tips only make sense while the player is learning, so they live on the first level only.
        private void UpdateTipsVisibility() {
            if (_tipsRoot == null)
                return;

            List<LevelConfiguration> levelsSequence = _levelsSequenceConfiguration.LevelsSequence;
            bool isFirstLevel = levelsSequence.Count > 0
                                && levelsSequence[0].LevelBehaviour == _levelsModel.CurrentLevelPrefab;

            _tipsRoot.SetActive(isFirstLevel);
        }

        private void SetButtonsNotInteractable() {
            _restartButton.interactable = false;
            _goToMainMenuButton.interactable = false;
        }
    }
}