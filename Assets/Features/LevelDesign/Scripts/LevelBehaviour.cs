using System;
using Features.CharacterModule.Scripts;
using Features.GameContextsModule.Scripts;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Features.LevelDesign.Scripts {
    public class LevelBehaviour : MonoBehaviour {
        [SerializeField] private Transform _spawnRoot;
        [SerializeField] private Tilemap _spawnPoint;
        [field: SerializeField, Min(1)] public int MaxEdgeCount { get; private set; }

        private void Awake() {
            ServiceLocator.Get<ICharacterFactory>().CreateCharacter(_spawnRoot, GetSpawnPosition());
            _spawnPoint.gameObject.SetActive(false);
        }

        private Vector3 GetSpawnPosition() {
            foreach (Vector3Int cell in _spawnPoint.cellBounds.allPositionsWithin) {
                if (_spawnPoint.HasTile(cell))
                    return _spawnPoint.GetCellCenterWorld(cell);
            }

            throw new InvalidOperationException("SpawnPoint Tilemap has no tile.");
        }
    }
}