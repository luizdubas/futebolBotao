using UnityEngine;
using System.Collections;

public class MenuCamera : MonoBehaviour {

	float multiplicadorRotacao = -0.25f;
	
	// Use this for initialization 
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(transform.eulerAngles.y >= 315 || transform.eulerAngles.y <= 45){
			multiplicadorRotacao *= -1;
			Debug.Log("mudou rotação y = "+transform.eulerAngles.y);
		}
		transform.eulerAngles = new Vector3(15,transform.eulerAngles.y+multiplicadorRotacao,0);
	}
}
