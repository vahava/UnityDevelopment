using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;                            // The amount of health the player starts the game with.
    public int currentHealth;                                   // The current health the player has.
    public Slider healthSlider;                                 // Reference to the UI's health bar.
    public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
    public AudioClip deathClip;                                 // The audio clip to play when the player dies.
    public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.

	// let's also update the text field just for fun
	public Text healthField;

    //Animator anim;                                              // Reference to the Animator component.
    //AudioSource playerAudio;                                    // Reference to the AudioSource component.
    //PlayerMovement playerMovement;                              // Reference to the player's movement.
    bool isDead;                                                // Whether the player is dead.
    bool damaged;                                               // True when the player gets damaged.


    void Awake ()
    {
        // Setting up the references.
        //anim = GetComponent <Animator> ();
        //playerAudio = GetComponent <AudioSource> ();
        //playerMovement = GetComponent <PlayerMovement> ();


		// check for current scene? 
		if (SceneManager.GetActiveScene().name == "BattleScene")
		{
			healthField.text = "<color='yellow'>" + currentHealth + "</color><color='white'> / " + maxHealth + "</color>";
		}
    }


    void Update ()
    {
        // If the player has just been damaged...
        if(damaged)
        {
            // ... set the colour of the damageImage to the flash colour.
            //damageImage.color = flashColour;
        }
        // Otherwise...
        else
        {
            // ... transition the colour back to clear.
           // damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        // Reset the damaged flag.
        damaged = false;
    }


	public void HealCharacter(int amount)
	{
		currentHealth += amount;

		if (currentHealth >= maxHealth)
			currentHealth = maxHealth;

		// Set the health bar's value to the current health.
		healthSlider.value = currentHealth;
		healthField.text = "<color='yellow'>" + currentHealth + "</color><color='white'> / " + maxHealth + "</color>";


	}

    public void TakeDamage (int amount)
    {
        // Set the damaged flag so the screen will flash.
        damaged = true;

        // Reduce the current health by the damage amount.
        currentHealth -= amount;

        // Set the health bar's value to the current health.
        healthSlider.value = currentHealth;
		healthField.text = "<color='yellow'>" + currentHealth + "</color><color='white'> / " + maxHealth + "</color>";

        // Play the hurt sound effect.
        //playerAudio.Play ();

        // If the player has lost all it's health and the death flag hasn't been set yet...
        if(currentHealth <= 0 && !isDead)
        {
            // ... it should die.
            Death ();
        }
    }


    void Death ()
    {
        // Set the death flag so this function won't be called again.
        isDead = true;


		BattleManager batMan = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BattleManager>();
		batMan.currentState = BattleManager.BATTLE_STATES.LOSE;

        // Tell the animator that the player is dead.
        //anim.SetTrigger ("Die");

        // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
        //playerAudio.clip = deathClip;
        //playerAudio.Play ();

        // Turn off the movement and shooting scripts.
        //playerMovement.enabled = false;


		//Toolbox toolboxInstance = Toolbox.Instance;
		// get the battle panel and destroy
		Destroy(GameObject.Find("BattlePanel").GetComponent<BattleMenu>());

		// spawn at least location?
		// what if the grue is still there?
		// we'll get this in a moment.
		SceneManager.LoadScene("OpeningScene");
    }       
}
