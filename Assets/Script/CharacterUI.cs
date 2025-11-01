using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace StarterAssets
{ 
    public class CharacterUI : MonoBehaviour
    {
        private CombatHandler _combat;
        public Slider _healthSlider;
        public Slider _actionSlider;
        public TextMeshProUGUI _healthText;
        public GameObject _pauseMenu;  
 
        public void Start()
        {
            _combat = GetComponent<CombatHandler>();
        }
        public void Update()
        {
            SetMaxHealth(_combat._maxHealth);
            SetHealth(_combat._health);
            SetMaxAction(_combat._maxAction);
            SetAction(_combat._action);
            if (_healthText != null)
            {
                _healthText.text = _combat._health + "/" + _combat._maxHealth;
            }
            MainMenu();
            
        }
        public void SetMaxHealth(int health)
        {
            _healthSlider.maxValue = health;
        }

        public void SetHealth(int health)
        {
            _healthSlider.value = health;
        }
        public void SetMaxAction(int action)
        {
            _actionSlider.maxValue = action;
        }

        public void SetAction(int action)
        {
            _actionSlider.value = _combat._action;
        }
       public void MainMenu()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LoadingScene.Instance.LoadLevelBtn("MainMenu");
            }
        }
    }
}