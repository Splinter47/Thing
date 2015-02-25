using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Feed_ThingV4 : Feed {
	
	public GameObject feedTween;

	void Start(){
		downloadURL = "http://www.samdavies.info/social/downloadBlocksV4.php";

		grid = gridObject.GetComponent<UIGrid>();
		grid.Reposition();

		filterList.Add("thingID", "0");
		Download();
	}

	protected override void SetFeedReference(GameObject feedObject){
		feedObject.GetComponent<Data_PostV4>().theFeed = this;
	}

	protected override void LocalFilter(){
	}

	public void ShowDetails(Data_PostV4 dataRef){
	}
}
