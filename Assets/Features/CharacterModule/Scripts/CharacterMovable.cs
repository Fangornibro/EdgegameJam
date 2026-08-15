using System;
using System.Collections.Generic;
using Features.EdgePaintingModule.Scripts;
using Features.GameContextsModule.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.CharacterModule.Scripts {
    public class CharacterMovable : MonoBehaviour {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private CharacterEntity _characterEntity;
        
        [Header("Input")]
        [SerializeField] private InputActionReference _moveAction;
        [SerializeField] private InputActionReference _jumpAction;

        [Header("Move")]
        [SerializeField] private float _moveSpeed = 8f;
        [SerializeField] private float _acceleration = 90f;
        [SerializeField] private float _deceleration = 120f;

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

        private float _defaultGravityScale;
        private float _horizontalInput;
        private float _coyoteTimeLeft;
        private float _jumpBufferLeft;
        private IEdgesService _edgesService;
        private bool _isGravityInverted;
        private float _gravitySign = 1f;

        public event Action Jumped;
        public event Action<bool> OnGravityInvertedChanged;

        public bool IsGrounded { get; private set; }
        public float HorizontalInput => _horizontalInput;
        public Vector2 Velocity => _rigidbody.linearVelocity;

        private void Awake() {
            _defaultGravityScale = _rigidbody.gravityScale;
            _rigidbody.freezeRotation = true;

            if (_groundCheckOrigin == null)
                _groundCheckOrigin = transform;
        }

        private void Start() {
            _edgesService = ServiceLocator.Get<IEdgesService>();
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
            if (_characterEntity.IsDead || _characterEntity.IsWin)
                return;
            
            _horizontalInput = _moveAction.action.ReadValue<Vector2>().x;

            if (_jumpBufferLeft > 0f)
                _jumpBufferLeft -= Time.deltaTime;

            List<EdgeSide> sideOfLastEdge = _edgesService.GetSideOfEdges(transform.position);
            bool isGravityInverted = sideOfLastEdge.Count > 0 && !sideOfLastEdge.Contains(EdgeSide.Left);

            if (_isGravityInverted != isGravityInverted) {
                _isGravityInverted = isGravityInverted;
                OnGravityInvertedChanged?.Invoke(_isGravityInverted);
            }
            
            _gravitySign = isGravityInverted ? -1f : 1f;
        }
        
        private void FixedUpdate() {
            if (_characterEntity.IsDead || _characterEntity.IsWin) {
                _rigidbody.bodyType = RigidbodyType2D.Static;
                return;
            }
            
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
            if (_jumpBufferLeft <= 0f || _coyoteTimeLeft <= 0f || _isGravityInverted)
                return;

            _jumpBufferLeft = 0f;
            _coyoteTimeLeft = 0f;

            float jumpSpeed = Mathf.Sqrt(2f * Mathf.Abs(Physics2D.gravity.y) * _defaultGravityScale * _jumpHeight);
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, jumpSpeed * _gravitySign);
            IsGrounded = false;
            Jumped?.Invoke();
        }

        private void ApplyBetterGravity() {
            float verticalSpeed = _rigidbody.linearVelocity.y;
            bool isFalling = verticalSpeed * _gravitySign < 0f;
            float gravityScale = isFalling ? _defaultGravityScale * _fallGravityMultiplier : _defaultGravityScale;

            _rigidbody.gravityScale = gravityScale * _gravitySign;

            if (verticalSpeed * _gravitySign < -_maxFallSpeed)
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, -_maxFallSpeed * _gravitySign);
        }

        private void OnJumpPerformed(InputAction.CallbackContext context) {
            if(_characterEntity.IsDead || _characterEntity.IsWin)
                return;
            
            _jumpBufferLeft = _jumpBufferTime;
        }

        private void OnJumpCanceled(InputAction.CallbackContext context) {
            if(_characterEntity.IsDead || _characterEntity.IsWin)
                return;
            
            if (_rigidbody.linearVelocity.y * _gravitySign > 0f)
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _rigidbody.linearVelocity.y * _jumpCutMultiplier);
        }

        private void OnDrawGizmosSelected() {
            Transform origin = _groundCheckOrigin == null ? transform : _groundCheckOrigin;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(origin.position, _groundCheckSize);
        }
    }
}
