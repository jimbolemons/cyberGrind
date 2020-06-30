using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour
{
    [SerializeField] private Button button;

    public void SelectTheButton()
    {
        Selectable defaultButton = button;
        defaultButton.Select();
        defaultButton.OnSelect(null);
    }
}
