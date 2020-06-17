using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ECCMusicSliderControllerScript : MonoBehaviour {

	public Slider slider;
	public float sliderValue;

	public AudioSource audioSource;
	public float musicVolume;

	void Start () {
		audioSource.Play();
		slider.value = PlayerPrefs.GetFloat("saveMusic", sliderValue);
	}

	void Update()
	{
		audioSource.volume = musicVolume;
	}

	public void changeSlider(float value)
	{
		sliderValue = value;
		musicVolume = value;
		PlayerPrefs.SetFloat("saveMusic", sliderValue);
	}
}
