using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int coins = 0;
    public float speed, scaler, fireRate, speedOfBullet, damage, health = 100, maxHealth;
    public FloatingJoystick movementJoystick;
    public Transform Cannon, target, canvas;
    public GameObject bulletPrefab, coinAnim, radiusImage, sinkingShipPrefab, playerCanvas, tipText;
    public Slider slider;
    public Text coinText;
    public TextMeshProUGUI healthText;
    public AudioSource pickUpCoinSound, ExplosionSound, HitSound, ShootSound;
    public Animator crossFade;
    public LevelController level;
    public bool dead = false;
    public static bool needToHideTip = false;
    Rigidbody Rb;

    private void Start()
    {
        Rb = GetComponent<Rigidbody>();
        Shoot();
        maxHealth = health;
        healthText.text = ((int)health).ToString();
    }

    private void FixedUpdate()
    {
        if (!dead)
        {
            Camera.main.transform.position = new Vector3(Mathf.Clamp(transform.position.x, -150, 150), scaler,
                Mathf.Clamp(transform.position.z - scaler, -150, 150));
            slider.transform.position = new Vector3(transform.position.x, slider.transform.position.y, transform.position.z);
            Rb.velocity = ((Vector3.forward * movementJoystick.Vertical) + (Vector3.right * movementJoystick.Horizontal)) * Time.deltaTime * speed;
            if (transform.position.x > 200 || transform.position.x < -200 || transform.position.z > 200 || transform.position.z < -200) Die();
            if (movementJoystick.Vertical != 0 || movementJoystick.Horizontal != 0)
            {
                tipText.SetActive(false);
                Rotate((Vector3.forward * movementJoystick.Vertical) + (Vector3.right * movementJoystick.Horizontal), transform);
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
            else
            {
                if (needToHideTip) tipText.SetActive(false);
                else tipText.SetActive(true);
            }
            if (Mathf.Abs(movementJoystick.Vertical) > Mathf.Abs(movementJoystick.Horizontal))
                scaler = Mathf.Clamp(Mathf.Abs(movementJoystick.Vertical) * 25, 10, 15);
            else scaler = Mathf.Clamp(Mathf.Abs(movementJoystick.Horizontal) * 25, 10, 15);
        }
    }

    public void TakeDamage(float damage)
    {
        if(!HitSound.isPlaying) HitSound.Play();
        health -= damage;
        slider.value = health/maxHealth;
        healthText.text = ((int)health).ToString();
        if (health <= 0) Die();
    }

    public void Die()
    {
        if (!dead)
        {
            level.SafeZone();
            canvas.gameObject.SetActive(false);
            ExplosionSound.Play();
            dead = true;
            playerCanvas.SetActive(false);
            transform.GetChild(0).GetComponent<Animator>().SetTrigger("Destroy");
            transform.GetChild(1).gameObject.SetActive(false);
            StartCoroutine(Restart());
        }
    }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(3f);
        crossFade.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
    }

    private void Rotate(Vector3 rotate, Transform objectOfRotation)
    {
        Quaternion rotation = Quaternion.LookRotation(rotate, Vector3.up);
        objectOfRotation.rotation = rotation;
    }

    private void Shoot()
    {
        if (target)
        {
            radiusImage.SetActive(true);
            ShootSound.Play();
            Transform curBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity).transform;
            curBullet.GetComponent<Rigidbody>().velocity = Vector3.Normalize(target.position - transform.position) * speedOfBullet;
                //new Vector3((target.position.x - transform.position.x) * speedOfBullet, (target.position.y - transform.position.y) * speedOfBullet, (target.position.z - transform.position.z) * speedOfBullet);
            curBullet.GetComponent<BulletScript>().tagOfOwner = tag;
            curBullet.GetComponent<BulletScript>().damage = damage;
        }
        else
        {
            radiusImage.SetActive(false);
            if (health < maxHealth)
            {
                health += 3;
                slider.value = health / maxHealth;
                healthText.text = ((int)health).ToString();
            }
        }
        Invoke("Shoot", fireRate);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Chest")
        {
            pickUpCoinSound.Play();
            col.GetComponent<Animator>().SetTrigger("Destroy");
            coins += Random.Range(15, 50);
            coinText.text = "" + coins;
            col.tag = "Untagged";
            Destroy(col.gameObject, 4f);
            Destroy(Instantiate(coinAnim, canvas), 1f);
        }
        else if (col.tag == "Treasure")
        {
            col.GetComponent<Animator>().SetTrigger("Destroy");
            for (int i = 0; i < level.treasureIndex.Length; i++)
                if (level.treasureIndex[i] == byte.Parse(col.name)) level.treasureIndex[i] = 0;
            level.GenerateChest(col.transform.GetChild(0).position, col.transform.rotation);
            Destroy(col.gameObject, 2f);
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.tag == "Enemy" && !target) { target = col.transform; }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.transform == target) { target = null; }
    }
}
