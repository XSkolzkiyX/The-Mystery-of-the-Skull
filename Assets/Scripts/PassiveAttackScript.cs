using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveAttackScript : MonoBehaviour
{
    public float damage;

    private void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player") col.GetComponent<PlayerController>().TakeDamage(Time.deltaTime * damage);
    }
}
