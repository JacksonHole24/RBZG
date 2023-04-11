using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ammoCount;

    [SerializeField] TextMeshProUGUI pickupText;

    [SerializeField] TextMeshProUGUI roundText;

    [SerializeField] GameObject crosshair;

    public void UpdateAmmoCount(int clip, int stash)
    {
        ammoCount.text = clip + " // " + stash;
    }

    public void UpdateCrosshair(bool onOff)
    {
        crosshair.SetActive(onOff);
    }

    public void UpdatePickUpText(bool onOff)
    {
        pickupText.gameObject.SetActive(onOff);
    }

    public void UpdateRoundText(int round)
    {
        roundText.text = round.ToString();
    }
}
