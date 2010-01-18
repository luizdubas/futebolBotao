using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other){
		Debug.Log("GOL "+other.gameObject.name);
		GameStatus game = GameObject.Find("Game").GetComponent<GameStatus>();
		GameObject trigger = other.gameObject;
		if(game.bolaChutada && trigger.name == "TriggerGolTimeA"){
			game.gol(false);
		}else if(game.bolaChutada && trigger.name == "TriggerGolTimeB"){
			game.gol(true);
		}else{
			game.foraDeJogo();
		}
	}
}
