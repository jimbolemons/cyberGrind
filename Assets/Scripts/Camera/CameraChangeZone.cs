using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChangeZone : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Constants.cameraWallTag))
        {
            BSGameManager.instance.mainCameraFollow.StartMoveAndScale(transform.localScale.y / 2, transform.position.y);
        }
    }
}
