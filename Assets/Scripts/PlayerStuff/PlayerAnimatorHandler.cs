using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorHandler
{
    public enum PlayerState
    {
        Idle,
        Grinding,
        Running,
        Charging,
        JumpToRail,
        Releasing,
        Slowed,
        Win
    }

    public PlayerState playerState
    {
        get { return _playerState; }

        set
        {
            if (value == _playerState) { return; }

            Debug.Log("Setting player state to: " + value.ToString());

            switch (value)
            {
                case PlayerState.Idle:
                    anim.SetTrigger("Idle");
                    _playerState = value;
                    break;
                case PlayerState.Grinding:
                    anim.SetTrigger("Grind");
                    _playerState = value;
                    break;
                case PlayerState.Running:
                    if (_playerState == PlayerState.Charging) 
                    { break; }
                    anim.SetTrigger("Run");
                    _playerState = value;
                    break;
                case PlayerState.Charging:
                    anim.SetTrigger("Charge");
                    _playerState = value;
                    break;
                case PlayerState.JumpToRail:
                    anim.SetTrigger("JumpToRail");
                    _playerState = value;
                    break;
                case PlayerState.Releasing:
                    anim.SetTrigger("Release");
                    _playerState = value;
                    break;
                case PlayerState.Slowed:
                    anim.SetTrigger("Slowed");
                    _playerState = value;
                    break;
                case PlayerState.Win:
                    anim.SetTrigger("Win");
                    _playerState = value;
                    break;
            }
        }
    }

    private PlayerState _playerState;
    private Animator anim;

    public PlayerAnimatorHandler(BSPlayerController playerController, Animator animator)
    {
        anim = animator;

        playerController.onChargeStart.AddListener(() => { playerState = PlayerState.Charging; });
        playerController.onBlast.AddListener(() => { playerState = PlayerState.Releasing;});
        playerController.onGrindStart.AddListener(() => { playerState = PlayerState.Grinding; });
        playerController.onGrindEnd.AddListener(() => { playerState = PlayerState.Running; });
        playerController.onJumpToRail.AddListener(() => { playerState = PlayerState.JumpToRail; });
        playerController.onCountdownStart.AddListener(() => { playerState = PlayerState.Idle; });
        playerController.onCountdownEnd.AddListener(() => { playerState = PlayerState.Running; });
        playerController.onWin.AddListener(() => { playerState = PlayerState.Win; });

        // Got rid of this since countdown listener handles it now 
        // playerState = PlayerState.Running;
    }

    public void SetGrounded(bool isGrounded)
    {
        anim.SetBool("isGrounded", isGrounded);
    }
}
