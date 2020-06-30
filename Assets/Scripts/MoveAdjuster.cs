using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAdjuster : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private string[] tagsToEffect;
    [SerializeField] private Vector2 exitAdditiveForce = Vector2.zero;

    [Header("Velocity")]
    [SerializeField] private bool addVelocity = false;
    [SerializeField] private Vector2 velocityToAdd = Vector2.one;

    private List<Rigidbody2D> collidedRBs = new List<Rigidbody2D>();
    private Dictionary<BSPlayerController, Rigidbody2D> players = new Dictionary<BSPlayerController, Rigidbody2D>();

    #region UnityCallbacks

    private void FixedUpdate()
    {
        foreach (BSPlayerController player in players.Keys)
        {
            if (players[player] && player.isGrinding)
            {
                if (addVelocity)
                    AddVelocity(players[player]);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (TagCheck(collision.gameObject))
        {
            BSPlayerController player = collision.gameObject.GetComponent<BSPlayerController>();

            if (!collision.rigidbody || !player) { return; }

            players[player] = collision.rigidbody;
            player.isOnRail = true;
            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.rigidbody && players.ContainsValue(collision.rigidbody))
        {
            BSPlayerController player = collision.gameObject.GetComponent<BSPlayerController>();
            player.onGoodGrindEnd.Invoke();
            player.isOnRail = false;
            players[player].AddForce(exitAdditiveForce);
            players.Remove(player);
        }
    }

    #endregion

    #region Private Functions

    private bool TagCheck(GameObject obj)
    {
        return Array.FindIndex(tagsToEffect, (x) => { return obj.CompareTag(x); }) != -1;
    }

    private void AddVelocity(Rigidbody2D rb)
    {
        rb.velocity += velocityToAdd;
    }

    #endregion
}
