using UnityEngine;
using System.Collections;
using System;

public class Item : MonoBehaviour {

	public PostMaker postMaker;
	public GameObject image;
	public GameObject whiteBack;
	public GameObject text;

	public Texture2D rawImage;

	public void SelectThis(){
		print ("slection made");
		postMaker.selected = this;
	}

	public string GetOrder(){
		return gameObject.name;
	}

	public void SetOrder(string to){
		gameObject.name = to;
	}

	public void SetWidth(int newWidth, int padding, int textInset){
		if(image != null){
			UITexture tex = image.GetComponent<UITexture>();
			tex.width = newWidth-padding;
			image.GetComponent<UITexture>().mainTexture = GetCentredSubTexture(rawImage, 4*tex.width, 4*tex.height);
		}
		if(whiteBack != null){
			whiteBack.GetComponent<UISprite>().width = newWidth-padding;
		}
		if(text != null){
			text.GetComponent<UILabel>().width = newWidth-textInset;
		}
	}

	private void DestroyTexture(){

		//TODO this
	}

	public Texture2D GetCentredSubTexture(Texture2D inputTex, int newWidth, int newHeight){
		
		int targetWidth = newWidth;
		int targetHeight = newHeight;
		
		//differnce between current width/height and new width/height
		int widthDiff = inputTex.width - newWidth;
		int heighDiff = inputTex.height - newHeight;
		
		//ratio between current width/height and new width/height
		double widthRatio = (1.0*inputTex.width)/newWidth;
		double heighRatio = (1.0*inputTex.height)/newHeight;
		
		//check the new dimensions are smaller
		if(widthRatio < 1 || heighRatio < 1){
			if(widthRatio<1 && heighRatio < 1){
				//both too big here
				if(widthRatio<heighRatio){
					//calc from old width
					newHeight = ScaleAToB(widthRatio, newHeight);
					newWidth = inputTex.width;
				}else{
					//calc from old height
					newWidth = ScaleAToB(heighRatio, newWidth);
					newHeight = inputTex.height;
				}
			}else if(widthRatio<1){
				//calc from old width
				newHeight = ScaleAToB(widthRatio, newHeight);
				newWidth = inputTex.width;
			}else{
				//calc from old height
				newWidth = ScaleAToB(heighRatio, newWidth);
				newHeight = inputTex.height;
			}
			
			//recalculate
			widthDiff = inputTex.width - newWidth;
			heighDiff = inputTex.height - newHeight;
		}
		
		//find centre
		int x = (widthDiff)/2;
		int y = (heighDiff)/2;
		
		Color[] pix = inputTex.GetPixels(x, y, newWidth, newHeight);
		Texture2D destTex = new Texture2D(newWidth, newHeight);
		destTex.SetPixels(pix);
		destTex.Apply();
		
		print ("Target: (" + targetWidth + "," + targetHeight + ")" + 
		       " max: (" + inputTex.width + "," + inputTex.height + ")" +
		       " Actual: (" + newWidth + "," + newHeight + ")");
		
		return destTex;
	}
		
	//used in GetCentredSubTexture
	private int ScaleAToB(double ratioA, int newB){
		return (int)Math.Floor(newB * ratioA);
	}
}
