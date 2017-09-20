using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SpriteChooser : MonoBehaviour {

	public enum typee {
		TOP_LEFT_CORNER,
		TOP_RIGHT_CORNER,
		BOTTOM_RIGHT_CORNER,
		BOTTOM_LEFT_CORNER,
		HORIZONTAL,
		VERTICAL,
		BLANK
	}

	public typee type;

	public Sprite topLeftCorner;
	public Sprite topRightCorner;
	public Sprite bottomRightCorner;
	public Sprite bottomLeftCorner;
	public Sprite horizontalWall;
	public Sprite verticalWall;
	public Sprite blankWall;

	// choose texture
	void Start () {
		Sprite chosenSprite = new Dictionary<typee, Sprite> () {
			{ typee.TOP_LEFT_CORNER, topLeftCorner },
			{ typee.TOP_RIGHT_CORNER, topRightCorner },
			{ typee.BOTTOM_RIGHT_CORNER, bottomRightCorner },
			{ typee.BOTTOM_LEFT_CORNER, bottomLeftCorner },
			{ typee.HORIZONTAL, horizontalWall },
			{ typee.VERTICAL, verticalWall },
			{ typee.BLANK, blankWall }
		} [type];

		Assert.IsNotNull (chosenSprite);
		gameObject.GetComponent<SpriteRenderer> ().sprite = chosenSprite;
	}

}
