using UnityEngine;
using UnityEngine.SceneManagement;

namespace Features.GameContextsModule.Scripts {
    public class MonoGlobalBootstrap : MonoBehaviour {
        [SerializeField] private string _firstSceneName = "MainMenuScene";

        public void Awake() {
            SceneManager.LoadScene(_firstSceneName, LoadSceneMode.Single);
        }    
    }

    public class MonoLevelBootstrap : MonoBehaviour {
        
    }
}
