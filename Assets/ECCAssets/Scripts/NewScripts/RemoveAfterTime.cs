using UnityEngine;
using System.Collections;

namespace CarGame
{
	/// <summary>
	/// Скрипт удаляет объект
	/// </summary>
	public class ECCRemoveAfterTime : MonoBehaviour
	{
		[Tooltip("Сколько ждать секунд до удаления этого объекта")]
		public float removeAfterTime = 1;

		void Start()
		{
			Destroy(gameObject, removeAfterTime);
		}
	}
}
