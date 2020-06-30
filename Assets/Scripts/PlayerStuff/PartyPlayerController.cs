/*
 * PartyPlayerController.cs
 * 
 * By: Jerod D'Epifanio
 * 
 * Edit Log:
 *      Jerod D'Epifanio:
 *          Added a header
 *          
 *      Collin Nicaise:
 *          * Used this for reference: https://guavaman.com/projects/rewired/docs/QuickStart.html
 *          + Comments for rewired things
 *          + Horizontal/vertical movement handling 
 *          + More movement tweaking
 *          
 *      Jerod D'Epifanio:
 *          + Exposed moveSpeed and jumpForce
 *          + Moved groundcheck to a function
 *          + added some regions
 *          
 *                
 *      CN:
 *          + Comments on functions and variables because I skipped them originally 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PartyPlayerController : MonoBehaviour
{
    #region Public Vars

    public int playerId { 
        set 
        { 
            _playerId = value;
            SetupInputDelegates();
        } 
        get { return _playerId; } } // Rewired playerId for this character 

    public Player player;                              // Rewired player 

    #endregion


    #region Private Vars

    private int _playerId; 

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 20.0f;   // Movement speed of this character
    [SerializeField] protected float jumpForce = 200.0f;  // Jump force of this character 

    
    protected Vector2 moveVector = Vector2.zero;      // Current movement vector
    protected Vector2 aimVector = Vector2.zero;         // Current aim vector
    private CircleCollider2D groundCollider;            // Collider near player feet for ground checking
    protected bool grounded = false;                      // Bool to track if player is currently grounded

    protected Rigidbody2D rigidBody;                    // Player's rigidbody for movement 

    #endregion


    #region Unity Callbacks

    protected virtual void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        groundCollider = gameObject.GetComponent<CircleCollider2D>();
    }

    private void Awake()
    {
        //SetupInputDelegates();
    }

    protected virtual void Update()
    {
        Aim();
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GroundedCheck(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        GroundedCheck(other);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GroundLeaveCheck(collision);
    }

    #endregion

    #region Private Functions

    #region Input Delegates

    /// <summary>
    /// Sets up input delegates with Rewired
    /// </summary>
    protected void SetupInputDelegates()
    {
        player = ReInput.players.GetPlayer(playerId); // Get the Rewired Player object for this player

        if (player == null)
        {
            Debug.LogError("Player " + playerId + " does not exist");
            return;
        }

        // GamePlay inputs
        player.AddInputEventDelegate(OnMoveHorizontal, UpdateLoopType.Update, InputActionEventType.AxisActiveOrJustInactive, "MoveHorizontal");
        player.AddInputEventDelegate(OnMoveVertical, UpdateLoopType.Update, InputActionEventType.AxisActiveOrJustInactive, "MoveVertical");
        player.AddInputEventDelegate(OnAimHorizontal, UpdateLoopType.Update, InputActionEventType.AxisActiveOrJustInactive, "AimHorizontal");
        player.AddInputEventDelegate(OnAimVertical, UpdateLoopType.Update, InputActionEventType.AxisActiveOrJustInactive, "AimVertical");
        player.AddInputEventDelegate(OnJumpDown, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Jump");
        player.AddInputEventDelegate(OnJumpUp, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Jump");
        player.AddInputEventDelegate(OnSlideDown, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Slide");
        player.AddInputEventDelegate(OnSlideUp, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Slide");
        player.AddInputEventDelegate(OnFireUp, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Fire");
        player.AddInputEventDelegate(OnFireDown, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Fire");
        player.AddInputEventDelegate(OnUsePowerup, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "UsePowerup");
        player.AddInputEventDelegate(OnPause, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Pause");

        // Menu inputs
        // MoveUp, MoveDown, ClickButton, Unpause
        player.AddInputEventDelegate(OnMoveUp, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "MoveUp");
        player.AddInputEventDelegate(OnMoveDown, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "MoveDown");
        player.AddInputEventDelegate(OnClickButton, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "ClickButton");
        player.AddInputEventDelegate(OnPause, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Unpause");
    }



    private void OnMoveHorizontal(InputActionEventData data)
    {
        if (!enabled) { return; }
        moveVector.x = data.GetAxis();
    }

    private void OnMoveVertical(InputActionEventData data)
    {
        if (!enabled) { return; }
        moveVector.y = data.GetAxis();
    }

    private void OnAimHorizontal(InputActionEventData data)
    {
        if (!enabled) { return; }
        aimVector.x = data.GetAxis();
    }

    private void OnAimVertical(InputActionEventData data)
    {
        if (!enabled) { return; }
        aimVector.y = data.GetAxis();
    }

    private void OnJumpDown(InputActionEventData data)
    {
        if (!enabled) { return; }
        JumpDown();
    }

    private void OnJumpUp(InputActionEventData data)
    {
        if (!enabled) { return; }
        JumpUp();
    }

    private void OnSlideDown(InputActionEventData data)
    {
        if (!enabled) { return; }
        SlideDown();
    }

    private void OnSlideUp(InputActionEventData data)
    {
        if (!enabled) { return; }
        SlideUp();
    }

    private void OnFireDown(InputActionEventData data)
    {
        if (!enabled) { return; }
        FireUp();
    }

    private void OnFireUp(InputActionEventData data)
    {
        if (!enabled) { return; }
        FireDown();
    }

    private void OnUsePowerup(InputActionEventData data)
    {
        if (!enabled) { return; }
        UsePowerup();
    }

    private void OnPause(InputActionEventData data)
    {
        if (!enabled) { return; }
        Pause();
    }

    // Menu stuff
    private void OnMoveUp(InputActionEventData data)
    {
        if (!enabled) { return; }
        MenuMoveUp();
    }

    private void OnMoveDown(InputActionEventData data)
    {
        if (!enabled) { return; }
        MenuMoveDown();
    }

    private void OnClickButton(InputActionEventData data)
    {
        if (!enabled) { return; }
        MenuClickButton();
    }

    #endregion

    #endregion

    #region Protected Functions
    // Handles moving vertically/horizontally


    virtual protected void GroundedCheck(Collider2D other)
    {
        // Grounded check for jumping 
        if (other.tag == "Ground" || other.CompareTag("Player"))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }


    virtual protected void GroundLeaveCheck(Collider2D other)
    {
        // Grounded check for jumping 
        if (other.tag == "Ground" || other.CompareTag("Player"))
        {
            grounded = false;
        }
    }

    #endregion

    #region Abstract Functions
    virtual protected void FireUp() {}
    virtual protected void FireDown() {}
    virtual protected void UsePowerup() {}
    virtual protected void Move() {}
    virtual protected void JumpDown() {}
    virtual protected void JumpUp() {}
    virtual protected void Aim() {}
    virtual protected void SlideDown() {}
    virtual protected void SlideUp() {}
    virtual protected void Pause() {}
    virtual protected void MenuMoveUp() {}
    virtual protected void MenuMoveDown() {}
    virtual protected void MenuClickButton() {}

    #endregion


}

