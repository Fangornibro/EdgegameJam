using Features.GameContextsModule.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Features.LevelHUDModule.Scripts {
    public class HUDView : MonoBehaviour {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _goToMainMenuButton;

        private SceneLoader _sceneLoader;
        private bool _isSwitchingScene;

        private void Start() {
            _sceneLoader = ServiceLocator.Get<SceneLoader>();
        }

        private void OnEnable() {
            _restartButton.onClick.AddListener(Restart);
            _goToMainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        private void OnDisable() {
            _restartButton.onClick.RemoveListener(Restart);
            _goToMainMenuButton.onClick.RemoveListener(GoToMainMenu);
        }

        private async void Restart() {
            if (_isSwitchingScene)
                return;

            _isSwitchingScene = true;
            SetButtonsInteractable(false);
            await _sceneLoader.SwitchToAsync(SceneNames.Level);
        }

        private async void GoToMainMenu() {
            if (_isSwitchingScene)
                return;

            _isSwitchingScene = true;
            SetButtonsInteractable(false);
            await _sceneLoader.SwitchToAsync(SceneNames.MainMenu);
        }

        private void SetButtonsInteractable(bool isInteractable) {
            _restartButton.interactable = isInteractable;
            _goToMainMenuButton.interactable = isInteractable;
        }
    }
}