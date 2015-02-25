using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public static class STools{

	//sets the uvs to make the image fit the space without stretching
	static public void GetCentredSubImage(RawImage image){
		
		RectTransform rectTrans = (RectTransform)image.transform;
		float imageAspectRatio = 1.0f*image.texture.width/image.texture.height;
		float transAspectRatio = 1.0f*rectTrans.sizeDelta.x/rectTrans.sizeDelta.y;
		
		float uvW;
		float uvX;
		float uvH;
		float uvY;
		
		if(imageAspectRatio<transAspectRatio){
			uvW = 1;
			uvH = imageAspectRatio/transAspectRatio;
		}else{
			uvW = transAspectRatio/imageAspectRatio;
			uvH = 1;
		}
		
		uvX = (1-uvW)/2;
		uvY = (1-uvH)/2;
		
		image.uvRect = new Rect(uvX, uvY, uvW, uvH);
	}
	
	//used in GetCentredSubImage
	static private int ScaleAToB(double ratioA, int newB){
		return (int)Math.Floor(newB * ratioA);
	}
	
	public static string BoolToString(bool isTrue){
		if(isTrue) { return "1"; }
		else { return "0"; }
	}
	
	static public GameObject AddChild (GameObject parent, GameObject prefab)
	{
		GameObject go = GameObject.Instantiate(prefab) as GameObject;
		
		if (go != null && parent != null)
		{
			Transform t = go.transform;
			t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			go.layer = parent.layer;
		}
		return go;
	}
	
	/// <summary>
	/// Destroy the specified object, immediately if in edit mode.
	/// </summary>
	
	static public void Destroy (UnityEngine.Object obj)
	{
		if (obj != null)
		{
			if (Application.isPlaying)
			{
				if (obj is GameObject)
				{
					GameObject go = obj as GameObject;
					go.transform.parent = null;
				}
				
				UnityEngine.Object.Destroy(obj);
			}
			else UnityEngine.Object.DestroyImmediate(obj);
		}
	}
	
	//NGUI helpers

	static public string NGUIGetLabel(GameObject obj){
		return obj.GetComponent<UILabel>().text;
	}
	
	static public void NGUISetLabel(GameObject to, string newText){
		UILabel lbl = to.transform.GetComponent<UILabel>();
		lbl.text = newText;
		lbl.MarkAsChanged();
	}

	static public void NGUIPlayTween(GameObject obj, bool direction){
		obj.GetComponent<UITweener>().Play(direction);
	}

	
	static public string UIGetLabel(GameObject obj){
		return obj.GetComponent<Text>().text;
	}
	
	static public void UISetLabel(GameObject to, string newText){
		Text lbl = to.transform.GetComponent<Text>();
		lbl.text = newText;
	}
}
