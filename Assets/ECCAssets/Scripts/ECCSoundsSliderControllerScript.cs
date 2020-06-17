using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ECCSoundsSliderControllerScript : MonoBehaviour {

	public Slider slider;
	public float sliderValue;

	public AudioSource audioSource;
	public float soundVolume;

	void Start()
	{
		slider.value = PlayerPrefs.GetFloat("saveSound", sliderValue);
	}

	void Update()
	{
		audioSource.volume = soundVolume;
	}

	public void changeSlider(float value)
	{
		sliderValue = value;
		soundVolume = value;
		PlayerPrefs.SetFloat("saveSound", sliderValue);
	}
}
