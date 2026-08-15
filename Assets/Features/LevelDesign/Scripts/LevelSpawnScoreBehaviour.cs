using System.Collections.Generic;
using Features.GameContextsModule.Scripts;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Features.LevelDesign.Scripts {
    public class LevelSpawnScoreBehaviour : MonoBehaviour {
        [SerializeField] private Transform _spawnRoot;
        [SerializeField] private Tilemap _scoreSpawnPoint;
        
        private void Awake() {
            IScoreEntityFactory scoreEntityFactory = ServiceLocator.Get<IScoreEntityFactory>();
            List<Vector3> spawnPositions = GetSpawnPositions();

            foreach (Vector3 spawnPosition in spawnPositions)
                scoreEntityFactory.CreateScoreEntity(spawnPosition, _spawnRoot);
            
            _scoreSpawnPoint.gameObject.SetActive(false);
        }

        private List<Vector3> GetSpawnPositions() {
            List<Vector3> spawnPoints = new();
            foreach (Vector3Int cell in _scoreSpawnPoint.cellBounds.allPositionsWithin) {
                if (_scoreSpawnPoint.HasTile(cell))
                    spawnPoints.Add(_scoreSpawnPoint.GetCellCenterWorld(cell));
            }

            return spawnPoints;
        }
    }
}