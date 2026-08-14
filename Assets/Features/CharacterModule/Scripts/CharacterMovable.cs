using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.CharacterModule.Scripts {
    public class CharacterMovable : MonoBehaviour {
        [Header("Input")]
        [SerializeField] private InputActionReference _moveAction;
        [SerializeField] private InputActionReference _jumpAction;

        [Header("Move")]
        [SerializeField] private float _moveSpeed = 8f;
        [SerializeField] private float _acceleration = 90f;
        [SerializeField] private float _deceleration = 120f;
        [SerializeField] private bool _flipSpriteByDirection = true;

        [Header("Jump")]
        [SerializeField] private float _jumpHeight = 3f;
        [SerializeField] private float _coyoteTime = 0.1f;
        [SerializeField] private float _jumpBufferTime = 0.1f;
        [SerializeField, Range(0f, 1f)] private float _jumpCutMultiplier = 0.5f;
        [SerializeField] private float _fallGravityMultiplier = 1.8f;
        [SerializeField] private float _maxFallSpeed = 20f;

        [Header("Ground Check")]
        [SerializeField] private Transform _groundCheckOrigin;
        [SerializeField] private Vector2 _groundCheckSize = new(0.45f, 0.1f);
        [SerializeField] private LayerMask _groundLayers;

        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private float _defaultGravityScale;
        private float _horizontalInput;
        private float _coyoteTimeLeft;
        private float _jumpBufferLeft;

        public bool IsGrounded { get; private set; }
        public Vector2 Velocity => _rigidbody.linearVelocity;

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _defaultGravityScale = _rigidbody.gravityScale;
            _rigidbody.freezeRotation = true;

            if (_groundCheckOrigin == null)
                _groundCheckOrigin = transform;
        }

        private void OnEnable() {
            _moveAction.action.Enable();
            _jumpAction.action.Enable();
            _jumpAction.action.performed += OnJumpPerformed;
            _jumpAction.action.canceled += OnJumpCanceled;
        }

        private void OnDisable() {
            _jumpAction.action.performed -= OnJumpPerformed;
            _jumpAction.action.canceled -= OnJumpCanceled;
            _moveAction.action.Disable();
            _jumpAction.action.Disable();

            _horizontalInput = 0f;
            _jumpBufferLeft = 0f;
        }

        private void Update() {
            _horizontalInput = _moveAction.action.ReadValue<Vector2>().x;

            if (_jumpBufferLeft > 0f)
                _jumpBufferLeft -= Time.deltaTime;

            if (_flipSpriteByDirection && _spriteRenderer != null && !Mathf.Approximately(_horizontalInput, 0f))
                _spriteRenderer.flipX = _horizontalInput < 0f;
        }

        private void FixedUpdate() {
            UpdateGroundState();
            ApplyHorizontalMovement();
            TryConsumeBufferedJump();
            ApplyBetterGravity();
        }

        private void UpdateGroundState() {
            IsGrounded = Physics2D.OverlapBox(_groundCheckOrigin.position, _groundCheckSize, 0f, _groundLayers) != null;
            _coyoteTimeLeft = IsGrounded ? _coyoteTime : _coyoteTimeLeft - Time.fixedDeltaTime;
        }

        private void ApplyHorizontalMovement() {
            float targetSpeed = _horizontalInput * _moveSpeed;
            float rate = Mathf.Approximately(targetSpeed, 0f) ? _deceleration : _acceleration;
            float newSpeed = Mathf.MoveTowards(_rigidbody.linearVelocity.x, targetSpeed, rate * Time.fixedDeltaTime);

            _rigidbody.linearVelocity = new Vector2(newSpeed, _rigidbody.linearVelocity.y);
        }

        private void TryConsumeBufferedJump() {
            if (_jumpBufferLeft <= 0f || _coyoteTimeLeft <= 0f)
                return;

            _jumpBufferLeft = 0f;
            _coyoteTimeLeft = 0f;

            float jumpSpeed = Mathf.Sqrt(2f * Mathf.Abs(Physics2D.gravity.y) * _defaultGravityScale * _jumpHeight);
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, jumpSpeed);
        }

        private void ApplyBetterGravity() {
            bool isFalling = _rigidbody.linearVelocity.y < 0f;
            _rigidbody.gravityScale = isFalling ? _defaultGravityScale * _fallGravityMultiplier : _defaultGravityScale;

            if (_rigidbody.linearVelocity.y < -_maxFallSpeed)
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, -_maxFallSpeed);
        }

        private void OnJumpPerformed(InputAction.CallbackContext context) {
            _jumpBufferLeft = _jumpBufferTime;
        }

        private void OnJumpCanceled(InputAction.CallbackContext context) {
            if (_rigidbody.linearVelocity.y > 0f)
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _rigidbody.linearVelocity.y * _jumpCutMultiplier);
        }

        private void OnDrawGizmosSelected() {
            Transform origin = _groundCheckOrigin == null ? transform : _groundCheckOrigin;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(origin.position, _groundCheckSize);
        }
    }
}
