using UnityEngine;
using System.Collections;

public class TexWidthControl : MonoBehaviour {

	public UITexture sourceImage;
	private Texture2D sourceTex;
	public UIPanel clippingPanel;

	public GameObject blockImageObject;
	private UITexture tex;
	private ExampleDragDropItem dDI;

	private int maxWidth;
	private int minWidth = 50;


	void Start(){
		//preview movement
		tex = blockImageObject.GetComponent<UITexture>();
		dDI = blockImageObject.GetComponent<ExampleDragDropItem>();
		maxWidth = tex.width;

		sourceTex = sourceImage.mainTexture as Texture2D;

	}

	public void setRectangleWidth(){
		//match the texture with to the crop width
		tex.width = (int)clippingPanel.width;
		tex.MarkAsChanged();

		//snap to closest edge
		dDI.updatePosition();

		//set the image to match the new crop size
		float proportionalWidth = (1.0f*tex.width/maxWidth) * sourceTex.width;
		float newX = (sourceTex.width - proportionalWidth)/2;

		//for now leave the y-coordinate in the centre
		Rect cropRectangle = new Rect(newX, 0, proportionalWidth, sourceTex.height);
		setToRectangle(tex, cropRectangle);
	}

	public void increaseWidth(){
		//increase th crop width
		if((clippingPanel.width+2)<=maxWidth){
			float currentWidth = clippingPanel.width;
			float currentHeight = clippingPanel.height;
			clippingPanel.SetRect(0,0,currentWidth+2,currentHeight);	
			print ("width increased to: " + clippingPanel.width);
		}
	}

	public void decreaseWidth(){
		//decrease the crop width
		if((clippingPanel.width-2)>=minWidth){
			float currentWidth = clippingPanel.width;
			float currentHeight = clippingPanel.height;
			clippingPanel.SetRect(0,0,currentWidth-2,currentHeight);
			print ("width increased to: " + clippingPanel.width);
		}
	}
	
	void setToRectangle(UITexture image, Rect rectangle) {
		int x = Mathf.FloorToInt(rectangle.x);
		int y = Mathf.FloorToInt(rectangle.y);
		int width = Mathf.FloorToInt(rectangle.width);
		int height = Mathf.FloorToInt(rectangle.height);

		Color[] pix = sourceTex.GetPixels(x, y, width, height);
		Texture2D tempTex = new Texture2D(width, height);
		tempTex.SetPixels(pix);
		tempTex.Apply();
		image.mainTexture = tempTex;
		image.MarkAsChanged();
	}
}
