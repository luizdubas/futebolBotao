using UnityEngine;
using System.Collections;

public class GameStartCamera : MonoBehaviour {
	
	private GameStatus game;
	
	void Awake(){
		game = GameObject.Find("Game").GetComponent<GameStatus>();
		Screen.showCursor = false;
	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine(changeCameraPosition());
	}
	
	IEnumerator changeCameraPosition(){
		yield return new WaitForSeconds (2f);
		// Move the object upward in world space 1 unit/second.
		Quaternion target = Quaternion.Euler (0, 270, 0);
		transform.Translate(new Vector3(3,0,-3)*Time.deltaTime, Space.Self);
		transform.rotation = Quaternion.Slerp(transform.rotation, target,Time.deltaTime);
		StartCoroutine(changeCamera());
	}
	IEnumerator changeCamera(){
		yield return new WaitForSeconds (10f);
		this.enabled = false;
		(GetComponent(typeof(SmoothFollow)) as SmoothFollow).enabled = true;
		game.gameStarted = true;
	}
}
