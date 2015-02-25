using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FitRawImage : MonoBehaviour {

	public RawImage image;
	public bool continuouslyResize = false;

	// Use this for initialization
	void Start () {
		STools.GetCentredSubImage(image);
	}
	
	// Update is called once per frame
	void Update () {
		if(continuouslyResize){
			STools.GetCentredSubImage(image);
		}
	}
}
