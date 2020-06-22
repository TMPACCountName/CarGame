using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace CarGame
{
    /// <summary>
    /// Переключение источника звука при нажатии. Он также записывает состояние звука (вкл / выкл) в PlayerPrefs.
    /// Для обнаружения кликов  необходимо прикрепить этот скрипт к кнопке пользовательского интерфейса и установить соответствующее событие OnClick ().
    /// </summary>
    public class AudioControl : MonoBehaviour
    {
        [Header("Звуки")]
        [Tooltip("Слайдер управления звуком")]
        public Slider soundSlider;

        [Tooltip("Картинка которую нужно затемнить или поменять при смене on off звука")]
        public Image soundImage;

        [Tooltip("Текстовое именования для определения Сохранения громкости звуков")]
        public string soundSaveName = "soundSave";

        [Header("Музыка")]
        [Tooltip("Слайдер управления музыкой")]
        public Slider musicSlider;

        [Tooltip("Картинка которую нужно затемнить или поменять при смене on off музыки")]
        public Image musicImage;

        [Tooltip("Текстовое именования для определения сохранения громкости музыки")]
        public string musicSaveName = "musicSave";

        // Громкость всех звуков, кроме музыки
        static float currentSoundVolume = 1;
        // Громкость музыки
        static float currentMusicVolume = 1;



        void Awake()
        {
            // Отделение громкости музыки от других звуков
            GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().ignoreListenerVolume = true;

            // Получение громкости музыки из PlayerPrefs;
            currentMusicVolume = PlayerPrefs.GetFloat(musicSaveName, currentMusicVolume);

            // Установить громкость музыки
            GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().volume = currentMusicVolume;

            // Получить громкость звуков
            currentSoundVolume = PlayerPrefs.GetFloat(soundSaveName, currentSoundVolume);

            // Установить громкость звуков кроме музыки
            AudioListener.volume = currentSoundVolume;

            if (transform.Find("Text"))
            {
                if (transform.Find("Text").GetComponent<Text>().text.Contains("MUSIC"))
                {
                    if (currentMusicVolume > 0) transform.Find("Text").GetComponent<Text>().text = "MUSIC:ON";
                    else if (currentMusicVolume <= 0) transform.Find("Text").GetComponent<Text>().text = "MUSIC:OFF";
                } 
                else if (transform.Find("Text").GetComponent<Text>().text.Contains("SOUND"))
                {
                    if (currentSoundVolume > 0) transform.Find("Text").GetComponent<Text>().text = "SOUND:ON";
                    else if (currentSoundVolume <= 0) transform.Find("Text").GetComponent<Text>().text = "SOUND:OFF";
                }
            }
        }

        /// <summary>
        /// Переключеие громкости музыки между 0 и 1
        /// </summary>
        public void ToggleMusic()
        {
            // Если звук музыки = 1, то поставить 0
            if (currentMusicVolume > 0)
            {
                // Mute the music volume
                currentMusicVolume = 0;

                Color newColor = musicImage.material.color;
                newColor.a = 0.5f;
                musicImage.color = newColor;

                // Set the relevant text
                if (transform.Find("Text")) transform.Find("Text").GetComponent<Text>().text = "MUSIC:OFF";
            }
            else // Если = 0, то  выставить 1
            {
                // Set the music volume to full
                currentMusicVolume = 1;

                Color newColor = musicImage.material.color;
                newColor.a = 1f;
                musicImage.color = newColor;

                // Set the relevant text
                if (transform.Find("Text")) transform.Find("Text").GetComponent<Text>().text = "MUSIC:ON";
            }

            GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().volume = currentMusicVolume;
            if (musicSlider) musicSlider.value = currentMusicVolume;
            PlayerPrefs.SetFloat(musicSaveName, currentMusicVolume);
        }

        /// <summary>
        /// Переключение громкости звуков между 0 и 1
        /// </summary>
        public void ToggleSound()
        {

            if (currentSoundVolume > 0)
            {
                currentSoundVolume = 0;

                Color newColor = soundImage.material.color;
                newColor.a = 0.5f;
                soundImage.color = newColor;

                if (transform.Find("Text")) transform.Find("Text").GetComponent<Text>().text = "SOUND:OFF";
            }
            else 
            {
               
                currentSoundVolume = 1;

                Color newColor = soundImage.material.color;
                newColor.a = 1;
                soundImage.color = newColor;

                if (transform.Find("Text")) transform.Find("Text").GetComponent<Text>().text = "SOUND:ON";
            }

            AudioListener.volume = currentSoundVolume;
            if (musicSlider) soundSlider.value = currentSoundVolume;
            PlayerPrefs.SetFloat(soundSaveName, currentSoundVolume);
        }

        public void changeMusicSlider(float value)
        {
            GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().volume = value;
            if (musicSlider) musicSlider.value = value;
            PlayerPrefs.SetFloat(musicSaveName, value);
        }
        public void changeSoundSlider(float value)
        {
            AudioListener.volume = value;
            if (soundSlider) soundSlider.value = value;
            PlayerPrefs.SetFloat(soundSaveName, value);
        }
    }
}