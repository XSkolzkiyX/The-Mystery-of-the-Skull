using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopScript : MonoBehaviour
{
    public int[] prices;
    public float[] maxStats;
    public Slider[] progressBars;
    public TextMeshProUGUI[] progressTexts;
    public Image[] progressImages;
    public GameObject tipText, upgradePrefab;
    public Animator shopAnimator;
    public PlayerController player;
    public Button[] buttons;
    public Text[] pricesTexts;
    public AudioSource powerUp;
    float[] stats = new float[3], minStats = new float[3], deltaStats = new float[3], steps = { 20, 10, 15 };
    float fireValue = 120;

    void Start()
    {
        minStats = FillStats(minStats);
        stats = FillStats(stats);
        for(int i = 0; i < deltaStats.Length; i++)
            deltaStats[i] = maxStats[i] - minStats[i];
    }
        
    float[] FillStats(float[] array)
    {
        array[0] = player.health;
        array[1] = player.damage;
        array[2] = player.fireRate;
        return array;
    }

    private void FixedUpdate()
    {
        if (stats[0] >= maxStats[0] || prices[0] > player.coins) buttons[0].interactable = false;
        else buttons[0].interactable = true;
        if (stats[1] >= maxStats[1] || prices[1] > player.coins) buttons[1].interactable = false;
        else buttons[1].interactable = true;
        if (stats[2] <= maxStats[2] || prices[2] > player.coins) buttons[2].interactable = false;
        else buttons[2].interactable = true;
    }

    public void Buy(int typeOfOperation)
    {
        powerUp.Play();
        switch(typeOfOperation)
        {
            case 0:
                player.health += steps[typeOfOperation];
                player.maxHealth += steps[typeOfOperation];
                player.healthText.text = ((int)player.health).ToString();
                ChangeProgress(typeOfOperation, player.maxHealth, player.maxHealth.ToString());
                Transaction(typeOfOperation, 10);
                break;
            case 1:
                player.damage += steps[typeOfOperation];
                ChangeProgress(typeOfOperation, player.damage, player.damage.ToString());
                Transaction(typeOfOperation, 15);
                break;
            case 2:
                player.fireRate -= steps[typeOfOperation] / 300.0f;
                fireValue += steps[typeOfOperation];
                ChangeProgress(typeOfOperation, fireValue, fireValue.ToString());
                Transaction(typeOfOperation, 20);
                break;
        }
    }

    void ChangeProgress(int typeOfOperation, float value, string text)
    {
        progressBars[typeOfOperation].value = value;
        progressTexts[typeOfOperation].text = text;
        progressImages[typeOfOperation].color = new Color(1f - ((stats[typeOfOperation] - minStats[typeOfOperation]) / deltaStats[typeOfOperation]), 1f, 0f);
        //progressImages[typeOfOperation].color = new Color(1f - ((stats[typeOfOperation] - minStats[typeOfOperation]) / deltaStats[typeOfOperation]), (stats[typeOfOperation] - minStats[typeOfOperation]) / deltaStats[typeOfOperation], 0f);
        GameObject curUpgarde = Instantiate(upgradePrefab, progressBars[typeOfOperation].transform.position, Quaternion.identity, transform);
        curUpgarde.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + steps[typeOfOperation];
        Destroy(curUpgarde, 1f);
    }

    void Transaction(int curIndex, int percentageRiseInPrice)
    {
        stats = FillStats(stats);
        player.coins -= prices[curIndex];
        player.coinText.text = player.coins.ToString();
        prices[curIndex] += prices[curIndex] / percentageRiseInPrice;
        if (stats[curIndex] == maxStats[curIndex]) pricesTexts[curIndex].text = "Max";
        else pricesTexts[curIndex].text = "" + prices[curIndex];
    }

    public void OpenShop()
    {
        if (!player.dead)
        {
            BaseScript.isShopOpened = !BaseScript.isShopOpened;
            //StartCoroutine(SetPause(PlayerController.needToHideTip ? 1f : 0.1f));
            PlayerController.needToHideTip = !PlayerController.needToHideTip;
            shopAnimator.SetBool("Open", !shopAnimator.GetBool("Open"));
        }
    }
}