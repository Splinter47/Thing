using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public abstract class UIData : MonoBehaviour {

	public Dictionary<string,string> data = new Dictionary<string, string>();
	public bool saveImagesLocally = false;
	public List<string> taxonomies;

	//visual elements to display on seperate page
	public GameObject[] displayElementLinks;
	protected Dictionary<string, GameObject> displayElements = new Dictionary<string, GameObject>();

	//list of images
	protected Dictionary<string,Texture2D> imagesToDealWith = new Dictionary<string, Texture2D>();
	public bool imagesDownloadsAllowed{ get; private set; }
	protected string imagesURL = "";

	public void AddVariables(string bigString, bool createNow){

		//divide into its data components
		//remove empty entries since at least the key should exist
		string[] attStringSeparators = {"<!>"};
		string[] fields = bigString.Split(attStringSeparators, System.StringSplitOptions.RemoveEmptyEntries);

		//split into key, value pair and add to data dictionary
		foreach(string field in fields){
			string[] keyValueSeparators = {"<!!!>"};
			string[] keyValues = field.Split(keyValueSeparators, System.StringSplitOptions.None);
			data.Add(keyValues[0], keyValues[1]);
		}

		if(createNow){
			LoadTextures();
		}
	}

	//this method is called upon creation of a data prefab
	//use it in the inherited class to add data to your gameobjects
	abstract public void Create();

	protected void DealWithImage(Texture2D tex, string url){
		imagesToDealWith.Add(url, tex);
	}

	//this method is used to safely destroy the texture in order to prevent memory leaks
	public void DestroyTextures(){
		foreach(var link in imagesToDealWith){
			if(link.Value != null){
				Destroy(link.Value);
			}
		}
		imagesDownloadsAllowed = false;
		imagesToDealWith.Clear();
	}

	public void LoadTextures(){
		Create();
		imagesDownloadsAllowed = true;
	}

	protected IEnumerator DownloadTexture(string url, string fileName, Action<Texture2D> onFinish, int attempt=0){

		WWW download = new WWW(url + fileName);
		yield return download;

		//check we haven't deleted images while waiting
		if(download.error != null){
			print(download.error);
			if(attempt < 10){
				StartCoroutine(DownloadTexture(url, fileName, onFinish, attempt+1));
			}else{
				print ("Download failed after " + attempt + " attempts");
			}
		}else{
			if(imagesDownloadsAllowed){
				print ("image downloaded");
				if(saveImagesLocally){
					SaveTexture(fileName, download.texture);
				}
				onFinish(download.texture);
			}
		}
		
	}

	protected IEnumerator DownloadTexture(string url, string fileName, Action<string, Texture2D> onFinish, int attempt=0){
		
		WWW download = new WWW(url + fileName);
		yield return download;

		//check we haven't deleted images while waiting
		if(download.error != null){
			print(download.error);
			if(attempt < 10){
				StartCoroutine(DownloadTexture(url, fileName, onFinish, attempt+1));
			}else{
				print ("Download failed after " + attempt + " attempts");
			}
		}else{
			if(imagesDownloadsAllowed){
				print ("image downloaded");
				if(saveImagesLocally){
					SaveTexture(fileName, download.texture);
				}
				onFinish(fileName, download.texture);
			}
		}

	}

	protected IEnumerator DownloadTexture(string url, string fileName, Action<GameObject, Texture2D> onFinish, GameObject target, int attempt){
		
		WWW download = new WWW(url + fileName);
		yield return download;

		//check we haven't deleted images while waiting
		if(download.error != null){
			print(download.error);
			if(attempt < 10){
				StartCoroutine(DownloadTexture(url, fileName, onFinish, target, attempt+1));
			}else{
				print ("Download failed after " + attempt + " attempts");
			}
		}else{
			if(imagesDownloadsAllowed){
				print ("image downloaded");
				if(saveImagesLocally){
					SaveTexture(fileName, download.texture);
				}
				onFinish(target, download.texture);
			}
		}
		
	}

	public IEnumerator LoadLocalTexture(string url, string fileName, Action<Texture2D> onFinish){
		print ("loading image locally");
		string path;
		if(Application.platform == RuntimePlatform.OSXEditor){
			path = "file://" + Application.dataPath + "/Images/";
		}else{
			path = "file://" + Application.persistentDataPath + "/";
		}
		
		WWW download = new WWW(path + fileName);
		yield return download;


		if(download.error != null){
			print(download.error);
			//find from web
			StartCoroutine(DownloadTexture(url, fileName, onFinish, 0));
		}else{
			if(imagesDownloadsAllowed){
				onFinish(download.texture);
			}
		}
		
	}
	
	protected void SaveTexture(string fileName, Texture2D texture){

		string path;
		if(Application.platform == RuntimePlatform.OSXEditor){
			path = Application.dataPath + "/Images/";
		}else{
			path = Application.persistentDataPath + "/";
		}

		Byte[] bytes = texture.EncodeToPNG();
		File.WriteAllBytes(path+fileName, bytes);
		print ("Saved image");
	}

	//used in GetCentredSubTexture
	private int ScaleAToB(double ratioA, int newB){
		return (int)Math.Floor(newB * ratioA);
	}

	protected string GetLabel(GameObject obj){
		return obj.GetComponent<UILabel>().text;
	}
	
	protected void SetLabel(GameObject to, string newText){
		UILabel lbl = to.transform.GetComponent<UILabel>();
		lbl.text = newText;
		lbl.MarkAsChanged();
	}
}


