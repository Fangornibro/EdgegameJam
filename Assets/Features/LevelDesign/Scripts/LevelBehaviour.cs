using UnityEngine;

namespace Features.LevelDesign.Scripts {
    public class LevelBehaviour : MonoBehaviour {
        [field: SerializeField, Min(1)] public int MaxEdgeCount { get; private set; }
    }
}