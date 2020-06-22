using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class Car : MonoBehaviour
    {

        //Быстрый доступ
        internal Transform thisTransform;
        static GameController gameController;
        static Transform targetPlayer;
        internal Vector3 targetPosition;

        //нужно для формул
        internal RaycastHit groundHitInfo;
        internal Vector3 groundPoint;
        internal Vector3 forwardPoint;
        internal float forwardAngle;
        internal float rightAngle;

        //индекс
        internal int index;

        //Для работы со здоровьем, если есть в префабе
        internal Transform healthBar;
        internal Image healthBarFill;

        [Header("Основные настройки")]
        [Tooltip("1 столкновение = 1 здоровье")]
        public float health = 10;
        internal float healthMax;

        [Tooltip("Урон от этой машинки при столкновении")]
        public int damage = 1;

        [Tooltip("Скорость")]
        public float speed = 10;

        [Tooltip("Скорость поворота")]
        public float rotateSpeed = 200;
        internal float currentRotation = 0;

        [Tooltip("Время неуязвимости после потери здоровья")]
        public float hurtDelay = 2;
        internal float hurtDelayCount = 0;

        [Tooltip("Цвет когда машина теряет здоровье")]
        public Color hurtFlashColor = new Color(0.5f, 0.5f, 0.5f, 1);

        [Tooltip("Эффект получения урона")]
        public Transform hitEffect;

        [Tooltip("Эффект смерти")]
        public Transform deathEffect;

        [Tooltip("Дрифт эффект")]
        public float driftAngle = 50;

        [Tooltip("Наклон при входе в поворот")]
        public float leanAngle = 10;

        [Tooltip("Объект шасси при наклоне")]
        public Transform chassis;

        [Tooltip("Колеса машины, объекты с компонентом сферой")]
        public Transform[] wheels;

        [Tooltip("сколько колес  вращать в сторону поворта машины")]
        public int frontWheels = 2;

        [Header("Настройки ИИ")]
        [Tooltip("Значение которое добовляется к базовой скорости ИИ для разнообразия")]
        public float speedVariation = 2;
        
        [Tooltip("Случайный угол позиции врага")]
        public Vector2 chaseAngleRange = new Vector2(0, 30);

        [Tooltip("True - боты избегали объекты Obstacle ")]
        public bool avoidObstacles = true;

        [Tooltip("Боковая детекция")]
        public float detectAngle = 2;

        [Tooltip("Передняя детекция")]
        public float detectDistance = 3;

        //Угол начала погони
        internal float chaseAngle;
        

        private void Start()
        {
            thisTransform = this.transform;

            // поиск объекта геймкотроля если его не передали
            if (gameController == null) gameController = GameObject.FindObjectOfType<GameController>();
            //если машинка не передана, то ее получаем
            if (targetPlayer == null && gameController.gameStarted == true && gameController.playerObject) targetPlayer = gameController.playerObject.transform;


            RaycastHit hit;

            if (Physics.Raycast(thisTransform.position + Vector3.up * 5 + thisTransform.forward * 1.0f, -10 * Vector3.up, out hit, 100, gameController.groundLayer)) 
                forwardPoint = hit.point;

            thisTransform.Find("Base").LookAt(forwardPoint);// + thisTransform.Find("Base").localPosition);

            // если этот объект не игрок
            if (gameController.playerObject != this)
            {
                // Случайный угол погони
                chaseAngle = Random.Range(chaseAngleRange.x, chaseAngleRange.y);

                // Установка рандомной скорости
                speed += Random.Range(0, speedVariation);
            }

            // Если есть панель здоровья, назначит ее
            if (thisTransform.Find("HealthBar"))
            {
                healthBar = thisTransform.Find("HealthBar");

                healthBarFill = thisTransform.Find("HealthBar/Empty/Full").GetComponent<Image>();
            }

            // Максимальное здоровье машины
            healthMax = health;

            // Update the health value
           // ChangeHealth(0);
        }

        //Фунеция когда мы меняем значение в компоненте
        private void OnValidate()
        {
            // Лимит передних колес фактиескими, что у нас есть
            frontWheels = Mathf.Clamp(frontWheels, 0, wheels.Length);
        }

        void Update()
        {
            // Если ничего не началось
            if (gameController && !gameController.gameStarted) return;

            // Едем вперед
            thisTransform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);

            // Получить текущую позицию игрока
            if (health > 0)
            {
                if (targetPlayer) targetPosition = targetPlayer.transform.position;

                if (healthBar) healthBar.LookAt(Camera.main.transform);
            }
            else
            {
                if (healthBar && healthBar.gameObject.activeSelf == true) healthBar.gameObject.SetActive(false);
            }

            // Управление для ИИ
            if (gameController.playerObject != this)
            {
                // Shoot a ray at the position to see if we hit something
                //Ray ray = new Ray(thisTransform.position + Vector3.up * 0.2f + thisTransform.right * Mathf.Sin(Time.time * 20) * detectAngle, transform.TransformDirection(Vector3.forward) * detectDistance);

                // Бросить 2 луча по бокам машины что бы боты могли увидеть препятствия
                Ray rayRight = new Ray(thisTransform.position + Vector3.up * 0.2f + thisTransform.right * detectAngle * 0.5f + transform.right * detectAngle * 0.0f * Mathf.Sin(Time.time * 50), transform.TransformDirection(Vector3.forward) * detectDistance);
                Ray rayLeft = new Ray(thisTransform.position + Vector3.up * 0.2f + thisTransform.right * -detectAngle * 0.5f - transform.right * detectAngle * 0.0f * Mathf.Sin(Time.time * 50), transform.TransformDirection(Vector3.forward) * detectDistance);

                RaycastHit hit;

                // Если препяттвие справа поворачиваем влево
                if (avoidObstacles == true && Physics.Raycast(rayRight, out hit, detectDistance) && (hit.transform.GetComponent<Obstacle>() || (hit.transform.GetComponent<Car>() && gameController.playerObject != this)))
                {
                    // Индикация
                    //if (hit.transform.GetComponent<MeshRenderer>() ) hit.transform.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.red);

                    // Поворот влево
                    Rotate(-1);

                    //obstacleDetected = 0.1f;
                }
                else if (avoidObstacles == true && Physics.Raycast(rayLeft, out hit, detectDistance) && (hit.transform.GetComponent<Obstacle>() || (hit.transform.GetComponent<Car>() && gameController.playerObject != this))) // Otherwise, if we detect an obstacle on our left side, swerve to the right
                {
                    // Индикация
                    //if (hit.transform.GetComponent<MeshRenderer>()) hit.transform.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.red);

                    // Поворот право
                    Rotate(1);

                    //obstacleDetected = 0.1f;
                }
                else// Если все окей то переследуем игрока
                {
                    // Вращаем машину пока она не достигнет желаемого угла погоди с обеих сторон игрока
                    if (Vector3.Angle(thisTransform.forward, targetPosition - thisTransform.position) > chaseAngle)
                    {
                        Rotate(ChaseAngle(thisTransform.forward, targetPosition - thisTransform.position, Vector3.up));
                        
                    }
                    else //Стоп вращения
                    {
                        Rotate(0);
                    }
                }
            }

            // Если нет назначенного наземного объекта или он выключен, то юзаем рейкст шоб ехать
            if (gameController.groundObject == null || gameController.groundObject.gameObject.activeSelf == false) 
                DetectGround();

            //if (obstacleDetected > 0) obstacleDetected -= Time.deltaTime;

            // Задержка во время которой автомобиль не получает урон
            if (hurtDelayCount > 0 && health > 0)
            {
                hurtDelayCount -= Time.deltaTime;

                // Делаем визуальную задержку на автомобиле, шоб было видно, что урон получен
                if (GetComponentInChildren<MeshRenderer>())
                {
                    foreach (Material part in GetComponentInChildren<MeshRenderer>().materials)
                    {
                        if (Mathf.Round(hurtDelayCount * 10) % 2 == 0) part.SetColor("_EmissionColor", Color.black);
                        else part.SetColor("_EmissionColor", hurtFlashColor);

                        //hurtFlashObject.material.SetColor("_EmissionColor", hurtFlashColor);
                    }
                }

            }
        }


        /// <summary>
        /// Вычисляет угол приближения объекта к другому объекту
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="targetDirection"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public float ChaseAngle(Vector3 forward, Vector3 targetDirection, Vector3 up)
        {
            //  Угол приближения
            float approachAngle = Vector3.Dot(Vector3.Cross(up, forward), targetDirection);

            // Если угол больше 0, мы подходим слева ( поэтому мы должны повернуть направо )
            if (approachAngle > 0f)
            {
                return 1f;
            }
            else if (approachAngle < 0f) // В противном случае, если угол меньше 0, мы подходим справа ( поэтому мы должны повернуть влево )
            {
                return -1f;
            }
            else // Не поворачиваем
            {
                return 0f;
            }
        }


        /// <summary>
        /// Поворачивает объект влево или право и применяет lean и  drift
        /// </summary>
        /// <param name="rotateDirection"></param>
        public void Rotate(float rotateDirection)
        {
            //thisTransform.localEulerAngles = new Vector3(Quaternion.FromToRotation(Vector3.up, groundHitInfo.normal).eulerAngles.x, thisTransform.localEulerAngles.y, Quaternion.FromToRotation(Vector3.up, groundHitInfo.normal).eulerAngles.z);

            //thisTransform.rotation = Quaternion.FromToRotation(Vector3.up, groundHitInfo.normal);


            // Дрифт и наклон при повороте
            if (rotateDirection != 0)
            {
                //thisTransform.localEulerAngles = Quaternion.FromToRotation(Vector3.up, groundHitInfo.normal).eulerAngles + Vector3.up * currentRotation;

                // Поворот автомобиля в зависимости от направления управления
                thisTransform.localEulerAngles += Vector3.up * rotateDirection * rotateSpeed * Time.deltaTime;

                thisTransform.eulerAngles = new Vector3(thisTransform.eulerAngles.x, thisTransform.eulerAngles.y, thisTransform.eulerAngles.z);

                //thisTransform.eulerAngles = new Vector3(rightAngle, thisTransform.eulerAngles.y, forwardAngle);

                currentRotation += rotateDirection * rotateSpeed * Time.deltaTime;

                if (currentRotation > 360) currentRotation -= 360;
                //print(forwardAngle);
                // Сделать основание автомобиля дрейфующим на основе угла поворота
                thisTransform.Find("Base").localEulerAngles = new Vector3(rightAngle, Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle + Mathf.Sin(Time.time * 50) * hurtDelayCount * 50, Time.deltaTime), 0);//  Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime);

                //Налон шасси
                if (chassis) chassis.localEulerAngles = Vector3.forward * Mathf.LerpAngle(chassis.localEulerAngles.z, rotateDirection * leanAngle, Time.deltaTime);//  Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime);

                //Анимация заноса, илил юбая другая
                GetComponent<Animator>().Play("Skid");

                // Проход по всем колесам и их вращение
                for (index = 0; index < wheels.Length; index++)
                {
                    // Поворот передних колес
                    if (index < frontWheels) 
                        wheels[index].localEulerAngles = Vector3.up * Mathf.LerpAngle(wheels[index].localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime * 10);

                    // Крутить колеса
                    wheels[index].Find("WheelObject").Rotate(Vector3.right * Time.deltaTime * speed * 20, Space.Self);
                }
            }
            else // Выпрямление
            {
                //Возврат угла в 0
                thisTransform.Find("Base").localEulerAngles = Vector3.up * Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, 0, Time.deltaTime * 5);
                
                //Возврат Шаси в угол 0
                if (chassis) chassis.localEulerAngles = Vector3.forward * Mathf.LerpAngle(chassis.localEulerAngles.z, 0, Time.deltaTime * 5);//  Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime);

                // Запуск анимации движения
                GetComponent<Animator>().Play("Move");

                // возврат колес
                for (index = 0; index < wheels.Length; index++)
                {
                    // Вернуть угол 0 колесам
                    if (index < frontWheels) wheels[index].localEulerAngles = Vector3.up * Mathf.LerpAngle(wheels[index].localEulerAngles.y, 0, Time.deltaTime * 5);

                    // Вращать их быстрее
                    wheels[index].Find("WheelObject").Rotate(Vector3.right * Time.deltaTime * speed * 30, Space.Self);
                }
            }
        }

        /// <summary>
        /// Для триггера коллайдера
        /// </summary>
        /// <param name="other"><see cref="Collider"/></param>
        void OnTriggerStay(Collider other)
        {
            //Повредить автомобиль если тот был стукнут
            if (hurtDelayCount <= 0 && other.GetComponent<Car>())
            {
               //Сброс задержки
                hurtDelayCount = hurtDelay;

                // Урон по машине
                other.GetComponent<Car>().ChangeHealth(-damage);

                // Если есть эффект урона
                if (health - damage > 0 && hitEffect) Instantiate(hitEffect, transform.position, transform.rotation);
            }
        }

        /// <summary>
        /// Работает с хп, смреть при 0 хп
        /// </summary>
        /// <param name="changeValue"></param>
        public void ChangeHealth(float changeValue)
        {
            // Изменить здоровье
            health += changeValue;

            // Максимальное зщачение
            if (health > healthMax) health = healthMax;

            // Обновления значения бара хп
            if (healthBar)
            {
                healthBarFill.fillAmount = health / healthMax;
            }

            // Анимация встраски
            
            if (changeValue < 0 && gameController.playerObject == this) Camera.main.transform.parent.GetComponent<Animation>().Play();

            // Смерть при <=0 хп
            if (health <= 0)
            {
                if (gameController.playerObject && gameController.playerObject != this)
                {
                    DelayedDie();
                }
                else
                {
                    Die();
                }

                //Смерть скрин
                if (gameController.playerObject && gameController.playerObject == this)
                {
                    gameController.SendMessage("GameOver", 1.2f);

                    // Play a slowmotion effect
                    Time.timeScale = 0.5f;
                }
            }

            // Обновление здоровья
            if (gameController.playerObject && gameController.playerObject == this && gameController.healthCanvas)
            {
                // Обновить здоровье
                if (gameController.healthCanvas.Find("Full")) gameController.healthCanvas.Find("Full").GetComponent<Image>().fillAmount = health / healthMax;

                if (gameController.healthCanvas.Find("Text")) gameController.healthCanvas.Find("Text").GetComponent<Text>().text = health.ToString();

                // Анимация значка здоровья
                if (gameController.healthCanvas.GetComponent<Animation>()) gameController.healthCanvas.GetComponent<Animation>().Play();
            }
        }

        /// <summary>
        /// Эффект смерти
        /// </summary>
        public void Die()
        {
            //Создает эффект смерти на позиции игрока
            if (deathEffect) Instantiate(deathEffect, transform.position, transform.rotation);

            // Удалить игрока
            Destroy(gameObject);
        }

        /// <summary>
        /// Потеря управления и убийство машинки
        /// </summary>
        public void DelayedDie()
        {
            //targetPlayer = null;

            // Коллеса как шасси прямо едут вместе
            for (index = 0; index < wheels.Length; index++)
            {
                wheels[index].transform.SetParent(chassis);
            }

            targetPosition = thisTransform.forward * -10;

            leanAngle = Random.Range(100, 300);

            driftAngle = Random.Range(100, 150); ;

            //rotateSpeed *= 2;

            Invoke("Die", Random.Range(0, 0.8f));
        }

        /// <summary>
        /// Определяет местность земли и выравнивает по ней
        /// </summary>
        public void DetectGround()
        {
            // Бростиь луч вниз
            Ray carToGround = new Ray(thisTransform.position + Vector3.up * 10, -Vector3.up * 20);

            // Detect terrain under the car
            if (Physics.Raycast(carToGround, out groundHitInfo, 20, gameController.groundLayer))
            {
                //transform.position = new Vector3(transform.position.x, groundHitInfo.point.y, transform.position.z);
            }

            // Установить авто вдоль рельефа местности
            thisTransform.position = new Vector3(thisTransform.position.x, groundHitInfo.point.y + 0.1f, thisTransform.position.z);

            RaycastHit hit;

            // Обнаружить точку местности чтобы можно было вращать авто 
            if (Physics.Raycast(thisTransform.position + Vector3.up * 5 + thisTransform.forward * 1.0f, -10 * Vector3.up, out hit, 100, gameController.groundLayer))
            {
                forwardPoint = hit.point;
            }
            else if (gameController.groundObject && gameController.groundObject.gameObject.activeSelf == true)
            {
                forwardPoint = new Vector3(thisTransform.position.x, gameController.groundObject.position.y, thisTransform.position.z);
            }

            // Заставить авто смотреть вдоль местности
            thisTransform.Find("Base").LookAt(forwardPoint);
        }


        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawRay(transform.position + Vector3.up * 0.2f + transform.right * detectAngle * 0.5f + transform.right * detectAngle * 0.0f * Mathf.Sin(Time.time * 50), transform.TransformDirection(Vector3.forward) * detectDistance);
            Gizmos.DrawRay(transform.position + Vector3.up * 0.2f + transform.right * -detectAngle * 0.5f - transform.right * detectAngle * 0.0f * Mathf.Sin(Time.time * 50), transform.TransformDirection(Vector3.forward) * detectDistance);

            Gizmos.DrawSphere(forwardPoint, 0.5f);
        }
    }
}