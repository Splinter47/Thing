using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Feed_General : MonoBehaviour {

	public UIScrollView scrollView;
	public GameObject gridObject;
	public GameObject displayPrefab;
	protected UIGrid grid; 

	protected List<string> downloadedStrings = new List<string>();
	protected List<GameObject> displayedBlocks = new List<GameObject>();

	public string downloadURL = "http://www.samdavies.info/Systech/DownloadPeople.php";
	public string uploadURL = "http://www.samdavies.info/Systech/UpdatePeople.php";
	protected Dictionary<int, string> filterList = new Dictionary<int, string>();

	//upload queue
	protected LinkedList<WWWForm> uploadQueue = new LinkedList<WWWForm>();
	private bool uploading = false;
	
	void Start () {
		grid = gridObject.GetComponent<UIGrid>();
		grid.Reposition();
		//Download();
	}

	void Update(){

		//push updates if they exist
		if(!uploading){
			if(uploadQueue.Count > 0){
				print (uploadQueue.Count);
				uploading = true;
				WWWForm form = uploadQueue.First.Value;
				StartCoroutine(UploadBlock(form, uploadURL));
			}
		}
	}
	
	protected void Download(){
		StartCoroutine(DownloadBlocks(filterList, downloadURL, 0));
	}

	protected IEnumerator DownloadBlocks(Dictionary<int, string> filters, string dataURL, int attempt){
		print ("downloading...");
		string URL = dataURL;
		WWWForm form = new WWWForm();

		//add all filters
		string methods = "";
		foreach(var filter in filters){
			methods += filter.Key + "," + filter.Value + ";";
		}
		//send all methods in one string
		form.AddField(("methods"), methods);

		//add auth feilds
		form.AddField("userCookie", PlayerPrefs.GetString("userCookie"));
		form.AddField("passCookie", PlayerPrefs.GetString("passCookie"));
		
		WWW download = new WWW(URL, form);
		yield return download;
		
		if(download.error != null){
			print(download.error);
			if(attempt < 10){
				StartCoroutine(DownloadBlocks(filters, dataURL, attempt+1));
			}else{
				print ("Download failed after " + attempt + " attempts");
			}
		}else{
			if(!download.text.Equals("failed")){
				print ("found data: " + download.text);
				//destroy old data
				RemoveDataObjects();
				//add new data
				AddPeopleData(download.text);
			}else{
				//you must login in again
				print ("Authorisation invalid");
			}
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

	void AddPeopleData(string stringToCut){
		// split the input string into people and add new people
		string[] stringSeparators = {"<!!>"};
		string[] people = stringToCut.Split(stringSeparators, System.StringSplitOptions.None);
		
		foreach(string block in people){
			//remove the last empty thing
			if(block.Length>1){
				downloadedStrings.Add(block);
			}
		}

		// important!
		DisplayPeople();
	}

	void DisplayPeople(){
		//create real objects
		scrollView.ResetPosition();
		foreach (string stringData in downloadedStrings){
			// create a new block Thumbnail
			GameObject blockObject = NGUITools.AddChild(gridObject, displayPrefab);
			displayedBlocks.Add(blockObject);
			SetFeedReference(blockObject);
			blockObject.GetComponent<Data_General>().AddVariables(stringData);
		}
		grid.Reposition();
		scrollView.ResetPosition();
	}

	abstract protected void SetFeedReference(GameObject feedObject);

	void RemoveDataObjects(){
		// destroy all objects
		scrollView.ResetPosition();
		foreach (GameObject block in displayedBlocks){
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
	
	protected void FilterBy(int methodNum, string search){
		//remove old filter
		filterList.Remove(methodNum);
		//check search is not empty
		if(!search.Equals("")){
			//add the filter
			filterList.Add(methodNum, search);
		}
		//download with filter applied
	}
}
