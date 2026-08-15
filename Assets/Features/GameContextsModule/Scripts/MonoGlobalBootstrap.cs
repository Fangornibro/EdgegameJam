using Features.CharacterModule.Scripts;
using Features.LevelDesign.Scripts;
using Features.MainMenuModule.Scripts;
using UnityEngine;

namespace Features.GameContextsModule.Scripts {
    public class MonoGlobalBootstrap : MonoBehaviour {
        [SerializeField] private string _firstSceneName = SceneNames.MainMenu;
        [SerializeField] private CharacterEntity _mainCharacter;
        [SerializeField] private LevelsSequenceConfiguration _levelsSequenceConfiguration;

        private void Awake() {
            CurrentLevelModel currentLevelModel = new();
            SceneLoader sceneLoader = new(gameObject.scene);
            ServiceLocator.Register(currentLevelModel);
            ServiceLocator.Register(sceneLoader);
            ServiceLocator.Register(_levelsSequenceConfiguration);
            ServiceLocator.Register<ICharacterFactory>(new CharacterFactory(_mainCharacter));
            ServiceLocator.Register<ILevelSceneService>(new LevelSceneService(sceneLoader, currentLevelModel, _levelsSequenceConfiguration));
        }

        private async void Start() {
            await ServiceLocator.Get<SceneLoader>().LoadAsync(_firstSceneName);
        }

        private void OnDestroy() {
            ServiceLocator.Clear();
        }
    }
}
