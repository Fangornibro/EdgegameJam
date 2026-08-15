using UnityEngine;

namespace Features.LevelDesign.Scripts {
    public class ScoreEntityFactory : IScoreEntityFactory {
        private readonly ScoreEntity _scoreEntityPrefab;
        private readonly LevelsModel _levelsModel;

        public ScoreEntityFactory(ScoreEntity scoreEntityPrefab, LevelsModel levelsModel) {
            _scoreEntityPrefab = scoreEntityPrefab;
            _levelsModel = levelsModel;
        }
        
        public ScoreEntity CreateScoreEntity(Vector3 position, Transform parent) {
            ScoreEntity scoreEntity = Object.Instantiate(_scoreEntityPrefab, position, Quaternion.identity, parent);
            scoreEntity.InjectDependencies(_levelsModel);
            return scoreEntity;
        }
    }
}