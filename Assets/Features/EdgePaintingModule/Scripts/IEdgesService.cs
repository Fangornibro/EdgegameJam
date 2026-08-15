using System.Collections.Generic;
using UnityEngine;

namespace Features.EdgePaintingModule.Scripts {
    public interface IEdgesService {
        public List<EdgeSide> GetSideOfEdges(Vector2 position);

        // Same rule as the TilemapEdgeSwap shader: the swapped area is the intersection of the
        // right half planes when they overlap, otherwise their union.
        public bool IsInSwappedArea(Vector2 position);
        public bool IsInSwappedArea(Vector2 position, bool includePreview);
        public bool IsIntersectionMode { get; }
        public bool IsPreviewIntersectionMode { get; }
    }
}
