using UnityEngine;
using System.Collections;

public class GameStatus : MonoBehaviour {

	public int golTimeA;
	public int golTimeB;
	public int tempoDecorrido;
	public int tempoJogo;
	public GameObject jogadorSelecionado;
	public GameObject highlightSelecionado;
	private GameObject goleiroTimeA;
	private GameObject goleiroTimeB;
	public bool existeJogadorSelecionado;
	public bool gameStarted;
	public bool turnoTimeA;
	public bool chute;
	public bool goleiroPronto;
	public bool bolaChutada;
	private Vector3 posicaoInicialBola;
	private Vector3[] posicaoInicialTimeA;
	private Vector3[] posicaoInicialTimeB;
	public SharedData data;
	private ChuteScript chuteScript;
	
	// Use this for initialization
	void Start () {
		data = GameObject.FindWithTag ("data").GetComponent<SharedData>();
		chuteScript = GetComponent<ChuteScript>();
		GameObject[] timeA = GameObject.FindGameObjectsWithTag("jogador time a");
		GameObject[] timeB = GameObject.FindGameObjectsWithTag("jogador time b");
		posicaoInicialTimeA = new Vector3[11];
		posicaoInicialTimeB = new Vector3[11];
		mudaUniforme(timeA, data.kitTimeCasa);
		mudaUniforme(timeB, data.kitTimeFora);
		salvaPosicaoTime(timeA,true);
		salvaPosicaoTime(timeB,false);
		goleiroTimeA = GameObject.Find("/TimeA/goleiro");
		goleiroTimeB = GameObject.Find("/TimeB/goleiro");
		Application.LoadLevelAdditive("estadio-hakata");
		posicaoInicialBola = GameObject.FindWithTag("ball").transform.position;
		chute = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.S) && !chute){
			chute = true;
			if(turnoTimeA){
				chuteScript.goleiroUtilizado = goleiroTimeB;
				chuteScript.nomeCollider = "golColliderTimeB";
			}else{
				chuteScript.goleiroUtilizado = goleiroTimeA;
				chuteScript.nomeCollider = "golColliderTimeA";
			}
		}
	}
	
	public void selecionaJogador(GameObject jogador){
		existeJogadorSelecionado = true;
		jogadorSelecionado = jogador;
		if(chute){
			chuteScript.jogadorUtilizado = jogadorSelecionado;
		}
	}
	
	public void deselecionaJogador(){
		existeJogadorSelecionado = false;
		PlayerControl control = jogadorSelecionado.GetComponent<PlayerControl>();
		MeshRenderer[] meshRenders = jogadorSelecionado.GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer render in meshRenders){
			if(render.gameObject.name == "Default_Default_material1" || render.gameObject.name == "highlight" ){
				render.material = control.efeitoSelecionadoDesativo;
			}
		}
		jogadorSelecionado = null;
		highlightSelecionado = null;
	}
	
	public void gol(bool timeA){
		if(timeA){
			golTimeA++;
		}else{
			golTimeB++;
		}
		reposicionarBola();
	}
	
	public void foraDeJogo(){
		reposicionarBola();
	}
	
	private void reposicionarBola(){
		GameObject[] timeA = GameObject.FindGameObjectsWithTag("jogador time a");
		GameObject[] timeB = GameObject.FindGameObjectsWithTag("jogador time b");
		GameObject ball = GameObject.FindWithTag("ball");
		ball.rigidbody.AddForce(-ball.rigidbody.velocity, ForceMode.VelocityChange);
		ball.transform.position = posicaoInicialBola;
		recuperaPosicaoTime(timeA,true);
		recuperaPosicaoTime(timeB,false);
		turnoTimeA = !turnoTimeA;
	}
	
	private void mudaUniforme(GameObject[] time, Material uniforme){
		foreach(GameObject jogador in time){
			if(jogador.name != "goleiro"){
				MeshRenderer[] meshRenders = jogador.GetComponentsInChildren<MeshRenderer>();
				foreach(MeshRenderer render in meshRenders){
					if(render.gameObject.name == "jogador"){
						render.material = uniforme;
					}
				}
			}
		}
	}
	
	private void salvaPosicaoTime(GameObject[] time, bool timeA){
		int numJogador = 0;
		foreach(GameObject jogador in time){
			if(timeA){
				posicaoInicialTimeA[numJogador] = jogador.transform.position;
			}else{
				posicaoInicialTimeB[numJogador] = jogador.transform.position;
			}
			numJogador++;
		}
	}
	
	private void recuperaPosicaoTime(GameObject[] time, bool timeA){
		int numJogador = 0;
		foreach(GameObject jogador in time){
			if(timeA){
				jogador.transform.position = posicaoInicialTimeA[numJogador] ;
			}else{
				jogador.transform.position = posicaoInicialTimeB[numJogador];
			}
			numJogador++;
		}
	}
}
