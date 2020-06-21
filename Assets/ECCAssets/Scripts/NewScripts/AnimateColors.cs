using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CarGame
{
	/// <summary>
	/// Скрипт анимирующий цвет любого объекта
	/// </summary>
	public class AnimateColors : MonoBehaviour
	{
		[Tooltip("Цвета для анимирования")]
		public Color[] colorList;

		[Tooltip("Индекс текущего цвета в списке")]
		public int colorIndex = 0;

		[Tooltip("Как долго длится анимация и счетчик для ее отслеживания")]
		public float changeTime = 1;
		public float changeTimeCount = 0;

		[Tooltip("Скорость смены цвета")]
		public float changeSpeed = 1;

		[Tooltip("Анимация остановлена?")]
		public bool isPaused = false;

		[Tooltip("Анимация зациклена?")]
		public bool isLooping = true;

		[Tooltip("Начать со случайного цвета?")]
		public bool randomColor = false;

		void Start()
		{
			//Установить цвет
			SetColor();
		}

		void Update()
		{
			//Если паузы выключена
			if (!isPaused)
			{
				if (changeTime > 0)
				{
					//Обратнный отсчет до следующего изменения
					if (changeTimeCount > 0)
					{
						changeTimeCount -= Time.deltaTime;
					}
					else
					{
						changeTimeCount = changeTime;

						//Сменить цвет на следующий
						if (colorIndex < colorList.Length - 1)
						{
							colorIndex++;
						}
						else
						{
							if (isLooping == true) colorIndex = 0;
						}
					}
				}

				//Если прикреплен рендер, то установить цвет
				if (GetComponent<Renderer>())
				{
					GetComponent<Renderer>().material.color = Color.Lerp(GetComponent<Renderer>().material.color, colorList[colorIndex], changeSpeed * Time.deltaTime);
				}

				if (GetComponent<TextMesh>())
				{
					GetComponent<TextMesh>().color = Color.Lerp(GetComponent<TextMesh>().color, colorList[colorIndex], changeSpeed * Time.deltaTime);
				}

				if (GetComponent<SpriteRenderer>())
				{
					GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, colorList[colorIndex], changeSpeed * Time.deltaTime);
				}

				if (GetComponent<Image>())
				{
					GetComponent<Image>().color = Color.Lerp(GetComponent<Image>().color, colorList[colorIndex], changeSpeed * Time.deltaTime); ;
				}
			}
			else
			{
				//Применить выбранный цвет
				SetColor();
			}
		}

		/// <summary>
		/// Применить цвет на основе индекса списка цветов
		/// </summary>
		void SetColor()
		{
			//Рандом цвет
			int tempColor = 0;

			//Установить цвет рандомный
			if (randomColor) tempColor = Mathf.FloorToInt(Random.value * colorList.Length);

			//Для объекта
			if (GetComponent<Renderer>())
			{
				GetComponent<Renderer>().material.color = colorList[tempColor];
			}

			//Для текста
			if (GetComponent<TextMesh>())
			{
				GetComponent<TextMesh>().color = colorList[tempColor];
			}

			//Для спрайта
			if (GetComponent<SpriteRenderer>())
			{
				GetComponent<SpriteRenderer>().color = colorList[tempColor];
			}

			// Для картинки
			if (GetComponent<Image>())
			{
				GetComponent<Image>().color = colorList[tempColor];
			}
		}

		/// <summary>
		/// Состояние паузы
		/// </summary>
		/// <param name="pauseState">Pause state, true paused, false unpaused</param>
		void Pause(bool pauseState)
		{
			isPaused = pauseState;
		}

	}
}
