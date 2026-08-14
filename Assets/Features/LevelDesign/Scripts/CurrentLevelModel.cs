namespace Features.LevelDesign.Scripts {
    public class CurrentLevelModel {
        public LevelBehaviour LevelPrefab { get; set; }
        public LevelBehaviour LevelInstance { get; set; }

        public void Clear() {
            LevelPrefab = null;
            LevelInstance = null;
        }
    }
}
