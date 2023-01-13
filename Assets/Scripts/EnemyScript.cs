using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    public bool needToSpawn, convoyShip;
    public float health, speed, fireRate, speedOfBullet, damage;
    public GameObject chestPrefab, bulletPrefab, canvasPrefab, sinkingShipPrefab;
    public LevelController level;
    public PlayerController player;
    public Transform Path, target;
    bool canCallConvoy = true, needToShoot = false, dead = false;
    int curPoint = 0;
    float maxHealth;
    Rigidbody Rb;
    Transform canvas;
    TextMeshProUGUI healthText;
    //NavMeshAgent agent;

    void Awake()
    {
        maxHealth = health;
        Rb = GetComponent<Rigidbody>();
        //agent = GetComponent<NavMeshAgent>();
        level = GameObject.Find("Level").GetComponent<LevelController>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        Shoot();
        canvas = Instantiate(canvasPrefab).transform;
        healthText = canvas.GetChild(2).GetComponent<TextMeshProUGUI>();
        healthText.text = health.ToString();
        if (needToSpawn) transform.GetChild(0).GetComponent<Animator>().SetTrigger("Spawn");
    }

    void FixedUpdate()
    {
        if (!dead)
        {
            canvas.position = new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z);
            if (target)
            {
                if (level.doNotTouchPlayer) ForgetTarget();
                else if (!needToShoot)
                {
                    Quaternion rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
                    transform.rotation = rotation;

                    transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                    //new System
                    //Rb.velocity = Vector3.Normalize(target.position - transform.position) * speed * Time.deltaTime;
                    //agent.SetDestination(target.position);
                }
            }
            else
            {
                if (Mathf.RoundToInt(transform.position.x) == Mathf.RoundToInt(Path.GetChild(curPoint).position.x) && Mathf.RoundToInt(transform.position.z) == Mathf.RoundToInt(Path.GetChild(curPoint).position.z))
                {
                    if (curPoint >= Path.childCount - 1) curPoint = 0;
                    else curPoint++;
                }                           
                Quaternion rotation = Quaternion.LookRotation(Path.GetChild(curPoint).position - transform.position, Vector3.up);
                transform.rotation = rotation;

                transform.position = Vector3.MoveTowards(transform.position, Path.GetChild(curPoint).position, speed * Time.deltaTime);
                //new System
                //Rb.velocity = Vector3.Normalize(Path.GetChild(curPoint).position - transform.position) * speed * Time.deltaTime;
                //agent.SetDestination(Path.GetChild(curPoint).position);
            }
        }
    }
    
    void Shoot()
    {
        if (target && needToShoot && !dead)
        {
            Transform curBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity).transform;
            curBullet.GetComponent<Rigidbody>().velocity = Vector3.Normalize(target.position - transform.position) * speedOfBullet;
            //new Vector3((target.position.x - transform.position.x) * speedOfBullet, (target.position.y - transform.position.y) * speedOfBullet, (target.position.z - transform.position.z) * speedOfBullet);
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
            needToShoot = false;
            tag = "Untagged";
            GetComponent<BoxCollider>().enabled = false;
            transform.GetChild(0).GetComponent<Animator>().SetTrigger("Destroy");
            if(!convoyShip) Instantiate(chestPrefab, transform.position, transform.rotation);
            Destroy(canvas.gameObject);
            Destroy(gameObject, 1f);
        }
    }

    void ForgetTarget()
    {
        if(!convoyShip) target = null;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Bullet" && !level.doNotTouchPlayer)
        {
            target = player.transform;
            if (canCallConvoy)
            {
                canCallConvoy = false;
                level.SpawnConvoy(target);
            }
        }
        else if(col.tag == "Player")
        {
            needToShoot = true;
            Invoke("ForgetTarget", 3f);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            needToShoot = false;
        }
    }
}
