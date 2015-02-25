using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Feed_ThingV4UI : UIFeed {

	public GameObject detailsPanel;
	private int thingID;
	private bool isMyThing;
	public GameObject mainTexture;

	//padding obj
	public GameObject paddingObj;

	void Start(){
		downloadURL = "http://www.stuffcluster.com/social/downloadBlocksV4.php";
		uploadURL = "http://www.stuffcluster.com/social/uploadBlocks.php";

		//load the thing id 
		GetThingID();
		filterList.Add("thingID", thingID.ToString());
		Download();
		//FilterBy("thingID", thingID.ToString());
	}



	void OnEnable(){
		// Start the event listener
		this.OnDownload += AddFeedPadding;
		this.OnDownload += SetMainImage;
	}

	private void AddFeedPadding(){
		STools.AddChild(gridObject, paddingObj);
		STools.AddChild(gridObject, paddingObj);
		STools.AddChild(gridObject, paddingObj);
	}

	private void SetMainImage(){
		print ("new blocks downloaded");
		if(displayedBlocks.Count > 0){
			Data_PostV4UI topBlock = displayedBlocks[0].GetComponent<Data_PostV4UI>();
			Main_Image mainBlock = mainTexture.GetComponent<Main_Image>();
			mainBlock.data = topBlock.data;
			mainBlock.LoadTextures();

			//remove the top block
			topBlock.DestroyTextures();
			STools.Destroy(displayedBlocks[0]);
			displayedBlocks.RemoveAt(0);
		}
	}

	public void GetThingID(){
		thingID = PlayerPrefs.GetInt("chosenThingID");
	}

	public void IsThisMyThing(){
		if(PlayerPrefs.GetInt("myThingID") == thingID){
			isMyThing = true;
		}
	}

	public void Upload(Texture2D toUpload, string title){
		WWWForm form = new WWWForm();

		//get the image and its name
		byte[] bytes = toUpload.EncodeToPNG();
		string imageName = toUpload.name + ".png";

		//add fileds
		form.AddBinaryData("fileUpload", bytes, imageName, "image/png");
		form.AddField("title", title);
		form.AddField("thingID", thingID.ToString());

		//print (form.headers);
		uploadQueue.AddLast(form);
	}
	
	protected override void SetFeedReference(GameObject feedObject){
		feedObject.GetComponent<Data_PostV4UI>().theFeed = this;
	}

	protected override void LocalFilter(){
	}

	public void ShowDetails(Data_PostV4UI dataRef){
		PlayTween(detailsPanel, true);
	}

	public void HideDetails(){
		PlayTween(detailsPanel, false);
	}
}
