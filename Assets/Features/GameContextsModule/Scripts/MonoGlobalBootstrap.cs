using Features.CharacterModule.Scripts;
using Features.LevelDesign.Scripts;
using UnityEngine;

namespace Features.GameContextsModule.Scripts {
    public class MonoGlobalBootstrap : MonoBehaviour {
        [SerializeField] private string _firstSceneName = SceneNames.MainMenu;
        [SerializeField] private CharacterEntity _mainCharacter;

        private void Awake() {
            ServiceLocator.Register(new CurrentLevelModel());
            ServiceLocator.Register(new SceneLoader(gameObject.scene));
            ServiceLocator.Register<ICharacterFactory>(new CharacterFactory(_mainCharacter));
        }

        private async void Start() {
            await ServiceLocator.Get<SceneLoader>().LoadAsync(_firstSceneName);
        }

        private void OnDestroy() {
            ServiceLocator.Clear();
        }
    }
}
