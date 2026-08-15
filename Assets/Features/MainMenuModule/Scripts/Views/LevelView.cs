using System.Collections.Generic;
using Features.GameContextsModule.Scripts;
using Features.LevelDesign.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Features.MainMenuModule.Scripts.Views {
    public class LevelView : MonoBehaviour {
        [SerializeField] private Image _levelImage;
        [SerializeField] private Button _levelButton;
        [SerializeField] private List<ScoreView> _scoreViews;
        private LevelConfiguration _levelConfiguration;

        public void Initialize(LevelConfiguration levelConfiguration, int score, bool isUnLocked) {
            _levelConfiguration = levelConfiguration;
            _levelImage.sprite = _levelConfiguration.LevelIcon;
            for (int i = 0; i < _scoreViews.Count; i++) {
                if (i < score) {
                    _scoreViews[i].ActiveScoreObject.SetActive(true);
                    _scoreViews[i].UnActiveScoreObject.SetActive(false);
                }
                else {
                    _scoreViews[i].ActiveScoreObject.SetActive(false);
                    _scoreViews[i].UnActiveScoreObject.SetActive(true);
                }
            }

            _levelButton.interactable = isUnLocked;
            _levelButton.onClick.AddListener(StartLevel);
        }

        public void Dispose() {
            _levelButton.onClick.RemoveListener(StartLevel);
        }

        private async void StartLevel() {
            _levelButton.interactable = false;
            ServiceLocator.Get<LevelsModel>().CurrentLevelPrefab = _levelConfiguration.LevelBehaviour;
            await ServiceLocator.Get<SceneLoader>().SwitchToAsync(SceneNames.Level);
        }
    }
}
