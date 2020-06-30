using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullCharacter : MonoBehaviour
{
    [SerializeField] private bool requireGrind = false;
    [SerializeField] private bool inAntiGravity = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCheck(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerCheck(collision);
    }

    private void PlayerCheck(Collider2D collision)
    {
        if (!collision.CompareTag(Constants.playerTag)) { return; }

        BSPlayerController player = collision.GetComponent<BSPlayerController>();

        if (requireGrind)
        {
            if (!player.isGrinding) { return; }
        }

        player.onJumpToRail.Invoke();
        MoveTransform(collision.transform);
    }

    private void MoveTransform(Transform trans)
    {
        // Get Capsule height
        CapsuleCollider2D col = trans.GetComponent<CapsuleCollider2D>();
        float height = col.size.y;

        // Get distance
        float currentDistance = trans.position.y - transform.position.y; ;

        // See if we need to account for angles
        if (transform.localEulerAngles.z == 0)
        {
            // Move er
            trans.position += new Vector3(0, height - currentDistance, 0) * (inAntiGravity ? -1 : 1);
        } else
        {
            // Figure out the xy with rotated platform
            float d = height - currentDistance;
            float x = Mathf.Cos(transform.localEulerAngles.z) * d;
            float y = Mathf.Sin(transform.localEulerAngles.z) * d;

            trans.position += new Vector3(x, y, 0) * (inAntiGravity ? -1 : 1);
        }
        
        
    }
}
