using UnityEngine;
using System.Collections;

public class GoalieControl : MonoBehaviour {

	private Vector3 posicaoInicial;
	private Vector3 eulerAnglesInicial;
	
	void Awake () {
		eulerAnglesInicial = gameObject.transform.eulerAngles;
		posicaoInicial = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(gameObject.transform.eulerAngles.x <= 180){
			gameObject.transform.eulerAngles = eulerAnglesInicial;
		}
		if(gameObject.transform.position.y < -5 || gameObject.transform.position.y > 5 ){
			gameObject.transform.position = posicaoInicial;
		}	
	}
}
