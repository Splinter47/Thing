using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ElementToTextSize : MonoBehaviour {

	public Text textbox;
	public LayoutElement element;
	public float padding;

	// Use this for initialization
	void Start () {
		FitToTextbox();
	}
	
	public void FitToTextbox(){
		element.minHeight = textbox.preferredHeight + padding;
	}
}
