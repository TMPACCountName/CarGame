using UnityEngine;
using UnityEngine.UI;

namespace CarGame   
{
    /// <summary>
    /// Воспроизведения звука из источника
    /// </summary>
    public class PlaySound : MonoBehaviour
    {
        [Tooltip("Звук для игры")]
        public AudioClip sound;

        [Tooltip("Тег зувукового изсточника")]
        public string soundSourceTag = "Sound";

        [Tooltip("Воспроизвести звук когда объект активирован")]
        public bool playOnStart = true;

        [Tooltip("Звук при нажатии на кнопку")]
        public bool playOnClick = false;

        [Tooltip("Рандомный звук для высоты звука шоб тот был рандомней")]
        public Vector2 pitchRange = new Vector2(0.9f, 1.1f);
        void Start()
        {
            if (playOnStart == true) PlayCurrentSound();

            if (playOnClick && GetComponent<Button>()) GetComponent<Button>().onClick.AddListener(delegate { PlayCurrentSound(); });

        }

        /// <summary>
        /// Проиграть звук
        /// </summary>
        public void Play(AudioClip sound)
        {
            if (soundSourceTag != string.Empty && sound)
            {
                // Дать звуку случайную высоту звучания
                GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().pitch = Random.Range(pitchRange.x, pitchRange.y) * Time.timeScale;

                // Играть звук
                GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(sound);
            }
            else if (GetComponent<AudioSource>())
            {
                // Придайтть звуку случайную высоту, ограниченную шкалой времени игры
                GetComponent<AudioSource>().pitch = Random.Range(pitchRange.x, pitchRange.y) * Time.timeScale;

                // Проиграть
                GetComponent<AudioSource>().PlayOneShot(sound);
            }

        }

        public void PlayCurrentSound()
        {
            if (soundSourceTag != string.Empty && sound)
            {
                GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().pitch = Random.Range(pitchRange.x, pitchRange.y) * Time.timeScale;
                GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(sound);
            }
            else if (GetComponent<AudioSource>())
            {
                GetComponent<AudioSource>().pitch = Random.Range(pitchRange.x, pitchRange.y) * Time.timeScale;
                GetComponent<AudioSource>().PlayOneShot(sound);
            }
        }
    }
}