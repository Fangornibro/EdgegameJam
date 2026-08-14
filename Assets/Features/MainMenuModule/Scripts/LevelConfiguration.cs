using System;
using Features.LevelDesign.Scripts;
using UnityEngine;

namespace Features.MainMenuModule.Scripts {
    [Serializable]
    public class LevelConfiguration {
        [field: SerializeField] public Sprite LevelIcon { get; private set; }
        [field: SerializeField] public LevelBehaviour LevelBehaviour { get; private set; }
    }
}