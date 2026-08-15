namespace Features.LevelDesign.Scripts {
    public class LevelCompletionData {
        public bool IsCompleted { get; set; }
        public int Score { get; set; }

        public LevelCompletionData(bool isCompleted, int score) {
            IsCompleted = isCompleted;
            Score = score;
        }
    }
}