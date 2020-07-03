using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// This class allows you to implement a stick in the game.
/// </summary>
public class Stick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	[Header("Stick sprites")]
	[Tooltip("0 - Background Stick\n1 - Knob")]
	public Image[] images = new Image[2];

	[Header("Optional")]
	[Tooltip("Display axes if true")]
	public bool doDisplayAxes = false;

	[Tooltip("List of text components to display the X axis value")]
	public Text[] textXAxis;

	[Tooltip("List of text components to display the Y axis value")]
	public Text[] textYAxis;

	/// Stick axes' values.
	internal Vector2 inputVector;
	
	void Update () {
		if (doDisplayAxes)
			DisplayAxes();
	}

	public void OnDrag(PointerEventData eventData)
	{
		//Check for sprites
		for (int i = 0; i < images.Length; i++)
			if (!images[i])
				return;

		Vector2 tmp;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(images[0].rectTransform, eventData.position, eventData.pressEventCamera, out tmp))
		{
			tmp.x = (tmp.x / images[0].rectTransform.sizeDelta.x);
			tmp.y = (tmp.y / images[0].rectTransform.sizeDelta.y);
		}

		inputVector = new Vector2(tmp.x*2.0f, tmp.y*2.0f);
		inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

		images[1].rectTransform.anchoredPosition = new Vector2(inputVector.x * (images[0].rectTransform.sizeDelta.x/2), inputVector.y * (images[0].rectTransform.sizeDelta.y/2 ));
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		
		OnDrag(eventData);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		inputVector = Vector2.zero;
		images[1].rectTransform.anchoredPosition = Vector2.zero;
	}

	/// <summary>
	/// Get horizontal axis -1 to 1
	/// </summary>
	/// <returns> horizontal axis -1.0 to 1.0 </returns>
	public float GetHorizontal()
	{
		return inputVector.x;
	}

	/// <summary>
	/// Get vertical axis -1 to 1
	/// </summary>
	/// <returns> Vertical axis -1.0 to 1.0 </returns>
	public float GetVertical()
	{
		return inputVector.y;
	}

	/// <summary>
	/// Display axes in text fields if they exist and doDisplayAxes = true;
	/// </summary>
	public void DisplayAxes()
	{
		for (int i = 0; i < textXAxis.Length; i++)
			textXAxis[i].text = "X:" + GetHorizontal().ToString();
		for (int i = 0; i < textYAxis.Length; i++)
			textYAxis[i].text = "Y: " + GetVertical().ToString();
	}
}
