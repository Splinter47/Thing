using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PostMaker : MonoBehaviour {

	int layout = 0;
	public GameObject imagePrefab;
	public GameObject textPrefab;
	public GameObject tableObj;
	public UITable table;

	public int textInset;

	public Texture2D tempImage;

	SortedList<string, Item> items = new SortedList<string, Item>();

	public Item selected;

	void Awake(){
		table = tableObj.GetComponent<UITable>();
	}

	public void AddImageItem(){
		AddItem(imagePrefab);
	}

	public void AddTextItem(){
		AddItem(textPrefab);
	}

	private void AddItem(GameObject prefab){
		if(items.Count < 3){
			GameObject justAdded = NGUITools.AddChild(tableObj, prefab);
			Item newItem = justAdded.GetComponent<Item>();
			newItem.postMaker = this;

			//temp
			newItem.rawImage = tempImage;

			//resize all items and return new item width
			int rawWidth = GetNewItemWidth();

			//set the new items width
			newItem.SetWidth(rawWidth, (int)(table.padding.x * 2), textInset);

			//add the item to the list
			string newKey = items.Count.ToString();
			newItem.SetOrder((string) newKey);
			items.Add(newKey, newItem);

			//sort the table
			table.Reposition();

			SimpleUpdateLayout();
		}
	}

	private void SimpleUpdateLayout(){
		if(items.Count == 1){
			layout = 0;
		}else if(items.Count == 2){
			layout = 1;
		}else if(items.Count == 3){
			layout = 4;
		}
	}

	private int GetNewItemWidth(){
		//resize the old widths
		if(items.Count == 0){
			return 450;
		}else if(items.Count == 1){
			SetAllItemWidths(225, (int)(table.padding.x * 2), textInset);
			return 225;
		}else{
			SetAllItemWidths(150, (int)(table.padding.x * 2), textInset);
			return 150;
		}
	}

	private void SetAllItemWidths(int newWidth, int padding, int textInset){
		foreach(var item in items){
			item.Value.SetWidth(newWidth, padding, textInset);
		}
	}

	public void RemoveItem(){
		//two pushes ensure selected is at last index
		if(items.Count > 0){
			PushRight();
		}
		if(items.Count > 1){
			PushRight();
		}

		items.RemoveAt(items.IndexOfValue(selected));

		NGUITools.Destroy(selected.gameObject);

		SimpleUpdateLayout();

		//resize the old widths
		if(items.Count == 1){
			SetAllItemWidths(450, (int)(table.padding.x * 2), textInset);
		}else if(items.Count == 2){
			SetAllItemWidths(225, (int)(table.padding.x * 2), textInset);
		}else{
			SetAllItemWidths(150, (int)(table.padding.x * 2), textInset);
		}

		//apply
		table.Reposition();
	}

	public void PushLeft(){
		int index = items.IndexOfValue(selected);
		if(index > 0){
			Swap(index, index-1);
		}
	}

	public void PushRight(){
		int index = items.IndexOfValue(selected);
		if(index < (items.Count-1)){
			Swap(index, index+1);
		}
	}

	private void Swap(int current, int swapWith){
		//save values
		Item newValue1 = items.Values[current];
		Item newValue2 = items.Values[swapWith];
		
		//swap keys
		string newKey1 = items.Keys[swapWith];
		string newKey2 = items.Keys[current];
		
		//remove larger index first
		if(swapWith>current){
			items.Remove(items.Keys[swapWith]);
			items.Remove(items.Keys[current]);
		}else{
			items.Remove(items.Keys[current]);
			items.Remove(items.Keys[swapWith]);
		}
		
		//add new
		items.Add(newKey1, newValue1);
		items.Add(newKey2, newValue2);
		
		//update recorded orders
		items.Values[current].SetOrder(items.Keys[current]);
		items.Values[swapWith].SetOrder(items.Keys[swapWith]);

		//apply
		table.Reposition();
	}

	public void GrowItem(){
		if(items.Count == 2){
			Grow(selected);
		}
	}
	
	public void ShrinkItem(){
		if(items.Count == 2){
			int index = items.IndexOfValue(selected);

			if(index == 0){
				Grow(items.Values[1]);
			}else{
				Grow(items.Values[0]);
			}
		}
	}

	private void Grow(Item toGrow){
		//get the index
		int index = items.IndexOfValue(toGrow);
		
		// layout = [---][---]
		if(layout == 1){
			toGrow.SetWidth(300, (int)(table.padding.x * 2), textInset);
			if(index == 0){
				items.Values[1].SetWidth(150, (int)(table.padding.x * 2), textInset);
				layout = 2;
			}
			if(index == 1){
				items.Values[0].SetWidth(150, (int)(table.padding.x * 2), textInset);
				layout = 3;
			}
		}else 
			
			// layout = [----][--]
		if(layout == 2){
			if(index == 1){
				toGrow.SetWidth(225, (int)(table.padding.x * 2), textInset);
				items.Values[0].SetWidth(225, (int)(table.padding.x * 2), textInset);
				layout = 1;
			}
		}else
			
			// layout = [--][---]
		if(layout == 3){
			if(index == 0){
				toGrow.SetWidth(225, (int)(table.padding.x * 2), textInset);
				items.Values[1].SetWidth(225, (int)(table.padding.x * 2), textInset);
				layout = 1;
			}
		}
		
		//apply
		table.Reposition();
	}

	public void ConvertToPost(){

	}
}
