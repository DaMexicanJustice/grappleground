using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// public
	public int speed;
	public GameObject jointPrefab;
	public bool visualizeRope;
	public GameObject jointContainer;
	public int maxJoints = 10;
	public GameObject hookPrefab;
	public int projectileSpeed;
	public int jumpForce;
	public int maxSpeed;

	// private
	Rigidbody2D rb2d;
	LineRenderer lineRenderer;
	List<GameObject> joints = new List<GameObject>();
	bool isAttachedToRope = false;
	bool isRopeFired;
	bool isGrounded;
	Transform hookObject;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		lineRenderer = GetComponent<LineRenderer> ();
	}

	// Update is called once per frame
	void Update () {
		// Movement
		float horizontal = Input.GetAxis ("Horizontal");
		rb2d.AddForce (new Vector2 (horizontal, 0) * speed);
		if (rb2d.velocity.magnitude > maxSpeed) {
			rb2d.velocity = rb2d.velocity.normalized * maxSpeed;
		}

		// Jump/ Hook
		Aim();
		if (Input.GetMouseButtonDown(0)) {
			Debug.Log ("Mouse time");
			if (!isRopeFired) {
				ShootHook ();
			} else {
				if (isAttachedToRope) {
					DetachPlayerFromRope ();
				}
			}
		} 

		if (Input.GetKeyDown (KeyCode.Space)) {
			Jump ();
		}

		if (visualizeRope && isAttachedToRope) {
			// Line Renderer
			lineRenderer.SetPosition (0, new Vector3 (transform.position.x, transform.position.y, 0));
			lineRenderer.SetPosition (1, new Vector3 (hookObject.position.x, hookObject.position.y, 0));
		}
	}

	void CalculateJoints() {
		Vector2 direction = hookObject.position - transform.position;
		float distance = direction.magnitude;
		float jointLength = 1f;
		int numberOfJoints = Mathf.RoundToInt(distance / jointLength);
		if (numberOfJoints > maxJoints) {
			numberOfJoints = maxJoints;
		}

		Vector2 spawnPosition = hookObject.position;
		for (int i = 1; i <= numberOfJoints; i++) {
			GameObject obj = Instantiate (jointPrefab, spawnPosition, Quaternion.identity);
			if (joints.Count > 0) {
				obj.transform.parent = joints [joints.Count - 1].transform;
				//obj.GetComponent<HingeJoint2D> ().connectedBody = joints [joints.Count - 1].GetComponent<Rigidbody2D> ();
				joints[joints.Count - 1].GetComponent<HingeJoint2D>().connectedBody = obj.GetComponent<Rigidbody2D>();
			} else {
				obj.transform.parent = jointContainer.transform;
				obj.GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Kinematic;
			}
			joints.Add (obj);
			spawnPosition = new Vector2 (hookObject.position.x - direction.normalized.x * i , hookObject.position.y - direction.normalized.y * i);
		}
	}

	void AttachPlayerToRope() {
		if (joints.Count > 0) {
			joints [joints.Count - 1].GetComponent<HingeJoint2D> ().connectedBody = GetComponent<Rigidbody2D> ();
			isAttachedToRope = true;
		}
	}

	void DetachPlayerFromRope() {
		if (joints.Count > 0) {
			joints [joints.Count - 1].GetComponent<HingeJoint2D> ().connectedBody = null;
			isAttachedToRope = false;
			isRopeFired = false;
			lineRenderer.SetPosition (0, Vector3.zero);
			lineRenderer.SetPosition (1, Vector3.zero);
		}
	}

	void Aim() {
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePosition.z = 0;
		Debug.DrawRay(transform.position, mousePosition - transform.position);
	}

	void ShootHook() {
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePosition.z = 0;
		RaycastHit2D hit2d = Physics2D.Raycast (transform.position, mousePosition - transform.position, 10f);
		if (hit2d.collider != null) {
			GameObject obj = Instantiate (hookPrefab, transform.position, Quaternion.identity);
			HookBehavior hookBehavior = obj.AddComponent<HookBehavior> ();
			hookBehavior.Init (transform, hit2d.collider.transform, projectileSpeed);
			isRopeFired = true;
			hookObject = obj.transform;
			Invoke ("CalculateJoints", GetJourneyLength (Vector3.Distance(transform.position, hit2d.transform.position), projectileSpeed));
			Invoke ("AttachPlayerToRope", GetJourneyLength (Vector3.Distance(transform.position, hit2d.transform.position), projectileSpeed));
		}
	}

	float GetJourneyLength(float distance, int speed) {
		Debug.Log (distance / speed);
		return distance / speed;
	}

	void Jump() {
		RaycastHit2D hit2d = Physics2D.Raycast (transform.position, Vector2.down, 1f);
		if (hit2d.collider != null) {
			isGrounded = true;
		} else {
			isGrounded = false;
		}

		if (isGrounded) {
			Debug.Log ("Let's jump");
			rb2d.AddForce (Vector2.up * jumpForce, ForceMode2D.Impulse);
		} else {
			Debug.Log ("Can't jump");
		}
	}
}
