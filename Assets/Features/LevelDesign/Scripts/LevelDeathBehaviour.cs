using System.Collections;
using Features.CharacterModule.Scripts;
using Features.GameContextsModule.Scripts;
using UnityEngine;

namespace Features.LevelDesign.Scripts {
    public class LevelDeathBehaviour : MonoBehaviour {
        [SerializeField] private CollisionTriggerReactor _levelDeathCollisionTriggerReactor;
        
        private ILevelSceneService _levelSceneService;
        private Coroutine _delayResetCoroutine;

        private void Start() {
            _levelSceneService = ServiceLocator.Get<ILevelSceneService>();
        }

        private void OnEnable() {
            _levelDeathCollisionTriggerReactor.OnTriggerEnterEvent += Death;
        }

        private void OnDisable() {
            _levelDeathCollisionTriggerReactor.OnTriggerEnterEvent -= Death;
            if(_delayResetCoroutine != null) 
                StopCoroutine(_delayResetCoroutine);
        }

        private void Death(Collider2D other) {
            if (!other.TryGetComponent(out IKillable killable))
                return;

            if(killable.IsDead)
                return;
                
            killable.Kill();
            _delayResetCoroutine = StartCoroutine(DelayedRestartLevel(killable.DeathTime));
        }

        private IEnumerator DelayedRestartLevel(float delay) {
            yield return new WaitForSeconds(delay);
            
            _levelSceneService.RestartCurrentLevel();
        }
    }
}