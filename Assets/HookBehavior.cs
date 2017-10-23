using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookBehavior : MonoBehaviour {

	Transform startMarker;
	Transform endMarker;
	float speed = 1.0F;
	float startTime;
	float journeyLength;
	public bool isInitialized;

	public void Init(Transform startPosition, Transform endPosition, int speed) {
		startMarker = startPosition;
		endMarker = endPosition;
		this.speed = speed;
		startTime = Time.time;
		journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
		isInitialized = true;
	}

	void Update() {
		if (isInitialized) {
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp (startMarker.position, endMarker.position, fracJourney);
		}
	}

}
