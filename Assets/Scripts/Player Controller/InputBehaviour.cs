using System;
using UnityEngine;

namespace Player_Controller
{
    public class InputBehaviour : MonoBehaviour
    {
        public Action OnRoll;
        public Action OnJump;
        
        public Action<int> OnAttack;
        public Action<int> OnChangeAnimState;
        
        public Action<bool> OnBlock;
        public Action<bool> OnGround;

        public float X { get; private set; }
        
        public bool IsRolling { get; private set; }
        public bool Grounded { get; private set; }
        public bool IsWallSliding { get; private set; }
        public bool IsBlocking { get; private set; }

        private SpriteRenderer _spriteRenderer;
        
        private Sensor _groundSensor;
        private Sensor _wallSensorRightDown;
        private Sensor _wallSensorRightUp;
        private Sensor _wallSensorLeftDown;
        private Sensor _wallSensorLeftUp;

        private float _timeSinceAttack;
        private float _rollCurrentTime;
        private float _delayToIdle;

        private int _currentAttack = 1;
        
        private const float RollDuration = 8.0f / 14.0f;

        private void Awake()
        {
            TryGetComponent(out _spriteRenderer);
            
            transform.GetChild(0).TryGetComponent(out _groundSensor);
            transform.GetChild(1).TryGetComponent(out _wallSensorRightDown);
            transform.GetChild(2).TryGetComponent(out _wallSensorRightUp);
            transform.GetChild(3).TryGetComponent(out _wallSensorLeftDown);
            transform.GetChild(4).TryGetComponent(out _wallSensorLeftUp);
        }

        private void Update()
        {
            _timeSinceAttack += Time.deltaTime;
            
            if (IsRolling)
                _rollCurrentTime += Time.deltaTime;
        
            if (_rollCurrentTime > RollDuration)
                IsRolling = false;
            
            if (!Grounded && _groundSensor.State())
            {
                Grounded = true;
                OnGround?.Invoke(Grounded);
            }

            if (Grounded && !_groundSensor.State())
            {
                Grounded = false;
                OnGround?.Invoke(Grounded);
            }
            
            IsWallSliding = (_wallSensorRightDown.State() && _wallSensorRightUp.State()) || 
                            (_wallSensorLeftDown.State() && _wallSensorLeftUp.State());
            
            X = Input.GetAxis("Horizontal");

            _spriteRenderer.flipX = X switch
            {
                > 0 => false,
                < 0 => true,
                _ => _spriteRenderer.flipX
            };
            
            if (Input.GetMouseButtonDown(0) && _timeSinceAttack > 0.25f && !IsRolling)
            {
                _currentAttack++;
            
                if (_currentAttack > 3)
                    _currentAttack = 1;
            
                if (_timeSinceAttack > 1.0f)
                    _currentAttack = 1;
            
                OnAttack?.Invoke(_currentAttack);

                _timeSinceAttack = 0.0f;
            }
            
            else if (Input.GetMouseButtonDown(1) && !IsRolling)
            {
                IsBlocking = true;
                OnBlock?.Invoke(true);
            }

            else if (Input.GetMouseButtonUp(1))
            {
                IsBlocking = false;
                OnBlock?.Invoke(false);
            }
            
            else if (Input.GetKeyDown(KeyCode.LeftShift) && !IsRolling && !IsWallSliding)
            {
                IsRolling = true;
                OnRoll?.Invoke();
            }
        
            else if (Input.GetKeyDown(KeyCode.Space) && Grounded && !IsRolling)
            {
                OnJump?.Invoke();
                Grounded = false;
                OnGround?.Invoke(false);
                _groundSensor.Disable(0.2f);
            }
        
            else if (Mathf.Abs(X) > Mathf.Epsilon)
            {
                _delayToIdle = 0.05f;
                OnChangeAnimState?.Invoke(1);
            }
        
            else
            {
                _delayToIdle -= Time.deltaTime;
                if (_delayToIdle < 0)
                    OnChangeAnimState?.Invoke(0);
            }
        }
    }
}