using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private GameObject previousMenu, characterSelectMenu, readyText, optionsMenu, MainMenuVFXObject, loadingScreen, postProcessing;
    [SerializeField] private GameObject[] characterSelectMenuesInOrder, joinTextInOrder, readyTextInOrder;
    [SerializeField] private Carousel[] carousels;
    [SerializeField] private AK.Wwise.Event onSelect, onBack, onMove;
    [SerializeField] private GameObject previousButtonObject;
    [SerializeField] private AK.Wwise.RTPC masterVolume;

    public List<PlayerObj> players = new List<PlayerObj>();
    private GameObject mainCamera;
    private bool inMenu = true;
    
    private void Start()
    {
        mainCamera = Camera.main.gameObject;


        // Player Prefs setup

        // Make default volume if it does not yet exist
        if (!PlayerPrefs.HasKey("volume"))
        {
            PlayerPrefs.SetFloat("volume", 60.0f);
        }
        // Set volume for start
        masterVolume.SetGlobalValue(PlayerPrefs.GetFloat("volume"));

        // Make default value if one does not exist
        if (!PlayerPrefs.HasKey("PostProcessing"))
        { 
            PlayerPrefs.SetInt("PostProcessing", 1);
        }
        // Set VFX based on PlayerPref
        MainMenuVFXObject.SetActive(Convert.ToBoolean(PlayerPrefs.GetInt("PostProcessing")));
    }

    private void Update()
    {
        if (!inMenu)
            return;


        // Don't check for start when other menus are still active 
        if (! (optionsMenu.activeInHierarchy || previousMenu.activeInHierarchy))
        {
            CheckForGameStart();
        }
        

        foreach (PlayerObj player in players)
        {
            int oldCount = players.Count;
            player.ControlsCheck();
            if (players.Count == 0 || oldCount != players.Count)
                break;
        }


        // Watch for Select/Back action in each Player
        for (int i = 0; i < ReInput.players.playerCount; i++)
        {
            if (ReInput.players.GetPlayer(i).GetButtonDown("Select"))
            {
                onSelect.Post(gameObject);
            }

                if (!(optionsMenu.activeInHierarchy || previousMenu.activeInHierarchy))
            { 
                if (ReInput.players.GetPlayer(i).GetButtonDown("Select"))
                {
                    

                    if (!doesIDExist(i))
                        AssignPlayer(i);

                } else if (ReInput.players.GetPlayer(i).GetButtonDown("Back"))
                {
                    onBack.Post(gameObject);

                    BackCheck(i);
                }
            }

            bool horiz = ReInput.players.GetPlayer(i).GetButtonDown("Horizontal") || ReInput.players.GetPlayer(i).GetNegativeButtonDown("Horizontal");
            bool vert = ReInput.players.GetPlayer(i).GetButtonDown("Vertical") || ReInput.players.GetPlayer(i).GetNegativeButtonDown("Vertical");

            if (vert)
            {
                onMove.Post(gameObject);
            }
        }

        // FOR TESTING
        if (Input.GetKeyDown(KeyCode.N))
        {
            AssignPlayer(5 + UnityEngine.Random.Range(0, 100));
        }
    }

    public void RemoveCharacter(int characterIndex)
    {
        foreach(Carousel carousel in carousels)
        {
            carousel.RemoveAtIndex(characterIndex);
        }
    }

    public void AddCharacter(int characterIndex)
    {
        foreach(Carousel carousel in carousels)
        {
            carousel.RestoreAtIndex(characterIndex);
        }
    }

    private void AssignPlayer(int playerID)
    {
        // Find open playerNum
        int playerNum = players.Count;

        for (int i = 0; i < players.Count; i++)
        {
            bool doBreak = false;
            foreach(PlayerObj player in players)
            {
                if (player.playerNumber == i)
                {
                    doBreak = true;
                    break;
                } 
            }
            if (!doBreak)
            {
                playerNum = i;
                break;
            }
        }

        // Create player
        PlayerObj newPlayer = new PlayerObj(playerID, playerNum, joinTextInOrder[playerNum], 
            characterSelectMenuesInOrder[playerNum], readyTextInOrder[playerNum]);
        players.Add(newPlayer);
    }


    /// <summary>
    /// See if the player exists, if so back out
    /// </summary>
    private void BackCheck(int playerID)
    {
        //bool existCheck = doesIDExist(playerID);

        if (players.Count == 0)
        {
            // Do different things if we were in the options menu

            if (optionsMenu.activeInHierarchy)
            {
                //previousMenu.SetActive(true);
                // optionsMenu.SetActive(false);
                // Select default button again 
                // Select resume button by default
                Selectable defaultButton = previousButtonObject.GetComponent<Button>();
                defaultButton.Select();
                defaultButton.OnSelect(null);
            }
            else
            {
                //previousMenu.SetActive(true);
                //characterSelectMenu.SetActive(false);
                previousMenu.GetComponentInParent<Animator>().SetTrigger("In");
                characterSelectMenu.GetComponent<Animator>().SetTrigger("Out");
                mainCamera.SetActive(true);
                players.Clear();
            }

            
        }
    }


    public void ExitPlayer(PlayerObj player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
        }
    }


    private bool doesIDExist(int id)
    {
        foreach (PlayerObj player in players)
        {
            if (player.playerID == id)
            {
                return true;
            }

        }

        return false;
    }


    private IEnumerator StartGame()
    {
        inMenu = false;
        previousMenu.SetActive(false);
        characterSelectMenu.SetActive(false);
        postProcessing.SetActive(false);
        loadingScreen.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Constants.levelName);
        asyncLoad.allowSceneActivation = false;
        float timeWaited = 0;
        while (!asyncLoad.isDone && timeWaited < 4f)
        {
            yield return new WaitForEndOfFrame();
            timeWaited += Time.deltaTime;
        }

        asyncLoad.allowSceneActivation = true;

    }

    private void CheckForGameStart()
    {
        // Watch for Select/Back action in each Player
        bool canStart = true;

        bool triedToStart = false;
        foreach (PlayerObj player in players)
        {
            if (player.state != PlayerObj.SelectionState.Ready)
            {
                canStart = false;
            }

            // Check if it's a test player
            if (player.playerID > 4)
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    triedToStart = true;
                }
            } else
            {
                if (ReInput.players.GetPlayer(player.playerID).GetButtonDown("Select"))
                {
                    triedToStart = true;
                }
            }
            
        }
        if (canStart && players.Count > 0)
            readyText.SetActive(true);
        else if (readyText.activeSelf)
            readyText.SetActive(false);

        if (canStart && triedToStart && players.Count > 0)
            StartCoroutine(StartGame());
    }
}
