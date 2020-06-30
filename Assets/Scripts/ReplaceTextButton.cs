using Rewired;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReplaceTextButton : MonoBehaviour
{
    [SerializeField] private string actionName;
    [SerializeField] private int playerNumber;

    [Tooltip("If the indicator is 'x' then in the text it should be 'Press {x} to join'")]
    [SerializeField] private string buttonIndicator;

    private TextMeshProUGUI text;

    private void OnEnable()
    {
        // Start Button Check Loop
        StartCoroutine(ButtonCheckLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    private IEnumerator ButtonCheckLoop()
    {
        bool didFail = true;

        while (didFail)
        {
            try
            {
                ButtonCheck();
                didFail = false;
                yield break;
            }
            catch (System.Exception e)
            {
                didFail = true;
                if (text)
                    text.text = text.text.Replace("{" + buttonIndicator + "}", buttonIndicator);
            }

            yield return new WaitForSeconds(1f);
        }
        
    }

    private void ButtonCheck()
    {
        text = GetComponent<TextMeshProUGUI>();

        Player player = Rewired.ReInput.players.GetPlayer(playerNumber);
        string buttonName = player.controllers.maps.GetFirstButtonMapWithAction(actionName, true).elementIdentifierName;

        string currentText = text.text;
        text.text = currentText.Replace("{" + buttonIndicator + "}", buttonName);
    }


    
}
