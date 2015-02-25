using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public abstract class Data : MonoBehaviour {
	
	//protected string[] data;
	protected Dictionary<string,string> data = new Dictionary<string, string>();
	public bool saveImagesLocally = false;
	public List<string> taxonomies;

	//visual elements to display on seperate page
	public GameObject[] displayElementLinks;
	protected Dictionary<string, GameObject> displayElements = new Dictionary<string, GameObject>();

	//list of images
	protected Dictionary<string,Texture2D> imagesToDealWith = new Dictionary<string, Texture2D>();
	public bool imagesExist{ get; private set; }
	private bool imagesAllowed = false;
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
		imagesExist = false;
		imagesAllowed = false;
		imagesToDealWith.Clear();
	}
	
	public void LoadTextures(){
		Create();
		imagesExist = true;
		imagesAllowed = true;
	}
	
	private bool StopMultiDownloads(){
		if(imagesAllowed){
			imagesAllowed = false;
			return true;
		}else{
			return false;
		}
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
			if(imagesExist && StopMultiDownloads()){
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
			if(imagesExist && StopMultiDownloads()){
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
			if(imagesExist && StopMultiDownloads()){
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
			if(imagesExist && StopMultiDownloads()){
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

	//creates a new texture in the centre of the old one based on the height and width given, 
	//(warning) computationally intensive therefore by default has been hidden
	/*public Texture2D GetCentredSubTexture(Texture2D inputTex, int newWidth, int newHeight){

		int targetWidth = newWidth;
		int targetHeight = newHeight;

		//differnce between current width/height and new width/height
		int widthDiff = inputTex.width - newWidth;
		int heighDiff = inputTex.height - newHeight;

		//ratio between current width/height and new width/height
		double widthRatio = (1.0*inputTex.width)/newWidth;
		double heighRatio = (1.0*inputTex.height)/newHeight;

		//check the new dimensions are smaller
		if(widthRatio < 1 || heighRatio < 1){
			if(widthRatio<1 && heighRatio < 1){
				//both too big here
				if(widthRatio<heighRatio){
					//calc from old width
					newHeight = ScaleAToB(widthRatio, newHeight);
					newWidth = inputTex.width;
				}else{
					//calc from old height
					newWidth = ScaleAToB(heighRatio, newWidth);
					newHeight = inputTex.height;
				}
			}else if(widthRatio<1){
				//calc from old width
				newHeight = ScaleAToB(widthRatio, newHeight);
				newWidth = inputTex.width;
			}else{
				//calc from old height
				newWidth = ScaleAToB(heighRatio, newWidth);
				newHeight = inputTex.height;
			}

			//recalculate
			widthDiff = inputTex.width - newWidth;
			heighDiff = inputTex.height - newHeight;
		}

		//find centre
		int x = (widthDiff)/2;
		int y = (heighDiff)/2;

		Color[] pix = inputTex.GetPixels(x, y, newWidth, newHeight);
		Texture2D destTex = new Texture2D(newWidth, newHeight);
		destTex.SetPixels(pix);
		destTex.Apply();

		print ("Target: (" + targetWidth + "," + targetHeight + ")" + 
		       " max: (" + inputTex.width + "," + inputTex.height + ")" +
		       " Actual: (" + newWidth + "," + newHeight + ")");

		return destTex;
	}*/

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


