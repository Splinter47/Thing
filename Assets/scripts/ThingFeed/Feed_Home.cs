using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Feed_Home : UIFeed {

	public TweenPosition feedOuterTween;

	private bool fullView = true;

	void Start(){
		downloadURL = "http://www.stuffcluster.com/social/downloadThings.php";

		filterList.Add("cluster", "0");
		//filterList.Add("search", "t");
		Download();
	}

	void OnEnable(){
		// Start the event listener
		this.OnDownload += DownloadHappened;
	}

	public void DownloadHappened(){

	}

	public void GoToThing(int toThingID){
		PlayerPrefs.SetInt("chosenThingID", toThingID);
		Application.LoadLevel("MyThingV5");
	}

	/*public void SwitchView(){
		if(fullView){
			ListView();
			fullView = false;
		}else{
			FullView();
			fullView = true;
		}
	}

	private void FullView(){
		scrollView.ResetPosition();
		UIPanel pan = scrollView.panel;
		scrollView.panel.SetRect(0, 0, pan.width, (pan.height*2));
		feedOuterTween.PlayReverse();

		foreach(GameObject obj in displayedBlocks){
			obj.GetComponent<Data_Home>().ToggleSize(false);
		}

		grid.cellHeight = 456f;
		grid.Reposition();
		grid.gameObject.GetComponent<UICenterOnChild>().enabled = true;
		scrollView.ResetPosition();
	}

	private void ListView(){
		scrollView.ResetPosition();
		grid.gameObject.GetComponent<UICenterOnChild>().enabled = false;
		UIPanel pan = scrollView.panel;
		scrollView.panel.SetRect(0, 0, pan.width, (pan.height/2));
		feedOuterTween.PlayForward();

		foreach(GameObject obj in displayedBlocks){
			obj.GetComponent<Data_Home>().ToggleSize(true);
		}

		grid.cellHeight = 175f;
		grid.Reposition();
		scrollView.ResetPosition();
	}*/

	protected override void SetFeedReference(GameObject feedObject){
		feedObject.GetComponent<Data_Home>().theFeed = this;
	}

	protected override void LocalFilter(){
	}

	public void ShowDetails(Data_Home dataRef){
	}
}
