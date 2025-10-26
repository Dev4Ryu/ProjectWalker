using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace StarterAssets
{ 
    public class CharacterUI : MonoBehaviour
    {
        private CombatHandler _combat;
        public Slider _slider;
        public TextMeshProUGUI _healthText;
        public GameObject _pauseMenu;  
 
        public void Start()
        {
            _combat = GetComponent<CombatHandler>();
        }
        public void Update()
        {
            // SetMaxHealth(_combat._maxHealth);
            // SetHealth(_combat._health);
            // if (_healthText != null)
            // {
            //     _healthText.text = _combat._health + "/" + _combat._maxHealth;
            // }
            MainMenu();
            
        }
        public void SetMaxHealth(int health)
        {
            _slider.maxValue = health;
        }

        public void SetHealth(int health)
        {
            _slider.value = health;
        }
       public void MainMenu()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _pauseMenu.SetActive(!_pauseMenu.activeSelf);
                Time.timeScale = _pauseMenu.activeSelf ? 2f : 1f;
            }
        }
    }
}