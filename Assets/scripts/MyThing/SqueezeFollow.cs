using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SqueezeFollow : MonoBehaviour {

	public RectTransform follower;
	public RawImage followerImage;
	public RectTransform target;

	public float minHeight;
	public float baseTargetY;
	private float baseHeight;

	private float imageHeight = 401f;

	void Start () {
		baseHeight = follower.sizeDelta.y;
	}
	
	void Update () {
		//change height by the tagets y pos
		float height = Mathf.Clamp(imageHeight - target.anchoredPosition.y, minHeight, imageHeight);
		follower.sizeDelta = new Vector2(follower.sizeDelta.x, height);

		//resize image rect
		float uvX = followerImage.uvRect.x;
		float uvW = followerImage.uvRect.width;

		float uvH = height/imageHeight;
		float uvY = (1-uvH)/2;
		followerImage.uvRect = new Rect(uvX, uvY, uvW, uvH);
	}
}
