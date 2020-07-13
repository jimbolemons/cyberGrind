using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BSPlayerController : PartyPlayerController
{
    public enum GrindState
    {
        Good,
        Bad,
        Not
    }

    [SerializeField] private float frictionAcceleration = .1f;
    [SerializeField] private string[] tagsToRestoreJump;

    [Header("Attack")]
    [SerializeField] private Vector2 explosivePower = new Vector2(10f, 100f);
    [SerializeField] private float explosiveRadius = 10f;
    [SerializeField] private float explosiveUpMod = 1f;
    [Tooltip("Tags that won't be effected by the explosion")]
    [SerializeField] private string[] tagsToIgnore;
    [Tooltip("How long it takes to charge a hit fully, in seconds")]
    [SerializeField] private float maxCharge = 2f;
    [SerializeField] private float chargeSpeed = .1f;
    [SerializeField] private float aimThreshold = .25f;
    [SerializeField] private float maxAimBarScale = 1f;
    [SerializeField] private float chargeSlowdownRate = .2f;
    [Tooltip("The minimum charge required for blast to work")]
    [SerializeField] private float chargeThreshold = .2f;
    [SerializeField] private float maxBlastAmount = 10f;
    [SerializeField] private float blastCost = 1f;
    [SerializeField] private float blastGainSpeed = 5f;
    [SerializeField] private float passiveBlastRegen = .1f;
    [SerializeField] private GameObject overheatIndicator;

    [Header("Grind")]
    [SerializeField] private float grindDownForce = 1f;

    [Header("VFX")]
    [SerializeField] private ParticleSystem attackVFX;
    [SerializeField] private ParticleSystem badChargeVFX;
    [SerializeField] private ParticleSystem[] grindVFX;
    [SerializeField] private ParticleSystem badGrindVFX;

    [Header("SFX")]
    [SerializeField] private AK.Wwise.Event chargeSoundEvent;
    [SerializeField] private AK.Wwise.Event chargeBlastSoundEvent;
    [SerializeField] private AK.Wwise.Event grindSoundEvent;
    [SerializeField] private AK.Wwise.Event stopGrindSoundEvent;
    [SerializeField] private AK.Wwise.RTPC chargeRTPC;
    [SerializeField] private AK.Wwise.RTPC chargeBlastRTPC;
    [SerializeField] private AK.Wwise.RTPC grindRTPC;
    [SerializeField] private AK.Wwise.Event normalGravityState;
    [SerializeField] private AK.Wwise.Event antiGravityState;
    [SerializeField] private AK.Wwise.Event noBoostSoundEvent;

    



    [Header("Other")]
    public string playerName = "x";
    [SerializeField] public GameObject aimingLine;
    public bool isGrinding;

    public bool HitBox;
    public float Stuntimer;

    [Header("Events")]
    public UnityEvent onChargeStart;
    public UnityEvent onChargeRelease;
    public UnityEvent onBlast;
    public UnityEvent onGrindStart;
    public UnityEvent onGrindEnd;
    public UnityEvent onGoodGrindStart;
    public UnityEvent onGoodGrindEnd;
    public UnityEvent onJumpToRail;
    public UnityEvent onOverheatedBlast;
    public UnityEvent onSlowed;
    public UnityEvent onWin;
    public UnityEvent onBadGrindStart;
    public UnityEvent onBadGrindEnd;
    public UnityEvent onCountdownStart;
    public UnityEvent onCountdownEnd;


    
    [HideInInspector] public PlayerAnimatorHandler animHandler;
    [HideInInspector] public bool isOnRail = false;
    [HideInInspector] private bool isInGravityZone = false;
    [HideInInspector] public bool isSlowed = false;
    
    public bool IsInGravityZone
    {
        get { return isInGravityZone; }
        set
        {
            isInGravityZone = value;

            // Handle sound distortion things 
            if (value) { antiGravityState.Post(gameObject); }
            else { normalGravityState.Post(gameObject); }
        }
    }
    [HideInInspector] public Powerup currentPowerup = null;


    private float currentCharge = 0f;
    private bool isCharging = false;
    private Coroutine chargeRoutine = null;
    private float frictionValue = 0f;
    private GameObject ground = null;
    private float chargeAimScalePercent = 0;
    private float aimingLineStartingScale = 0;
    private Collider2D forceHitZone = null;
    private bool isSliding = false;
    private int upgradeCount = 0;
    private float blastAmount = 0;
    private GrindState grindState = GrindState.Not;


    // UI Things 
    private string nameUI = "";
    [HideInInspector] public GameObject playerUI = null;
    private TMPro.TextMeshProUGUI scoreText = null;
    private string powerupName = "";
    public Material chargeBarMaterial;
    public Image chargeBar;


    // For countdown 
    private bool isInCountdown_ = true;
    public bool isInCountdown
    {
        get => isInCountdown_;
        set
        {
            isInCountdown_ = value;

            switch (isInCountdown_)
            {
                case (true):
                    onCountdownStart.Invoke();
                    break;

                case (false):
                    onCountdownEnd.Invoke();
                    break;
            }
        }
    }


    #region Public Functions

    public void AddUpgrade()
    {
        upgradeCount++;
    }

    public void AddUpgrade(int amount)
    {
        upgradeCount += amount;
    }

    public int GetUpgradeCount()
    {
        return upgradeCount;
    }

    public IEnumerator SlowTimer()
    {
        onSlowed.Invoke();
        isSlowed = true;
        yield return new WaitForSeconds(2f);
        isSlowed = false;
    }

    #endregion

    #region Protected Functions

    protected override void Start()
    {
        base.Start();

        // Get what 1 percent of charge is
        chargeAimScalePercent = (maxAimBarScale - aimingLine.transform.localScale.x) / 100;
        aimingLineStartingScale = aimingLine.transform.localScale.x;

        forceHitZone = aimingLine.GetComponent<Collider2D>();

        // Save name for UI things 
        nameUI = "Player" + playerId + " UI";

        // create the animation handler
        animHandler = new PlayerAnimatorHandler(this, GetComponentInChildren<Animator>());

        // Start in idle for the countdown
        animHandler.playerState = PlayerAnimatorHandler.PlayerState.Idle;

        blastAmount = maxBlastAmount;

        onGrindStart.AddListener(() => { grindSoundEvent.Post(gameObject); });
        onGrindEnd.AddListener(() => { grindSoundEvent.Stop(gameObject); });

        onGoodGrindStart.AddListener(() => { Array.ForEach(grindVFX, (x) => x.Play()); });
        onGoodGrindEnd.AddListener(() => { Array.ForEach(grindVFX, (x) => x.Stop()); });

        onBadGrindStart.AddListener(() => { badGrindVFX.Play(); });
        onBadGrindEnd.AddListener(() => { badGrindVFX.Stop(); });

    }

    protected override void Update()
    {
        base.Update();

        VelocityCap();
        PassiveBoost();
        if (HitBox)
        {
            Invoke("HitBoxTimer",Stuntimer);
        }
    }

    protected override void FixedUpdate()
    {
        Move();

        UpdateUI();
    }

    protected override void FireDown()
    {
        if (chargeRoutine == null)
            chargeRoutine = StartCoroutine(Charge());
    }

    protected override void FireUp()
    {
        if (isCharging)
            isCharging = false;
    }

    protected override void JumpDown()
    {
        onGrindStart.Invoke();
        isGrinding = true;

        if (grounded)
        {
            if (isOnRail)
            {
                grindState = GrindState.Good;
                onGoodGrindStart.Invoke();
            } else
            {
                grindState = GrindState.Bad;
                onBadGrindStart.Invoke();
            }
        }
    }

    protected override void JumpUp()
    {
        onGrindEnd.Invoke();
        if (grindState == GrindState.Bad)
            onBadGrindEnd.Invoke();
        else
            onGoodGrindEnd.Invoke();
        grindState = GrindState.Not;
        isGrinding = false;
    }

    protected override void Move()
    {
        if (isGrinding && !isOnRail && !grounded)
        {

            if (rigidBody.velocity.y > 0)
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

            rigidBody.velocity -= new Vector2(0, grindDownForce * Time.deltaTime);
        }

        if (isGrinding && grounded)
        {
            grindRTPC.SetValue(gameObject, rigidBody.velocity.magnitude);
        }

        if (rigidBody.velocity.x < BSGameManager.instance.minVelocityCap + upgradeCount && !isGrinding && !isCharging)
            rigidBody.velocity += new Vector2(moveSpeed * Time.deltaTime, /*moveVector.y * */ 0);

        if (grounded && isGrinding && !isOnRail)
        {
            frictionValue += frictionAcceleration * Time.deltaTime;
            if (rigidBody.velocity.x - frictionValue > 0)
                rigidBody.velocity -= new Vector2(frictionValue, 0);

        }
        else if (isGrinding && isOnRail)
        {

            if (blastAmount < maxBlastAmount)
            {
                blastAmount += blastGainSpeed * Time.deltaTime;
            }

            if (blastAmount > maxBlastAmount)
                blastAmount = maxBlastAmount;
        }
    }

    protected override void Aim()
    {
        if (moveVector.x > aimThreshold || moveVector.y > aimThreshold ||
            moveVector.x < -aimThreshold || moveVector.y < -aimThreshold)
        {
            aimingLine.SetActive(true);

            if (moveVector.x > 0)
                moveVector.x = 0;

            float theta = Mathf.Atan2(moveVector.y, moveVector.x) * Mathf.Rad2Deg;
            if (theta == 0)
            {
                return;
            }

            float x = 0.0f;
            if (IsInGravityZone) { x = 180.0f; }

            aimingLine.transform.localEulerAngles = new Vector3(x, 0, theta);
        }
        else
        {
            aimingLine.SetActive(false);
        }
    }

    protected override void SlideDown()
    {
        isSliding = true;
    }

    protected override void SlideUp()
    {
        isSliding = false;
    }

    protected override void Pause()
    {
        BSGameManager.instance.Pause(); // Make the manager handle pausing
    }

    protected override void MenuMoveUp()
    {
        BSGameManager.instance.MenuMoveUp();
    }

    protected override void MenuMoveDown()
    {
        BSGameManager.instance.MenuMoveDown();
    }

    protected override void MenuClickButton()
    {
        BSGameManager.instance.MenuClickButton();
    }


    /// <summary>
    /// Activates the powerup that the player is holding, if they have one
    /// </summary>
    protected override void UsePowerup()
    {
        Debug.Log(playerId + " / " + playerName + " hit powerup button and has '" + currentPowerup.powerupName + "'");

        // Check that the player has a powerup 
        if ((currentPowerup.powerupName != null) && (currentPowerup != null))
        {
            Debug.Log("-Activating " + currentPowerup.powerupName + " for player " + playerId + "/" + playerName);
            // They do, so we use it up and set back to null
            currentPowerup.ActivatePowerup(this.gameObject);

            // Remove the powerup from the player
            currentPowerup = null;
        }
    }

    protected override void GroundedCheck(Collider2D other)
    {
        // Grounded check for jumping 
        if (Array.IndexOf(tagsToRestoreJump, other.tag) != -1)
        {
            grounded = true;
            animHandler.SetGrounded(true);
            ground = other.gameObject;
            if (isGrinding && grindState == GrindState.Not)
            {
                if (isOnRail)
                {
                    grindState = GrindState.Good;
                    onBadGrindEnd.Invoke();
                    onGoodGrindStart.Invoke();
                } else
                {
                    grindState = GrindState.Bad;
                    onGoodGrindEnd.Invoke();
                    onBadGrindStart.Invoke();
                }
            }
        }
        else
        {
            grounded = false;
            animHandler.SetGrounded(false);
            frictionValue = 0;
            if (isGrinding)
            {
                if (grindState == GrindState.Good)
                    onGoodGrindEnd.Invoke();
                else if (grindState == GrindState.Bad)
                    onBadGrindEnd.Invoke();

                grindState = GrindState.Not;
            }
        }
    }

    protected override void GroundLeaveCheck(Collider2D other)
    {
        // Grounded check for jumping 
        if (Array.IndexOf(tagsToRestoreJump, other.tag) != -1)
        {
            frictionValue = 0;
            grounded = false;
            animHandler.SetGrounded(false);
        }
    }

    #endregion

    #region Private Functions

    private void PassiveBoost()
    {
        if (blastAmount == maxBlastAmount)
        {
            return;
        }
        else if (blastAmount > maxBlastAmount)
        {
            blastAmount = maxBlastAmount;
            return;
        }
        else
        {
            blastAmount += passiveBlastRegen * Time.deltaTime;
            if (blastAmount > maxBlastAmount)
            {
                blastAmount = maxBlastAmount;
            }
        }
    }

    private void VelocityCap()
    {
        if (rigidBody.velocity.x > BSGameManager.instance.minVelocityCap)
        {
            float diff = rigidBody.velocity.x - BSGameManager.instance.minVelocityCap;
            if (diff > 0)
                diff /= 10;
            else
                diff = 1;
            rigidBody.velocity -= new Vector2(moveSpeed * Time.deltaTime * diff, 0);
        }
    }

    /// <summary>
    /// Makes an explosive force around the player
    /// </summary>
    private void ExplosiveForce()
    {
        Debug.Log("BIG FUCK");
        //PlayBlastSound Wwise
        chargeBlastRTPC.SetValue(gameObject, currentCharge);
        chargeBlastSoundEvent.Post(gameObject);

        onBlast.Invoke();

        Vector3 explosionPos = transform.position;
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, explosiveRadius);
        Collider2D[] colliders = new Collider2D[3];
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useDepth = true;
        Physics2D.OverlapCollider(forceHitZone, contactFilter, colliders);
        bool good = false;
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider2D hit = colliders[i];

            if (hit == null || (Array.IndexOf(tagsToIgnore, hit.tag) != -1 && hit.gameObject != gameObject)) { 
                if (i == colliders.Length - 1)
                {
                    hit = transform.GetComponent<Collider2D>();
                } else
                {
                    continue;
                }
            }

            good = true;

            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 total = Vector2.zero;
                float charge = Mathf.Pow(currentCharge, 2);
                charge = charge == 0 ? 1f : charge;
                if (hit.gameObject != gameObject)
                {
                    Vector2 positionVector = ((hit.transform.position - transform.position).normalized);
                    Vector2 scaledAimVector = new Vector2(positionVector.x * explosivePower.x, positionVector.y * explosivePower.y) * charge;
                    scaledAimVector = scaledAimVector == Vector2.zero ? -explosivePower : scaledAimVector;
                    total = scaledAimVector + (rigidBody.gravityScale * Physics2D.gravity);
                }
                else if (hit.gameObject == gameObject)
                {
                    Vector2 positionVector = moveVector.normalized;
                    Vector2 scaledAimVector = new Vector2(positionVector.x * explosivePower.x, positionVector.y * explosivePower.y) * charge;
                    scaledAimVector = scaledAimVector == Vector2.zero ? -explosivePower : scaledAimVector;
                    total = scaledAimVector/* + (rigidBody.gravityScale * Physics2D.gravity)*/;
                    total *= -1;
                }
                Debug.Log(total);
                if(Mathf.Abs(total.x) <= 10f && Mathf.Abs(total.y) <= 10f )
                {
                    total = new Vector2(72,0);
                }
                rb.AddForce(total, ForceMode2D.Impulse);
            }
        }

        // Show VFX
        if (attackVFX)
        {
            attackVFX.Play();
        }
    }


    /// <summary>
    /// Increments the current charge while fire button is held down
    /// Explodes when fire button is let up
    /// </summary>
    private IEnumerator Charge()
    {
        //CHARGE        
        if (blastAmount < blastCost)
        {
            OnOverHeatedBlast();
            yield break;
        }
        if (HitBox)
        {
            OnOverHeatedBlast();
            //player cannot boost
            Invoke("HitBoxTimer",Stuntimer);
             yield break;
        }


        onChargeStart.Invoke();
        isCharging = true;
        chargeSoundEvent.Post(gameObject);
        while (isCharging)
        {

            if (currentCharge < maxCharge)
                currentCharge += chargeSpeed;

            if (currentCharge > maxCharge)
                currentCharge = maxCharge;

            //aimingLine.transform.localScale = new Vector3(
            //aimingLineStartingScale + (chargeAimScalePercent * ( Mathf.Pow(currentCharge, 2) / (maxCharge / 100))), 1, 1);

            chargeRTPC.SetValue(gameObject, currentCharge);

            yield return new WaitForSeconds(.1f);
        }

        onChargeRelease.Invoke();

        //StopChargeSFX Wwise 
        chargeSoundEvent.Stop(gameObject);

        ExplosiveForce();
        blastAmount -= blastCost;

        aimingLine.transform.localScale = new Vector3(aimingLineStartingScale, 1, 1);
        currentCharge = 0;
        chargeRoutine = null;

        yield return null;
    }
    private void HitBoxTimer()
    {
        HitBox = false;


    }

    private void OnOverHeatedBlast()
    {
        // Wwise Trigger
        noBoostSoundEvent.Post(gameObject);

        if (badChargeVFX)
            badChargeVFX.Play();

        onOverheatedBlast.Invoke();
    }

    private void UpdateUI()
    {
        if (playerUI == null) { return; }

        // Update the UI for score
        if (scoreText == null)
        {
            // Get text for score if we have not yet
            scoreText = playerUI.transform.Find("Score Text").GetComponent<TMPro.TextMeshProUGUI>();
        }
        else
        {
            scoreText.SetText("Upgrades: " + upgradeCount);
        }

        // Charge Bar handling
        if (chargeBar == null)
        {
            chargeBar = playerUI.transform.Find("Battery Fill").GetComponent<Image>();
        }
        else
        {
            float percent = ((100 / maxBlastAmount) * blastAmount) / 100;
            chargeBar.fillAmount = percent;
        }
    }

    /// <summary>
    /// Saving this in case we add powerups back in the future 
    /// </summary>
    private void UpdateUIWithPowerups()
    {
        if (playerUI == null) { return; }

        // Set name for the powerup
        if (currentPowerup == null) { powerupName = ""; }
        else { powerupName = currentPowerup.powerupName; }

        // Debug.Log("currentPowerup for player " + playerName + " is: " + powerupName);

        // Update the UI for powerup 
        if (scoreText == null)
        {
            // Get text for powerup if we have not yet
            scoreText = playerUI.transform.Find("Powerup Label").GetComponent<TMPro.TextMeshProUGUI>();
        }
        else
        {
            // Debug.Log("Setting powerup text for: " + playerName + " with ID: " + playerId);
            scoreText.SetText(Constants.powerupLabel + "\n" + powerupName + "\n" + "Upgrades: " + upgradeCount);
        }

        // Charge Bar handling
        if (chargeBar == null)
        {
            chargeBar = playerUI.transform.Find("Battery Fill").GetComponent<Image>();
        }
        else
        {
            float percent = ((100 / maxBlastAmount) * blastAmount) / 100;
            chargeBar.fillAmount = percent;

        }
    }
    #endregion
}
