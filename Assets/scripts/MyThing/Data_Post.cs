using UnityEngine;
using System.Collections;

public class Data_Post: Data{

	//reference to the Feed_Thing
	public Feed_Thing theFeed;

	public int id;
	public int rank;
	public int thingID;

	public itemType item1Type;
	public string item1Text;
	public int item1Size;

	public itemType item2Type;
	public string item2Text;
	public int item2Size;

	public itemType item3Type;
	public string item3Text;
	public int item3Size;

	private GameObject profilePage;

	private GameObject item1;
	private GameObject item2;
	private GameObject item3;

	private string imagesURL = "http://www.samdavies.info/social/images/";

	public override void Create(){

		id = int.Parse(data["id"]);
		thingID = int.Parse(data["thingID"]);

		item1Type = getType(data["item1Type"]);
		item1Text = data["item1Text"];

		item2Type = getType(data["item2Type"]);
		item2Text = data["item2Text"];

		item3Type = getType(data["item3Type"]);
		item3Text = data["item3Text"];

		//some strange need to activate panel after creation
		gameObject.transform.FindChild("LikeScroll").gameObject.GetComponent<UIPanel>().enabled = true;

		GameObject contentParent = gameObject.transform.FindChild("LikeScroll/Content").gameObject;
		addItem(contentParent, item1, item1Type, item1Text, -1);
		addItem(contentParent, item2, item2Type, item2Text, 0);
		addItem(contentParent, item3, item3Type, item3Text, 1);
	}

	public void ShowDetails(){
		theFeed.ShowDetails(this);
	}

	public itemType getType(string input){
		if (input.Equals("image")){
			return itemType.image;
		}else if(input.Equals("text")){
			return itemType.text;
		}else if(input.Equals("boyGirlRatio")){
			return itemType.boyGirlRatio;
		}else{
			return itemType.blank;
		}
		
	}
	
	public enum itemType{
		blank,
		image,
		text,
		boyGirlRatio
	}

	public void addItem(GameObject parent, GameObject item, itemType type, string text, int pos){

		if(type.Equals(itemType.image)){
			// download image
			item = NGUITools.AddChild(parent, theFeed.imagePrefab);
			StartCoroutine(DownloadTexture(imagesURL, text, addTexture, item, 0));
		}
		else if(type.Equals(itemType.text)){
			// add text component
			item = NGUITools.AddChild(parent, theFeed.textPrefab);
			SetLabel(item, text);
		}else{
			item = new GameObject();
		}

		//set the position
		int xPos = pos * 150;
		item.transform.localPosition = new Vector3(xPos, 0, 0);

	}

	public void addTexture(GameObject item, Texture2D tex){
		UITexture image = item.GetComponent<UITexture>();
		image.mainTexture = tex;
	}

	public void addLabel(GameObject item, string text){
		UILabel label = item.GetComponent<UILabel>();
		label.text = text;

		label.trueTypeFont = theFeed.textFont;
		label.fontSize = 18;
		label.color = new Color(0.3f, 0.3f, 0.3f, 1f);
		label.alignment = NGUIText.Alignment.Left;
		label.MarkAsChanged();
	}

	/*
	void OnClick(){
		DisplayFullProfile pageContent = new DisplayFullProfile(person, profilePage);
		pageContent.create();
	}*/
}
