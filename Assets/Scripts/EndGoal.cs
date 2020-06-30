using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndGoal : MonoBehaviour
{
    // UI stuff
    [SerializeField] private Image[] scoreUIs;
    [SerializeField] private Image[] icons;
    [SerializeField] private Image[] placeSlots;

    [SerializeField] private GameObject endGameCanvas, gameUICanvas;
    [SerializeField] private ParticleSystem[] confetti;
    [SerializeField] private Transform[] placementPositions;
    [SerializeField] private SpriteRenderer[] platformSprites;
    [SerializeField] private UnityEngine.Experimental.Rendering.LWRP.Light2D goalLight;
    [SerializeField] private AK.Wwise.Event stopSounds;
    [SerializeField] private AK.Wwise.Event endGameMusic;

    private List<BSPlayerController> finishedPlayers = new List<BSPlayerController>();
    private List<int> placementScores = new List<int>();
    private List<int> originalScores = new List<int>();
    private int winningIndex = 0;
    private SelectButton selectButton;

    #region Unity Callbacks

    private void Awake()
    {
        winningIndex = 0;
    }

    private void Start()
    {
        selectButton = GetComponent<SelectButton>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.playerTag))
        {
            OnPlayerWin(collision.gameObject.GetComponent<BSPlayerController>());
        }
    }

    #endregion

    #region Private Functions

    // Tracks player win order for later scoring 
    private void OnPlayerWin(BSPlayerController player)
    {
        if (finishedPlayers.Contains(player)) { return; }

        finishedPlayers.Add(player);

        if (finishedPlayers.Count == 1)
        {
            foreach (ParticleSystem con in confetti)
            {
                con.Play();
            }
        }

        // Change finishline color
        goalLight.color = Constants.PlayerColors[player.playerName];

        // Move player to platform and start idle anim
        player.enabled = false;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        player.transform.position = placementPositions[finishedPlayers.Count - 1].position;
        player.animHandler.playerState = PlayerAnimatorHandler.PlayerState.Idle;

        // Set platform color
        platformSprites[finishedPlayers.Count - 1].color = Constants.PlayerColors[player.playerName];

        // Save OG score for usage in text later
        originalScores.Add(player.GetUpgradeCount());

        // Give player extra points based on placement 
        int scoreForPlacement = Mathf.Abs(finishedPlayers.Count - 4) * 9;
        player.AddUpgrade(scoreForPlacement);

        // Save placement score for usage in text later
        placementScores.Add(scoreForPlacement);
         

        if (finishedPlayers.Count == PlayerManager.instance.players.Count)
        {
            // Turn off finish line
            goalLight.enabled = false;

            // Stop sounds
            stopSounds.Post(gameObject);

            finishedPlayers.Sort(SortByUpgrades);

            SetUpUI();
            selectButton.SelectTheButton();
        }

        player.onWin.Invoke();

    }

    private void SetUpUI()
    {
        gameUICanvas.SetActive(false);
        endGameCanvas.SetActive(true);

        for (int i = 0; i < finishedPlayers.Count; i++)
        {
            switch (finishedPlayers[i].playerName)
            {
                case ("Nathan"):
                    icons[i].sprite = BSGameManager.instance.NathanSprite;
                    break;
                case ("OwOBot"):
                    icons[i].sprite = BSGameManager.instance.OwOBotSprite;
                    break;
                case ("LilBlu"):
                    icons[i].sprite = BSGameManager.instance.LilBluSprite;
                    break;
                case ("MechaHarpy"):
                    icons[i].sprite = BSGameManager.instance.MechaHarpySprite;
                    break;
            }

            scoreUIs[i].color = Constants.PlayerColors[finishedPlayers[i].playerName + "TextBottom"];
            scoreUIs[i].transform.GetChild(0).GetComponent<Image>().color = Constants.PlayerColors[finishedPlayers[i].playerName + "TextTop"];
            // Make string for score
            string scoreText = "Score:" + finishedPlayers[i].GetUpgradeCount();

            // WIP: SHOW HOW MUCH SCORE FROM PLACEMENT 
            // scoreText += "\n(" + originalScores[i].ToString() + " upgrades + " + placementScores[i].ToString() + ")";

            scoreUIs[i].transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = scoreText;
            placeSlots[i].color = Constants.PlayerColors[finishedPlayers[i].playerName];
            placeSlots[i].gameObject.SetActive(true);
        }

        endGameMusic.Post(gameObject);
    }

    #endregion

    static int SortByUpgrades(BSPlayerController p1, BSPlayerController p2)
    {
        return p2.GetUpgradeCount().CompareTo(p1.GetUpgradeCount());
    }

    public void KillPlatforms()
    {
        foreach (SpriteRenderer sp in platformSprites)
        {
            sp.gameObject.SetActive(false);
        }
    }
}
