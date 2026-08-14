using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Features.GameContextsModule.Scripts {
    public class SceneLoader {
        private readonly Scene _globalScene;
        private readonly List<string> _loadedScenes = new();

        public SceneLoader(Scene globalScene) {
            _globalScene = globalScene;
        }

        public IReadOnlyList<string> LoadedScenes => _loadedScenes;

        public bool IsLoaded(string sceneName) =>
            _loadedScenes.Contains(sceneName);

        public async Task LoadAsync(string sceneName) {
            if (IsLoaded(sceneName))
                throw new InvalidOperationException($"Scene {sceneName} is already loaded.");

            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            _loadedScenes.Add(sceneName);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }

        public async Task UnloadAsync(string sceneName) {
            if (!IsLoaded(sceneName))
                throw new InvalidOperationException($"Scene {sceneName} is not loaded.");

            _loadedScenes.Remove(sceneName);
            await SceneManager.UnloadSceneAsync(sceneName);

            if (_loadedScenes.Count == 0)
                SceneManager.SetActiveScene(_globalScene);
        }

        public async Task SwitchToAsync(string sceneName) {
            foreach (string loadedScene in _loadedScenes.ToArray())
                await UnloadAsync(loadedScene);

            await LoadAsync(sceneName);
            await Resources.UnloadUnusedAssets();
        }
    }
}
