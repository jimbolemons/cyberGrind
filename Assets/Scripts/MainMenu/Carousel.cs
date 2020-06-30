using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carousel : MonoBehaviour
{
    [HideInInspector] public bool isLocked = false;

    [SerializeField] private float moveSpeed = 20f;

    private List<GameObject> children = new List<GameObject>();
    private List<CoroutineMonkey> monkeys = new List<CoroutineMonkey>();
    private List<int> removedIndexes = new List<int>();
    private int activeIndex = 0;
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        

        // Get kids
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
            monkeys.Add(child.GetComponent<CoroutineMonkey>());
        }

        startPos = children[0].transform.position;      
        foreach (Transform child in transform)
        {
            child.transform.position += new Vector3(0, 15, 0);
        }
        monkeys[0].StartCoroutine(SendIn(children[0], 1));

    }

    private IEnumerator SendOut(GameObject child, int directionMod)
    {
        Vector3 desiredPosition = startPos - new Vector3(0, 15, 0);

        if (directionMod == -1)
        {
            desiredPosition = startPos + new Vector3(0, 15, 0);
        }

        bool logic = directionMod == 1 ? child.transform.position.y > desiredPosition.y : child.transform.position.y < desiredPosition.y;
        while (logic)
        {
            child.transform.Translate(Vector3.down * directionMod * Time.deltaTime * moveSpeed);
            yield return new WaitForEndOfFrame();
            logic = directionMod == 1 ? child.transform.position.y > desiredPosition.y : child.transform.position.y < desiredPosition.y;
        }

        child.transform.position = desiredPosition;
    }

    private IEnumerator SendIn(GameObject child, int directionMod)
    {
        if (directionMod == 1)
            child.transform.position = startPos + new Vector3(0,15,0);
        else
            child.transform.position = startPos - new Vector3(0, 15, 0);
        bool logic = directionMod == 1 ? child.transform.position.y > startPos.y : child.transform.position.y < startPos.y;
        while (logic)
        {
            child.transform.Translate(Vector3.down * directionMod * Time.deltaTime * moveSpeed);
            yield return new WaitForEndOfFrame();
            logic = directionMod == 1 ? child.transform.position.y > startPos.y : child.transform.position.y < startPos.y;
        }

        child.transform.position = startPos;
    }

    public void Forward()
    {
        if (monkeys.Count > activeIndex && monkeys[activeIndex])
        {
            monkeys[activeIndex].StopAllCoroutines();
            monkeys[activeIndex].StartCoroutine(SendOut(children[activeIndex], -1));
        }

        if (removedIndexes.Count == children.Count) { return; }

        int timeOut = 0;
        do
        {
            if (timeOut++ == 10) { break; }

            if (activeIndex == children.Count - 1)
                activeIndex = -1;
            activeIndex++;
        } while (removedIndexes.Contains(activeIndex));

        if (children.Count - 1 >= activeIndex)
        {
            monkeys[activeIndex].StopAllCoroutines();
            monkeys[activeIndex].StartCoroutine(SendIn(children[activeIndex], -1));
        }
            
    }


    public void Back()
    {
        monkeys[activeIndex].StopAllCoroutines();
        monkeys[activeIndex].StartCoroutine(SendOut(children[activeIndex], 1));

        do
        {
            if (activeIndex == 0)
                activeIndex = children.Count;
            activeIndex--;
        } while (removedIndexes.Contains(activeIndex));

        monkeys[activeIndex].StopAllCoroutines();
        monkeys[activeIndex].StartCoroutine(SendIn(children[activeIndex], 1));

    }

    public GameObject GetActiveCharacter()
    {
        children[activeIndex].GetComponent<CharacterPrefabHolder>().PlayVoiceline();
        return children[activeIndex].GetComponent<CharacterPrefabHolder>().characterPrefab;
    }

    public int GetActiveCharacterIndex()
    {
        return activeIndex;
    }

    public void RemoveAtIndex(int index)
    {
        removedIndexes.Add(index);
        if (activeIndex == index && !isLocked)
            Forward();
    }

    public void RestoreAtIndex(int index)
    {
        if (removedIndexes.Contains(index))
            removedIndexes.Remove(index);
    }

    private void OnDisable()
    {
        foreach(GameObject child in children)
        {
            child.transform.position = startPos + new Vector3(0, 15, 0);
        }

        children[activeIndex].transform.position = startPos;
    }
}
