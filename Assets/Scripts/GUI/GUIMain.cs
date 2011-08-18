using UnityEngine;
using System.Collections;

public class GUIMain : MonoBehaviour {

	enum MenuState { mainMenu, options, howTo, regras }
	
	public GUISkin guiSkin;
	public float areaWidth;
	public float areaHeight;
	private SharedData data;
	private MenuState state;
	
	public Texture2D logo;
	
	public Texture2D[] comoJogarImages;
	public Texture2D[] howToImages;
	private int numeroAjuda = 0;
	
	private int opcaoAtual = 3;
	private int[] tempoJogo = {10,4,2,1};
	private string[] tempoJogoNome = {"2","5","10","20"};
	private string[] linguagemNome = {"English","Português"};
	private string[] estadios = {"icicle","slasherfield","tealworld","tigersden","devilslair","destructo"};
	
	void Start(){
		data = GameObject.FindWithTag("data").GetComponent<SharedData>();
		int numeroEstadio = Random.Range(0,5);
		if (System.DateTime.Now.TimeOfDay.Hours > 6 && System.DateTime.Now.TimeOfDay.Hours < 18){
            Application.LoadLevelAdditive("estadio-"+estadios[numeroEstadio]);
        }else{
			//Camera.main.GetComponent<Skybox>().material = nightSky; 
            Application.LoadLevelAdditive("estadio-"+estadios[numeroEstadio]+"-noite");
        }
		data.linguagem = label.cdLinguagem;
	}
	
	void OnGUI(){
		if(state == MenuState.mainMenu) montaMainMenu();
		else if(state == MenuState.howTo) montaAjuda();
		else if(state == MenuState.options) montaOpcoes();
	}
	
	void montaAjuda(){
		Texture2D[] currentHowTo;
		if(label.cdLinguagem == 0) currentHowTo = howToImages;
		else currentHowTo = comoJogarImages;
		//Define a skin que será usada para montar a interface
		GUI.skin = guiSkin;
		
		//Cria a matriz da GUI, isso faz com que a interface seja visualizada de maneira correta em qualquer resolução
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (Screen.width / 1024f, Screen.height / 768f, 1));
		GUI.Box(new Rect(0,0,1024f, 768f),currentHowTo[numeroAjuda]);
		
			
		if(GUI.Button(new Rect(182,685,303,62),"",guiSkin.GetStyle("buttonVoltar"))){
			voltar();
		}
		if(GUI.Button(new Rect(539,685,303,62),"",guiSkin.GetStyle("buttonAvancar"))){
			avancar();
		}
	}
	
	void voltar(){
		if(numeroAjuda == 0)
			state = MenuState.mainMenu;
		else
			numeroAjuda--;
	}
	
	void avancar(){
		numeroAjuda++;
		if(numeroAjuda == howToImages.Length){
			state = MenuState.mainMenu;
			numeroAjuda = 0;
		}
	}
	
	void montaMainMenu(){
		//Define a skin que será usada para montar a interface
		GUI.skin = guiSkin;
		
		//Cria a matriz da GUI, isso faz com que a interface seja visualizada de maneira correta em qualquer resolução
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (Screen.width / 1024f, Screen.height / 768f, 1));
		//Começa a area de desenho da GUI (Graphical User Interface)
		GUILayout.BeginArea (new Rect (0f, 76f, areaWidth, areaHeight));
		
		//Cria botão do Modo Amistoso
		if(GUI.Button (new Rect(0f, 0f, 370, 36),""))
		{
			//Define o que vai ocorrer quando o botão for clicado
			Application.LoadLevel("selectscreen");
		}
		//Cria o label que vai aparecer para o botão
		GUI.Label(new Rect(0f,-6f,370,40),label.modoAmistoso);
		
		if(GUI.Button (new Rect(-20f, 50f, 370, 36),""))
		{
			//Application.LoadLevel("selectscreen");
		}
		GUI.Label(new Rect(0f,44f,370,40),label.modoLiga);
		if(GUI.Button (new Rect(-40f, 100f, 370, 36),""))
		{
			//Application.LoadLevel("selectscreen");
		}
		GUI.Label(new Rect(0f,94f,370,40),label.modoCopa);
		if(GUI.Button (new Rect(-60f, 150f, 370, 36),""))
		{
			//Application.LoadLevel("loginscreen");
		}
		GUI.Label(new Rect(0f,144f,370,40),label.multiplayer);
		if(GUI.Button (new Rect(-80f, 200f, 370, 36),""))
		{
			state = MenuState.howTo;
		}
		GUI.Label(new Rect(0f,194f,370,40),label.comoJogar);
		if(GUI.Button (new Rect(-100f, 250f, 370, 36),""))
		{
			state = MenuState.options;
		}
		GUI.Label(new Rect(0f,244f,370,40),label.opcoes);
		if(GUI.Button (new Rect(-120f, 300f, 370, 36),""))
		{
			Application.Quit();
		}
		GUI.Label(new Rect(0f,294f,370,40),label.sair);
		GUILayout.EndArea();
	}
	
	void montaOpcoes(){
		//Define a skin que será usada para montar a interface
		GUI.skin = guiSkin;
		
		//Cria a matriz da GUI, isso faz com que a interface seja visualizada de maneira correta em qualquer resolução
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (Screen.width / 1024f, Screen.height / 768f, 1));
		
		GUI.Box(new Rect(0f, 26f, 370, 36),"",guiSkin.GetStyle("customBox"));
		GUI.Label(new Rect(0f,20f,370,40),label.opcoes);
		
		GUI.Box(new Rect(0f, 76f, 370, 36),"",guiSkin.GetStyle("customBox"));
		GUI.Label(new Rect(0f,70f,370,40),label.tempoDeJogo);
		
		if(GUI.Button(new Rect(365f, 76f, 57, 36),"",guiSkin.GetStyle("optionLeft"))){
			mudaTempoJogo(-1);
		}
		
		GUI.Box(new Rect(415f, 76f, 327, 36),"",guiSkin.GetStyle("optionBox"));
		GUI.Label(new Rect(465f,70f,370,40),tempoJogoNome[opcaoAtual]+" "+label.minutos);
		
		if(GUI.Button(new Rect(735f, 76f, 57, 36),"",guiSkin.GetStyle("optionRight"))){
			mudaTempoJogo(1);
		}
		
		GUI.Box(new Rect(0f, 126f, 370, 36),"",guiSkin.GetStyle("customBox"));
		GUI.Label(new Rect(0f,120f,370,40),label.mostraReplay);
		
		data.mostraReplay = GUI.Toggle(new Rect(365f, 126f, 57, 36),data.mostraReplay,"");
		
		GUI.Box(new Rect(0f, 176f, 370, 36),"",guiSkin.GetStyle("customBox"));
		GUI.Label(new Rect(0f,170f,370,40),label.toquesPorTime);
		
		if(GUI.Button(new Rect(365f, 176f, 57, 36),"",guiSkin.GetStyle("optionLeft"))){
			mudaValor(-1,1,15,ref data.numeroToqueMaximo);
		}
		
		GUI.Box(new Rect(415f, 176f, 327, 36),"",guiSkin.GetStyle("optionBox"));
		GUI.Label(new Rect(470f,170f,370,40),data.numeroToqueMaximo.ToString()+" toques");
		
		if(GUI.Button(new Rect(735f, 176f, 57, 36),"",guiSkin.GetStyle("optionRight"))){
			mudaValor(1,1,15,ref data.numeroToqueMaximo);
		}
		
		GUI.Box(new Rect(0f, 226f, 370, 36),"",guiSkin.GetStyle("customBox"));
		GUI.Label(new Rect(0f,220f,370,40),label.linguagem);
		
		if(GUI.Button(new Rect(365f, 226f, 57, 36),"",guiSkin.GetStyle("optionLeft"))){
			mudaValor(1,0,1,ref label.cdLinguagem);
			label.trocaLinguagem(label.cdLinguagem);
		}
		
		GUI.Box(new Rect(415f, 226f, 327, 36),"",guiSkin.GetStyle("optionBox"));
		GUI.Label(new Rect(470f,220f,370,40),linguagemNome[label.cdLinguagem]);
		
		if(GUI.Button(new Rect(735f, 226f, 57, 36),"",guiSkin.GetStyle("optionRight"))){
			mudaValor(-1,0,1,ref label.cdLinguagem);
			label.trocaLinguagem(label.cdLinguagem);
		}
		
		/*GUI.Box(new Rect(0f, 226f, 370, 36),"",guiSkin.GetStyle("customBox"));
		GUI.Label(new Rect(0f,220f,370,40),"Toques por botão");
		
		if(GUI.Button(new Rect(365f, 226f, 57, 36),"",guiSkin.GetStyle("optionLeft"))){
			mudaValor(-1,1,5,ref data.numeroToqueJogadorSeguido);
		}
		
		GUI.Box(new Rect(415f, 226f, 327, 36),"",guiSkin.GetStyle("optionBox"));
		GUI.Label(new Rect(410f,220f,370,40),data.numeroToqueJogadorSeguido.ToString()+" toques seguidos");
		
		if(GUI.Button(new Rect(735f, 226f, 57, 36),"",guiSkin.GetStyle("optionRight"))){
			mudaValor(1,1,5,ref data.numeroToqueJogadorSeguido);
		}
		*/
		if(GUI.Button(new Rect(0f, 276f, 370, 36),"")){
			state = MenuState.mainMenu;
		}
		GUI.Label(new Rect(0f,270f,370,40),label.voltaAoMenu);
	}
	
	void mudaTempoJogo(int soma){
		opcaoAtual += soma;
		if(opcaoAtual < 0) opcaoAtual = tempoJogo.Length-1;
		if(opcaoAtual == tempoJogo.Length) opcaoAtual = 0;
		data.tempoJogo = tempoJogo[opcaoAtual];
	}
	
	void mudaValor(int soma, int minimo, int maximo, ref int valor){
		valor += soma;
		if(valor < minimo) valor = maximo;
		else if(valor > maximo) valor = minimo;
	}
}
