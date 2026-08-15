using UnityEngine;

namespace Features.CharacterModule.Scripts {
    public class CharacterAnimator : MonoBehaviour {
        private static readonly int HorizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
        private static readonly int VerticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int JumpHash = Animator.StringToHash("Jump");

        [SerializeField] private CharacterMovable _characterMovable;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _inputThreshold = 0.1f;
        
        private void OnEnable() {
            _characterMovable.Jumped += OnJumped;
        }

        private void OnDisable() {
            _characterMovable.Jumped -= OnJumped;
        }

        private void Update() {
            float horizontalInput = _characterMovable.HorizontalInput;
            bool isMoving = Mathf.Abs(horizontalInput) > _inputThreshold;

            _animator.SetFloat(HorizontalSpeedHash, Mathf.Abs(_characterMovable.Velocity.x));
            _animator.SetFloat(VerticalSpeedHash, _characterMovable.Velocity.y);
            _animator.SetBool(IsGroundedHash, _characterMovable.IsGrounded);

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
            _animator.SetTrigger(JumpHash);
        }
    }
}
