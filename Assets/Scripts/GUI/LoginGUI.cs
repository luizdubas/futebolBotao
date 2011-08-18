using UnityEngine; 
using System;
using System.Collections;

public class LoginGUI : MonoBehaviour {

	private string serverIP = "127.0.0.1";
	private string serverPort = "3000";
	public bool debug = true;

	public GUISkin gSkin;
	private SharedData data;

	void Awake() {
		data = GameObject.FindWithTag ("data").GetComponent<SharedData>();
	}

	void OnGUI() {

		GUI.skin = gSkin;
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (Screen.width / 1024f, Screen.height / 768f, 1));
		
		GUI.Box(new Rect(0f, 26f, 370, 36),"",gSkin.GetStyle("customBox"));
		GUI.Label(new Rect(0f,20f,370,40),label.multiplayer);

		if (Network.peerType == NetworkPeerType.Disconnected || Network.peerType == NetworkPeerType.Connecting) {
			if(Network.peerType == NetworkPeerType.Disconnected){
				GUI.Label(new Rect(0, 70, 200, 40), "IP: ");
				serverIP = GUI.TextField(new Rect(140, 68, 400, 50), serverIP, 25);
				
				GUI.Label(new Rect(0, 120, 200, 40), "Port: ");
				serverPort = GUI.TextField(new Rect(140,118, 400, 50), serverPort, 25);
				
				if (GUI.Button(new Rect(-20, 176, 370, 40), "")  || (Event.current.type == EventType.keyDown && Event.current.character == '\n')) {
					Network.Connect(serverIP, Int32.Parse(serverPort));
				}
				GUI.Label(new Rect(0f,170f,370,40),label.conectar);
				
				if (GUI.Button(new Rect(-40, 226, 370, 40), "")) {
					Network.InitializeSecurity();
					Network.InitializeServer(2, Int32.Parse(serverPort));
				}
				GUI.Label(new Rect(0f,220f,370,40),label.iniciarServidor);
			}else{
				GUI.Label(new Rect(0, 70, 200, 40), label.conectando);
			}
			
			if (GUI.Button(new Rect(-60, 276, 370, 40), "")) {
				Network.Disconnect();
				GameObject.Destroy(data);
				Application.LoadLevel("startscreen");
			}
			GUI.Label(new Rect(0f,270f,370,40),label.cancelar);
		
		}
		else{
			if(Network.connections.Length >= 1)
				Application.LoadLevel("selectscreen");
			else{
				GUI.Label(new Rect(0, 70, 600, 50), label.esperando);
			
			if (GUI.Button(new Rect(-60, 126, 370, 40), "")) {
				Network.Disconnect();
				GameObject.Destroy(data);
				Application.LoadLevel("startscreen");
			}
			GUI.Label(new Rect(0f,120f,370,40),label.cancelar);
			}
		}
	}
}