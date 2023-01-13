using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseScript : MonoBehaviour
{
    public GameObject shopButton, shopPanel;
    public LevelController level;
    public static bool isShopOpened = false;

    private void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player")
        {
            level.doNotTouchPlayer = true;
            shopButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player" && !isShopOpened)
        {
            level.doNotTouchPlayer = false;
            shopButton.SetActive(false);
        }
    }
}
