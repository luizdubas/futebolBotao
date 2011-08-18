using UnityEngine; 
using System.Collections;

public class Apresentacao : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		StartCoroutine(wait());
	}
	
	void Update(){
		if(Input.GetKeyDown(KeyCode.Escape)){
			StopCoroutine("wait");
			Application.LoadLevel("startscreen");
		}
	}
	
	IEnumerator wait(){
        yield return new WaitForSeconds(4f);
		Application.LoadLevel("startscreen");
	}
}
