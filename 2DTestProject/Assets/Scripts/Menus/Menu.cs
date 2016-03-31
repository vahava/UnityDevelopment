using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// Menu Class that controls all of the interfaces with GUI elements in our project
/// like the options in conversations or the main menu etc
/// </summary>
public class Menu : MonoBehaviour 
{
	public AudioClip sting;
	public AudioSource stingSource;


	public Texture2D background;		// unused at the moment - texture background for menu

	public GameObject optionsBox;		// this is the options box panel that we are displaying in
	public GameObject prefabButton;		// this is the current button we are using for display
	//public Texture2D wip3Logo;


	public List<Options> menuOptions;	// list of all the options in menu
	public string menuType;				// conversation, main menu, etc
	private bool isActive = true; 		// if our menu is active


	private string commands;			// string of commands made by choosing something
	public bool selectionMade = false;	// if we have made a selection of a button
	private int indexSelected = 0;     	// button selected

	public WaitingForTime waitingObject; // object to pause the game for something



	/// <summary>
	/// Start this instance.
	/// 
	/// This just runs a loop that we only need to run once before we destroy the object.
	/// The Menu comes and goes and waits for input and gives options and highlights and
	/// then does commands based on inputit
	/// </summary>
	public IEnumerator Start ()
	{
		

		if (optionsBox != null) 
		{

			yield return StartCoroutine (Initialize ());
		} 




	}


	public IEnumerator Initialize()
	{


		selectionMade = false;
		isActive = true;

		if (indexSelected <= 0 || indexSelected >= 4)
			indexSelected = 0;

		if (optionsBox == null)
		{
			yield return null;
		}
			

		optionsBox.AddComponent<WaitingForTime> ();
		waitingObject = optionsBox.GetComponent<WaitingForTime> ();

		if (menuType != "PauseMenu")
		{
			yield return StartCoroutine (waitingObject.PauseBeforeInput ());
		}


		// check and see which item is highlighted here before we enter and make that
		// our indexselected
		for (var i = 0; i < menuOptions.Count; i++) 
		{
			// if the item is highlighted, set our value to that
			// set i to max
			Button currentButton = optionsBox.transform.GetChild(i).gameObject.GetComponent<Button>();
			currentButton.interactable = true;
		
		}

		while (selectionMade == false && optionsBox.activeInHierarchy) 
		{
			yield return StartCoroutine (waitingObject.WaitForKeyDown ());


			if (EventSystem.current.currentSelectedGameObject == null)
			{
				// if we have no more object
				if (optionsBox.activeInHierarchy == false)
				{
					yield return null;
				}
				else if (indexSelected >= 0 && indexSelected <= menuOptions.Count - 1)
				{
					optionsBox.GetComponentsInChildren<Button> () [indexSelected].Select ();
				}
				else 
				{
					optionsBox.GetComponentsInChildren<Button> () [0].Select ();
				}
			}


			if (!selectionMade && isActive == true && Input.anyKey) 
			{

				if (Input.GetKeyDown (KeyCode.DownArrow) == true) 
				{
					if (indexSelected < menuOptions.Count - 1)
					{
						indexSelected += 1;
					} 
					else 
					{
						indexSelected = 0;
						optionsBox.GetComponentsInChildren<Button> () [0].Select ();
					}


					/// if we have stings and we are in a pause menu
					if (menuType == "PauseMenu" && stingSource != null) 
					{
						stingSource.PlayOneShot (sting);
					}

				}

				if (Input.GetKeyDown (KeyCode.UpArrow) == true) 
				{
					if (indexSelected > 0) 
					{
						indexSelected -= 1;
					} 
					else 
					{
						indexSelected = menuOptions.Count - 1;
						optionsBox.GetComponentsInChildren<Button> () [indexSelected].Select ();
					}


					/// if we have stings and we are in a pause menu
					if (menuType == "PauseMenu" && stingSource != null) 
					{
						stingSource.PlayOneShot (sting);
					}
				}

				if (Input.GetKey (KeyCode.Return)) {
					selectionMade = true;
					ButtonClicked (menuOptions [indexSelected]);
				}

				if (Input.GetKey (KeyCode.X)) {

					selectionMade = true;

					ButtonClicked (menuOptions [indexSelected]);
				}

			}


		}



	}
		

	public bool menuIsActive()
	{
		return isActive;
	}


	/// <summary>
	/// Renames the options without having to delete them all and start over
	/// </summary>
	/// <param name="options">Options.</param>
	public void renameOptions(List<Options> options)
	{
		if (optionsBox != null && menuOptions != null &&  optionsBox.GetComponentsInChildren<Button>().Length > 0)
		{
			
			// get buttons from children
			Button[] panelButtons = optionsBox.GetComponentsInChildren<Button>(true);

			for (int i = 0; i < panelButtons.Length; i++)
			{

				// check if we can update the button
				// get the i number of our option
				panelButtons [i].GetComponentInChildren<Text> ().text = options [i].option;


				// if we have no selected buttons, set our first to selected
				if (i == 0 && (indexSelected <= 0 || indexSelected >= panelButtons.Length))
				{
					panelButtons [i].Select ();
				}
				else if (i == indexSelected)
				{
					panelButtons [i].Select ();
				}

				panelButtons [i].interactable = false; 

				//goButton.AddComponent(
				panelButtons [i].onClick.AddListener(
					() => {  ButtonClicked(options[i]); }
				);

			}

		}
		else 
		{
			loadOptions (options);
		}


	}



	/// <summary>
	/// Toggles the options display so we can hide this when it is the enemy's turn
	/// </summary>
	/// <param name="isEnabled">If set to <c>true</c> is enabled.</param>
	public void toggleOptionsDisplay(bool isEnabled)
	{
		if (optionsBox != null && menuOptions != null &&  optionsBox.GetComponentsInChildren<Button>().Length > 0)
		{

			// get buttons from children
			Button[] panelButtons = optionsBox.GetComponentsInChildren<Button>(true);



			// walk through each of the buttons and either hide or display
			// the whole set based parameter input
			for (int i = 0; i < panelButtons.Length; i++)
			{
				panelButtons [i].enabled = isEnabled;
			

				// if we are activating, set colors and alpha
				if (isEnabled)
				{
					panelButtons [i].GetComponentInChildren<CanvasRenderer> ().SetAlpha (255);
					panelButtons [i].GetComponentInChildren<Text> ().color = Color.red;
				}

				// otherwise, no alpha and clear button text
				else
				{
					panelButtons [i].GetComponentInChildren<CanvasRenderer> ().SetAlpha (0);
					panelButtons [i].GetComponentInChildren<Text> ().color = Color.clear;
				}

			}

		}
	}
		


	/// <summary>
	/// Loads the options for the menu
	/// </summary>
	/// <param name="options">Options.</param>
	public void loadOptions(List<Options> options)
	{
		if (menuOptions == null || menuOptions.Count == 0)
		{
			menuOptions.AddRange (options);
		}
		isActive = true;




		// for each of our options, create some sort of button
		// in our panel and put it at the right spot
		for (int i = 0; i < options.Count; i++)
		{
			GameObject goButton = (GameObject)Instantiate (prefabButton);
			goButton.GetComponentInChildren<Text>().text = menuOptions[i].option;

			RectTransform rect = goButton.GetComponentInChildren<Text> ().GetComponent<RectTransform>();
			rect.offsetMax = new Vector2 (-10f, -10f);
			rect.offsetMin = new Vector2 (10f, 10f);


			Options optionItem = new Options ();
			optionItem = menuOptions [i];

			// if we have no selected buttons, set our first to selected
			if (i == 0 && indexSelected <= 0)
			{
				goButton.GetComponent<Button> ().Select ();
			}
			//goButton.AddComponent(
			goButton.GetComponent<Button>().onClick.AddListener(
				() => {  ButtonClicked(optionItem); }
			);
			goButton.transform.SetParent (optionsBox.transform, false);
			goButton.transform.localScale = new Vector3(1, 1, 1);
			goButton.GetComponent<Button> ().interactable = false; 

		}
			
			
	}



	/// <summary>
	/// When one of our menu buttons is clicked, we go here to deal with the command issued
	/// </summary>
	/// <param name="buttonCommand">Button command.</param>
	void ButtonClicked(Options buttonCommand)
	{
		isActive = false;
		Destroy (waitingObject);

		if (selectionMade)
		{


			/// what type of options list do we have? conversation or more main menu?
			if (menuType == "conversation")
			{
				Commands command = new Commands ();
				command.resolveConversationCommands (buttonCommand);


			}

			// if we have a main menu, we can send those commands over to commands as well
			// and just run our functions for that.
			else if (menuType == "PauseMenu")
			{

				Commands command = new Commands ();
				command.resolvePauseMenuCommands (buttonCommand);
				// what is our command? 
			} 
			else if (menuType == "BattleMenu")
			{
				// let's resolve our battle commands
				Commands command = new Commands();
				buttonCommand.playerToAlter = "Player";
				command.resolveBattleCommands (buttonCommand);
			}


		}

	}








}
