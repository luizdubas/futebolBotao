using UnityEngine;
using System.Collections;

public class GUIPausa : MonoBehaviour {

	public GUISkin guiSkin;
	public GUISkin labelSkin;
	private GameStatus game;
	private ReplayScript replay;
	private bool servidorPronto = false;
	private bool clientePronto = false;
	void Awake(){
		game = GameObject.Find("Game").GetComponent<GameStatus>();
		replay = GameObject.Find("Game").GetComponent<ReplayScript>();
	}
	
	// Update is called once per frame 
	void OnGUI () {
		if(game.pauseState == GameStatus.PauseState.none || game.pauseState == GameStatus.PauseState.replaying) return;
		
		//Cria a matriz da GUI, isso faz com que a interface seja visualizada de maneira correta em qualquer resolução
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (Screen.width / 1024f, Screen.height / 768f, 1));
		
		//Define o skin utilizado
		GUI.skin = labelSkin;
		GUI.Box(new Rect(0,0,1024f, 768f),"");
		
		//Redefine o skin utilizado
		GUI.skin = guiSkin;
		//Cria botão para continuar o jogo
		GUI.Box (new Rect(0f, 26f, 370, 36),"");
		if(game.pauseState == GameStatus.PauseState.pause){
			//Cria o label que vai aparecer para o botão
			GUI.Label(new Rect(0f,20f,370,40),"Jogo Pausado");
		}else if(game.pauseState == GameStatus.PauseState.halfTime){
			//Cria o label que vai aparecer para o botão
			GUI.Label(new Rect(0f,20f,370,40),"Intervalo");
		}else{
			//Cria o label que vai aparecer para o botão
			GUI.Label(new Rect(0f,20f,370,40),"Fim de Jogo");
		}
		//Cria botão para continuar o jogo
		if(GUI.Button (new Rect(0f, 76f, 370, 36),""))
		{
			if(game.pauseState != GameStatus.PauseState.fullTime){
				game.pauseState = GameStatus.PauseState.none;
				Time.timeScale = 1; //faz o jogo retornar
			}else{
				Time.timeScale = 1; //faz o jogo retornar
				Application.LoadLevel("selectscreen");
			}
		}
		if(game.pauseState != GameStatus.PauseState.fullTime){
			//Cria o label que vai aparecer para o botão
			GUI.Label(new Rect(0f,70f,370,40),"Continuar Jogo");
		}else{
			//Cria o label que vai aparecer para o botão
			GUI.Label(new Rect(0f,70f,370,40),"Jogar com outro time");
		}
		
		if(game.pauseState != GameStatus.PauseState.pause && game.data.mostraReplay){
			
			if(GUI.Button (new Rect(-20f, 130f, 370, 36),"")){
				Time.timeScale = 1;
				replay.startAllReplay(game.pauseState, GameObject.Find("Game").GetComponent<GUIPlacar>().tempo );
				game.pauseState = GameStatus.PauseState.replaying;
			}
			GUI.Label(new Rect(0f,126f,370,40),"Melhores Momentos");		
			
			if(GUI.Button (new Rect(-40f, 180f, 370, 36),"")){
				Time.timeScale = 1; //faz o jogo retornar
				GameObject.Destroy(game.data);
				Application.LoadLevel("startscreen");
			}
			GUI.Label(new Rect(0f,176f,370,40),"Sair");
		}else{
			
			if(GUI.Button (new Rect(-40f, 130f, 370, 36),"")){
				Time.timeScale = 1; //faz o jogo retornar
				GameObject.Destroy(game.data);
				Application.LoadLevel("startscreen");
			}
			GUI.Label(new Rect(0f,126f,370,40),"Sair");
		}
	
		GUI.Box(new Rect(46,191,387,387),game.data.pausaLogoTimeCasa);
		GUI.Box(new Rect(594,191,387,387),game.data.pausaLogoTimeFora);
		
		GUI.skin = labelSkin;
		GUI.Label(new Rect(230,536,387,387),game.golTimeA.ToString());
		GUI.Label(new Rect(738,536,387,387),game.golTimeB.ToString());
	}
	
	[RPC]
	void serverReady(){
		servidorPronto = true;
	}
	
	[RPC]
	void clientReady(){
		clientePronto = true;
	}
	
}
