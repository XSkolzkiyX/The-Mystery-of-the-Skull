using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossScript : MonoBehaviour
{
    public bool needToSpawn;
    public byte typeOfBoss;
    public float health, damage, fireRate, speedOfBullet, speed, heightOfHealthBar, startHealth, startDamage;
    public PlayerController player;
    public Animator bossAnimator;
    public Transform firePoint, canvas, Paths, myPath, spawnPoints;
    public Transform[] firePoints;
    public LevelController level;
    public TextMeshProUGUI healthText;
    public GameObject bigBulletPrefab, chestPrefab, attackShipPrefab;
    public GameObject[] bosses;
    Transform target, Path;
    Rigidbody Rb;
    float maxHealth;
    byte curPoint = 0;
    bool dead = false;

    public void Awake()
    {
        dead = false;
        tag = "Enemy";
        if (typeOfBoss != 1) GetComponent<SphereCollider>().enabled = true;
        else GetComponent<BoxCollider>().enabled = true;
        health = (int)Random.Range(startHealth - 750, startHealth + 750);
        maxHealth = health;
        healthText.text = health.ToString();
        canvas.GetComponent<Slider>().value = health / maxHealth;
        damage = (int)Random.Range(startDamage - 15, startDamage + 15);
        Rb = GetComponent<Rigidbody>();
        if (needToSpawn) bossAnimator.SetTrigger("Spawn");
        if (typeOfBoss != 0)
        {
            Path = Paths.GetChild(Random.Range(0, Paths.childCount));
            for(int i = 0; i < 3 * typeOfBoss; i++)
            {
                Instantiate(attackShipPrefab, new Vector3(transform.position.x - 5f, 0, transform.position.z + (i * 15)),
                    transform.rotation).GetComponent<EnemyScript>().Path = myPath;
            }
        }
        Shoot();
    }

    void Update()
    {
        if (!dead)
        {
            canvas.position = new Vector3(transform.position.x, heightOfHealthBar, transform.position.z);
            switch (typeOfBoss)
            {
                case 0:
                    //Kraken
                    transform.GetChild(0).LookAt(player.transform, Vector3.up);
                    break;
                case 1:
                    //Ship
                    if (Mathf.RoundToInt(transform.position.x) == Mathf.RoundToInt(Path.GetChild(curPoint).position.x)
                        && Mathf.RoundToInt(transform.position.z) == Mathf.RoundToInt(Path.GetChild(curPoint).position.z))
                    {
                        if (curPoint >= Path.childCount - 1) curPoint = 0;
                        else curPoint++;
                    }
                    transform.position = Vector3.MoveTowards(transform.position, Path.GetChild(curPoint).position, speed * Time.deltaTime);
                    //Rb.velocity = Vector3.Normalize(Path.GetChild(curPoint).position - transform.position) * speed * Time.deltaTime;
                    Quaternion rotation = Quaternion.LookRotation(Path.GetChild(curPoint).position - transform.position, Vector3.up);
                    transform.rotation = rotation;
                    break;
                case 2:
                    //Fortress
                    if(target)
                    {
                        Vector3 delta = Vector3.Normalize(target.position - transform.position);
                        if (delta.x > 0 && delta.z > 0) firePoint = firePoints[0];
                        else if (delta.x < 0 && delta.z > 0) firePoint = firePoints[1];
                        else if (delta.x < 0 && delta.z < 0) firePoint = firePoints[2];
                        else firePoint = firePoints[3];
                    }
                    break;
                default:
                    Debug.LogError("There is no type of Boss");
                    break;
            }
        }
    }

    void Shoot()
    {
        if (target && !dead)
        {
            GameObject curBullet = Instantiate(bigBulletPrefab, firePoint.position, Quaternion.identity);
            curBullet.GetComponent<Rigidbody>().velocity = Vector3.Normalize(target.position - transform.position) * speedOfBullet;
            //new Vector3((target.position.x - firePoint.position.x) * speedOfBullet, (target.position.y - firePoint.position.y) * speedOfBullet, (target.position.z - firePoint.position.z) * speedOfBullet);
            curBullet.GetComponent<BulletScript>().tagOfOwner = tag;
            curBullet.GetComponent<BulletScript>().damage = damage;
        }
        Invoke("Shoot", fireRate);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        canvas.GetComponent<Slider>().value = health / maxHealth;
        healthText.text = health.ToString();
        if (health <= 0) Die();
    }

    void Die()
    {
        if (!dead)
        {
            dead = true;
            target = null;
            level.SafeZone();
            needToSpawn = true;
            bossAnimator.SetTrigger("Destroy");
            if (typeOfBoss != 1) GetComponent<SphereCollider>().enabled = false;
            else GetComponent<BoxCollider>().enabled = false;
            player.target = null;
            tag = "Untagged";
            StartCoroutine(Wait(1f));
            byte typeOfNewBoss, spawnPoint;
            do
            {
                spawnPoint = (byte)Random.Range(0, spawnPoints.childCount);
                typeOfNewBoss = (byte)Random.Range(0, bosses.Length);
            } while (typeOfNewBoss == typeOfBoss || spawnPoints.GetChild(spawnPoint).position == transform.position);
            bosses[typeOfNewBoss].transform.position = spawnPoints.GetChild(spawnPoint).position;
            bosses[typeOfNewBoss].SetActive(true);
            bosses[typeOfNewBoss].GetComponent<BossScript>().Awake();
            for (int i = 0; i < Random.Range(5, 15); i++)
            {
                Instantiate(chestPrefab, new Vector3(transform.position.x + (Random.Range(-5, 5)) * 2, 0,
                    transform.position.z + (Random.Range(-5, 5)) * 2), transform.rotation);
            }
        }
    }

    IEnumerator Wait(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player") target = collider.transform;
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player") target = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.name == "Player" && typeOfBoss == 0) player.TakeDamage(damage);
    }
}
