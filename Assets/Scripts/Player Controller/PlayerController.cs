using AI;
using UI;
using UnityEngine;

namespace Player_Controller
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;
        
        [Header("Settings")]
        [SerializeField] private float speed = 4.0f;
        [SerializeField] private float jumpForce = 7.5f;
        [SerializeField] private float rollForce = 6.0f;

        public Stats Stats { get; private set; }
        public InputBehaviour Input { get; private set; }

        public float AirSpeed { get; private set; }

        private EnemyBattleController _enemy;
        private AnimationBehaviour _animation;
        
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;

        private const string EnemyTag = "Enemy";

        private void Awake()
        {
            Instance = this;

            Stats = GetComponent<Stats>();
            Input = GetComponent<InputBehaviour>();
            TryGetComponent(out _rigidbody);
            TryGetComponent(out _spriteRenderer);
        }

        private void OnEnable()
        {
            Input.OnAttack += Attack;
            Input.OnRoll += Roll;
            Input.OnJump += Jump;
        }

        private void OnDisable()
        {
            Input.OnAttack -= Attack;
            Input.OnRoll -= Roll;
            Input.OnJump -= Jump;
        }

        private void Update()
        {
            if (!Input.IsRolling)
                _rigidbody.velocity = new Vector2(Input.X * speed, _rigidbody.velocity.y);

            AirSpeed = _rigidbody.velocity.y;
        }

        private void Attack(int index)
        {
            if (_enemy == null)
                return;

            if (_enemy.Stats == null)
                return;
            
            _enemy.Stats.Damage(1);
            UIController.OnAddScore?.Invoke();
        }

        private void Roll()
        {
            int facingDirection = _spriteRenderer.flipX ? -1 : 1;
            _rigidbody.velocity = new Vector2(facingDirection * rollForce, _rigidbody.velocity.y);
        }

        private void Jump()
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpForce);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(EnemyTag))
                return;

            other.TryGetComponent(out _enemy);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(EnemyTag))
                return;
            
            _enemy = null;
        }
    }
}
