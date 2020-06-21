using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace CarGame
{
	/// <summary>
	/// Выбирает кнопку когда этот объект включен
	/// </summary>
	public class SelectButton : MonoBehaviour
	{
		// TКнопка выбора
		public GameObject selectedButton;

		void OnEnable()
		{
			if (selectedButton)
			{
				if (EventSystem.current) EventSystem.current.SetSelectedGameObject(selectedButton);
			}
		}
	}
}
