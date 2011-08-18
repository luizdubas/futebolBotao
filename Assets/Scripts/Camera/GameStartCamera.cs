using UnityEngine;
using System.Collections;

public class GameStartCamera : MonoBehaviour {
	 
	private GameStatus game;
	
	void Start(){
		game = GameObject.Find("Game").GetComponent<GameStatus>();
        StartCoroutine(changeCamera());
	}
	
	void Awake(){
		game = GameObject.Find("Game").GetComponent<GameStatus>();
        StartCoroutine(changeCamera());
	}

	
	IEnumerator changeCamera(){
		yield return new WaitForSeconds (2f);
		this.enabled = false;
		(Camera.main.GetComponent(typeof(SmoothFollow)) as SmoothFollow).enabled = true;
		game.gameStarted = true;
	}
	
	public void changeFollowObject(GameObject objeto){
		(GetComponent(typeof(SmoothFollow)) as SmoothFollow).target = objeto.transform;
	}
}
