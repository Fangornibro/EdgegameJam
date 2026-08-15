using System.Collections;
using Features.CharacterModule.Scripts;
using Features.GameContextsModule.Scripts;
using UnityEngine;

namespace Features.LevelDesign.Scripts {
    public class LevelExitBehaviour : MonoBehaviour {
        [SerializeField] private CollisionTriggerReactor _levelExitCollisionTriggerReactor;
        
        private ILevelSceneService _levelSceneService;
        
        private Coroutine _delayGoToNextLevelCoroutine;

        private void Start() {
            _levelSceneService = ServiceLocator.Get<ILevelSceneService>();
        }

        private void OnEnable() {
            _levelExitCollisionTriggerReactor.OnTriggerEnterEvent += GoToNextLevel;
        }

        private void OnDisable() {
            _levelExitCollisionTriggerReactor.OnTriggerEnterEvent -= GoToNextLevel;
            if(_delayGoToNextLevelCoroutine != null) 
                StopCoroutine(_delayGoToNextLevelCoroutine);
        }

        private void GoToNextLevel(Collider2D other) {
            if (!other.TryGetComponent(out IWinnable winnable))
                return;

            if(winnable.IsWin)
                return;
                
            winnable.Win();
            _delayGoToNextLevelCoroutine = StartCoroutine(DelayedGoToNextLevel(winnable.WinTime));
            
            
        }

        private IEnumerator DelayedGoToNextLevel(float delay) {
            yield return new WaitForSeconds(delay);
            
            _levelSceneService.GoToNextLevel();
        }
    }
}