using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour {

	public bool disparaTrigger = false;
	public bool isReplay = false;
	private GameStatus game;
	// Use this for initialization 
	void Awake () {
		game = GameObject.Find("Game").GetComponent<GameStatus>();
	}
	
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Network.peerType != NetworkPeerType.Disconnected && (game.turnoDoJogador || (!game.turnoDoJogador && game.botaoEmMovimento && game.tipoToque == GameStatus.TipoToque.chute))){
			networkView.RPC("sincroniza",RPCMode.Others,transform.position,transform.eulerAngles,rigidbody.velocity,rigidbody.angularVelocity,rigidbody.useGravity.ToString(),rigidbody.isKinematic.ToString());
		}
	}
	
	[RPC]
	void sincroniza(Vector3 syncPOS, Vector3 syncEA, Vector3 syncV, Vector3 syncAV, string syncUG, string syncIK){
		transform.position = syncPOS;
		transform.eulerAngles = syncEA;
		rigidbody.useGravity = bool.Parse(syncUG);
		rigidbody.isKinematic = bool.Parse(syncIK);
		rigidbody.velocity = syncV;	
		rigidbody.angularVelocity = syncAV;
	}
	
	void OnCollisionEnter(Collision collision){
		if(collision != null && collision.rigidbody != null){
			if(collision.rigidbody.name == "goleiro"){
				if(collision.rigidbody.tag == "jogador time a") game.turnoTimeA = true;
				else if(collision.rigidbody.tag == "jogador time b") game.turnoTimeA = false;
				if(rigidbody.velocity == new Vector3(0,0,0)){
					game.numeroToqueTime = 0;
					game.tiroDeMeta(false);
				}
			}
		}
	}
	
	void OnCollisionStay(Collision collision){
		if(collision != null && collision.rigidbody != null){
			if(collision.rigidbody.name == "goleiro"){
				if(collision.rigidbody.tag == "jogador time a") game.turnoTimeA = true;
				else if(collision.rigidbody.tag == "jogador time b") game.turnoTimeA = false;
				game.numeroToqueTime = 0;
				game.tiroDeMeta(false);
			}
		}
	}
	
	public void callOnTriggerEnter(Collider other){
		disparaTrigger = false;
		isReplay = true;
		OnTriggerEnter(other);
	}
	
	void OnTriggerEnter(Collider other){		
		if(!disparaTrigger ){
			ReplayScript replay = GameObject.Find("Game").GetComponent<ReplayScript>();
			if(!replay.isPlayingReplay()){
				GameStatus game = GameObject.Find("Game").GetComponent<GameStatus>();
				SharedData data = GameObject.FindWithTag ("data").GetComponent<SharedData>();
				if(game.pauseState == GameStatus.PauseState.replaying) return;
				GameObject trigger = other.gameObject;
				disparaTrigger = true;
				Debug.Log("onTriggerEnter "+other.gameObject.name+" disparaTrigger = "+disparaTrigger);
				if(game.tipoToque == GameStatus.TipoToque.chute && trigger.name == "TriggerGolTimeA"){
					Debug.Log("Gol time B "+isReplay+" mostraReplay = "+data.mostraReplay);
					if(!data.mostraReplay){
						game.GetComponent<AudioSource>().clip = game.gritoGolTimeB;
						game.GetComponent<AudioSource>().Play();
						game.golTimeB++;
					}
					if(data.mostraReplay && !isReplay){
						game.GetComponent<AudioSource>().clip = game.gritoGolTimeB;
						game.GetComponent<AudioSource>().Play();
						game.golTimeB++;
						//replay.startReplay(other);
						StartCoroutine(replay.startReplayWithWait(other));
					}else{
						game.gol(false);
						if(Network.peerType != NetworkPeerType.Disconnected)
							networkView.RPC("gol",RPCMode.Others,false.ToString());
					}
				}else if(game.tipoToque == GameStatus.TipoToque.chute && trigger.name == "TriggerGolTimeB"){
					Debug.Log("Gol time A "+isReplay+" mostraReplay = "+data.mostraReplay);
					if(!data.mostraReplay){
						game.GetComponent<AudioSource>().clip = game.gritoGolTimeA;
						game.GetComponent<AudioSource>().Play();
						game.golTimeA++;
					}
					if(data.mostraReplay && !isReplay){
						game.GetComponent<AudioSource>().clip = game.gritoGolTimeA;
						game.GetComponent<AudioSource>().Play();
						game.golTimeA++;
						//replay.startReplay(other);
						StartCoroutine(replay.startReplayWithWait(other));
					}else{
						game.gol(true);
						if(Network.peerType != NetworkPeerType.Disconnected)
							networkView.RPC("gol",RPCMode.Others,true.ToString());
					}
				}else if(!game.bolaParada && trigger.name == "Lateral Esquerda"){
					game.lateral();
					if(Network.peerType != NetworkPeerType.Disconnected)
						networkView.RPC("lateral",RPCMode.Others);
				}else if(!game.bolaParada &&trigger.name == "Lateral Direita"){
					game.lateral();
					if(Network.peerType != NetworkPeerType.Disconnected)
						networkView.RPC("lateral",RPCMode.Others);
				}else if(trigger.name == "TIro de Meta B" || trigger.name == "Tiro de Meta B" || trigger.name == "Perto B"){
					if(data.mostraReplay && game.tipoToque == GameStatus.TipoToque.chute && trigger.name == "Perto B"){
						if(!isReplay){
							//replay.startReplay(other);
							StartCoroutine(replay.startReplayWithWait(other));
						}else{
							game.tiroDeMetaEscanteio();
							if(Network.peerType != NetworkPeerType.Disconnected)
								networkView.RPC("tiroDeMetaEscanteio",RPCMode.Others);
						}
					}else{
						game.tiroDeMetaEscanteio();
						if(Network.peerType != NetworkPeerType.Disconnected)
							networkView.RPC("tiroDeMetaEscanteio",RPCMode.Others);
					}
				}else if(trigger.name == "TIro de Meta A" || trigger.name == "Tiro de Meta A" || trigger.name == "Perto A"){
					if(data.mostraReplay && game.tipoToque == GameStatus.TipoToque.chute && trigger.name == "Perto A"){
						if(!isReplay){
							//replay.startReplay(other);
							StartCoroutine(replay.startReplayWithWait(other));
						}else{
							game.tiroDeMetaEscanteio();
							if(Network.peerType != NetworkPeerType.Disconnected)
								networkView.RPC("tiroDeMetaEscanteio",RPCMode.Others);
						}
					}else{
						game.tiroDeMetaEscanteio();
						if(Network.peerType != NetworkPeerType.Disconnected)
							networkView.RPC("tiroDeMetaEscanteio",RPCMode.Others);
					}
				}else if(trigger.name == "Inferno"){
					//Debug.Log("Bola No Inferno");
					game.foraDeJogo();
				}else{
					game.foraDeJogo();
				}
			}
		}
		isReplay = false;
	}
	
	[RPC]
	void tiroDeMetaEscanteio(){
		game.tiroDeMetaEscanteio();
	}
	
	[RPC]
	void lateral(){
		game.lateral();
	}
	
	[RPC]
	void gol(string timeA){
		game.gol(bool.Parse(timeA));
	}
}