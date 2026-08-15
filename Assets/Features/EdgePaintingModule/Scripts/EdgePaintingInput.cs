using System;
using Features.GameContextsModule.Scripts;
using Features.LevelDesign.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Features.EdgePaintingModule.Scripts {
    public class EdgePaintingInput : MonoBehaviour {
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _pointPrefab;
        [SerializeField] private GameObject _pointEndPrefab;
        [SerializeField] private LineRenderer _linePrefab;
        [SerializeField] private Transform _edgesContainer;

        private EdgesModel _edgesModel;

        private int _edgesCount;

        private LineRenderer _currentLine;
        private Edge _currentEdge;
        private EdgeData _currentEdgeData;
        private Transform _currentEndPoint;
        private Vector2 _pressPosition;

        private void Awake() {
            if (_camera == null)
                _camera = Camera.main;
        }

        private void Start() {
            _edgesModel = ServiceLocator.Get<EdgesModel>();
        }

        private void Update() {
            Mouse mouse = Mouse.current;

            if (mouse == null)
                return;

            Vector2 pointerPosition = GetWorldPosition(mouse.position.ReadValue());

            if (mouse.leftButton.wasPressedThisFrame && !IsPointerOverUI())
                BeginEdge(pointerPosition);

            if (_currentLine != null) {
                _currentLine.SetPosition(1, pointerPosition);
                _currentEdgeData.SetPoints(_pressPosition, pointerPosition);
                UpdateEndPoint(pointerPosition);
            }

            if (mouse.leftButton.wasReleasedThisFrame)
                EndEdge(pointerPosition);
        }

        private void BeginEdge(Vector2 position) {
            if(_edgesCount >= ServiceLocator.Get<CurrentLevelModel>().LevelInstance.MaxEdgeCount)
                return;

            _pressPosition = position;
            SpawnPoint(position);

            _currentLine = Instantiate(_linePrefab, _edgesContainer);
            _currentLine.useWorldSpace = true;
            _currentLine.positionCount = 2;
            _currentLine.SetPosition(0, _pressPosition);
            _currentLine.SetPosition(1, _pressPosition);

            _currentEdge = _currentLine.GetComponent<Edge>();

            if (_currentEdge == null)
                _currentEdge = _currentLine.gameObject.AddComponent<Edge>();

            _currentEdgeData = new EdgeData(_pressPosition, _pressPosition);
            _currentEdge.Initialize(_currentEdgeData);

            _currentEndPoint = SpawnPointEnd(_pressPosition, Vector2.zero);
        }

        private void EndEdge(Vector2 position) {
            if (_currentLine == null)
                return;

            _currentLine.SetPosition(1, position);
            _currentEdgeData.SetPoints(_pressPosition, position);
            _edgesModel.Add(_currentEdgeData);
            UpdateEndPoint(position);

            _currentLine = null;
            _currentEdge = null;
            _currentEdgeData = null;
            _currentEndPoint = null;
            _edgesCount++;
        }

        private void SpawnPoint(Vector2 position) {
            if (_pointPrefab == null)
                return;

            Instantiate(_pointPrefab, position, Quaternion.identity, _edgesContainer);
        }
        
        private Transform SpawnPointEnd(Vector2 position, Vector2 direction) {
            if (_pointEndPrefab == null)
                return null;

            return Instantiate(_pointEndPrefab, position, GetRotation(direction), _edgesContainer).transform;
        }

        private void UpdateEndPoint(Vector2 position) {
            if (_currentEndPoint == null)
                return;

            _currentEndPoint.SetPositionAndRotation(position, GetRotation(position - _pressPosition));
        }

        private Quaternion GetRotation(Vector2 direction) {
            if (direction.sqrMagnitude <= Mathf.Epsilon)
                return Quaternion.identity;

            return Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }

        private bool IsPointerOverUI() =>
            EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        private Vector2 GetWorldPosition(Vector2 screenPosition) =>
            _camera == null ? screenPosition : (Vector2)_camera.ScreenToWorldPoint(screenPosition);
    }
}
