using UnityEngine;
using System.Collections;

public class GUIPlacar : MonoBehaviour {

	public GUISkin guiSkin;
	public Texture2D placarBackground;
	public Texture2D mainPlacar;
	public Vector2 placarBackgroundOffset = new Vector2(0,0);
	public Vector2 nomeTimeAOffset = new Vector2(0,0);
	public Vector2 nomeTimeBOffset = new Vector2(0,0);
	public Vector2 resultadoTimeAOffset = new Vector2(0,0);
	public Vector2 resultadoTimeBOffset = new Vector2(0,0);
	public float nativeVerticalResolution = 1200.0f;
	private int tempo = 0;
	private GameStatus game;
	
	void Awake(){
		game = GameObject.Find("Game").GetComponent<GameStatus>();
	}
	
	void OnGUI(){
		GUI.skin = guiSkin;
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (Screen.height / nativeVerticalResolution, Screen.height / nativeVerticalResolution, 1)); 
		if(game.gameStarted){
			int golTimeA = game.golTimeA;
			int golTimeB = game.golTimeB;
			tempo++;
			mostraImagem(placarBackgroundOffset, placarBackground );
			mostraLabel(nomeTimeAOffset, game.data.apelidoTimeCasa );
			mostraLabel(nomeTimeBOffset, game.data.apelidoTimeFora );
			mostraLabel(resultadoTimeAOffset, golTimeA );
			mostraLabel(resultadoTimeBOffset, golTimeB );
			mostraLabel(new Vector2(190,33), "-" );
			if(game.turnoTimeA){
				mostraLabel(new Vector2(100,33), "*" );
			}else{
				mostraLabel(new Vector2(270,33), "*" );
			}
			mostraLabel(new Vector2(150,68), minutosPassados(tempo),guiSkin.GetStyle("tempo"));
		}else{
			mostraImagemBottom(new Vector2(261,20),1334,132, mainPlacar);
			mostraLabelBottom(new Vector2(372,140), game.data.nomeTimeCasa );
			mostraLabelBottom(new Vector2(990,140), game.data.nomeTimeFora );
			mostraImagemBottom(new Vector2(167,55),162,137, game.data.logoTimeCasa );
			mostraImagemBottom(new Vector2(1207,55),162,137, game.data.logoTimeFora );
		}
	}
	
	void mostraImagem(Vector2 pos, Texture2D image){
		GUI.Label(new Rect(0+pos.x, 0+pos.y, image.width, image.height), image);
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
	
	private string minutosPassados(int tempo){
		int minutos = tempo/3600;
		int segundos =  (int) ((tempo/3600.0 - minutos)*60.0);
		return (minutos < 10 ? "0"+minutos.ToString() : minutos.ToString())+":"+(segundos < 10 ? "0"+segundos.ToString() : segundos.ToString());
	}
}