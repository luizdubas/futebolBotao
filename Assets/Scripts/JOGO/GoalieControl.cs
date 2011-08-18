using UnityEngine;
using System.Collections;

public class GoalieControl : MonoBehaviour {

	private Vector3 posicaoInicial;
	private Vector3 eulerAnglesInicial;
	private GameStatus game;
	
	void Awake () {
		game = GameObject.Find("Game").GetComponent<GameStatus>();
		eulerAnglesInicial = gameObject.transform.eulerAngles;
		posicaoInicial = gameObject.transform.position;
	}
	
	// Update is called once per frame 
	void Update () {
		if(game.tipoToque == GameStatus.TipoToque.tiroDeMeta && gameObject.transform.position != posicaoInicial){
			gameObject.transform.position = new Vector3(posicaoInicial.x,posicaoInicial.y,posicaoInicial.z);
		}
		
		float eulerAngleX = gameObject.transform.eulerAngles.x;
		if(eulerAngleX >= 10 && eulerAngleX <= 350){
			gameObject.transform.eulerAngles = eulerAnglesInicial;
		}
		if(gameObject.transform.position.x < -50 || gameObject.transform.position.x > 50 ){
			gameObject.transform.position = new Vector3(posicaoInicial.x,0.5f,posicaoInicial.z);
		}
		if(gameObject.transform.position.z < -30 || gameObject.transform.position.z > 30 ){
			gameObject.transform.position = new Vector3(posicaoInicial.x,0.5f,posicaoInicial.z);
		}	
		if(gameObject.transform.position.y < -1 || gameObject.transform.position.y > 10 ){
			gameObject.transform.position = new Vector3(posicaoInicial.x,0.5f,posicaoInicial.z);
		}
		if(Network.peerType != NetworkPeerType.Disconnected && !game.turnoDoJogador){
			networkView.RPC("sincroniza",RPCMode.Others,transform.position,transform.eulerAngles,rigidbody.velocity,rigidbody.angularVelocity);
		}
	}
	[RPC]
	void sincroniza(Vector3 syncPOS, Vector3 syncEA, Vector3 syncV, Vector3 syncAV){
		transform.position = syncPOS;
		transform.eulerAngles = syncEA;
		rigidbody.velocity = syncV;
		rigidbody.angularVelocity = syncAV;
	}
	
	void OnCollisionEnter(Collision collision){
		if(game.chute){
			bool timeA = true;
			float soma = -2f;
			if(gameObject.rigidbody.tag == "jogador time b"){ soma = 2f; timeA = false; }
			Vector3 position = gameObject.transform.position;
			if(collision != null && collision.rigidbody != null){
				Debug.Log("GOLEIRO MATOU UM tag = "+collision.rigidbody.tag );
				if(collision.rigidbody.tag == "jogador time a" || collision.rigidbody.tag == "jogador time b"){
					gameObject.transform.position = new Vector3(position.x+soma, 0.3f,position.z);
				}else if(collision.rigidbody.tag == "ball"){
					mudaTurno(timeA.ToString());
					if(Network.peerType != NetworkPeerType.Disconnected){
						networkView.RPC("mudaTurno",RPCMode.Others,timeA.ToString());
					}
				}
			}
		}
	}
	
	[RPC]
	void mudaTurno(string timeA){
		game.turnoTimeA = bool.Parse(timeA);
		if(game.tipoToque == GameStatus.TipoToque.chute){ 
			game.numeroToqueTime = 0;
			game.tipoToque = GameStatus.TipoToque.normal;
		}
	}
}
