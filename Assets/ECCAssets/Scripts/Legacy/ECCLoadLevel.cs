using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EndlessCarChase
{
    /// <summary>
    /// Включает функции для загрузки уровней и URL-адресов. Он предназначен для использования с кнопками пользовательского интерфейса
    /// </summary>
    public class ECCLoadLevel : MonoBehaviour
    {
        [Tooltip("Ожидание перед загрузкой")]
        public float loadDelay = 1;

        [Tooltip("Юрл ссылка для загрузки")]
        public string urlName = "";

        [Tooltip("Название уровня")]
        public string levelName = "";

        [Tooltip("Музыкуа играющая при перезагрузке")]
        public AudioClip soundLoad;

        [Tooltip("Тег откуда воспроизводить взук")]
        public string soundSourceTag = "Sound";

        [Tooltip("Исходный источник звука")]
        public GameObject soundSource;

        [Tooltip("Название анимации которую мы юзаем когда грузим уровень")]
        public string loadAnimation;

        [Tooltip("Эффект перехода")]
        public Transform transition;

        [Tooltip("Перегружать по клику?")]
        public bool loadOnClick = false;

        void Start()
        {
            // If there is no sound source assigned, try to assign it from the tag name
            //if (!soundSource && GameObject.FindGameObjectWithTag(soundSourceTag)) soundSource = GameObject.FindGameObjectWithTag(soundSourceTag);

            if (loadOnClick == true) GetComponent<Button>().onClick.AddListener(LoadLevel);

        }


        /// <summary>
        /// Грузить юрл
        /// </summary>
        /// <param name="urlName">URL/URI</param>
        public void LoadURL()
        {
            Time.timeScale = 1;

            // Звук
            if (soundLoad && soundSource) soundSource.GetComponent<AudioSource>().PlayOneShot(soundLoad);

            // Выполнить функцию
            Invoke("ExecuteLoadURL", loadDelay);
        }

        /// <summary>
        /// Открыть юрл 
        /// </summary>
        void ExecuteLoadURL()
        {
            Application.OpenURL(urlName);
        }

        /// <summary>
        /// Загрузить уровень
        /// </summary>
        /// <param name="levelName">Level name.</param>
        public void LoadLevel()
        {
            Time.timeScale = 1;

            // музыка
            if (soundSource && soundLoad) soundSource.GetComponent<AudioSource>().PlayOneShot(soundLoad);

            if (transition) Invoke("ShowTransition", loadDelay - 1);

            // Загрузка
            Invoke("ExecuteLoadLevel", loadDelay);
        }

        public void ShowTransition()
        {
            Instantiate(transition);
        }

        /// <summary>
        /// Загрузить лвл
        /// </summary>
        void ExecuteLoadLevel()
        {
            SceneManager.LoadScene(levelName);
        }

        /// <summary>
        /// Рестар текущего уровня
        /// </summary>
        public void RestartLevel()
        {
            Time.timeScale = 1;
            if (soundSource && soundLoad) soundSource.GetComponent<AudioSource>().PlayOneShot(soundLoad);
            if (transition) Instantiate(transition);
            Invoke("ExecuteRestartLevel", loadDelay);
        }

        /// <summary>
        /// Загрузка уровня функция
        /// </summary>
        void ExecuteRestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}