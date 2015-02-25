using UnityEngine;
using System.Collections;

public class HomeMenu : MonoBehaviour {

	public GameObject homePanel;

	public TweenAlpha homeButton;

	public UITweener button1;
	public UITweener button2;

	public TweenAlpha button3;
	public TweenHeight button3h;
	public TweenWidth button3w;

	public UITweener button4;
	public UITweener button5;

	public UITweener circle1;
	public UITweener circle2;
	public UITweener square;

	private bool isOpen = false;

	void Start(){
		DontDestroyOnLoad(homePanel);
	}

	public void loadMyThing(){
		Application.LoadLevel("MyThingV4");
	}

	public void ToggleMenu(){
		print ("toggling");
		if(!isOpen){
			OpenMenu();
			isOpen = true;
		}else{
			CloseMenu();
			isOpen = false;
		}
	}

	private void OpenMenu(){
		//move background
		PlayBackground();

		//hide home
		homeButton.PlayForward();

		//show buttons
		Invoke("Play3", 0f);
	}

	private void CloseMenu(){
		//move background
		ReverseBackground();
		
		//hide home
		Invoke("ReverseHome", 0.3f);
		
		//show buttons
		button3h.PlayReverse(); 
		button3w.PlayReverse();
		Invoke("Reverse3", 0.3f);
	}

	private void ReverseBackground(){
		circle1.delay = 0.2f;
		circle1.duration = 0.4f;
		circle1.PlayReverse();
		circle2.delay = 0.2f;
		circle2.duration = 0.4f;
		circle2.PlayReverse();
		square.delay = 0.2f;
		square.duration = 0.4f;
		square.PlayReverse();
	}

	private void PlayBackground(){
		circle1.delay = 0f;
		circle1.duration = 0.6f;
		circle1.PlayForward();
		circle2.delay = 0f;
		circle2.duration = 0.6f;
		circle2.PlayForward();
		square.delay = 0f;
		square.duration = 0.6f;
		square.PlayForward();
	}

	private void ReverseHome(){
		homeButton.PlayReverse();
	}

	private void Play3(){ button3.PlayForward(); button3h.PlayForward(); button3w.PlayForward();}

	private void Reverse3(){ button3.PlayReverse();}
}
