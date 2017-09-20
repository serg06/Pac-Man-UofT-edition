using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public Texture2D openMouth;
	public Texture2D closedMouth;

	private enum direction {
		LEFT,
		RIGHT,
		UP,
		DOWN,
		NONE
	}


	private Dictionary<direction, Vector2> directionToVelocity;
	private Dictionary<direction, Quaternion> directionToRotation;


	private direction nextDirection = direction.NONE;
	private Text week;
	private Image winScreen;

	private Collider2D collider;
	private Rigidbody2D rb;

	// temp
	public float speed;
	private int weekLength = 5;


	void Start () {
		directionToVelocity = new Dictionary<direction, Vector2> () {
			{direction.LEFT, new Vector2(-1, 0)},
			{direction.RIGHT, new Vector2(1, 0)},
			{direction.UP, new Vector2(0, 1)},
			{direction.DOWN, new Vector2(0, -1)},
			{direction.NONE, new Vector2(0, 0)}
		};

		directionToRotation = new Dictionary<direction, Quaternion> () {
			{ direction.LEFT, Quaternion.Euler(new Vector3 (0, 0, 180)) },
			{ direction.RIGHT, Quaternion.Euler(new Vector3 (0, 0, 0)) },
			{ direction.UP, Quaternion.Euler(new Vector3 (0, 0, 90)) },
			{ direction.DOWN, Quaternion.Euler(new Vector3 (0, 0, 270)) }
		};

		rb = gameObject.GetComponent<Rigidbody2D> ();
		collider = gameObject.GetComponent<Collider2D> ();

		week = GameObject.FindGameObjectWithTag ("week").GetComponent<Text> ();
		winScreen = GameObject.FindGameObjectWithTag ("youWin").GetComponent<Image> ();

		goInDirection (direction.NONE);
		Invoke ("updateWeeks", weekLength);
	}

	void OnCollisionEnter2D (Collision2D coll) {
		tryToUpdateDirection ();
	}

	private void tryToUpdateDirection() {
		direction next = nextDirection;

		direction current = getDirection ();
		bool blockedCurrent = blockedInDirection (current);
		bool blockedNext = blockedInDirection (next);

		if (next != direction.NONE && !blockedInDirection (next)) {
			goInDirection (next);
			nextDirection = direction.NONE;
		} else if (blockedInDirection (getDirection ())) {
			goInDirection (direction.NONE);
			nextDirection = direction.NONE;
		}
			
	}

	private void goInDirection(direction d) {
		if (d != direction.NONE) {
			transform.rotation = directionToRotation [d];
		}

		rb.velocity = directionToVelocity [d] * speed;
	}

	private bool blockedInDirection(direction d) {
		Vector2 circlePoint = transform.position;
		//circlePoint += new Vector2 (0.08f, -1f);
		circlePoint += directionToVelocity [d] * 0.06f;
		//circlePoint = Camera.main.ScreenToWorldPoint (circlePoint);
		Collider2D[] collidersInDirection = Physics2D.OverlapCircleAll (circlePoint, 0.04f);

		foreach (Collider2D collision in collidersInDirection) {
			if (collision.gameObject.name.StartsWith ("wall")) {
				return true;
			}
		}

		return false;
	}

	private direction getDirection() {
		if (rb.velocity.x > 0) {
			return direction.RIGHT;
		} else if (rb.velocity.x < 0) {
			return direction.LEFT;
		} else if (rb.velocity.y > 0) {
			return direction.UP;
		} else if (rb.velocity.y < 0) {
			return direction.DOWN;
		} else {
			return direction.NONE;
		}
	}
	
	// Update is called once per frame
	void Update () {
		direction next = nextDirection;

		if (Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow)) {
			nextDirection = direction.UP;
		} else if (Input.GetKeyDown (KeyCode.S) || Input.GetKeyDown (KeyCode.DownArrow)) {
			nextDirection = direction.DOWN;
		} else if (Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.LeftArrow)) {
			nextDirection = direction.LEFT;
		} else if (Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.RightArrow)) {
			nextDirection = direction.RIGHT;
		}

		if (getDirection() == direction.NONE) {
			tryToUpdateDirection ();
		}
	}

	private void updateWeeks() {
		char[] delimeters = { ' ', '/' };
		string w = week.text;
		string wnum = w.Split(delimeters, 3)[1];
		int wnum_ = int.Parse (wnum);

		if (wnum_ == 12) {
			week.text = "YOU WIN!";
			YouWin ();
		} else {
			week.text = "Week " + (wnum_ + 1) + "/12";
			Invoke ("updateWeeks", weekLength);
		}
	}

	private void YouWin() {
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy")) {
			go.GetComponent<Enemy> ().enabled = false;
		}
		winScreen.enabled = true;
	}
}
