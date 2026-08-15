using UnityEngine;

namespace Features.CharacterModule.Scripts {
    public class CharacterFactory : ICharacterFactory {
        private readonly CharacterEntity _characterEntityPrefab;
        private readonly CharacterModel _characterModel;

        public CharacterFactory(CharacterEntity characterEntityPrefab, CharacterModel characterModel) {
            _characterEntityPrefab = characterEntityPrefab;
            _characterModel = characterModel;
        }

        public void CreateCharacter(Transform parent, Vector2 position) {
            CharacterEntity characterEntity =
                Object.Instantiate(_characterEntityPrefab, position, Quaternion.identity, parent);

            _characterModel.CurrentCharacter = characterEntity;
        }
    }
}
