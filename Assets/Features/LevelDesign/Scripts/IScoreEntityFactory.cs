using UnityEngine;

namespace Features.LevelDesign.Scripts {
    public interface IScoreEntityFactory {
        public ScoreEntity CreateScoreEntity(Vector3 position, Transform parent);
    }
}