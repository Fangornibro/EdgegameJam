using UnityEngine;

namespace Features.EdgePaintingModule.Scripts {
    public class Edge : MonoBehaviour {
        public EdgeData Data { get; private set; }

        public void Initialize(EdgeData data) {
            Data = data;
        }

        public void SetPoints(Vector2 startPoint, Vector2 endPoint) {
            Data?.SetPoints(startPoint, endPoint);
        }
    }
}
