using Features.GameContextsModule.Scripts;
using UnityEngine;

namespace Features.LevelDesign.Scripts {
    public class LevelDeathBehaviour : MonoBehaviour {
        [SerializeField] private CollisionTriggerReactor _levelDeathCollisionTriggerReactor;
        
        private ILevelSceneService _levelSceneService;

        private void Start() {
            _levelSceneService = ServiceLocator.Get<ILevelSceneService>();
        }

        private void OnEnable() {
            _levelDeathCollisionTriggerReactor.OnTriggerEnterEvent += Death;
        }

        private void OnDisable() {
            _levelDeathCollisionTriggerReactor.OnTriggerEnterEvent -= Death;
        }

        private void Death(Collider2D other) =>
            _levelSceneService.RestartCurrentLevel();
    }
}