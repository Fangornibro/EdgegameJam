using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Features.MainMenuModule.Scripts.Views {
    public class SelectLevelView : MonoBehaviour {
        [SerializeField] private Button _backButton;
        [SerializeField] private LevelsSequenceConfiguration _levelsSequenceConfiguration;
        [SerializeField] private Transform _levelsContainer;
        [SerializeField] private LevelView _levelViewPrefab;
        private readonly List<LevelView> _levelViews = new();

        private void Awake() {
            foreach (LevelConfiguration levelConfiguration in _levelsSequenceConfiguration.LevelsSequence) {
                LevelView levelView = Instantiate(_levelViewPrefab, _levelsContainer);
                levelView.Initialize(levelConfiguration);
                _levelViews.Add(levelView);
            }
        }

        private void OnDestroy() {
            foreach (LevelView levelView in _levelViews)
                levelView.Dispose();
            
            _levelViews.Clear();
        }

        private void OnEnable() {
            _backButton.onClick.AddListener(CloseView);
        }

        private void OnDisable() {
            _backButton.onClick.RemoveListener(CloseView);
        }

        private void CloseView() =>
            Destroy(gameObject);
    }
}