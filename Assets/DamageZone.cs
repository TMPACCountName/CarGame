using CarGame;
using EndlessCarChase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{

    private float nextDamage; //Когда пройдет след урон
    public float hurtDelay; //Задержка урона 
    public float damage; 

    public Transform hitEffect;
    public float health;

    public Vector3 minSize; // Размер, до которого будет уменьшена зона
    public float shrinkSpeed; // Скорость сужения зоны, чем больше показатель, тем медленнее сужение

    //Сужение зоны//
    void Update()
    {
        shrinkSpeed -= 1.0f * Time.deltaTime;
        transform.localScale = Vector3.Lerp(transform.localScale, minSize, Time.deltaTime / shrinkSpeed) ;

    }

    //Урон от зоны//
    void OnTriggerStay(Collider other)
    {
     
        if (nextDamage <= Time.time)
        {
           
            other.GetComponent<Car>().ChangeHealth(-damage);
            nextDamage = Time.time + hurtDelay;
            
        }
    }   
}
