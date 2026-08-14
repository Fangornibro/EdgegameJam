using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Features.MainMenuModule.Scripts.Views {
    public class LevelView : MonoBehaviour {
        [SerializeField] private Image _levelImage;
        [SerializeField] private Button _levelButton;
        private LevelConfiguration _levelConfiguration;

        public void Initialize(LevelConfiguration levelConfiguration) {
            _levelConfiguration = levelConfiguration;
            _levelImage.sprite = _levelConfiguration.LevelIcon;
            _levelButton.onClick.AddListener(StartLevel);
        }

        public void Dispose() {
            _levelButton.onClick.RemoveListener(StartLevel);
        }

        private void StartLevel() {
            SceneManager.LoadScene("TestLevel", LoadSceneMode.Single);
        }
    }
}