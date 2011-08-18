using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	public bool ativo = false;
	public Vector3 posicaoFinal;
	public Vector3 posicaoInicial;
	public Vector3 posicaoFinalAntiga;
	public Vector3 eulerAnglesInicial;
	public float forca;
	private Rigidbody body;
	private GameStatus game;
	private Vector3 Zero = new Vector3(0,0,0);
	public int numeroToqueJogador;
	public bool emMovimento = false;
	public float wait = 0;
	private Collision collision;
	
	// Use this for initialization  
	void Awake () {
		body = rigidbody;
		game = GameObject.Find("Game").GetComponent<GameStatus>();
		eulerAnglesInicial = gameObject.transform.eulerAngles;
		posicaoInicial = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(game.pauseState == GameStatus.PauseState.halfTime){
			posicaoFinal = Zero;
			body.velocity = Zero;
			emMovimento = false;
			game.botaoEmMovimento = false;
		}
		
		if(game.pauseState != GameStatus.PauseState.none)
			return;
		
		if(gameObject.transform.position.x < -50 || gameObject.transform.position.x > 50 ){
			gameObject.transform.position = new Vector3(posicaoInicial.x,0.5f,posicaoInicial.z);
			gameObject.rigidbody.velocity = new Vector3(0,0,0);
		}
		if(gameObject.transform.position.z < -30 || gameObject.transform.position.z > 30 ){
			gameObject.transform.position = new Vector3(posicaoInicial.x,0.5f,posicaoInicial.z);
			gameObject.rigidbody.velocity = new Vector3(0,0,0);
		}	
		if(gameObject.transform.position.y < -1 || gameObject.transform.position.y > 10 ){
			gameObject.transform.position = new Vector3(posicaoInicial.x,0.5f,posicaoInicial.z);
			gameObject.rigidbody.velocity = new Vector3(0,0,0);
		}	

		if(game.jogadorSelecionado != null)
			ativo = gameObject.name == game.jogadorSelecionado.name && gameObject.tag == game.jogadorSelecionado.tag;
		
		if(!game.botaoEmMovimento) wait = 0;
		
		if(ativo && emMovimento && body.velocity == Zero && !game.tocouBola && !game.bolaParada) wait += Time.deltaTime*2;
		
		if(game.tocouBola){ 
			wait = 0;
			game.botaoEmMovimento = false;
			if(Network.peerType == NetworkPeerType.Disconnected)
				verificaQuantidadeToques();
			else
				networkView.RPC("verificaQuantidadeToques",RPCMode.All);
		}
		
		if(wait >= 0.2 && !game.bolaParada && game.tipoToque != GameStatus.TipoToque.chute){
			mudaTurno();
		}
		
		if(wait >=0.5){
			mudaTurno();
		}
		
		if(wait == 0){
			emMovimento = body.velocity != Zero;
		}
		
		if(posicaoFinal != null && posicaoFinal.x !=0 && posicaoFinal.z !=0 && Network.peerType != NetworkPeerType.Disconnected)
			networkView.RPC("sendPosicaoFinal",RPCMode.All,posicaoFinal);
		
		if(body != null && posicaoFinal != null && posicaoFinal.x !=0 && posicaoFinal.z !=0){
			Vector3 targetVelocity = posicaoFinal;
			body.AddForce(new Vector3(targetVelocity.x, 0, targetVelocity.z),ForceMode.VelocityChange);
			emMovimento = true;
			game.botaoEmMovimento = true;
			posicaoFinalAntiga = posicaoFinal;
			posicaoFinal = Zero;
			if(Network.peerType != NetworkPeerType.Disconnected)
				networkView.RPC("sendPosicaoFinal",RPCMode.All,Zero);
		}
		if(Network.peerType != NetworkPeerType.Disconnected && (game.turnoDoJogador || (!game.turnoDoJogador && game.botaoEmMovimento && game.tipoToque == GameStatus.TipoToque.chute))){
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
	
	[RPC]
	void sendPosicaoFinal(Vector3 final){
		posicaoFinal = final;
	}
	
	void mudaTurno(){
		game.turnoTimeA = !game.turnoTimeA;
		game.numeroToqueTime = 0;
		Debug.Log( body.velocity+ " Mudou turno !! causa - "+gameObject.name+" wait?? "+wait);
		posicaoFinal = Zero;
		wait = 0;
		ativo = false;
		emMovimento = false;
		game.botaoEmMovimento = false;
		verificaQuantidadeToques();
	}
	
	[RPC]
	void verificaQuantidadeToques(){
		if(game.numeroToqueTime == game.data.numeroToqueMaximo){
			string timeSofreuFalta = "jogador time b", timeFezFalta = "jogador time a";
			if(!game.turnoTimeA){
				timeSofreuFalta = "jogador time a"; timeFezFalta = "jogador time b";
			}
			game.turnoTimeA = !game.turnoTimeA;
			game.numeroToqueTime = 0;
			if(game.tipoToque != GameStatus.TipoToque.chute && game.data.numeroToqueMaximo > 3){
				game.turnoTimeA = !game.turnoTimeA;
				game.verificaFalta(timeFezFalta,timeSofreuFalta,game.activeBall.transform.position);
			}
		}
	}
	
	void OnCollisionEnter(Collision collision){
		if(game.pauseState == GameStatus.PauseState.replaying) return;
		this.collision = collision;
		if(collision != null && collision.rigidbody != null){
			if(collision.rigidbody.tag == "ball"){
				if(Network.peerType == NetworkPeerType.Disconnected){
					toqueBola();
				}else{
					networkView.RPC("toqueBola",RPCMode.All);
				}
			}else if(!game.bolaParada && !game.tocouBola){
				if((body.tag == "jogador time a" && game.turnoTimeA) || (body.tag == "jogador time b" && !game.turnoTimeA)){
					falta();
				}
			}
		}
	}
	
	void OnCollisionStay(Collision collision){
	}
	
	public void ativarJogador(){
		if(game != null && game.existeJogadorSelecionado){
			game.deselecionaJogador();
		}
		ativo = true;
		game.selecionaJogador(gameObject);
	}
	
	void falta(){
		game.verificaFalta(body.tag,collision.rigidbody.tag, collision.transform.position);
	}
	
	[RPC]
	void toqueBola(){
		collision.gameObject.GetComponent<AudioSource>().Play();
		if(tag == "jogador time a"){
			game.timeAUltimoTocar = true;
			if(!game.turnoTimeA){ game.numeroToqueTime = 0;Debug.Log("Mudou turno !! causa - "+gameObject.name);}
			game.turnoTimeA = true;
		}else{
			game.timeAUltimoTocar = false;
			if(game.turnoTimeA){ game.numeroToqueTime = 0; Debug.Log("Mudou turno !! causa - "+gameObject.name);}
			game.turnoTimeA = false;
		}
		if(!game.tocouBola){
			if(game.tipoToque == GameStatus.TipoToque.normal){
				collision.rigidbody.AddForceAtPosition(new Vector3(0,0,0), ((new Vector3(posicaoFinalAntiga.x+15,posicaoFinalAntiga.y,posicaoFinalAntiga.z+15))*forca));
			}else{
				game.chute = false;
				game.goleiroPronto = false;
				Vector3 posFimBola = (posicaoFinalAntiga + collision.contacts[0].point);
				posFimBola = new Vector3(posFimBola.x * Random.Range(0.2f,0.8f) * (forca /5),(Random.Range(0.3f,8)),posicaoFinalAntiga.z);
				collision.rigidbody.AddForce(posFimBola, ForceMode.VelocityChange);
				game.turnoTimeA = !game.turnoTimeA;
				game.numeroToqueTime = 0;
			}
		}
		game.tocouBola = true;
	}
	
}
