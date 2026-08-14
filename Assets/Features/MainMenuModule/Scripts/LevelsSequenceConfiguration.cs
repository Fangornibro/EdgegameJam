using System.Collections.Generic;
using UnityEngine;

namespace Features.MainMenuModule.Scripts {
    [CreateAssetMenu(menuName = "Create LevelsSequenceConfiguration", fileName = "LevelsSequenceConfiguration", order = 0)]
    public class LevelsSequenceConfiguration : ScriptableObject {
        [field: SerializeField] public List<LevelConfiguration> LevelsSequence;
    }
}