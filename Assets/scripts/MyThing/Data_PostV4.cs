using UnityEngine;
using System.Collections;

public class Data_PostV4 : Data{

	//reference to the Feed_Thing
	public Feed_ThingV4 theFeed;

	public int id;
	public int rank;
	public int thingID;
	public Texture2D image;
	public GameObject imageObj;

	//private string imagesURL = "http://www.samdavies.info/social/images/";

	public override void Create(){
		StartCoroutine(DownloadTexture(imagesURL, data["image"], AfterDownload, 0));
	}

	public void AfterDownload(Texture2D newTex){
		image = newTex;

		//set
		SetTexture(imageObj, image);
	}

	public void ShowDetails(){
		theFeed.ShowDetails(this);
	}

	public void SetTexture(GameObject item, Texture2D tex){
		UITexture image = item.GetComponent<UITexture>();
		image.mainTexture = tex;
	}
}
