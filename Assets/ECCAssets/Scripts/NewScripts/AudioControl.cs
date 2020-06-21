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
        // Громкость всех звуков, кроме музыки
        static float currentSoundVolume = 1;

        // Громкость музыки
        static float currentMusicVolume = 1;

        void Awake()
        {
            // Отделение громкости музыки от других звуков
            GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().ignoreListenerVolume = true;

            // Получение громкости музыки из PlayerPrefs;
            currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume", currentMusicVolume);

            // Установить громкость музыки
            GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().volume = currentMusicVolume;

            // Получить громкость звуков
            currentSoundVolume = PlayerPrefs.GetFloat("SoundVolume", currentSoundVolume);

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
            if (currentMusicVolume == 1)
            {
                // Mute the music volume
                currentMusicVolume = 0;

                Color newColor = GetComponent<Image>().material.color;
                newColor.a = 0.5f;
                GetComponent<Image>().color = newColor;

                // Set the relevant text
                if (transform.Find("Text")) transform.Find("Text").GetComponent<Text>().text = "MUSIC:OFF";
            }
            else // Если = 0, то  выставить 1
            {
                // Set the music volume to full
                currentMusicVolume = 1;

                Color newColor = GetComponent<Image>().material.color;
                newColor.a = 1;
                GetComponent<Image>().color = newColor;

                // Set the relevant text
                if (transform.Find("Text")) transform.Find("Text").GetComponent<Text>().text = "MUSIC:ON";
            }

            GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().volume = currentMusicVolume;

            PlayerPrefs.SetFloat("MusicVolume", currentMusicVolume);
        }

        /// <summary>
        /// Переключение громкости звуков между 0 и 1
        /// </summary>
        public void ToggleSound()
        {

            if (currentSoundVolume == 1)
            {
                currentSoundVolume = 0;

                Color newColor = GetComponent<Image>().material.color;
                newColor.a = 0.5f;
                GetComponent<Image>().color = newColor;

                if (transform.Find("Text")) transform.Find("Text").GetComponent<Text>().text = "SOUND:OFF";
            }
            else 
            {
               
                currentSoundVolume = 1;

                Color newColor = GetComponent<Image>().material.color;
                newColor.a = 1;
                GetComponent<Image>().color = newColor;

                if (transform.Find("Text")) transform.Find("Text").GetComponent<Text>().text = "SOUND:ON";
            }

            AudioListener.volume = currentSoundVolume;

            PlayerPrefs.SetFloat("SoundVolume", currentSoundVolume);
        }
    }
}