using UnityEngine; 
using System.Collections;

public class GUIPointer : MonoBehaviour {

	public Texture2D mousePointer;
	private float posX = 0;
	private float posY = 0;
	
	void Start(){
		//Screen.showCursor = false;
	}
	
	void OnGUI () {
		/*posX = Input.mousePosition.x;
		posY = Screen.height-Input.mousePosition.y-3;
		GUI.Label(new Rect(posX,posY, 45, 45), mousePointer);*/
	}
}