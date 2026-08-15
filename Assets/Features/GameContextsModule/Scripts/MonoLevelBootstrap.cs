using Features.EdgePaintingModule.Scripts;
using Features.LevelDesign.Scripts;
using UnityEngine;

namespace Features.GameContextsModule.Scripts {
    public class MonoLevelBootstrap : MonoBehaviour {
        [SerializeField] private Transform _levelContainer;

        private void Awake() {
            EdgesModel edgesModel = new();
            ServiceLocator.Register(edgesModel);
            ServiceLocator.Register<IEdgesService>(new EdgesService(edgesModel));

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
