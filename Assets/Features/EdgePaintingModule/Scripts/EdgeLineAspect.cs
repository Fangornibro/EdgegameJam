using UnityEngine;

namespace Features.EdgePaintingModule.Scripts {
    [RequireComponent(typeof(LineRenderer))]
    public class EdgeLineAspect : MonoBehaviour {
        private static readonly int LineAspectId = Shader.PropertyToID("_LineAspect");

        [SerializeField] private LineRenderer _lineRenderer;

        private MaterialPropertyBlock _propertyBlock;

        private void Awake() {
            if (_lineRenderer == null)
                _lineRenderer = GetComponent<LineRenderer>();

            _propertyBlock = new MaterialPropertyBlock();
        }

        private void LateUpdate() {
            float width = Mathf.Max(_lineRenderer.widthMultiplier, 0.0001f);
            float length = GetLength();

            _lineRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(LineAspectId, length / width);
            _lineRenderer.SetPropertyBlock(_propertyBlock);
        }

        private float GetLength() {
            float length = 0f;

            for (int i = 1; i < _lineRenderer.positionCount; i++)
                length += Vector3.Distance(_lineRenderer.GetPosition(i - 1), _lineRenderer.GetPosition(i));

            return length;
        }
    }
}
