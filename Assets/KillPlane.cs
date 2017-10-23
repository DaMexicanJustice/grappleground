using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col) {
		UnityEngine.SceneManagement.SceneManager.LoadScene ("Main");
	}
}
