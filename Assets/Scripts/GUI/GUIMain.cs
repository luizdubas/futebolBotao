using UnityEngine;
using System.Collections;

public class GUIMain : MonoBehaviour {

	public GUISkin guiSkin;
	public float areaWidth;
	public float areaHeight;
	
	void OnGUI(){
		GUI.skin = guiSkin;
		GUILayout.BeginArea (new Rect (0f, 50f, areaWidth, areaHeight));
		if(GUILayout.Button ("Jogar Amistoso"))
		{
			Application.LoadLevel("selectscreen");
		}
		if(GUILayout.Button ("Sair"))
		{
			Application.Quit();
		}
		GUILayout.EndArea();
	}
}
