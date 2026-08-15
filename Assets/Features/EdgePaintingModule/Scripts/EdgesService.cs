using System.Collections.Generic;
using UnityEngine;

namespace Features.EdgePaintingModule.Scripts {
    public class EdgesService : IEdgesService {
        private const float FallbackExtent = 500f;

        private readonly EdgesModel _edgesModel;
        private readonly Camera _camera;
        private readonly float _onLineThreshold;
        private readonly float _minVisibleAreaFraction;
        private readonly List<Vector2> _clipBuffer = new();
        private readonly List<Vector2> _clippedPolygon = new();

        private int _cachedEdgeCount = -1;
        private Rect _cachedViewRect;
        private bool _isIntersectionMode;

        // minVisibleAreaFraction: how much of the visible screen the overlap must cover before it
        // counts as a real zone. Below it the overlap is treated as invisible and the zones stay united.
        public EdgesService(EdgesModel edgesModel, Camera camera, float onLineThreshold = 0.001f,
            float minVisibleAreaFraction = 0.01f) {
            _edgesModel = edgesModel;
            _camera = camera;
            _onLineThreshold = onLineThreshold;
            _minVisibleAreaFraction = minVisibleAreaFraction;
        }

        // True when the right half planes of all edges overlap inside the camera view,
        // so the swapped area is their intersection instead of their union.
        public bool IsIntersectionMode {
            get {
                Rect viewRect = GetViewRect();
                int activeCount = GetActiveEdgeCount();

                // While an edge is being dragged its geometry changes every frame, so the
                // cached answer would be one frame stale.
                bool isDragging = _edgesModel.PreviewEdge != null;

                if (isDragging || _cachedEdgeCount != activeCount || _cachedViewRect != viewRect) {
                    _cachedEdgeCount = activeCount;
                    _cachedViewRect = viewRect;
                    _isIntersectionMode = activeCount > 1 && IsIntersectionVisible(viewRect);
                }

                return _isIntersectionMode;
            }
        }

        public EdgeSide GetSide(EdgeData edge, Vector2 position) {
            if (edge == null)
                return EdgeSide.On;

            float signedDistance = edge.GetSignedDistance(position);

            if (Mathf.Abs(signedDistance) <= _onLineThreshold)
                return EdgeSide.On;

            return signedDistance > 0f ? EdgeSide.Left : EdgeSide.Right;
        }

        public bool IsInSwappedArea(Vector2 position) {
            if (GetActiveEdgeCount() == 0)
                return false;

            if (IsIntersectionMode) {
                foreach (EdgeData edgeData in GetActiveEdges()) {
                    if (GetSide(edgeData, position) == EdgeSide.Left)
                        return false;
                }

                return true;
            }

            foreach (EdgeData edgeData in GetActiveEdges()) {
                if (GetSide(edgeData, position) == EdgeSide.Right)
                    return true;
            }

            return false;
        }

        public List<EdgeSide> GetSideOfEdges(Vector2 position) {
            List<EdgeSide> edgeSides = new();

            foreach (EdgeData edgeData in _edgesModel.Edges)
                edgeSides.Add(GetSide(edgeData, position));

            return edgeSides;
        }

        private IEnumerable<EdgeData> GetActiveEdges() {
            foreach (EdgeData edgeData in _edgesModel.Edges)
                yield return edgeData;

            if (_edgesModel.PreviewEdge != null)
                yield return _edgesModel.PreviewEdge;
        }

        private int GetActiveEdgeCount() =>
            _edgesModel.Count + (_edgesModel.PreviewEdge == null ? 0 : 1);

        private Rect GetViewRect() {
            if (_camera == null)
                return new Rect(-FallbackExtent, -FallbackExtent, FallbackExtent * 2f, FallbackExtent * 2f);

            Vector2 bottomLeft = _camera.ViewportToWorldPoint(Vector3.zero);
            Vector2 topRight = _camera.ViewportToWorldPoint(Vector3.one);

            return new Rect(bottomLeft, topRight - bottomLeft);
        }

        // Clips the visible rect by every right half plane (Sutherland-Hodgman).
        // What survives is the part of the overlap the player can actually see.
        private bool IsIntersectionVisible(Rect viewRect) {
            _clippedPolygon.Clear();
            _clippedPolygon.Add(new Vector2(viewRect.xMin, viewRect.yMin));
            _clippedPolygon.Add(new Vector2(viewRect.xMax, viewRect.yMin));
            _clippedPolygon.Add(new Vector2(viewRect.xMax, viewRect.yMax));
            _clippedPolygon.Add(new Vector2(viewRect.xMin, viewRect.yMax));

            foreach (EdgeData edgeData in GetActiveEdges()) {
                ClipByEdge(edgeData);

                if (_clippedPolygon.Count < 3)
                    return false;
            }

            float viewArea = Mathf.Max(viewRect.width * viewRect.height, Mathf.Epsilon);

            return GetPolygonArea() / viewArea >= _minVisibleAreaFraction;
        }

        private void ClipByEdge(EdgeData edge) {
            _clipBuffer.Clear();
            _clipBuffer.AddRange(_clippedPolygon);
            _clippedPolygon.Clear();

            for (int i = 0; i < _clipBuffer.Count; i++) {
                Vector2 current = _clipBuffer[i];
                Vector2 next = _clipBuffer[(i + 1) % _clipBuffer.Count];

                // Negative distance means the point is on the right side, the side we keep.
                float currentDistance = edge.GetSignedDistance(current);
                float nextDistance = edge.GetSignedDistance(next);

                if (currentDistance <= 0f)
                    _clippedPolygon.Add(current);

                if (currentDistance * nextDistance < 0f) {
                    float t = currentDistance / (currentDistance - nextDistance);
                    _clippedPolygon.Add(Vector2.Lerp(current, next, t));
                }
            }
        }

        private float GetPolygonArea() {
            float doubleArea = 0f;

            for (int i = 0; i < _clippedPolygon.Count; i++) {
                Vector2 current = _clippedPolygon[i];
                Vector2 next = _clippedPolygon[(i + 1) % _clippedPolygon.Count];

                doubleArea += current.x * next.y - next.x * current.y;
            }

            return Mathf.Abs(doubleArea) * 0.5f;
        }
    }
}
