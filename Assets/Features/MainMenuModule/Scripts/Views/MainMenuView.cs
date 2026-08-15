using Features.GameContextsModule.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Features.MainMenuModule.Scripts.Views {
    public class MainMenuView : MonoBehaviour {
        [SerializeField] private Button _selectLevelButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private SelectLevelView _selectLevelViewPrefab;
        [SerializeField] private Canvas _window;

        private IUISoundService _uiSoundService;

        private void OnEnable() {
            _uiSoundService ??= ServiceLocator.Get<IUISoundService>();

            _selectLevelButton.onClick.AddListener(OpenSelectLevelView);
            _quitButton.onClick.AddListener(QuitApplication);
        }

        private void OnDisable() {
            _selectLevelButton.onClick.RemoveListener(OpenSelectLevelView);
            _quitButton.onClick.RemoveListener(QuitApplication);
        }

        private void OpenSelectLevelView() {
            _uiSoundService.PlayClick();
            Instantiate(_selectLevelViewPrefab, _window.transform);
        }

        private void QuitApplication() {
            _uiSoundService.PlayClick();
            Application.Quit();
        }
    }
}
