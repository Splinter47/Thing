using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Friend : MonoBehaviour {

	public Data_Home theData;
	public RawImage image;
	private string friend_id;
	private string imageStr;

	public void Create(string input){
		string[] attSeparators = {"|friend_id|"};
		string[] idImagePair = input.Split(attSeparators, System.StringSplitOptions.None);

		friend_id = idImagePair[0];
		imageStr = idImagePair[1];
		GetImages();
	}

	public void GetImages(){
		theData.DownloadFriendImage(imageStr, AfterDownload);
	}

	private void AfterDownload(string fileName, Texture2D newTex){
		image.texture = newTex;
		
		//image url needs to be set first
		theData.DealWithFriendImage(newTex, ("friend"+friend_id+fileName));
	}
}
