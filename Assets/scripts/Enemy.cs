using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

	public Texture2D openMouth;
	public Texture2D closedMouth;

	private enum direction {
		LEFT = 1,
		RIGHT = -1,
		UP = 2,
		DOWN = -2,
		NONE = 0
	}

	private direction[] directions = { direction.LEFT, direction.RIGHT, direction.UP, direction.DOWN };


	private Dictionary<direction, Vector2> directionToVelocity;
	private Dictionary<direction, Quaternion> directionToRotation;
	private Image loseScreen;


	private direction nextDirection = direction.NONE;


	private Collider2D collider;
	private Rigidbody2D rb;
	private SpriteRenderer sr;


	// temp
	public float speed;


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
		sr = gameObject.GetComponent<SpriteRenderer> ();
		loseScreen = GameObject.FindGameObjectWithTag ("youLose").GetComponent<Image> ();

		goInDirection (direction.NONE);
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.collider.gameObject.name.StartsWith ("player")) {
			coll.collider.gameObject.GetComponentInParent<Player> ().enabled = false;
			YouLose ();
		}

		tryToUpdateDirection ();
	}

	private void tryToUpdateDirection() {
		goInDirection (chooseNextDirection ());
	}

	private void goInDirection(direction d) {
		if (d != direction.NONE) {
			transform.rotation = directionToRotation [d];
			//sr.transform.rotation = Quaternion.Euler(-(directionToRotation [d].eulerAngles));
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

	private direction chooseNextDirection() {
		List<direction> l = new List<direction> ();

		foreach (direction d in directions) {
			if (((int) d) != -((int) getDirection ()) && !blockedInDirection (d)) {
				l.Add (d);
			}
		}

		// hopefully avoids problems
		if (l.Count == 0) {
			l.Add (directions[Random.Range(0, directions.Length)]);
		}
			
		return l[Random.Range (0, l.Count)];
	}

	// Update is called once per frame
	void Update () {
		if (getDirection() == direction.NONE) {
			tryToUpdateDirection ();
		}
	}

	void YouLose() {
		loseScreen.enabled = true;
	}
}
