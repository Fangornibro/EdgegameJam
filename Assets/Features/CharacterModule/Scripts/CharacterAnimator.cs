using UnityEngine;

namespace Features.CharacterModule.Scripts {
    public class CharacterAnimator : MonoBehaviour {
        private static readonly int _horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
        private static readonly int _verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        private static readonly int _isGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int _jumpHash = Animator.StringToHash("Jump");
        private static readonly int _isDead = Animator.StringToHash("IsDead");
        private static readonly int _isWin = Animator.StringToHash("IsWin");

        [SerializeField] private CharacterMovable _characterMovable;
        [SerializeField] private CharacterEntity _characterEntity;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _inputThreshold = 0.1f;
        private static readonly int _transitionToSoul = Animator.StringToHash("TransitionToSoul");
        private static readonly int _transitionToMarimo = Animator.StringToHash("TransitionToMarimo");
        private static readonly int _isSoul = Animator.StringToHash("IsSoul");

        private void OnEnable() {
            _characterMovable.Jumped += OnJumped;
            _characterMovable.OnGravityInvertedChanged += OnGravityInvertedChanged;
            _characterEntity.OnKilled += SetDeathAnimation;
            _characterEntity.OnWin += SetWinAnimation;
        }

        private void OnDisable() {
            _characterMovable.Jumped -= OnJumped;
            _characterMovable.OnGravityInvertedChanged -= OnGravityInvertedChanged;
            _characterEntity.OnKilled -= SetDeathAnimation;
            _characterEntity.OnWin -= SetWinAnimation;
        }

        private void Update() {
            float horizontalInput = _characterMovable.HorizontalInput;
            bool isMoving = Mathf.Abs(horizontalInput) > _inputThreshold;

            _animator.SetFloat(_horizontalSpeedHash, Mathf.Abs(_characterMovable.Velocity.x));
            _animator.SetFloat(_verticalSpeedHash, _characterMovable.Velocity.y);
            _animator.SetBool(_isGroundedHash, _characterMovable.IsGrounded);

            UpdateFlip(horizontalInput, isMoving);
        }

        private void UpdateFlip(float horizontalInput, bool isMoving) {
            if (_spriteRenderer == null)
                return;

            if (_characterMovable.IsGrounded && !isMoving) {
                _spriteRenderer.flipX = false;
                return;
            }

            if (isMoving)
                _spriteRenderer.flipX = horizontalInput < 0f;
        }

        private void OnJumped() {
            _animator.SetTrigger(_jumpHash);
        }

        private void SetDeathAnimation() {
            _animator.SetBool(_isDead, true);
        }

        private void SetWinAnimation() {
            _animator.SetBool(_isWin, true);
        }

        private void OnGravityInvertedChanged(bool isInverted) {
            _animator.SetTrigger(isInverted
                ? _transitionToSoul
                : _transitionToMarimo);

            _animator.SetBool(_isSoul, isInverted);
        }
    }
}
