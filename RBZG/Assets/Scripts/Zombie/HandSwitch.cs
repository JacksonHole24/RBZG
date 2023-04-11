using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSwitch : MonoBehaviour
{
    public BoxCollider leftHandCollider;
    public BoxCollider rightHandCollider; 
    
    void Start()
    {
        leftHandCollider.enabled = false;
        rightHandCollider.enabled = false;
    }

    public void EnableRightHand()
    {
        rightHandCollider.enabled = true;
    }

    public void DisableRightHand()
    {
        rightHandCollider.enabled = false;
    }

    public void EnableLeftHand()
    {
        leftHandCollider.enabled = true;
    }

    public void DisableLeftHand()
    {
        leftHandCollider.enabled = false;
    }
}
