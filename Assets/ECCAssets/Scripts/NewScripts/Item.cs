using UnityEngine;

namespace CarGame
{
    /// <summary>
    /// Определяет поднимаемый объект игроком
    /// </summary>
    public class Item : MonoBehaviour
    {
        static GameController gameController;

        [Tooltip("Функция,которая запускается при косании с объектом")]
        public string touchFunction = "ChangeScore";

        [Tooltip("Параметр который будет передан с функцией")]
        public float functionParameter = 100;

        [Tooltip("Тег целевого объекта, с которого будет воспроизводиться функция")]
        public string functionTarget = "GameController";

        [Tooltip("Эффект поднятия")]
        public Transform pickupEffect;

        [Tooltip("Случайное значние поворота объекта")]
        public float randomRotation = 360;

        void Start()
        {
            // Установить случайный угол поворота
            transform.eulerAngles += Vector3.up * Random.Range(-randomRotation, randomRotation);

            if (gameController == null) gameController = GameObject.FindObjectOfType<GameController>();
        }

        /// <summary>
        /// При Косании коллайдера итема
        /// </summary>
        /// <param name="other"><see cref="Collider"/></param>
        void OnTriggerEnter(Collider other)
        {
            // Проверка на правильность тега объекта к которому было косание
            if (gameController.playerObject && other.gameObject == gameController.playerObject.gameObject)
            {
                // Проверка, что у нас есть целевой тег и имя функции перед запуском
                if (touchFunction != string.Empty)
                {
                    // Запуск функции
                    GameObject.FindGameObjectWithTag(functionTarget).SendMessage(touchFunction, functionParameter);
                }

                // Установка эффекта взятия
                if (pickupEffect) Instantiate(pickupEffect, transform.position, transform.rotation);

                // Удаление объекта
                Destroy(gameObject);
            }
        }
    }
}