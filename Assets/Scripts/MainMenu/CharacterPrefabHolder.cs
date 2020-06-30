using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPrefabHolder : MonoBehaviour
{
    public GameObject characterPrefab;
    [SerializeField] private AK.Wwise.Event characterSwitch, pickedVoiceLine;

    private void Start()
    {
        characterSwitch.Post(gameObject);
    }

    public void PlayVoiceline()
    {
        pickedVoiceLine.Post(gameObject);
    }
}
