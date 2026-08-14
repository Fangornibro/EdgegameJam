using UnityEngine;

namespace Features.CharacterModule.Scripts {
    public class CharacterFactory : ICharacterFactory {
        private readonly CharacterEntity _characterEntityPrefab;

        public CharacterFactory(CharacterEntity characterEntityPrefab) {
            _characterEntityPrefab = characterEntityPrefab;
        }
        
        public void CreateCharacter(Transform parent, Vector2 position) {
            Object.Instantiate(_characterEntityPrefab, position, Quaternion.identity, parent);
        }
    }
}