using Features.GameContextsModule.Scripts;
using UnityEngine;

namespace Features.EdgePaintingModule.Scripts {
    public class EdgeShaderUniforms : MonoBehaviour {
        private const int MaxEdges = 16;

        private static readonly int EdgesId = Shader.PropertyToID("_Edges");
        private static readonly int EdgeCountId = Shader.PropertyToID("_EdgeCount");
        private static readonly int EdgeCombineModeId = Shader.PropertyToID("_EdgeCombineMode");

        [SerializeField] private EdgePaintingInput _edgePaintingInput;

        private readonly Vector4[] _edgeBuffer = new Vector4[MaxEdges];

        private EdgesModel _edgesModel;
        private IEdgesService _edgesService;

        private void Start() {
            _edgesModel = ServiceLocator.Get<EdgesModel>();
            _edgesService = ServiceLocator.Get<IEdgesService>();
        }

        private void LateUpdate() {
            int edgeCount = FillEdgeBuffer();

            Shader.SetGlobalVectorArray(EdgesId, _edgeBuffer);
            Shader.SetGlobalInt(EdgeCountId, edgeCount);
            Shader.SetGlobalInt(EdgeCombineModeId, _edgesService.IsPreviewIntersectionMode ? 1 : 0);
        }

        private void OnDisable() {
            Shader.SetGlobalInt(EdgeCountId, 0);
        }

        private int FillEdgeBuffer() {
            int edgeCount = 0;

            foreach (EdgeData edge in _edgesModel.Edges) {
                if (edgeCount >= MaxEdges)
                    break;

                _edgeBuffer[edgeCount++] = ToVector(edge);
            }

            EdgeData draggedEdge = _edgePaintingInput == null ? null : _edgePaintingInput.CurrentEdgeData;

            if (draggedEdge != null && edgeCount < MaxEdges)
                _edgeBuffer[edgeCount++] = ToVector(draggedEdge);

            for (int i = edgeCount; i < MaxEdges; i++)
                _edgeBuffer[i] = Vector4.zero;

            return edgeCount;
        }

        private Vector4 ToVector(EdgeData edge) {
            Vector2 direction = edge.EndPoint - edge.StartPoint;

            return new Vector4(edge.StartPoint.x, edge.StartPoint.y, direction.x, direction.y);
        }
    }
}
