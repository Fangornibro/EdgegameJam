using UnityEngine;

namespace Features.LevelDesign.Scripts {
    public class ScoreEntity : MonoBehaviour {
        private LevelsModel _levelsModel;

        public void InjectDependencies(LevelsModel levelsModel) {
            _levelsModel = levelsModel;
        }
        
        private void OnTriggerEnter2D(Collider2D other) {
            _levelsModel.LevelCompletionDatas[_levelsModel.CurrentLevelPrefab].Score++;
            Destroy(gameObject);
        }
    }
}