using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurretScript : MonoBehaviour
{
    public float health, fireRate, speedOfBullet, damage;
    public GameObject bulletPrefab, chestPrefab, healthBar;
    public LevelController level;
    public Transform firePoint;
    public Animator turret;
    Transform target;
    float maxHealth;
    bool dead = false;

    private void Start()
    {
        maxHealth = health;
        turret.SetTrigger("Spawn");
        Shoot();
        healthBar = Instantiate(healthBar, new Vector3(transform.position.x, 4, transform.position.z), Camera.main.transform.rotation, transform.parent);
        healthBar.GetComponent<Slider>().value = health / maxHealth;
        healthBar.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = ((int)health).ToString();
    }

    void FixedUpdate()
    {
        if (target) transform.parent.GetChild(0).LookAt(target, Vector3.up);
    }

    void Shoot()
    {
        if (target && !dead)
        {
            Transform curBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity).transform;
            curBullet.GetComponent<Rigidbody>().velocity = Vector3.Normalize(target.position - transform.position) * speedOfBullet;
            //new Vector3((target.position.x - transform.position.x) * speedOfBullet, (target.position.y - transform.position.y) * speedOfBullet, (target.position.z - transform.position.z) * speedOfBullet);
            curBullet.GetComponent<BulletScript>().tagOfOwner = tag;
            curBullet.GetComponent<BulletScript>().damage = damage;
        }
        Invoke("Shoot", fireRate);
    }

    public void TakeDamage(float damage)
    {
        if (!dead)
        {
            health -= damage;
            healthBar.GetComponent<Slider>().value = health / maxHealth;
            healthBar.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = ((int)health).ToString();
            if (health <= 0) Die();
        }
    }

    void Die()
    {
        if (!dead)
        {
            dead = true;
            Destroy(healthBar);
            turret.SetTrigger("Destroy");
            Instantiate(chestPrefab, transform.position, Quaternion.identity);
            Destroy(transform.parent.gameObject, 1f);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && !level.doNotTouchPlayer) target = col.transform;
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player") target = null;
    }
}
