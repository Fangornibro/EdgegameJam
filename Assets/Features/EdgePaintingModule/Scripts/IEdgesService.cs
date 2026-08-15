using System.Collections.Generic;
using UnityEngine;

namespace Features.EdgePaintingModule.Scripts {
    public interface IEdgesService {
        public List<EdgeSide> GetSideOfEdges(Vector2 position);
    }
}
