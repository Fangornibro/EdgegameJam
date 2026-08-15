using Features.CharacterModule.Scripts;
using Features.LevelDesign.Scripts;
using Features.MainMenuModule.Scripts;
using UnityEngine;

namespace Features.GameContextsModule.Scripts {
    public class MonoGlobalBootstrap : MonoBehaviour {
        [SerializeField] private string _firstSceneName = SceneNames.MainMenu;
        [SerializeField] private CharacterEntity _mainCharacter;
        [SerializeField] private LevelsSequenceConfiguration _levelsSequenceConfiguration;
        [SerializeField] private ScoreEntity _scoreEntityPrefab;

        private void Awake() {
            LevelsModel levelsModel = new();
            CharacterModel characterModel = new();
            SceneLoader sceneLoader = new(gameObject.scene);
            ServiceLocator.Register(levelsModel);
            ServiceLocator.Register(characterModel);
            ServiceLocator.Register(sceneLoader);
            ServiceLocator.Register(_levelsSequenceConfiguration);
            ServiceLocator.Register<ICharacterFactory>(new CharacterFactory(_mainCharacter, characterModel));
            ServiceLocator.Register<IScoreEntityFactory>(new ScoreEntityFactory(_scoreEntityPrefab, levelsModel));
            ServiceLocator.Register<ILevelSceneService>(new LevelSceneService(sceneLoader, levelsModel, _levelsSequenceConfiguration));
            levelsModel.SetupLevelCompletionDatas(_levelsSequenceConfiguration);
        }

        private async void Start() {
            await ServiceLocator.Get<SceneLoader>().LoadAsync(_firstSceneName);
        }

        private void OnDestroy() {
            ServiceLocator.Clear();
        }
    }
}
