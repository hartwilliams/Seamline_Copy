using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats _instance;

    public int maxHealth = 100;
    public int yarncooldown = 0;
    public int currentHealth;
    [SerializeField] private int maxYarn = 100; //start at 100%
    public float currentYarnCount;
    public HealthBar yarnTracker; //use Healthbar as yarn tracker 
    public HealthBar healthBar;
    public int potions = 0;
    public int healthFromPotion = 20;
    public int maxPotion = 8;
    public TextMeshProUGUI potionUI;
    public bool inFlippedWorld;  //keep track of what world player is in

    public bool blocking = false;// is block ability activated currently
    [SerializeField] private SpriteRenderer spRender;
    [SerializeField] private AudioSource hitSFX;

    [Min(0f)]
    [SerializeField] private float iFrameTime;

    private DefaultInputAction playerInputAction;

    private float iFrameTimer = 0f;
    private bool invincible = false;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        if (SaveSystem.isVersionSaved)
        {
            SaveSystem.LoadSave();
            healthBar.SetHealth(currentHealth);
            yarnTracker.SetHealth(Mathf.RoundToInt(currentYarnCount));
            Debug.Log("Saved version loaded");
        }
        else
        {
            Debug.Log("Saved version not found");
            currentHealth = maxHealth;
            currentYarnCount = maxYarn;
            //set player health to maxHealth to start
            healthBar.SetMaxHealth(maxHealth);
            yarnTracker.SetMaxHealth(maxYarn);
        }

        playerInputAction = new DefaultInputAction();
        playerInputAction.Player.UsePotion.started += UsePotion;
        inFlippedWorld = false;
    }

    private void OnEnable()
    {
        playerInputAction.Player.UsePotion.Enable();
    }

    private void OnDisable()
    {
        playerInputAction.Player.UsePotion.Disable();
    }

    public void OnPause(bool paused)
    {
        if (paused)
        {
            playerInputAction.Player.UsePotion.Disable();
        }
        else
        {
            playerInputAction.Player.UsePotion.Enable();
        }
    }

    void Update()
    {
        //to test if health bar works- can press space bar to trigger
        // if(Input.GetKeyDown(KeyCode.Space)) {
        //     TakeDamage(10); 
        // }

        /* TO ADD:
            enemy attack -> causes player health to decrease
        */

        //once player health gets below 0, go back to home screen (or load screen with saved checkpoints)
        if (currentHealth <= 0)
        {
            SceneManager.LoadSceneAsync("Game Over Screen");
        }
        // only count up yarncooldown by 1 in normal world -- Jing
        if (GameObject.FindWithTag("Player").transform.position.y > 0)
        {
            yarncooldown = yarncooldown + 1;
        }
        if (yarncooldown > 36 * 1)
        {
            yarncooldown = 0;
            this.GainYarn(20);
        }
    }

    private void FixedUpdate()
    {
        if (invincible)
        {
            iFrameTimer += Time.fixedDeltaTime;
            if (iFrameTimer > iFrameTime)
            {
                iFrameTimer = 0f;
                invincible = false;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        // check if player is blocking, only take damage if not blocking
        if (!invincible && !blocking)
        {
            currentHealth -= damage;

            invincible = true;

            healthBar.SetHealth(currentHealth);
            if (hitSFX)
            {
                hitSFX.Play();
            }
            if (spRender)
            {
                StartCoroutine(FlashColor(new Color(1f,0.5f,0.5f)));
            }
        }
    }
    //decrease yarn count
    public void UseYarn(float amount)
    {
        if (currentYarnCount < 0)
        {
            Debug.Log("player out of yarn");
        }
        else
        {
            currentYarnCount -= amount;

            // Added this to round the actual yarn enable to display using yarnTracker which only accept int -- Jing
            int yarnToDisplay = Mathf.RoundToInt(currentYarnCount);
            yarnTracker.SetHealth(yarnToDisplay);
        }
    }

    //increase yarn count
    public void GainYarn(float amount)
    {
        if (currentYarnCount + amount > maxYarn)
        {
            amount = maxYarn - currentYarnCount;
        }
        currentYarnCount += amount;

        // Added this to round the actual yarn enable to display using yarnTracker which only accept int -- Jing
        int yarnToDisplay = Mathf.RoundToInt(currentYarnCount);
        yarnTracker.SetHealth(yarnToDisplay);
    }

    public void UsePotion(InputAction.CallbackContext ctx)
    {
        if (potions > 0)
        {
            potions -= 1;
            currentHealth += healthFromPotion;
            potionUI.text = potions.ToString();
            healthBar.SetHealth(currentHealth);
        }
        else
        {
            Debug.Log("No Potion");
        }
    }

    public void GainPotion()
    {
        if (potions < maxPotion)
        {
            potions += 1;
            potionUI.text = potions.ToString();
        }
    }

    private IEnumerator FlashColor(Color color)
    {
        bool originalColor = false;

        while (invincible)
        {
            spRender.color = originalColor ? Color.white : color;
            originalColor = !originalColor;
            yield return new WaitForSeconds(0.1f);
        }

        spRender.color = Color.white;
    }

}
