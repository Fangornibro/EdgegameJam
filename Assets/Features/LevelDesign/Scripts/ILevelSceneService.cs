using System.Threading.Tasks;

namespace Features.LevelDesign.Scripts {
    public interface ILevelSceneService {
        public Task RestartCurrentLevel(bool withScoreCleanup = true);
        public Task GoToNextLevel();
        public Task GoToMainMenu(bool withScoreCleanup = true);
    }
}