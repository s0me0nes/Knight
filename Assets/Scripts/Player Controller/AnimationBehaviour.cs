using System;
using UnityEngine;

namespace Player_Controller
{
    public class AnimationBehaviour : MonoBehaviour
    {
        public Action OnDeath;
        public Action OnHurt;
        
        private Animator _animator;

        private PlayerController _playerController;
        private InputBehaviour _input;
        
        private static readonly int Grounded = Animator.StringToHash("Grounded");
        private static readonly int AirSpeedY = Animator.StringToHash("AirSpeedY");
        private static readonly int WallSlide = Animator.StringToHash("WallSlide");
        private static readonly int Death = Animator.StringToHash("Death");
        private static readonly int Hurt = Animator.StringToHash("Hurt");
        private static readonly int Block = Animator.StringToHash("Block");
        private static readonly int IdleBlock = Animator.StringToHash("IdleBlock");
        private static readonly int Roll = Animator.StringToHash("Roll");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int AnimState = Animator.StringToHash("AnimState");

        private const string Attack = "Attack";

        private void Awake()
        {
            TryGetComponent(out _animator);
            TryGetComponent(out _playerController);
            TryGetComponent(out _input);
        }

        private void OnEnable()
        {
            OnDeath += DeathAnimation;
            OnHurt += HurtAnimation;
            
            _input.OnGround += OnGround;
            _input.OnAttack += OnAttack;
            _input.OnBlock += OnBlock;
            _input.OnRoll += OnRoll;
            _input.OnJump += OnJump;
            _input.OnChangeAnimState += OnChangeAnimState;
        }

        private void OnDisable()
        {
            OnDeath -= DeathAnimation;
            OnHurt -= HurtAnimation;
            
            _input.OnGround -= OnGround;
            _input.OnAttack -= OnAttack;
            _input.OnBlock -= OnBlock;
            _input.OnRoll -= OnRoll;
            _input.OnJump -= OnJump;
            _input.OnChangeAnimState -= OnChangeAnimState;
        }

        private void Update()
        {
            _animator.SetFloat(AirSpeedY, _playerController.AirSpeed);
            _animator.SetBool(WallSlide, _input.IsWallSliding);
        }

        private void OnGround(bool grounded)
        {
            _animator.SetBool(Grounded, grounded);
        }

        private void OnAttack(int index)
        {
            _animator.SetTrigger(Attack + index);
        }

        private void OnBlock(bool state)
        {
            if (state)
            {
                _animator.SetTrigger(Block);
                _animator.SetBool(IdleBlock, true);
            }
            else
                _animator.SetBool(IdleBlock, false);
        }

        private void OnRoll()
        {
            _animator.SetTrigger(Roll);
        }

        private void OnJump()
        {
            _animator.SetTrigger(Jump);
        }

        private void OnChangeAnimState(int state)
        {
            _animator.SetInteger(AnimState, state);
        }

        private void DeathAnimation()
        {
            _animator.SetTrigger(Death);
        }

        private void HurtAnimation()
        {
            _animator.SetTrigger(Hurt);
        }
    }
}