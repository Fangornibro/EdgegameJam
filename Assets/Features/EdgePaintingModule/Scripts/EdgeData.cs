using UnityEngine;

namespace Features.EdgePaintingModule.Scripts {
    public class EdgeData {
        public EdgeData(Vector2 startPoint, Vector2 endPoint) {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Vector2 StartPoint { get; private set; }
        public Vector2 EndPoint { get; private set; }

        public Vector2 Direction => (EndPoint - StartPoint).normalized;
        public Vector2 Normal => new(-Direction.y, Direction.x);
        public float Length => (EndPoint - StartPoint).magnitude;

        public void SetPoints(Vector2 startPoint, Vector2 endPoint) {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public float GetSignedDistance(Vector2 position) {
            Vector2 edge = EndPoint - StartPoint;

            if (edge.sqrMagnitude <= Mathf.Epsilon)
                return 0f;

            Vector2 toPosition = position - StartPoint;

            return (edge.x * toPosition.y - edge.y * toPosition.x) / edge.magnitude;
        }

        public float GetDistanceToSegment(Vector2 position) {
            Vector2 edge = EndPoint - StartPoint;

            if (edge.sqrMagnitude <= Mathf.Epsilon)
                return Vector2.Distance(position, StartPoint);

            float projection = Mathf.Clamp01(Vector2.Dot(position - StartPoint, edge) / edge.sqrMagnitude);

            return Vector2.Distance(position, StartPoint + edge * projection);
        }
    }
}
