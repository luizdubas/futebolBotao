using UnityEngine; 
using System.Collections;

public class GUIPlacar : MonoBehaviour {
	 
	public GUISkin guiSkin;
	public GUISkin labelSkin;
	public Texture2D placarBackground;
	public Texture2D mainPlacar;
	public Texture2D forca;
	public Texture2D barraForca;
	public Texture2D indicadorForca;
	
	public Texture2D chute;
	public Texture2D pronto;
	public Texture2D lateral;
	public Texture2D tiroDeMeta;
	public Texture2D escanteio;
	public Texture2D falta;
	public Texture2D replay;
	public Texture2D gol;
	public Vector2 placarBackgroundOffset = new Vector2(0,0);
	public Vector2 nomeTimeAOffset = new Vector2(0,0);
	public Vector2 nomeTimeBOffset = new Vector2(0,0);
	public Vector2 resultadoTimeAOffset = new Vector2(0,0);
	public Vector2 resultadoTimeBOffset = new Vector2(0,0);
	public float nativeVerticalResolution = 768.0f;
	public float tempo = 0;
	private GameStatus game;
	private bool isShowMessage;
	private string tipo;
	public float tempoMessage;
	
	void Awake(){
		game = GameObject.Find("Game").GetComponent<GameStatus>();
	}
	
	void OnGUI(){
		if(game.pauseState != GameStatus.PauseState.none && game.pauseState != GameStatus.PauseState.replaying) return;
		
		montaPlacar();
		if(isShowMessage){
			showMessage();
		}else{
			tempoMessage = 0;
		}
	}
	
	void montaPlacar(){
		if( game.pauseState != GameStatus.PauseState.replaying){
			GUI.skin = guiSkin;
			GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (Screen.height / 768f, Screen.height / nativeVerticalResolution, 1)); 
			if(game.gameStarted){
				tempo += (Time.deltaTime / 2)*game.data.tempoJogo;
				int golTimeA = game.golTimeA;
				int golTimeB = game.golTimeB;
				mostraImagem(placarBackgroundOffset, placarBackground );
				mostraImagem(nomeTimeAOffset, game.data.logoTimeCasa, 90, 49);
				mostraImagem(nomeTimeBOffset, game.data.logoTimeFora, 90, 49);
				mostraLabel(resultadoTimeAOffset, golTimeA );
				mostraLabel(resultadoTimeBOffset, golTimeB );
				if(game.turnoTimeA){
					mostraLabel(new Vector2(18,20), "*", guiSkin.GetStyle("ponto") );
					mostraLabel(new Vector2(130,48), game.numeroToqueTime.ToString(),guiSkin.GetStyle("tempo"));
				}else{
					mostraLabel(new Vector2(310,20), "*", guiSkin.GetStyle("ponto") );
					mostraLabel(new Vector2(186,48), game.numeroToqueTime.ToString(),guiSkin.GetStyle("tempo"));
				}
				mostraLabel(new Vector2(350,19), minutosPassados(tempo),guiSkin.GetStyle("tempo"));
				if(game.chute && !game.goleiroPronto)
					mostraLabel(new Vector2(0,80), label.mensagemChute,guiSkin.GetStyle("tempo"));
			
				GUILayout.BeginArea (new Rect (28f, 707f, 174, 35));
				GUI.Box(new Rect (0, 0, 174, 35),forca);
				GUI.Box(new Rect (7, -2, 160, 26),barraForca);
				if(game.turnoTimeA)GUI.Box(new Rect (getPosicaoIndicadorForca(true), -2, 160, 26),indicadorForca);
				GUILayout.EndArea();
				
				GUILayout.BeginArea (new Rect (Screen.width-(20* (Screen.height/768f))-(174* (Screen.height/768f)), 707f, 174, 35));
				GUI.Box(new Rect (0, 0, 174, 35),forca);
				GUI.Box(new Rect (7, -2, 160, 26),barraForca);
				if(!game.turnoTimeA)GUI.Box(new Rect (getPosicaoIndicadorForca(false), -2, 160, 26),indicadorForca);
				GUILayout.EndArea();
			}else{
				GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (Screen.width / 1024f, Screen.height / nativeVerticalResolution, 1)); 
				GUI.skin = labelSkin;
				GUI.Box(new Rect(0,0,1024f, 768f),"");
				//Redefine o skin utilizado
				GUI.skin = guiSkin;
				GUI.Box(new Rect(46,191,387,387),game.data.pausaLogoTimeCasa);
				GUI.Box(new Rect(594,191,387,387),game.data.pausaLogoTimeFora);
			}
			
			if(!game.segundoTempo && Mathf.FloorToInt(tempo) == 600){
				game.pauseState = GameStatus.PauseState.halfTime;
				Time.timeScale = 0;
			}else if(Mathf.FloorToInt(tempo) == 1200){
				game.pauseState = GameStatus.PauseState.fullTime;
				Time.timeScale = 0;
			}
		}
	}
	
	void show(string tipo){
		isShowMessage = true;
		this.tipo = tipo;
	}
	
	void hide(){
		isShowMessage = false;
	}
	
	void showMessage(){
		float x = 462, y = 24;
		if(tipo == "chute"){
			GUI.Label(new Rect(x,y,chute.width/4, chute.height/4),chute);
		}else if(tipo == "pronto"){
			GUI.Label(new Rect(x,y,pronto.width/4, pronto.height/4),pronto);
		}else if(tipo == "gol"){
			GUI.Label(new Rect(x,y,gol.width/4, gol.height/4),gol);
		}else if(tipo == "lateral"){
			GUI.Label(new Rect(x,y,lateral.width/4, lateral.height/4),lateral);
		}else if(tipo == "tiroDeMeta"){
			GUI.Label(new Rect(x,y,tiroDeMeta.width/4, tiroDeMeta.height/4),tiroDeMeta);
		}else if(tipo == "falta"){
			GUI.Label(new Rect(x,y,falta.width/4, falta.height/4),falta);
		}else if(tipo == "replay"){
			GUI.Label(new Rect(x,y-6,replay.width/4, replay.height/4),replay);
		}
		if(tipo != "replay"){
			tempoMessage += Time.deltaTime;
			if(tempoMessage >= 2f){
				isShowMessage = false;
			}
		}
	}
	
	float getPosicaoIndicadorForca(bool timeA){
		if((timeA && game.turnoTimeA) || (!timeA && !game.turnoTimeA)){
			return ((game.forca*105)/25)-46;
		}
		return 7-3;
	}
	
	void mostraImagem(Vector2 pos, Texture2D image){
		GUI.Label(new Rect(0+pos.x, 0+pos.y, image.width, image.height), image);
	}
	
	void mostraImagem(Vector2 pos, Texture2D image, int width, int height){
		GUI.Label(new Rect(0+pos.x, 0+pos.y, width, height), image);
	}
	
	void mostraLabel(Vector2 pos, string label){
		GUI.Label(new Rect(0+pos.x, 0+pos.y, 200, 200), label);
	}
	
	void mostraLabel(Vector2 pos, int label){
		GUI.Label(new Rect(0+pos.x, 0+pos.y, 200, 200), label.ToString());
	}
	
	void mostraLabel(Vector2 pos, string label, GUIStyle style){
		GUI.Label(new Rect(0+pos.x, 0+pos.y, 200, 200), label.ToString(),style);
	}
	
	void mostraImagemBottom (Vector2 pos, Texture2D image)
	{
		GUI.Label(new Rect (pos.x, nativeVerticalResolution - image.height - pos.y, image.width, image.height), image);
	}
	
	void mostraImagemBottom (Vector2 pos,float width, float height, Texture2D image)
	{
		GUI.Label(new Rect (pos.x, nativeVerticalResolution - height - pos.y,  width, height), image);
	}
	
	void mostraLabelBottom (Vector2 pos, string label)
	{
		GUI.Label(new Rect (pos.x, nativeVerticalResolution - pos.y, 300, 300), label);
	}
	
	void mostraLabelBottom (Vector2 pos, string label, GUIStyle style)
	{
		GUI.Label(new Rect (pos.x, nativeVerticalResolution - pos.y, 300, 300), label,style);
	}
	
	private string minutosPassados(float tempo){
		int minutos = ((int)tempo)/60;
        int segundos = ((int)tempo) % 60;
		return (minutos < 10 ? "0"+minutos.ToString() : minutos.ToString())+":"+(segundos < 10 ? "0"+segundos.ToString() : segundos.ToString());
	}
}