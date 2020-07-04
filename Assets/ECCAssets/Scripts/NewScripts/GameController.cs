using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace CarGame
{

    public class GameController : MonoBehaviour
    {
        [Tooltip("Камера следующая за игроком")]
        public Transform cameraHolder;
        internal float cameraFieldOfView = 60f;
        // Содержит миникарту
        internal Transform miniMap;

        // Поворот Камеры
        internal float cameraRotate = 0;

        [Tooltip("Объект машинки игрока")]
        public Car playerObject;
        internal float playerDirection;

        [Tooltip("Пол")]
        public Transform groundObject;

        [Tooltip("Выбор слоя для езды машинки")]
        public LayerMask groundLayer;

        [Tooltip("Скорость движения текстуры наземного объекта, будто он движется")]
        public float groundTextureSpeed = -0.1f;

        [Tooltip("Время ожидания прежде чем начать игру")]
        public float startDelay = 0.5f;
        internal bool gameStarted = false;

        [Tooltip("Префаб ReadyGoEffect ")]
        public Transform readyGoEffect;

        [Tooltip("Очки юзера")]
        public int score = 0;

        [Tooltip("Сколько начислять очков в секунду")]
        public int scorePerSecond = 1;

        [Tooltip("Текстовый объект для отображения очков")]
        public Transform scoreText;

        [Tooltip("Денежный знак")]
        public string scoreTextSuffix = "$";

        [Tooltip("Общий счет")]
        public string moneyPlayerPrefs = "Money";

        internal int highScore = 0;
        internal int scoreMultiplier = 1;

        [Tooltip("Скрипт магазина")]
        public Shop shopMenu;

        [Tooltip("Скрипт джойстика")]
        public GameObject Joystick;
        internal Stick stick;
       

        [Header("Канвасы для UI")]
        public Transform gameCanvas;
        public Transform healthCanvas;
        public Transform pauseCanvas;
        public Transform gameOverCanvas;
        public Transform menuCanvas;


        // Закончена ли игра
        internal bool isGameOver = false;

        [Tooltip("Названия для главной сцены")]
        public string mainMenuLevelName = "StartMenu";

        [Tooltip("Звук конца игры")]
        public AudioClip soundGameOver;

        [Tooltip("Тег для звуков")]
        public string soundSourceTag = "Sound";
        internal GameObject soundSource;

        //Название кнопки в инпуте для подтверждения
        internal string confirmButton = "Submit";

        // Название кнопки в инпуте для паузы
        internal string pauseButton = "Cancel";
        internal bool isPaused = false;
        

        // главный индекс
        internal int index = 0;

        // Хуйня для вращения вареника
        internal Vector2 gameArea = new Vector2(10, 10);
        internal bool wrapAroundGameArea = false;
        

        void Awake()
        {
            Time.timeScale = 1;

            if (shopMenu)
            {
                //Получаем номер текущего элемента 
                shopMenu.currentItem = PlayerPrefs.GetInt(shopMenu.currentPlayerprefs, shopMenu.currentItem);

                // Update the player object based on the shop car we have selected
                playerObject = shopMenu.items[shopMenu.currentItem].itemIcon.GetComponent<Car>();
            }

        }
        void Start()
        {
            //Application.targetFrameRate = 30;
            shopMenu.SetUIMoney();
            //Стартануть 0 очков
            ChangeScore(0);

            //Скрыть все канвасы
            if (shopMenu) shopMenu.gameObject.SetActive(false);
            if (gameOverCanvas) gameOverCanvas.gameObject.SetActive(false);
            if (pauseCanvas) pauseCanvas.gameObject.SetActive(false);
            if (gameCanvas) gameCanvas.gameObject.SetActive(false);
            if (menuCanvas) menuCanvas.gameObject.SetActive(true);
            if (Joystick)
            {
                //Дадим стику стик для дальнейшего управления
                stick = Joystick.GetComponent<Stick>();
                Joystick.gameObject.SetActive(false);
            }
            //Установка самого большого балла игрока
            highScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "HighScore", 0);

            //Назначение источника звука для упрощения доступа
            if (GameObject.FindGameObjectWithTag(soundSourceTag)) soundSource = GameObject.FindGameObjectWithTag(soundSourceTag);
            Debug.Log(playerObject.speed);
        }

        public void StartGame()
        {
            gameStarted = false;
            // Спавним тачку
            if (playerObject)
            {
                // Создание объекта на сцене
                playerObject = Instantiate(playerObject);

                //Тег для для игрока
                playerObject.tag = "Player";
            }

            // Тру для спавна объектов
            if (GetComponent<SpawnAroundObject>()) GetComponent<SpawnAroundObject>().isSpawning = true;

            // Показать игровое меню
            if (gameCanvas) gameCanvas.gameObject.SetActive(true);

            // Создание ReadyGo эффекта
            if (readyGoEffect) Instantiate(readyGoEffect);

            // Игра была начата
            gameStarted = true;

            // Увеличивать скор игрока каждую секунду
            if (scorePerSecond > 0) InvokeRepeating("ScorePerSecond", startDelay, 1);

            //показать джойстик на телефоне
            if (Joystick && Application.isMobilePlatform)
            {
                Joystick.gameObject.SetActive(true);
            }
            
        }

        void Update()
        {
            //Если игра не начата, то всё
            if (!gameStarted) return;

            // Задержка начала игры
            if (startDelay > 0)
            {
                startDelay -= Time.deltaTime;
            }
            else
            {
                //Если проиграл, то дает возможность рестартнуть или выйти в меню при нажатие на кнопку
                if (isGameOver)
                {
                    if (Input.GetButtonDown(confirmButton))
                    {
                        Restart();
                    }

                    if (Input.GetButtonDown(pauseButton))
                    {
                        MainMenu();
                    }
                }
                else
                {
                    // Если есть игрок то играем
                    if (playerObject)
                    {
                        
                        float direction = Input.GetAxis("Vertical");
                        // Если мобилка, то юзаем фичи для управления в зависимости от бока экрана
                        if (stick && Application.isMobilePlatform)
                        {
                            direction = stick.GetVertical();
                            if (stick)
                            {
                                playerDirection = stick.GetHorizontal();
                                playerObject.Drive(direction);
                            }
                            else
                            {
                                playerDirection = 0f;
                            }
                        }
                        else // Геймпад или клава
                        {
                            playerDirection = Input.GetAxis("Horizontal");
                            playerObject.Drive(direction);
                        }
                        
                        if (direction > 0)
                            playerObject.Rotate(playerDirection,false);
                        if (direction < 0)
                            playerObject.Rotate(-playerDirection, true); ;

                        // Возврат на зону
                        if (wrapAroundGameArea)
                        {
                            if (playerObject.transform.position.x > gameArea.x * 0.5f) playerObject.transform.position -= Vector3.right * gameArea.x;
                            if (playerObject.transform.position.x < gameArea.x * -0.5f) playerObject.transform.position += Vector3.right * gameArea.x;
                            if (playerObject.transform.position.z > gameArea.y * 0.5f) playerObject.transform.position -= Vector3.forward * gameArea.y;
                            if (playerObject.transform.position.z < gameArea.y * -0.5f) playerObject.transform.position += Vector3.forward * gameArea.y;
                        }
                    }

                    //Переключение между паузой и не паузой
                    if (Input.GetButtonDown(pauseButton))
                    {
                        if (isPaused == true) Unpause();
                        else Pause(true);
                    }
                }
            }
        }

        void LateUpdate()
        {
            if (playerObject)
            {
                if (cameraHolder)
                {
                    // Камера следует за игроком
                    cameraHolder.position = playerObject.transform.position;
                    cameraHolder.rotation = playerObject.transform.rotation;

                    // Камера вращается за игроком
                    if (cameraRotate > 0)
                        cameraHolder.eulerAngles = Vector3.up * Mathf.LerpAngle(cameraHolder.eulerAngles.y, playerObject.transform.eulerAngles.y, Time.deltaTime * cameraRotate);
                }

                // Миникарта следует за игроком
                if (miniMap) miniMap.position = playerObject.transform.position;

                //Если есть гроунд объект, тоделаем его UV карту и перемещающийся в зависимости от игрока
                if (groundObject)
                {
                    // Наземный объект следует за игроком
                    groundObject.position = playerObject.transform.position;

                    // Обновление UV текстуры в ависимости от игрока
                    groundObject.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(playerObject.transform.position.x, playerObject.transform.position.z) * groundTextureSpeed);
                }
            }
        }

        /// <summary>
        /// Изменение здоровья игрока
        /// </summary>
        /// <param name="changeValue"></param>
        public void ChangeHealth(float changeValue)
        {
            if (playerObject) playerObject.ChangeHealth(changeValue);
        }

        /// <summary>
        /// Изменение счета и обновление его
        /// </summary>
        /// <param name="changeValue">Change value</param>
        public void ChangeScore(int changeValue)
        {
            // Увеличть счет
            score += changeValue;

            //Обновить текстовое поле очков
            if (scoreText)
            {
                scoreText.GetComponent<Text>().text = score.ToString();

                // Анимация объекта score
                if (scoreText.GetComponent<Animation>()) scoreText.GetComponent<Animation>().Play();
            }
        }

        /// <summary>
        /// Множитель баллов, можно увеличивать балл за  попадание и уничтожение целей
        /// </summary>
        void SetScoreMultiplier(int setValue)
        {
            // Установить множитель баллов
            scoreMultiplier = setValue;
        }

        /// <summary>
        /// Пассивное добавление баллов
        /// </summary>
        public void ScorePerSecond()
        {
            ChangeScore(scorePerSecond);
        }

        /// <summary>
        /// Пауза и отображение паузы
        /// </summary>
        /// <param name="showMenu">If set to <c>true</c> show menu.</param>
        public void Pause(bool showMenu)
        {
            isPaused = true;
            //Установить время 0 шоб ничего не происходило
            Time.timeScale = 0;

            //Работа со скринами
            if (showMenu == true)
            {
                if (pauseCanvas) pauseCanvas.gameObject.SetActive(true);
                if (gameCanvas) gameCanvas.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Прдолжить игру
        /// </summary>
        public void Unpause()
        {
            isPaused = false;

            //Установить 1 шоб все происходило
            Time.timeScale = 1;
            if(pauseCanvas) pauseCanvas.gameObject.SetActive(false);

            if (menuCanvas && menuCanvas.gameObject.activeSelf) 
                menuCanvas.gameObject.SetActive(true);
            else if (shopMenu && shopMenu.gameObject.activeSelf) 
                shopMenu.gameObject.SetActive(true);
            else
            {
                if (gameCanvas) gameCanvas.gameObject.SetActive(true);
            }

            
        }

        /// <summary>
        /// Запуск события геймовер и показ этого же экрана
        /// </summary>
        IEnumerator GameOver(float delay)
        {
            isGameOver = true;

            yield return new WaitForSeconds(delay);

            //Убрать экран паузы и игры
            if (pauseCanvas) pauseCanvas.gameObject.SetActive(false);
            if (gameCanvas) gameCanvas.gameObject.SetActive(false);

            //Получить деньги
            int totalMoney = PlayerPrefs.GetInt(moneyPlayerPrefs, 0);

            //Добавить бабло которое мы получили в игре
            totalMoney += score;

            //Сохранение того шо вышло
            PlayerPrefs.SetInt(moneyPlayerPrefs, totalMoney);

            //Показать гейм овер скрин
            if (gameOverCanvas)
            {
                gameOverCanvas.gameObject.SetActive(true);

                //Написать в текстовое поле TextScore
                gameOverCanvas.Find("Base/TextScore").GetComponent<Text>().text = "SCORE " + score.ToString();

                //Проверка на лучший счет
                if (score > highScore)
                {
                    highScore = score;

                    //Установить лучший счет
                    PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "HighScore", score);
                }

                //Написать лучший счет
                gameOverCanvas.Find("Base/TextHighScore").GetComponent<Text>().text = "HIGH SCORE " + highScore.ToString();

                //Если есть источник и звук то воспроизвести из него
                if (soundSource && soundGameOver)
                {
                    soundSource.GetComponent<AudioSource>().pitch = 1;

                    soundSource.GetComponent<AudioSource>().PlayOneShot(soundGameOver);
                }
            }
        }

        /// <summary>
        /// Рестарт сцены
        /// </summary>
        void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Выход в маин меню
        /// </summary>
        void MainMenu()
        {
            SceneManager.LoadScene(mainMenuLevelName);
        }

        void OnDrawGizmos()
        {
            //Gizmos.color = Color.blue;

            // Draw two lines to show the edges of the street
            //Gizmos.DrawWireCube(Vector3.zero, new Vector3(gameArea.x, 1, gameArea.y));
        }
    }
}
