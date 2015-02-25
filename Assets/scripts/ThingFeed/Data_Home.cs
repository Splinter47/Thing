using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class Data_Home: UIData{

	//reference to the Feed_Thing
	public Feed_Home theFeed;

	public int id;
	public RawImage image; 
	public Text title;

	public RectTransform bar;
	private float baseBarY;

	//info
	public Text postCount;
	public Text userCount;

	public GameObject friendGrid;
	public List<GameObject> friends = new List<GameObject>();
	public GameObject friendPrefab;
	public bool friendObjsCreated = false;

	public bool show = false;

	void Awake(){
		imagesURL = "http://www.stuffcluster.com/social/images/";
	}

	public override void Create(){
		id = int.Parse(data["id"]);

		//title 
		title.text = data["title"];

		//info
		postCount.text = data["post_count"];
		userCount.text = data["user_count"];

		StartCoroutine(DownloadTexture(imagesURL, data["image"], AfterDownload, 0));

		if(!friendObjsCreated){
			AddFriends(data["friends"]);
			friendObjsCreated = true;
		}else{
			foreach(GameObject friend in friends){
				friend.GetComponent<Friend>().GetImages();
			}
		}
	}

	private void AddFriends(string input){
		//divide into its data components
		string[] friendStringSeparators = {"|friend|"};
		string[] friends = input.Split(friendStringSeparators, System.StringSplitOptions.RemoveEmptyEntries);
		
		//split into key, value pair and add to data dictionary
		foreach(string friend in friends){
			GameObject newFriend = STools.AddChild(friendGrid, friendPrefab);
			newFriend.GetComponent<Friend>().theData = this;
			newFriend.GetComponent<Friend>().Create(friend);
		}
	}

	public void DownloadFriendImage(string fileName, Action<string, Texture2D> onFinish){

		StartCoroutine(DownloadTexture(imagesURL, fileName, onFinish));
	}

	public void DealWithFriendImage(Texture2D tex, string url){
		DealWithImage(tex, url);
	}

	public void OpenThisThing(){
		theFeed.GoToThing(id);
	}

	public void UpdateBarPos(){

		if(-theFeed.gridObject.transform.localPosition.y + 43f > transform.localPosition.y){
			bar.gameObject.SetActive(true);
		}else{
			bar.gameObject.SetActive(false);
		}
	}

	void Update(){
		//UpdateBarPos();
	}

	public void AfterDownload(Texture2D newTex){
		image.texture = newTex;

		//image url needs to be set first
		DealWithImage(newTex, (imagesURL+data["image"]));
	}

	public void ShowDetails(){
		theFeed.ShowDetails(this);
	}
}
