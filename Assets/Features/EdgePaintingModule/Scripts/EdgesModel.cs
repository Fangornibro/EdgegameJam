using System;
using System.Collections.Generic;

namespace Features.EdgePaintingModule.Scripts {
    public class EdgesModel {
        private readonly List<EdgeData> _edges = new();

        public event Action<EdgeData> EdgeAdded;
        public event Action Cleared;

        public IReadOnlyList<EdgeData> Edges => _edges;
        public int Count => _edges.Count;

        // The edge currently being dragged. It is not committed yet, but zones already react to it.
        public EdgeData PreviewEdge { get; set; }

        public void Add(EdgeData edge) {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            _edges.Add(edge);
            EdgeAdded?.Invoke(edge);
        }

        public void Clear() {
            _edges.Clear();
            Cleared?.Invoke();
        }
    }
}
