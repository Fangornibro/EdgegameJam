using System.Threading.Tasks;

namespace Features.LevelDesign.Scripts {
    public interface ILevelSceneService {
        public Task RestartCurrentLevel();
        public Task GoToNextLevel();
        public Task GoToMainMenu();
    }
}