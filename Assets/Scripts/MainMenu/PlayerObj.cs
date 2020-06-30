using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObj
{
    public enum SelectionState
    {
        Joining = 0,
        CharacterSelect = 1,
        Ready = 2,
        Testing = 3
    }

    public int playerID, playerNumber;
    public GameObject characterChoice = null;

    private GameObject joinText, characterSelectMenu, readyText;
    private Button leftArrow, rightArrow;
    private Carousel carousel;

    public SelectionState state {
        set {
            _state = value;
            switch(value)
            {
                case SelectionState.Joining:
                    joinText.SetActive(true);
                    characterSelectMenu.SetActive(false);
                    readyText.SetActive(false);
                    PlayerManager.instance.ExitPlayer(this);
                    break;
                case SelectionState.CharacterSelect:
                    carousel.isLocked = false;
                    if (characterChoice)
                    {
                        PlayerManager.instance.AddCharacter(carousel.GetActiveCharacterIndex());
                        characterChoice = null;
                    }
                    joinText.SetActive(false);
                    characterSelectMenu.SetActive(true);
                    leftArrow.gameObject.SetActive(true);
                    rightArrow.gameObject.SetActive(true);
                    readyText.SetActive(false);
                    break;
                case SelectionState.Ready:
                    characterChoice = carousel.GetActiveCharacter();
                    carousel.isLocked = true;
                    PlayerManager.instance.RemoveCharacter(carousel.GetActiveCharacterIndex());
                    joinText.SetActive(false);
                    //characterSelectMenu.SetActive(false);
                    leftArrow.gameObject.SetActive(false);
                    rightArrow.gameObject.SetActive(false);
                    readyText.SetActive(true);
                    break;
                case SelectionState.Testing:
                    break;
            }
        }

        get { return _state; }
    }

    private SelectionState _state = SelectionState.CharacterSelect;

    public PlayerObj(int playerID, int playerNumber, GameObject joinText, GameObject characterSelectMenu, GameObject readyText)
    {
        this.playerID = playerID;
        this.playerNumber = playerNumber;
        this.joinText = joinText;
        this.characterSelectMenu = characterSelectMenu;
        this.readyText = readyText;

        Button[] buttons = characterSelectMenu.GetComponentsInChildren<Button>();
        if (buttons[0].gameObject.name.Contains("Left"))
        {
            rightArrow = buttons[1];
            leftArrow = buttons[0];
        } else
        {
            leftArrow = buttons[1];
            rightArrow = buttons[0];
        }

        carousel = characterSelectMenu.GetComponentInChildren<Carousel>();

        state = SelectionState.CharacterSelect;
    }

    /// <summary>
    /// Simple constructor for testing purposes
    /// </summary>
    public PlayerObj(int playerID, int playerNumber)
    {
        this.playerID = playerID;
        this.playerNumber = playerNumber;
        state = SelectionState.Testing;
    }


    public void ControlsCheck()
    {
        if (playerID > 4)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                state = SelectionState.Ready;
            }

            return;
        }

        switch(state)
        {
            case SelectionState.CharacterSelect:
                if (ReInput.players.GetPlayer(playerID).GetButtonDown("Up"))
                {
                    leftArrow.onClick.Invoke();
                }
                else if (ReInput.players.GetPlayer(playerID).GetButtonDown("Down"))
                {
                    rightArrow.onClick.Invoke();
                }

                if (ReInput.players.GetPlayer(playerID).GetButtonDown("Select"))
                {
                    state = SelectionState.Ready;
                } else if (ReInput.players.GetPlayer(playerID).GetButtonDown("Back"))
                {
                    state = SelectionState.Joining;
                }
                break;
            case SelectionState.Ready:
                if (ReInput.players.GetPlayer(playerID).GetButtonDown("Back"))
                {
                    state = SelectionState.CharacterSelect;
                }
                break;
        } 
    }
}
