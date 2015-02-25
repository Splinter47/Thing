//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ever wanted to be able to auto-center on an object within a draggable panel?
/// Attach this script to the container that has the objects to center on as its children.
/// </summary>

public class UISnapToChild : MonoBehaviour
{
	public ScrollRect scrollRect;
	//GameObject centredObject;
	public GridLayoutGroup grid;

	//grid speed
	private Vector3 currentSpeed;
	private Vector3 lastPosition;

	private bool canSnap = false;
	private bool isMoving = false;
	

	public void HasDragged(){
		print ("dragging");
	}
	
	void Start(){
		lastPosition = grid.transform.localPosition;
	}

	void Update(){
		Vector3 currentSpeed = (lastPosition - grid.transform.localPosition)/Time.deltaTime;
		lastPosition = grid.transform.localPosition;

		if(currentSpeed.y!=0f){
			isMoving = true;
		}

		if(Mathf.Abs(currentSpeed.y) < 1f && isMoving){
			print ("snapping");
			SnapToGrid();
			isMoving = false;
		}


	}

	private void SnapToGrid(){
		scrollRect.intertia = false;
		Invoke("DelayedTurnOnInertia", 0.2f);

		//find the closest snap
		//grid.cellSize.y;
		RectTransform rectTran = (RectTransform)grid.transform;
		rectTran.anchoredPosition = new Vector2(0,153f);
	}

	private void DelayedTurnOnInertia(){
		scrollRect.intertia = true;
	}
	

}
