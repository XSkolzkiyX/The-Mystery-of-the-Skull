using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulletScript : MonoBehaviour
{
    public float damage;
    public string tagOfOwner;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.tag != tagOfOwner)
        {
            if(col.tag == "Player")
            {
                col.GetComponent<PlayerController>().TakeDamage(damage);
                Destroy(gameObject);
            }
            else if(col.tag == "Enemy")
            {
                if (col.TryGetComponent<EnemyScript>(out EnemyScript enemy))
                {
                    enemy.TakeDamage(damage);
                }
                else if (col.TryGetComponent<BossScript>(out BossScript boss))
                {
                    boss.TakeDamage(damage);
                }
                else if (col.TryGetComponent<TurretScript>(out TurretScript turret))
                {
                    turret.TakeDamage(damage);
                }
                Destroy(gameObject);
            }
        }
    }
}
