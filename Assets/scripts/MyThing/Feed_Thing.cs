using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Feed_Thing : Feed {

	public Font textFont;
	public GameObject textPrefab;
	public GameObject imagePrefab;
	public GameObject feedTween;

	void Start(){
		downloadURL = "http://www.samdavies.info/social/downloadBlocks.php";

		grid = gridObject.GetComponent<UIGrid>();
		grid.Reposition();

		filterList.Add("thingID", "0");
		Download();
	}

	protected override void SetFeedReference(GameObject feedObject){
		feedObject.GetComponent<Data_Post>().theFeed = this;
	}

	protected override void LocalFilter(){
	}

	public void ShowDetails(Data_Post dataRef){
	}

	public void CreateTask(){
		WWWForm form = new WWWForm();
		//form.AddField("id", "");
		//form.AddField("Tsk_Name", "New Task");
		//form.AddField("Tsk_Date", date.ToString("u").Substring(0,10));
		//form.AddField("Tsk_Description", "");
		
		string creationURL = "http://www.samdavies.info/Systech/CreateTask.php";
		StartCoroutine(InsertTask(form, creationURL));
	}
	
	protected IEnumerator InsertTask(WWWForm form, string URL){
		print ("inserting...");
		
		//add auth feilds
		form.AddField("userCookie", PlayerPrefs.GetString("userCookie"));
		form.AddField("passCookie", PlayerPrefs.GetString("passCookie"));
		
		WWW upload = new WWW(URL, form);
		yield return upload;
		
		if(upload.error != null){
			print(upload.error);
			StartCoroutine(InsertTask(form, URL));
		}else{
			if(upload.text.Equals("updated")){
				print ("Insert sucessful");
				Download();
			}else{
				print (upload.text);
				//you must login in again
				print ("Authorisation invalid");
			}
		}
	}
}
