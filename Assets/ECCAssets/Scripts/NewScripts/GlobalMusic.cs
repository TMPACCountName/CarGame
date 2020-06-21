using UnityEngine;
using System.Collections;

namespace CarGame
{
	/// <summary>
	/// Обрабатывает глобальный музыкальный источник, который переносится со сцены на сцену без сброса музыкальной дорожки.
	///  прикрепить этот сценарий к музыкальному объекту и включить этот объект в каждую сцену, 
	///  сценарий сохранит только самый старый музыкальный источник в сцене.
	/// </summary>
	public class GlobalMusic : MonoBehaviour
	{
		[Tooltip("Тег музыкального источника")]
		public string musicTag = "Music";

		//время когда он был в игре
		internal float instanceTime = 0;

		void Awake()
		{
			//Поиск всех объектов с тегом Music
			GameObject[] musicObjects = GameObject.FindGameObjectsWithTag(musicTag);

			//Сохраняем только музыкальный объект, который был в игре более 0 секунд
			if (musicObjects.Length > 1)
			{
				foreach (var musicObject in musicObjects)
				{
					if (musicObject.GetComponent<GlobalMusic>().instanceTime <= 0) Destroy(gameObject);
				}
			}
		}

		void Start()
		{
			//Не убивать объект при смене сцены
			DontDestroyOnLoad(transform.gameObject);
		}

	}
}
