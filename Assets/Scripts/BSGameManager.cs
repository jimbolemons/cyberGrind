using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;

public class BSGameManager : Singleton<BSGameManager>
{
    public enum GameState
    {
        Playing,
        Overview,
        Paused,
        CountingDown
    }

    public float minVelocityCap = 20f;
    private float minVelocityCapOG;
    public float maxVelocityCap = 100f;

    [HideInInspector] public CameraFollow mainCameraFollow;

    // Things for tracking first player
    public GameObject leadPlayer = null;
    public float leadXPos = 0f;

    [SerializeField] private GameObject[] playerUIs;

    private List<Transform> playerTransforms = new List<Transform>();

    private List<PlayerObj> players = null;
    private List<GameObject> playerGameObjects = new List<GameObject>();

    private List<GameObject> playerSpecificUI = new List<GameObject>();

    public GameObject[] testingPlayers;

    public Sprite NathanSprite;
    public Sprite OwOBotSprite;
    public Sprite MechaHarpySprite;
    public Sprite LilBluSprite;
    // Nathan, OwOBot, MechaHarpy, LilBlu

    public GameObject spawnLocationsObject;

    // Pause UI stuff
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] GameObject defaultButton;
    private Selectable selectedButton;

    // Wwise Things
    [SerializeField] public AK.Wwise.Event stopMenuMusicEvent;
    [SerializeField] public AK.Wwise.Event stopMusicEvent;
    [SerializeField] public AK.Wwise.Event musicEvent;
    [SerializeField] public AK.Wwise.Event countdownEvent;


    // VFX 
    [SerializeField] private GameObject LevelVFX;

    // Countdown Things 
    [SerializeField] private int countdownTime;
    [SerializeField] private Text countdownText;


    private GameState gameState;
    public GameState GameState_
    {
        get => gameState;
        set
        {
            gameState = value;

            switch (gameState)
            {
                case (GameState.CountingDown):
                    // Stop players from moving 
                    minVelocityCap = 0f;
                    break;
                case (GameState.Playing):
                    // Restart player movement 
                    minVelocityCap = minVelocityCapOG;
                    Time.timeScale = 1f;
                    break;
                case (GameState.Paused):
                    Time.timeScale = 0f;
                    break;
            }

            // Update controller scheme based on state
            SwitchControllers();
        }
    }


    #region Unity Callbacks

    private void Start()
    {
        // Toggle VFX
        LevelVFX.SetActive(Convert.ToBoolean(PlayerPrefs.GetInt("PostProcessing")));

        // Save value 
        minVelocityCapOG = minVelocityCap;

        // Set state to counting down 
        GameState_ = GameState.CountingDown;

        mainCameraFollow = Camera.main.GetComponent<CameraFollow>();

        if (!mainCameraFollow)
            Debug.LogError("Unable to find a CameraFollow script on the main camera");

        SpawnCharacters();

        SetupPlayerUI();

        // Get pause menu for later on
        pauseMenu = this.transform.Find("Pause UI").gameObject;

        //Wwise Stuff
        stopMenuMusicEvent.Post(gameObject);
        stopMusicEvent.Post(gameObject);
        musicEvent.Post(gameObject);

        // Start countdown 
        StartCoroutine(CountdownToStart());

        // Starts function for constantly getting player who is in first 
        StartCoroutine(CheckForLead());

        // Update controller scheme based on state
        SwitchControllers();
    }


    private void Update()
    {
        if ((gameState == GameState.Playing) && (minVelocityCap < maxVelocityCap))
            minVelocityCap += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("BroSpeedLevel1");
        }
    }

    #endregion


    #region Private Functions

    /// <summary>
    /// Gets the lead player's x position over and over while the game is in the playing state
    /// </summary>
    private IEnumerator CheckForLead()
    {
        while (instance.GameState_ != GameState.Overview)
        {
            foreach (Transform trans in playerTransforms)
            {
                if (trans.position.x > leadXPos)
                {
                    leadXPos = trans.position.x;
                    leadPlayer = trans.gameObject;
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }


    /// <summary>
    /// Gets the characters that each player chose from PlayerManager
    /// Spawns them into the level in player # order 
    /// Assigns them and their ID's
    /// </summary>
    private void SpawnCharacters()
    {
        //Debug.Log("Spawning characters");

        // Check that we have an instance of the player manager 
        if (PlayerManager.instance == null)
        {
            Debug.Log("No PlayerManager found! (Probably testing?)");
            players = new List<PlayerObj>();

            // Set up four players manually 
            for (int i = 0; i < 4; ++i)
            {
                int playerCount = players.Count;
                PlayerObj newPlayer = new PlayerObj(i, playerCount);

                // Assign the character prefab 
                newPlayer.characterChoice = testingPlayers[i];

                players.Add(newPlayer);
            }
        }
        else
        {
            // Get the players from the PlayerManager
            players = PlayerManager.instance.players;
        }

        if (spawnLocationsObject == null)
        {
            // Old code, need it for testing scenes 
            // Get spawn locations 
            spawnLocationsObject = GameObject.Find("SpawnLocations");
        }


        // Make list of the four specific spots 
        List<Transform> spawnLocations = new List<Transform>();
        foreach (Transform spawnLoc in spawnLocationsObject.transform)
        {
            spawnLocations.Add(spawnLoc);
        }

        int spawnCount = 0;

        // Loop through players and set them up 
        foreach (PlayerObj player in players)
        {
            // Get character choice
            GameObject characterChoice = player.characterChoice;

            // Don't do stuff if choice is null
            if (characterChoice != null)
            {
                // Get spawn location from the list we made earlier
                Vector3 spawnLocation = spawnLocations[spawnCount].position;

                // Get spawn rotation 
                Quaternion spawnRotation = new Quaternion(0, 0, 0, 0);

                // Spawn the character
                GameObject spawnedCharacter = Instantiate(characterChoice, spawnLocation, spawnRotation);

                // Save the character to the list for easy access later 
                playerGameObjects.Add(spawnedCharacter);

                // Get the player controller on this character
                BSPlayerController playerController = spawnedCharacter.GetComponent<BSPlayerController>();

                // Get ID 
                int playerID = player.playerID;

                // Debug.Log("Setting ID for " + playerID + " playing as " + characterChoice.name);

                // Set ID 
                playerController.playerId = playerID;

                if (playerUIs.Length > spawnCount)
                    playerController.playerUI = playerUIs[spawnCount];

                // Increment spawn counter
                ++spawnCount;

                // Add to player list
                playerTransforms.Add(spawnedCharacter.transform);
            }
        }
    }

    /// <summary>
    /// Sets up player UI, like how many icons to have, which characters to show, etc
    /// </summary>
    private void SetupPlayerUI()
    {
        if (playerUIs.Length == 0) { return; }

        // Get number of players 
        int numPlayers = players.Count;

        int maxPlayers = Constants.maxPlayerAmount;

        // Deactivate any UI elements that are not needed
        if (numPlayers < maxPlayers)
        {
            for (int i = numPlayers; i < maxPlayers; ++i)
            {
                // Disable it 
                playerUIs[i].SetActive(false);
            }
        }

        // Save away the UI elements that we need
        for (int i = 0; i < numPlayers; ++i)
        {
            // Get player 
            PlayerObj player = players[i];

            // Get player name
            string playerName = player.characterChoice.name; // Nathan, OwOBot, MechaHarpy, LilBlu

            ///Debug.Log("Setting up UI for " + playerName + " with PlayerId: " + player.playerID);

            // Get the right sprite 
            Sprite UIIcon = null;
            switch (playerName)
            {
                case "Nathan":
                    UIIcon = NathanSprite;
                    break;
                case "OwOBot":
                    UIIcon = OwOBotSprite;
                    break;
                case "MechaHarpy":
                    UIIcon = MechaHarpySprite;
                    break;
                case "LilBlu":
                    UIIcon = LilBluSprite;
                    break;
            }

            GameObject currentUI = playerUIs[i];

            // Set the icon
            Image playerIcon = currentUI.transform.Find("Player Icon").GetComponent<Image>();
            playerIcon.sprite = UIIcon;

            // Set name based on player
            //TMPro.TextMeshProUGUI playerNameText = 
            //   playerUIs[i].transform.Find("Player Name").GetComponent<TMPro.TextMeshProUGUI>();
            //playerNameText.SetText(playerName);

            // Set up colors for the text boxes
            // Get the text box parent
            Transform textBoxParent = currentUI.transform.Find("Text Box Parent");

            // Get the text box bottom/top images 
            Image textBoxBottom = textBoxParent.transform.Find("Text Box Bottom").GetComponent<Image>();
            Image textBoxTop = textBoxParent.transform.Find("Text Box Top").GetComponent<Image>();
            //Debug.Log("key =" + playerName + "Bottom");

            // Set the colors of the two images we just got 
            textBoxBottom.color = Constants.PlayerColors[playerName + "TextBottom"];
            textBoxTop.color = Constants.PlayerColors[playerName + "TextTop"];

            // Set up colors for the battery
            Transform batteryParent = currentUI.transform.Find("Battery Parent");
            Image batteryBackground = batteryParent.transform.Find("Battery Background").GetComponent<Image>();
            Image batteryFill = batteryParent.transform.Find("Battery Fill").GetComponent<Image>();
            ///Debug.Log("key =" + playerName + "Bottom");
            batteryBackground.color = Constants.PlayerColors[playerName + "BatteryBackground"];
            // Sets battery to just one color for now 
            batteryFill.color = Constants.PlayerColors[playerName + "BatteryTop"];

            // Save the battery fill bar to the player 
            playerGameObjects[i].GetComponent<BSPlayerController>().chargeBar = batteryFill;

            // Add it to the list  
            playerSpecificUI.Add(playerUIs[i]);
        }
    }

    private void SwitchControllers()
    {
        // Change players' controls to layout based on GameState_
        for (int i = 0; i < playerGameObjects.Count; ++i)
        {
            BSPlayerController playerController = playerGameObjects[i].GetComponent<BSPlayerController>();
            Rewired.Player rewiredPlayer = playerController.player;

            // Disable all controls    
            rewiredPlayer.controllers.maps.SetAllMapsEnabled(false);

            // Enable correct map 
            switch (GameState_)
            {
                case (GameState.CountingDown):
                    Debug.Log("Switch controllers for CountingDown ");
                    playerController.isInCountdown = true;
                    rewiredPlayer.controllers.maps.SetAllMapsEnabled(false);
                    break;

                case (GameState.Playing):
                    playerController.isInCountdown = false;
                    // Enable GamePlay map
                    rewiredPlayer.controllers.maps.SetMapsEnabled(true, "GamePlay");
                    break;

                case (GameState.Paused):
                    playerController.isInCountdown = false;
                    // Enable Menu map
                    rewiredPlayer.controllers.maps.SetMapsEnabled(true, "Menu");
                    break;
            }
        }
    }


    IEnumerator CountdownToStart()
    {
        // Countdown in seconds from given start time 
        while(countdownTime > 0)
        {
            countdownText.text = countdownTime.ToString();

            // Post countdown sound event for numbers
            countdownEvent.Post(gameObject);

            yield return new WaitForSeconds(0.86956f);

            countdownTime--;
        }

        // Set play state 
        GameState_ = GameState.Playing;

        // Post countdown sound event for go
        countdownEvent.Post(gameObject);
        countdownText.text = "Go!";

        yield return new WaitForSeconds(1f);

        // Disable countdown UI
        countdownText.gameObject.SetActive(false);
    }

    #endregion


    #region Public Functions

    // Pause menu buttons
    public void Resume()
    {
        Pause();
    }

    public void MainMenu()
    {
        // Diable countdown text if needed 
        countdownText.gameObject.SetActive(false);

        // Disable pause menu
        pauseMenu.SetActive(false);

        // Stop coroutines 
        StopAllCoroutines();

        // Fix time scale
        Time.timeScale = 1;

        // Disable pause menu
        pauseMenu.SetActive(false);

        SceneManager.LoadScene("MainMenu");
    }

    public void Exit()
    {
        // Diable countdown text if needed 
        countdownText.gameObject.SetActive(false);

        // Disable pause menu
        pauseMenu.SetActive(false);

        Application.Quit();
    }

    public void MenuMoveUp()
    {
        Selectable nextButton = selectedButton.FindSelectableOnUp();

        if (nextButton != null)
        {
            selectedButton = nextButton;
            selectedButton.Select();
        }
    }

    public void MenuMoveDown()
    {
        Selectable nextButton = selectedButton.FindSelectableOnDown();

        if (nextButton != null)
        {
            selectedButton = nextButton;
            selectedButton.Select();
        }
    }

    public void MenuClickButton()
    {
        selectedButton.GetComponent<Button>().onClick.Invoke();
    }


    /// <summary>
    /// Toggles pause state/menu
    /// </summary>
    public void Pause()
    {

        if (instance.GameState_ == GameState.Paused) // Game is already paused 
        { 
            instance.GameState_ = GameState.Playing;

            // Disable pause menu
            pauseMenu.SetActive(false);
        }
        
        else // Game is not yet paused 
        { 
            instance.GameState_ = GameState.Paused;

            selectedButton = defaultButton.GetComponent<Button>();

            // Enable pause menu
            pauseMenu.SetActive(true);

            // Select resume button by default
            selectedButton.Select();
            selectedButton.OnSelect(null);
        }
    }

    #endregion
}

