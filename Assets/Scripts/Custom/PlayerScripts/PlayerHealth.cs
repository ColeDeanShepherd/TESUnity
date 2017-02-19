using UnityEngine;
using UnityEngine.UI;



public class PlayerHealth : MonoBehaviour {

    TESUnity.TESUnity tesUnity = TESUnity.TESUnity.instance;

    private int health { get; set; }
    private int maxHealth { get; set; }

    private Slider healthSlider;


    private void Start()
    {
        
        healthSlider = tesUnity.UIManager.HUD.FindChild("HealthSlider").GetComponent<Slider>();

        SetGetMaxHealth = 50;
        SetGetHealth = SetGetMaxHealth;

        healthSlider.maxValue = SetGetMaxHealth;
        healthSlider.value = SetGetHealth;

    }
    public int SetGetHealth
    {
        get
        {
            return this.health;
        }

        set
        {
            this.health = value;
            healthSlider.value = SetGetHealth;
            
            if(SetGetHealth < 1)
            {
                Death();
            }
        }
    }


    public int SetGetMaxHealth
    {
        get
        {
            return this.health;
        }

        set
        {
            this.health = value;
        }
    }


    private void Death()
    {

    }

}
