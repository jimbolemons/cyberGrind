using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{

    [SerializeField] private float cameraLeadOffset = .5f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float cameraScaleSpeed = 10f;

    // So I can reference back to the OG values 
    private float cameraDiffToOffsetOriginal;
    private float moveSpeedOriginal;
    
    private float desiredXCameraPosition = 0f, desiredYCameraPosition = 0f;
     private float scaless = 0f;
    private Transform mainCamera;
    private Camera camera;
    private float cameraDiffToOffset = 0f;
    private Coroutine moveAndScaleCoroutine = null;
    private bool locked = false;
    private Transform[] walls;

    private string leadName = "";
    private string lastName = "";
    float lastPerson = 0f;
    float firstPerson = 0f;
    float lastPerson2 = 0f;
    float firstPerson2 = 0f;

    private void Start()
    {
        desiredXCameraPosition = Camera.main.transform.position.x;

        mainCamera = Camera.main.transform;
        camera = Camera.main;

        cameraDiffToOffset = (Camera.main.aspect * Camera.main.orthographicSize)- cameraLeadOffset;


        cameraDiffToOffsetOriginal = cameraDiffToOffset;
        moveSpeedOriginal = moveSpeed;


        BSGameManager.instance.leadXPos = mainCamera.position.x + cameraDiffToOffset;

        walls = GetComponentsInChildren<Transform>();

        StartCoroutine(UpdateFollowTarget());
    }

    private void Update()
    {
        if (locked)
            return;

        // Fix up offset if needed 
        if (cameraDiffToOffset < cameraDiffToOffsetOriginal)
        {
            cameraDiffToOffset += .1f;
        }
        else if (cameraDiffToOffset > cameraDiffToOffsetOriginal)
        {
            cameraDiffToOffset = cameraDiffToOffsetOriginal;
        }

        // Fix up speed if needed 
        if (moveSpeed < moveSpeedOriginal)
        {
            moveSpeed += .07f;

            // Need to account for the players being too fast 

        }
        else if (moveSpeed > moveSpeedOriginal)
        {
            moveSpeed = moveSpeedOriginal;
        }
        desiredXCameraPosition = (lastPerson + firstPerson)/2;
        desiredYCameraPosition = (lastPerson2 + firstPerson2)/2;
        scaless = firstPerson-lastPerson;
        if(scaless <= 30)
        {
                scaless= 30;
        }
        else if (scaless >=50)
        {
         scaless= 50;
        }

        
        StartMoveAndScale(scaless,1);
        float diff = desiredXCameraPosition - mainCamera.position.x;
         float diff2 = (desiredYCameraPosition - mainCamera.position.y) +10;

       // mainCamera.transform.position = new Vector3(desiredXCameraPosition,0, 0);

        if (Mathf.Abs(diff) > .01f)
        {
            mainCamera.Translate(new Vector3(diff, diff2, 0) * Time.deltaTime * moveSpeed);
           
        }
        
    }


    public void StartMoveAndScale(float desiredSize, float desiredYPos)
    {
        if (moveAndScaleCoroutine != null)
            StopCoroutine(moveAndScaleCoroutine);


        moveAndScaleCoroutine = StartCoroutine("MoveAndScale", new object[2] { desiredSize, desiredYPos });
    }

    public void SetCameraLock(bool isLocked)
    {
        locked = isLocked;
    }

    /// <summary>
    /// Checks for player who is in lead and matches camera to them
    /// </summary>
    private IEnumerator UpdateFollowTarget()
    {
        BSGameManager gameManager = BSGameManager.instance;
        while (gameManager.GameState_ != BSGameManager.GameState.Overview)
        {
            GameObject newLead = gameManager.leadPlayer; // Player that is now in lead 
            GameObject newLast = gameManager.lastPlayer;

            // New stuff for attempted smoothness
            if (newLead != null)
            {
                if (leadName != newLead.name)
                { // Someone else has taken the lead 
                    moveSpeed = moveSpeedOriginal / 10; // Change speed?
                     cameraDiffToOffset = cameraDiffToOffsetOriginal / 4; // Change offset?
                }

                // Reset name 
                leadName = newLead.name;
                firstPerson = newLead.transform.position.x;
                firstPerson2 = newLead.transform.position.y;
            }
            if (newLast != null)
            {
                if (lastName != newLast.name)
                { // Someone else has taken the lead 

                    moveSpeed = moveSpeedOriginal / 10; // Change speed?
                     cameraDiffToOffset = cameraDiffToOffsetOriginal / 4; // Change offset?
                }

                // Reset name 
                lastName = newLast.name;
                Debug.Log(lastName);
                lastPerson = newLast.transform.position.x;
                 lastPerson2 = newLast.transform.position.y;
            }
            
            

            float leadXPos = gameManager.leadXPos;
            if (leadXPos > mainCamera.position.x + cameraDiffToOffset)
            {
                desiredXCameraPosition = leadXPos - cameraDiffToOffset;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Moves and scales the camera to desired postion and scale
    /// </summary>
    /// <param name="desiredSizeAndPositionObj">Object with the first value being a float 
    /// of the desired size and the second being a Vector2 position of the desired postion</param>
    /// <returns>Nothing</returns>
    private IEnumerator MoveAndScale(object[] desiredSizeAndPositionObj)
    {
        if (!typeof(float).IsAssignableFrom(desiredSizeAndPositionObj[0].GetType()))
            Debug.LogError("Passed an unacceptable float type to MoveAndScale");
        if (!typeof(float).IsAssignableFrom(desiredSizeAndPositionObj[1].GetType()))
            Debug.LogError("Passed an unacceptable Yposition type to MoveAndScale");


        float desiredSize = (float)desiredSizeAndPositionObj[0];
        float desiredYPos = (float)desiredSizeAndPositionObj[1];






        if (desiredSize > camera.orthographicSize)
        {
            //&& transform.position.y != desiredYPos
            while (camera.orthographicSize < desiredSize )
            {
                // Scale and move camera
                camera.orthographicSize += cameraScaleSpeed * Time.deltaTime ;
               // transform.Translate(new Vector3(0,(desiredYPos - transform.position.y),0) * Time.deltaTime * moveSpeed);

                // Move Walls
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, transform.position.z));
                foreach (Transform wall in walls)
                {                   
                    wall.position -= new Vector3(cameraScaleSpeed * Time.deltaTime * Mathf.Sign(transform.position.x - wall.position.x),0,0);
                }

                yield return new WaitForEndOfFrame();
            }
        } else
        {
            //&& transform.position.y != desiredYPos
            while (camera.orthographicSize > desiredSize )
            {
                // Scale and move camera
                camera.orthographicSize -= cameraScaleSpeed * Time.deltaTime ;
                //transform.Translate(new Vector3(0, (desiredYPos - transform.position.y), 0) * Time.deltaTime * moveSpeed);

                // Move Walls
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, transform.position.z));
                foreach (Transform wall in walls)
                {
                    wall.position += new Vector3(cameraScaleSpeed * Time.deltaTime * Mathf.Sign(transform.position.x - wall.position.x), 0, 0);
                }

                yield return new WaitForEndOfFrame();
            }
        }


        moveAndScaleCoroutine = null;
    }
}
