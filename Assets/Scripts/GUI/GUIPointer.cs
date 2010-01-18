using UnityEngine;
using System.Collections;

public class GUIPointer : MonoBehaviour {

	public Texture2D mousePointer;
	private float posX = 0;
	private float posY = 0;
	
	void OnGUI () {
		posX = Input.mousePosition.x;
		posY = Screen.height-Input.mousePosition.y;
		GUI.Label(new Rect(posX,posY, 90, 90), mousePointer);
	}
}