using System.Collections.Generic;
using UnityEngine;

namespace Features.MainMenuModule.Scripts {
    [CreateAssetMenu(menuName = "Create LevelsSequenceConfiguration", fileName = "LevelsSequenceConfiguration", order = 0)]
    public class LevelsSequenceConfiguration : ScriptableObject {
        [field: SerializeField] public List<LevelConfiguration> LevelsSequence;

        [field: Header("Debug")]
        [field: Tooltip("Testing switch: every level is selectable regardless of what is completed.")]
        [field: SerializeField] public bool IsEverythingUnlocked { get; private set; }
    }
}