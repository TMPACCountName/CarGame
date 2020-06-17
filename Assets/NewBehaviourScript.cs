using EndlessCarChase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    

    private float nextDamage;
    public float DamDelay = 1;
    public float damage = 1f;


     void Start()
    {
        nextDamage = Time.time + DamDelay;
    }


    void OnTriggerStay(Collider other)
    {
        if (nextDamage <= Time.time)
        {
            nextDamage = Time.time + DamDelay;
            other.GetComponent<Standard>().ChangeHealth(-damage);
        }
    }   
}
