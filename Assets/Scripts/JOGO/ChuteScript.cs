using UnityEngine;
using System.Collections;

public class ChuteScript : MonoBehaviour {
	
	private GameStatus game;
	public GameObject goleiroUtilizado;
	public string nomeCollider;
	private bool chutePronto;
	private Vector3 goleiroPosition;
	private Quaternion rotacaoOriginal;
	public SharedData data;
	private bool turnoDoJogador;
	
	public AudioClip prontoTimeA;
	public AudioClip prontoTimeB;
	
	void Awake(){
		game = GameObject.Find("Game").GetComponent<GameStatus>();
		data = GameObject.FindWithTag ("data").GetComponent<SharedData>();
	}
	
	// Update is called once per frame
	void Update() {
		if(!game.gameStarted || !game.chute || game.goleiroPronto || game.pauseState != GameStatus.PauseState.none)
			return;
		
		turnoDoJogador = (Network.peerType == NetworkPeerType.Disconnected || (Network.isServer && !game.turnoTimeA) || (Network.isClient && game.turnoTimeA));
		if(turnoDoJogador){
			if (Input.GetMouseButton(1)){
				goleiroUtilizado.transform.RotateAround(goleiroUtilizado.transform.position,new Vector3(0,1,0),(Input.GetAxis("Mouse Y")*Mathf.Rad2Deg)*0.2f);
			}
			if(Input.GetMouseButton(0)){
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hitInfo = new RaycastHit();
				if (Physics.Raycast (ray,out hitInfo,Mathf.Infinity)) {
					float x = hitInfo.point.x, z = hitInfo.point.z;
					if((game.turnoTimeA && x > 41.5) || (!game.turnoTimeA && x < -41.5)){
							if(game.turnoTimeA) x = 41.5f;
							else x = -41.5f;
					}else if((game.turnoTimeA && x < 37f) || (!game.turnoTimeA && x > -37f)){
							if(game.turnoTimeA) x = 37f;
							else x = -37f;
					}
					
					if(z > 5.5f){
							z = 5.5f;
					}else if(z < -5.5f){
							z = -5.5f;
					}
					goleiroUtilizado.transform.position = new Vector3(x,1,z);
					goleiroUtilizado.transform.eulerAngles = new Vector3(0,goleiroUtilizado.transform.eulerAngles.y,0);
				}
			}else{
				if(Input.GetButtonDown("Pronto")){
					if(Network.peerType == NetworkPeerType.Disconnected)
						avisaPronto();
					else
						networkView.RPC("avisaPronto",RPCMode.All);
				}
			}
		}
	}
	
	[RPC]
	void avisaPronto(){
		SendMessage("show","pronto");
		AudioClip clipUtilizado = prontoTimeA;
		if(game.turnoTimeA) clipUtilizado = prontoTimeB;
		game.GetComponent<AudioSource>().clip = clipUtilizado;
		game.GetComponent<AudioSource>().Play();
		game.goleiroPronto = true;
	}
}
