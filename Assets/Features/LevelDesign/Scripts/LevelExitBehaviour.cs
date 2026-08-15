using Features.GameContextsModule.Scripts;
using UnityEngine;

namespace Features.LevelDesign.Scripts {
    public class LevelExitBehaviour : MonoBehaviour {
        [SerializeField] private CollisionTriggerReactor _levelExitCollisionTriggerReactor;
        
        private ILevelSceneService _levelSceneService;

        private void Start() {
            _levelSceneService = ServiceLocator.Get<ILevelSceneService>();
        }

        private void OnEnable() {
            _levelExitCollisionTriggerReactor.OnTriggerEnterEvent += GoToNextLevel;
        }

        private void OnDisable() {
            _levelExitCollisionTriggerReactor.OnTriggerEnterEvent -= GoToNextLevel;
        }

        private void GoToNextLevel(Collider2D other) =>
            _levelSceneService.GoToNextLevel();
    }
}