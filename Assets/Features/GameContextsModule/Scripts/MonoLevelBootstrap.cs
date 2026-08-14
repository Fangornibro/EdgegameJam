using Features.LevelDesign.Scripts;
using UnityEngine;

namespace Features.GameContextsModule.Scripts {
    public class MonoLevelBootstrap : MonoBehaviour {
        [SerializeField] private Transform _levelContainer;

        private void Awake() {
            CurrentLevelModel currentLevelModel = ServiceLocator.Get<CurrentLevelModel>();

            if (currentLevelModel.LevelPrefab == null) {
                Debug.LogError($"{nameof(CurrentLevelModel)}.{nameof(CurrentLevelModel.LevelPrefab)} is not set, nothing to spawn.", this);
                return;
            }

            currentLevelModel.LevelInstance = Instantiate(currentLevelModel.LevelPrefab, _levelContainer);
        }

        private void OnDestroy() {
            if (ServiceLocator.TryGet(out CurrentLevelModel currentLevelModel))
                currentLevelModel.Clear();
        }
    }
}
