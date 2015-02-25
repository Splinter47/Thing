﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using System;

public abstract class Feed : MonoBehaviour {

	//grid and scrollview referneces	
	public UIScrollView scrollView;
	public GameObject gridObject;
	protected UIGrid grid;

	//transform pos of the scroll view after reposition
	public Vector3 basePos { get; private set; }

	//prefab with Data class attached
	public GameObject displayPrefab;

	//do you want to save the downloaded data on local machine?
	public bool saveDataLocally;
	private bool isSaved = false;

	//URL's used in WWW
	public string downloadURL = "";
	public string uploadURL = "";

	//list of filter to apply in WWW
	protected Dictionary<string, string> filterList = new Dictionary<string, string>();

	//time of most recent download 
	private int latestDownloadRequest;
	protected bool isDownloading = false;

	//upload queue prevents older updates from overriting newer updates
	protected LinkedList<WWWForm> uploadQueue = new LinkedList<WWWForm>();
	private bool uploading = false;

	//raw string List from download
	protected List<string> downloadedStrings = new List<string>();

	//refernce to the current displayed Prefabs
	protected List<GameObject> displayedBlocks = new List<GameObject>();

	//the number of buffered images
	public int imagebufferCount;
	public int reloadAfter = 1;
	protected int indexOfBufferCentre;

	void Awake(){
		// Forces a different code path in the BinaryFormatter that doesn't rely on run-time code generation (which would break on iOS).
		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

		//find the grid before any start methods are called
		grid = gridObject.GetComponent<UIGrid>();

		//make sure reloadAfter is > 0
		if(reloadAfter<1){
			reloadAfter = 1;
		}

		//ofset the index of the buffer past the required reload amount
		indexOfBufferCentre = -1 - reloadAfter;
		print(indexOfBufferCentre);
	}

	void Update(){

		//push updates if they exist
		if(!uploading){
			if(uploadQueue.Count > 0){
				print ("Upload Queue size: " + uploadQueue.Count);
				uploading = true;
				WWWForm form = uploadQueue.First.Value;
				StartCoroutine(UploadBlock(form, uploadURL));
			}
		}

		if(displayedBlocks.Count > 0){
			if(imagebufferCount>0){
				RebuferImages();
			}
		}
	}

	public void RebuferImages(){
		//find the Gamobject who's offset is the closest to the grid
		// Calculate the panel's center in world coordinates
		Vector3[] corners = scrollView.panel.worldCorners;
		Vector3 panelCenter = (corners[2] + corners[0]) * 0.5f;

		// Offset this value by the momentum
		Vector3 pickingPoint = panelCenter - scrollView.currentMomentum * (scrollView.momentumAmount * 0.1f);
		
		float min = float.MaxValue;
		Transform closest = null;
		Transform trans = gridObject.transform;
		int minIndex = 0;

		// Determine the closest child
		for (int i = 0, imax = displayedBlocks.Count; i < imax; ++i){
			Transform t = displayedBlocks[i].transform;
			float sqrDist = Vector3.SqrMagnitude(t.position - pickingPoint);
			
			if (sqrDist < min){
				min = sqrDist;
				closest = t;
				minIndex = i;
			}
		}
		
		//change the buffered images
		if(Math.Abs(minIndex-indexOfBufferCentre) >= (reloadAfter)){
			indexOfBufferCentre = minIndex;
			int currentBuffered = 0;
			currentBuffered += BufferTexture(minIndex, currentBuffered);
			for(int i = 1; i<displayedBlocks.Count; i++){
				if(minIndex-i >= 0){
					//currentBuffered += BufferTexture(minIndex-i, currentBuffered);
					int increase = BufferTexture(minIndex-i, currentBuffered);
				}
				if(minIndex+i < displayedBlocks.Count){
					//currentBuffered += BufferTexture(minIndex+i, currentBuffered);
					int increase = BufferTexture(minIndex+i, currentBuffered);
					currentBuffered += increase;
				}
			}
		}
	}
	
	private int BufferTexture(int index, int currentBufferCount){
		Data bitOfData = displayedBlocks[index].GetComponent<Data>();
		if(currentBufferCount<imagebufferCount){
			if(!bitOfData.imagesExist){
				bitOfData.LoadTextures();
			}
			return 1;
		}else{
			if(bitOfData.imagesExist){
				bitOfData.DestroyTextures();
			}
			return 0;
		}
	}
	
	public void Download(){
		StartCoroutine(DownloadBlocks(downloadURL, 0));
	}

	protected IEnumerator DownloadBlocks(string dataURL, int attempt){
		print ("downloading...");
		//useful to know if a download is taking place
		isDownloading = true;

		//record the time of this download
		latestDownloadRequest = (int)Time.time;
		WWWForm form = new WWWForm();

		//add all filters
		print ("adding filters...");
		foreach(var filter in filterList){
			print (filter.Key + ": " + filter.Value);
			form.AddField(filter.Key, filter.Value);
		}

		//add auth feilds
		form.AddField("userCookie", PlayerPrefs.GetString("userCookie"));
		form.AddField("passCookie", PlayerPrefs.GetString("passCookie"));
		form.AddField("time", latestDownloadRequest);
		
		WWW download = new WWW(dataURL, form);
		yield return download;
		
		if(download.error != null){
			print(download.error);
			if(attempt < 10){
				StartCoroutine(DownloadBlocks(dataURL, attempt+1));
			}else{
				print ("Download failed after " + attempt + " attempts");
				//load from player prefs
				LoadLocalData();
			}
		}else{
			if(!download.text.Equals("failed")){
				print ("found data: " + download.text);
				//split into data and time
				if(!download.text.Equals("")){
					CheckIsLastestDownload(download.text, (filterList.Count == 0));
				}
			}else{
				//you must login in again
				print ("Authorisation invalid");
			}
		}
	}

	private void CheckIsLastestDownload(string downloadText, bool toSave){
		string[] stringSeparators = {"<!time!>"};
		string[] dataAndTime = downloadText.Split(stringSeparators, System.StringSplitOptions.None);
		
		if(Int32.Parse(dataAndTime[1]) == latestDownloadRequest){
			isDownloading = false;
			//destroy old data
			RemoveDataObjects();
			//add new data
			AddBlocksData(dataAndTime[0], toSave);
		}
	}

	protected IEnumerator UploadBlock(WWWForm form, string uploadURL){
		print ("uploading...");

		//add auth feilds
		form.AddField("userCookie", PlayerPrefs.GetString("userCookie"));
		form.AddField("passCookie", PlayerPrefs.GetString("passCookie"));
		
		WWW upload = new WWW(uploadURL, form);
		yield return upload;

		if(upload.error != null){
			print(upload.error);
			StartCoroutine(UploadBlock(form, uploadURL));
		}else{
			//removed the upload from the queue
			uploading = false;
			uploadQueue.RemoveFirst();

			if(upload.text.Equals("updated")){
				print ("upload sucessful");
			}else{
				print (upload.text);
				//you must login in again
				print ("Authorisation invalid");
			}
		}
	}

	private void AddBlocksData(string toCut, bool toSave){
		// split the rawData string into blocks and add new blocks
		string[] stringSeparators = {"<!!>"};
		string[] people = toCut.Split(stringSeparators, System.StringSplitOptions.None);
		
		foreach(string block in people){
			//remove the last empty thing
			if(block.Length>1){
				downloadedStrings.Add(block);
			}
		}

		// important!
		DisplayBlocks(toSave);
	}

	private void DisplayBlocks(bool toSave){
		//create real objects
		scrollView.ResetPosition();
		print ("creating " + downloadedStrings.Count + " blocks");
		foreach (string stringData in downloadedStrings){
			// create a new block Thumbnail
			GameObject blockObject = NGUITools.AddChild(gridObject, displayPrefab);
			displayedBlocks.Add(blockObject);
			SetFeedReference(blockObject);
			if(imagebufferCount<=0){
				blockObject.GetComponent<Data>().AddVariables(stringData, true);
			}else{
				blockObject.GetComponent<Data>().AddVariables(stringData, false);
			}
		}
		//save 
		if(toSave){
			SaveDataLocal();
		}
		grid.Reposition();
		scrollView.ResetPosition();
		basePos = scrollView.transform.localPosition;
	}

	abstract protected void SetFeedReference(GameObject feedObject);

	protected void RemoveDataObjects(){
		// destroy all objects
		scrollView.ResetPosition();
		foreach (GameObject block in displayedBlocks){
			block.GetComponent<Data>().DestroyTextures();
			NGUITools.Destroy(block);

		}
		//empty the list
		displayedBlocks.Clear();
		downloadedStrings.Clear();
		scrollView.ResetPosition();
	}

	//-------------- HELPER FUNCTIONS --------------------

	protected string GetLabel(GameObject obj){
		return obj.GetComponent<UILabel>().text;
	}
	
	protected void SetLabel(GameObject to, string newText){
		UILabel lbl = to.transform.GetComponent<UILabel>();
		lbl.text = newText;
		lbl.MarkAsChanged();
	}

	protected void SetLabelAndInput(GameObject to, string newText){
		UIInput input = to.transform.GetComponent<UIInput>();
		input.value = newText;
		SetLabel(to, newText);
	}
	
	protected void PlayTween(GameObject obj, bool direction){
		obj.GetComponent<UITweener>().Play(direction);
	}
	
	protected void SetGridColumns(int cols){
		grid.maxPerLine = cols;
		grid.Reposition();
	}
	
	protected void FilterBy(string methodNum, string search){
		//remove old filter
		filterList.Remove(methodNum);
		//check search is not empty
		if(!search.Equals("")){
			//add the filter
			filterList.Add(methodNum, search);
		}

		//----------filter locally----------
		LoadLocalData();

		if(EnableAutoDownloads.isEnabled == true){
			//download with filter applied
			StartCoroutine(DownloadBlocks(downloadURL, 0));
		}
	}

	abstract protected void LocalFilter();

	public void FilterByList(string key, List<string> list){

		string bigString = "";

		if(list.Count > 0){
			bigString += list[0];

			if(list.Count > 1){
				for(int i = 1; i<list.Count; i++){
					bigString += ("," + list[i]);
				}
			}
		}

		//currentFilters
		FilterBy(key, bigString);
	}

	protected bool ContainsFilter(List<string> toFilter, List<string> filterBy){
		
		bool doesNotContain = true;
		foreach(string filter in filterBy){
			if(toFilter.Contains(filter)){
				doesNotContain &= false;
			}
		}
		return !doesNotContain;
	}
	
	protected bool ContainsAllFilters(List<string> toFilter, List<string> filterBy){
		
		bool containsAll = true;
		foreach(string filter in filterBy){
			if(!toFilter.Contains(filter)){
				containsAll = false;
			}
		}
		return containsAll;
	}


	//------------ SAVE AND LOAD FUNCTIONS -----------------

	public void SaveDataLocal(){
		if(saveDataLocally){
			if(!isSaved){
				print ("Saved.");
				//only store first occurence
				isSaved = true;
				//use downloadURL since we know all saves of this name will be the same
				convertTo<string>(downloadURL, downloadedStrings);
			}
		}
	}
	
	private void convertTo<T>(string key, List<T> list){
		BinaryFormatter b = new BinaryFormatter();
		MemoryStream m = new MemoryStream();
		b.Serialize(m, list);
		PlayerPrefs.SetString(key, Convert.ToBase64String(m.GetBuffer()));
	}
	
	protected void LoadLocalData(){
		if(saveDataLocally){
			isDownloading = false;
			print ("Loading locally saved data");
			RemoveDataObjects();
			//load string from prefabs
			string data = PlayerPrefs.GetString(downloadURL);
			//convert string to list of strings
			convertBackGameObject(data);
			DisplayBlocks(false);
			LocalFilter();
			grid.Reposition();
			scrollView.ResetPosition();
		}
	}

	private void convertBackGameObject(string data){
		if(!String.IsNullOrEmpty(data)){
			//load the data from binary back into the list
			var b = new BinaryFormatter();
			var m = new MemoryStream(Convert.FromBase64String(data));
	
			downloadedStrings = b.Deserialize(m) as List<string>;
			print(downloadedStrings.Count);
		}
	}
}
