using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	public bool ativo = false;
	public Material efeitoSelecionado;
	public Material efeitoSelecionadoDesativo;
	public Vector3 posicaoFinal;
	public Vector3 posicaoInicial;
	public Vector3 eulerAnglesInicial;
	private Rigidbody body;
	private GameStatus game;
	private string time;
	public GameObject highlight;
	
	// Use this for initialization
	void Awake () {
		body = rigidbody;
		game = GameObject.Find("Game").GetComponent<GameStatus>();
		if(gameObject.tag == "jogador time a"){
			time = "TimeA";
		}else{
			time = "TimeB";
		}
		highlight = GameObject.Find("/"+time+"/"+gameObject.name+"/highlight");
		eulerAnglesInicial = gameObject.transform.eulerAngles;
		posicaoInicial = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(body != null && posicaoFinal != null && posicaoFinal.x !=0 && posicaoFinal.y !=0 && posicaoFinal.z !=0){
			Vector3 targetVelocity = posicaoFinal;
			Vector3 velocityChange = targetVelocity;
			body.AddForce(velocityChange, ForceMode.VelocityChange);
			posicaoFinal = new Vector3(0,0,0);
		}
		if(gameObject.transform.eulerAngles.x < 190){
			gameObject.transform.eulerAngles = eulerAnglesInicial;
			body.AddForce(new Vector3(0,0,0), ForceMode.VelocityChange);
		}
		if(gameObject.transform.position.y < -5 || gameObject.transform.position.y > 5 ){
			gameObject.transform.position = posicaoInicial;
			body.AddForce(new Vector3(0,0,0), ForceMode.VelocityChange);
		}
	}
	
	void OnCollisionEnter(Collision collision){
		if(!game.chute && collision != null && collision.rigidbody != null && collision.rigidbody.tag == "ball"){
			game.bolaChutada = false;
			if(tag == "jogador time a"){
				game.turnoTimeA = true;
			}else{
				game.turnoTimeA = false;
			}
			foreach (ContactPoint contact in collision.contacts){
				collision.rigidbody.AddForceAtPosition(collision.transform.position.normalized, contact.point);
			}
		}
	}
	
	public void ativarJogador(){
		GameStatus game = GameObject.Find("Game").GetComponent<GameStatus>();
		if(game != null && game.existeJogadorSelecionado){
			game.deselecionaJogador();
		}
		ativar();
		game.selecionaJogador(gameObject);
		game.highlightSelecionado = highlight;
	}
	
	private void ativar(){
		ativo = true;
		MeshRenderer[] meshRenders = gameObject.GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer render in meshRenders){
			if(render.gameObject.name == "Default_Default_material1" || render.gameObject.name == "highlight" ){
				render.material = efeitoSelecionado;
			}
		}
	}
}
