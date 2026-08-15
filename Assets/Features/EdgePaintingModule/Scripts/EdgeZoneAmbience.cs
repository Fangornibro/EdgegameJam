using System.Collections.Generic;
using Features.CharacterModule.Scripts;
using Features.GameContextsModule.Scripts;
using UnityEngine;

namespace Features.EdgePaintingModule.Scripts {
    [RequireComponent(typeof(AudioSource))]
    public class EdgeZoneAmbience : MonoBehaviour {
        [SerializeField] private AudioSource _audioSource;

        [Header("Falloff")]
        [Tooltip("Distance from the zone border at which the sound is no longer audible.")]
        [SerializeField] private float _maxDistance = 12f;
        [SerializeField] private float _maxVolume = 1f;
        [SerializeField] private AnimationCurve _falloff = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        [SerializeField] private float _volumeSmoothing = 6f;

        private EdgesModel _edgesModel;
        private IEdgesService _edgesService;
        private CharacterModel _characterModel;
        private Transform _listenerTarget;
        private float _currentVolume;

        private void Awake() {
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();

            _audioSource.loop = true;
            _audioSource.playOnAwake = false;
            _audioSource.volume = 0f;
        }

        private void Start() {
            _edgesModel = ServiceLocator.Get<EdgesModel>();
            _edgesService = ServiceLocator.Get<IEdgesService>();
            _characterModel = ServiceLocator.Get<CharacterModel>();

            // The character is spawned by the level, which may happen after this component starts.
            _characterModel.CharacterSpawned += OnCharacterSpawned;

            if (_characterModel.CurrentCharacter != null)
                OnCharacterSpawned(_characterModel.CurrentCharacter);

            if (_audioSource.clip != null)
                _audioSource.Play();
        }

        private void OnDestroy() {
            if (_characterModel != null)
                _characterModel.CharacterSpawned -= OnCharacterSpawned;
        }

        private void OnCharacterSpawned(CharacterEntity characterEntity) {
            _listenerTarget = characterEntity.transform;
        }

        private void Update() {
            if (_listenerTarget == null)
                return;

            float targetVolume = GetTargetVolume(_listenerTarget.position);

            _currentVolume = _volumeSmoothing <= 0f
                ? targetVolume
                : Mathf.MoveTowards(_currentVolume, targetVolume, _volumeSmoothing * Time.deltaTime);

            _audioSource.volume = _currentVolume;
        }

        private float GetTargetVolume(Vector2 position) {
            // The preview edge counts here: the zone should already be audible while drawing.
            if (_edgesModel.PreviewEdge == null && _edgesModel.Count == 0)
                return 0f;

            if (_edgesService.IsInSwappedArea(position, true))
                return _maxVolume;

            float distance = GetDistanceToZone(position);

            if (distance >= _maxDistance)
                return 0f;

            return _falloff.Evaluate(Mathf.Clamp01(distance / _maxDistance)) * _maxVolume;
        }

        // Outside the zone the signed distance to an edge is positive, and it is exactly how far
        // the listener would have to walk perpendicularly to step inside that half plane.
        private float GetDistanceToZone(Vector2 position) {
            bool isIntersection = _edgesService.IsPreviewIntersectionMode;
            float unionDistance = float.MaxValue;
            float intersectionDistance = 0f;

            foreach (EdgeData edge in GetEdges()) {
                float signedDistance = Mathf.Max(edge.GetSignedDistance(position), 0f);

                unionDistance = Mathf.Min(unionDistance, signedDistance);
                intersectionDistance = Mathf.Max(intersectionDistance, signedDistance);
            }

            return isIntersection ? intersectionDistance : unionDistance;
        }

        private IEnumerable<EdgeData> GetEdges() {
            foreach (EdgeData edge in _edgesModel.Edges)
                yield return edge;

            if (_edgesModel.PreviewEdge != null)
                yield return _edgesModel.PreviewEdge;
        }
    }
}
