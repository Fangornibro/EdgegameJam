using UnityEngine;
using UnityEngine.UI;

namespace Features.MainMenuModule.Scripts.Views {
    public class MainMenuView : MonoBehaviour {
        [SerializeField] private Button _selectLevelButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private SelectLevelView _selectLevelViewPrefab;
        [SerializeField] private Canvas _window;
        
        
        private void OnEnable() {
            _selectLevelButton.onClick.AddListener(OpenSelectLevelView);
            _quitButton.onClick.AddListener(QuitApplication);
        }

        private void OnDisable() {
            _selectLevelButton.onClick.RemoveListener(OpenSelectLevelView);
            _quitButton.onClick.RemoveListener(QuitApplication);
        }

        private void OpenSelectLevelView() {
            Instantiate(_selectLevelViewPrefab, _window.transform);
        }

        private void QuitApplication() =>
            Application.Quit();
    }
}
