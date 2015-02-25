using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Main_Image : UIData{

	//reference to the Feed that controls this
	public Feed_ThingV4UI theFeed;

	//public int id;
	public int rank;
	public int thingID;
	public Texture2D image;
	public GameObject imageObj;
	public RawImage rawImage;

	void Awake(){
		imagesURL = "http://www.stuffcluster.com/social/images/";
	}

	public override void Create(){
		StartCoroutine(DownloadTexture(imagesURL, data["image"], AfterDownload, 0));
		//StartCoroutine(LoadLocalTexture(imagesURL, data["image"], AfterDownload));
	}

	public void AfterDownload(Texture2D newTex){
		image = newTex;
		//set
		SetTexture(imageObj, image);
		//image url needs to be set first
		DealWithImage(image, data["image"]);

		STools.GetCentredSubImage(rawImage);
	}

	public void ShowDetails(){
		//theFeed.ShowDetails(this);
	}

	public void SetTexture(GameObject item, Texture2D tex){
		RawImage image = item.GetComponent<RawImage>();
		image.texture = tex;
	}
}
