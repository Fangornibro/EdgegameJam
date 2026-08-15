using System;

namespace Features.CharacterModule.Scripts {
    public class CharacterModel {
        private CharacterEntity _currentCharacter;

        public event Action<CharacterEntity> CharacterSpawned;

        public CharacterEntity CurrentCharacter {
            get => _currentCharacter;
            set {
                _currentCharacter = value;

                if (_currentCharacter != null)
                    CharacterSpawned?.Invoke(_currentCharacter);
            }
        }

        public void Clear() {
            _currentCharacter = null;
        }
    }
}
