using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class NicknameValidator : MonoBehaviour {

	public InputField text;
	public void OnValueChanged(string s)
	{
		Regex reg = new Regex("[a-z0-9 _]", RegexOptions.IgnoreCase);
		if (!reg.IsMatch(s[s.Length - 1].ToString()))
			text.text = s.Substring(0, s.Length - 1);
	}

}
