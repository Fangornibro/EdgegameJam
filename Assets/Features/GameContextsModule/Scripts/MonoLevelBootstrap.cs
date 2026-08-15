using Features.EdgePaintingModule.Scripts;
using Features.LevelDesign.Scripts;
using UnityEngine;

namespace Features.GameContextsModule.Scripts {
    public class MonoLevelBootstrap : MonoBehaviour {
        [SerializeField] private Transform _levelContainer;

        [Tooltip("How much of the screen the overlap must cover before the zones collapse to it.")]
        [SerializeField, Range(0f, 0.5f)] private float _minVisibleAreaFraction = 0.01f;

        private void Awake() {
            EdgesModel edgesModel = new();
            ServiceLocator.Register(edgesModel);
            ServiceLocator.Register<IEdgesService>(
                new EdgesService(edgesModel, Camera.main, minVisibleAreaFraction: _minVisibleAreaFraction));

            LevelsModel levelsModel = ServiceLocator.Get<LevelsModel>();

            if (levelsModel.CurrentLevelPrefab == null) {
                Debug.LogError($"{nameof(LevelsModel)}.{nameof(LevelsModel.CurrentLevelPrefab)} is not set, nothing to spawn.", this);
                return;
            }

            levelsModel.CurrentLevelInstance = Instantiate(levelsModel.CurrentLevelPrefab, _levelContainer);
        }

        private void OnDestroy() {
            if (ServiceLocator.TryGet(out LevelsModel currentLevelModel))
                currentLevelModel.ClearLevelInstance();

            ServiceLocator.Unregister<IEdgesService>();
            ServiceLocator.Unregister<EdgesModel>();
        }
    }
}
