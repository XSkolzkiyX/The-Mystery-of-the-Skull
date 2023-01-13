using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    public float speedOfBullet, fireRate;
    public Transform target;
    public GameObject Bullet;

    private void Start()
    {
        if (target)
        {
            Debug.Log("Shoot");
            Transform curBullet = Instantiate(Bullet, transform.position, Quaternion.identity).transform;
            curBullet.GetComponent<Rigidbody>().velocity = new Vector3((target.position.x - transform.position.x) * speedOfBullet, (target.position.y - transform.position.y) * speedOfBullet, (target.position.z - transform.position.z) * speedOfBullet);
            Destroy(curBullet, 10f);
        }
        Invoke("Start", fireRate);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Enemy") target = col.transform;
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.transform == target) target = null;
    }
}