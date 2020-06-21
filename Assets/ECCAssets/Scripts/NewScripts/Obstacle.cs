using UnityEngine;

namespace CarGame
{
    /// <summary>
    /// Класс определяющий объекты, которые можно поднять
    /// </summary>
    public class Obstacle : MonoBehaviour
    {
        [Tooltip("Ущерб препятствием")]
        public float damage = 1;

        [Tooltip("Эффект в месте его расположении при его попадании")]
        public Transform hitEffect;

        [Tooltip("Должно ли это припятствие быть устранено при столкновении с авто?")]
        public bool removeOnHit = false;

        [Tooltip("Случайное вращение по оси Y")]
        public float randomRotation = 360;

        void Start()
        {
            //Устанвока рандомного угла
            transform.eulerAngles += Vector3.up * Random.Range(-randomRotation, randomRotation);

            // цвет препятствия
            //InvokeRepeating("ResetColor", 0, 0.5f);
        }

        /// <summary>
        /// при касании срабатывает
        /// </summary>
        /// <param name="other"><see cref="Collider"/></param>
        void OnTriggerStay(Collider other)
        {
            // Если режим после стука закончился то работаем
            if (other.GetComponent<Car>())
            {
                //if ( other.GetComponent<Car>().hurtDelayCount <= 0 )
                //{
                // Reset the hurt delay
                //other.GetComponent<Car>().hurtDelayCount = other.GetComponent<Car>().hurtDelay;

                // Урон машине
                other.GetComponent<Car>().ChangeHealth(-damage);


                if (other.GetComponent<Car>().health - damage > 0 && other.GetComponent<Car>().hitEffect) Instantiate(other.GetComponent<Car>().hitEffect, transform.position, transform.rotation);
                //}

                // Эффект попадание
                if (hitEffect) Instantiate(hitEffect, transform.position, transform.rotation);

                // Удалить объект
                if (removeOnHit == true) Destroy(gameObject);
            }
        }

        public void ResetColor()
        {
            GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
        }
    }
}