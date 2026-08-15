using System.Collections.Generic;
using Features.GameContextsModule.Scripts;
using Features.LevelDesign.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Features.MainMenuModule.Scripts.Views {
    public class SelectLevelView : MonoBehaviour {
        [SerializeField] private Button _backButton;
        [SerializeField] private Transform _levelsContainer;
        [SerializeField] private LevelView _levelViewPrefab;
        private readonly List<LevelView> _levelViews = new();

        private void Start() {
            List<LevelConfiguration> list = ServiceLocator.Get<LevelsSequenceConfiguration>().LevelsSequence;
            for (int index = 0; index < list.Count; index++) {
                LevelConfiguration levelConfiguration = list[index];
                LevelConfiguration backLevelConfiguration = null;
                if(index != 0) 
                    backLevelConfiguration = list[index - 1];
                LevelView levelView = Instantiate(_levelViewPrefab, _levelsContainer);
                Dictionary<LevelBehaviour, LevelCompletionData> levelCompletionDatas = ServiceLocator.Get<LevelsModel>().LevelCompletionDatas;
                bool isUnLocked = backLevelConfiguration == null || levelCompletionDatas[backLevelConfiguration.LevelBehaviour].IsCompleted;
                levelView.Initialize(levelConfiguration, levelCompletionDatas[levelConfiguration.LevelBehaviour].Score, isUnLocked, index + 1);
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