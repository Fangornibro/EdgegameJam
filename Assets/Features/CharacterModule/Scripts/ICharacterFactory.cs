using UnityEngine;

namespace Features.CharacterModule.Scripts {
    public interface ICharacterFactory {
        public void CreateCharacter(Transform parent, Vector2 position);
    }
}