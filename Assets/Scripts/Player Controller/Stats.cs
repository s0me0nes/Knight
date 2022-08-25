using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Player_Controller
{
    public class Stats : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image healthBar;
        [Header("Settings")]
        [SerializeField] private float hp = 20;

        private PlayerController _playerController;
        private InputBehaviour _input;
        private AnimationBehaviour _animations;
        
        private float _smoothHp;

        private void Awake()
        {
            TryGetComponent(out _playerController);
            TryGetComponent(out _input);
            TryGetComponent(out _animations);
        }
        
        private void Update()
        {
            _smoothHp = Mathf.Lerp(_smoothHp, hp, Time.deltaTime / 0.1f);
            healthBar.fillAmount = Mathf.InverseLerp(0, 20, _smoothHp);
        }
        
        public void Damage(float damage)
        {
            hp -= damage;
            _animations.OnHurt?.Invoke();

            if (hp > 0)
                return;
            
            UIController.OnDeath?.Invoke();
            _animations.OnDeath?.Invoke();
            Destroy(_playerController);
            Destroy(_input);
            Destroy(_animations);
            Destroy(this);
        }
    }
}