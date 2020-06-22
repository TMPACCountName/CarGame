using UnityEngine;
using System.Collections;

namespace CarGame
{
	/// <summary>
	/// Анимирования UI Во время паузы
	/// </summary>
	public class AnimateUI : MonoBehaviour
	{
		// Текущее реальное время не связанное с Time.TimeScale
		internal float currentTime;

		// Прошлое зарегестрированное время
		internal float previousTime;

		// Дельта тайм ( изменение в времени ) просчет времени для обеспечения анимации
		internal float deltaTime;

		[Tooltip("Интро анимация для UI элемента")]
		public AnimationClip introAnimation;

		internal Animation animationObject;

		// Текущее время анимации
		internal float animationTime = 0;

		// Анимируем ли сейчас?
		internal bool isAnimating = false;

		[Tooltip("Должна ли анимация производиться сразу при включении интерфейса?")]
		public bool playOnEnabled = true;

		void Awake()
		{
			// Установить время
			previousTime = currentTime = Time.realtimeSinceStartup;

			// Объявить анимации объекта
			animationObject = GetComponent<Animation>();
		}

		void Update()
		{
			// Мы анимируем
			if (introAnimation && isAnimating == true)
			{

				// Получить текущее время в реал тайме
				currentTime = Time.realtimeSinceStartup;

				// Рассчитать разницу
				deltaTime = currentTime - previousTime;

				// Установить текущее время для следующей проверки в юпдейт
				previousTime = currentTime;

				// Рассчитать текущее время в текущей анимации
				
				animationObject[introAnimation.name].time = animationTime;

				animationObject.Sample();

				// Добавить время анимации
				animationTime += deltaTime;

				// Закончить анимации
				if (animationTime >= animationObject.clip.length)
				{
					// установить время анимации на длину клипа
					animationObject[introAnimation.name].time = animationObject.clip.length;

					animationObject.Sample();

					// Когда не анимируем
					isAnimating = false;
				}
			}
		}

		/// <summary>
		/// Запускать когда объект включен, если он был выключен
		/// </summary>
		void OnEnable()
		{
			// Играть анимацию
			if (playOnEnabled == true)
			{
				PlayAnimation();
			}
		}

		/// <summary>
		/// Играть анимацию не зависимо от времени
		/// </summary>
		public void PlayAnimation()
		{
			if (introAnimation)
			{
				// Переустановить время анимации
				animationTime = 0;

				// Установить текущее время
				previousTime = currentTime = Time.realtimeSinceStartup;

				// Показать шо анимация играет
				isAnimating = true;

				animationObject.Play();
			}
		}
	}
}