using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            PlayerController.Instance.isGround = true;
            PlayerController.Instance.isJump = false;
            PlayerController.Instance.playerAnimator.SetBool("IsGround", PlayerController.Instance.isGround);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            PlayerController.Instance.isGround = false;
            if(!PlayerController.Instance.isJump)
            {
                PlayerController.Instance.jumpDirection.y= 0;
            }
        }
    }
}
