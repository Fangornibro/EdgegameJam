using Features.GameContextsModule.Scripts;
using Features.LevelDesign.Scripts;
using UnityEngine;
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

        private async void StartLevel() {
            _levelButton.interactable = false;
            ServiceLocator.Get<CurrentLevelModel>().LevelPrefab = _levelConfiguration.LevelBehaviour;
            await ServiceLocator.Get<SceneLoader>().SwitchToAsync(SceneNames.Level);
        }
    }
}
