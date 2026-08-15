using System.Collections.Generic;
using UnityEngine;

namespace Features.EdgePaintingModule.Scripts {
    public class EdgesService : IEdgesService {
        private readonly EdgesModel _edgesModel;
        private readonly float _onLineThreshold;

        public EdgesService(EdgesModel edgesModel, float onLineThreshold = 0.001f) {
            _edgesModel = edgesModel;
            _onLineThreshold = onLineThreshold;
        }

        public EdgeSide GetSide(EdgeData edge, Vector2 position) {
            if (edge == null)
                return EdgeSide.On;

            float signedDistance = edge.GetSignedDistance(position);

            if (Mathf.Abs(signedDistance) <= _onLineThreshold)
                return EdgeSide.On;

            return signedDistance > 0f ? EdgeSide.Left : EdgeSide.Right;
        }

        public List<EdgeSide> GetSideOfEdges(Vector2 position) {
            List<EdgeSide> edgeSides = new();
            foreach (EdgeData edgeData in _edgesModel.Edges) {
                edgeSides.Add(GetSide(edgeData, position));
            }

            return edgeSides;
        }
    }
}
