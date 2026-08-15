using Features.GameContextsModule.Scripts;
using Features.LevelDesign.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Features.LevelHUDModule.Scripts {
    public class HUDView : MonoBehaviour {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _goToMainMenuButton;

        private ILevelSceneService _levelSceneService;
        private CurrentLevelModel _currentLevelModel;
        
        private void OnEnable() {
            _levelSceneService ??= ServiceLocator.Get<ILevelSceneService>();
            _currentLevelModel ??= ServiceLocator.Get<CurrentLevelModel>();
            
            _restartButton.onClick.AddListener(Restart);
            _goToMainMenuButton.onClick.AddListener(GoToMainMenu);
            _currentLevelModel.OnSceneSwitchStarted += SetButtonsNotInteractable;

        }

        private void OnDisable() {
            _restartButton.onClick.RemoveListener(Restart);
            _goToMainMenuButton.onClick.RemoveListener(GoToMainMenu);
            _currentLevelModel.OnSceneSwitchStarted -= SetButtonsNotInteractable;
        }

        private async void Restart() =>
            await _levelSceneService.RestartCurrentLevel();

        private async void GoToMainMenu() =>
            await _levelSceneService.GoToMainMenu();

        private void SetButtonsNotInteractable() {
            _restartButton.interactable = false;
            _goToMainMenuButton.interactable = false;
        }
    }
}