using UnityEngine;
using System.Collections;

public class GUISelect : MonoBehaviour {
	
	public GUISkin guiSkin;
	public GameObject[] times;
	public float areaWidth;
	public float areaHeight;
	private bool isSelectTime;
	private SharedData data;
	private int timeCasaAtual = 0;
	private int timeForaAtual = 1;
	private int numKitCasaAtual = 0;
	private int numKitForaAtual = 0;
	public Texture2D chosserLeftButton;
	public Texture2D chosserRightButton;
	private bool servidorPronto = false;
	private bool clientePronto = false;
	
	void Awake(){
		isSelectTime = true;
		data = GameObject.FindWithTag ("data").GetComponent<SharedData>();
		data.nomeTimeCasa = times[timeCasaAtual].GetComponent<TimeData>().name;
		data.nomeEstadio = times[timeCasaAtual].GetComponent<TimeData>().nomeEstadio;
		data.kitTimeCasa = times[timeCasaAtual].GetComponent<TimeData>().kits[0];
		data.goleiroTimeCasa = times[timeCasaAtual].GetComponent<TimeData>().goalieKit;
		data.logoTimeCasa = times[timeCasaAtual].GetComponent<TimeData>().scoreboardLogo;
		data.pausaLogoTimeCasa = times[timeCasaAtual].GetComponent<TimeData>().guiLogoLeft;
		data.gol = times[timeCasaAtual].GetComponent<TimeData>().gol;
		data.ball = times[timeCasaAtual].GetComponent<TimeData>().ball;
		data.field = times[timeCasaAtual].GetComponent<TimeData>().field;
		data.nomeTimeFora = times[timeForaAtual].GetComponent<TimeData>().name;
		data.kitTimeFora = times[timeForaAtual].GetComponent<TimeData>().kits[0];
		data.logoTimeFora = times[timeForaAtual].GetComponent<TimeData>().scoreboardLogo;
		data.pausaLogoTimeFora = times[timeForaAtual].GetComponent<TimeData>().guiLogoRight;
		data.goleiroTimeFora = times[timeForaAtual].GetComponent<TimeData>().goalieKit;
		if(Network.isServer){
			networkView.RPC("ajustaSharedData",RPCMode.Others,data.tempoJogo,data.numeroToqueMaximo);
		}
	}
	
	void OnGUI(){
		//Define o skin utilizado
		GUI.skin = guiSkin;
		
		//Cria a matriz da GUI, isso faz com que a interface seja visualizada de maneira correta em qualquer resolução
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (Screen.width / 1024f, Screen.height / 768f, 1)); 
		
		if(Network.peerType != NetworkPeerType.Disconnected && servidorPronto && clientePronto)
			avancar();
		
		//Verifica se existe o objeto data existe, senão o jogo não foi inicializado de forma correta
		if(data != null){
			//Cria os labels que vai carregar os nomes dos times
			GUI.Label(new Rect(38,40,246,58),label.timeCasa,guiSkin.label);
			GUI.Label(new Rect(739,40,246,58),label.timeFora,guiSkin.label);
			GUI.Label(new Rect(38,124,246,58),data.nomeTimeCasa,guiSkin.label);
			GUI.Label(new Rect(739,124,246,58),data.nomeTimeFora,guiSkin.label);
			
			//Verfica se estamos selecionando o time
			if(isSelectTime){
				//Mostra o logo dos dois times selecionados
				GUI.Box(new Rect(46,191,387,387),times[timeCasaAtual].GetComponent<TimeData>().guiLogoLeft);
				GUI.Box(new Rect(594,191,387,387),times[timeForaAtual].GetComponent<TimeData>().guiLogoRight);
			}else{
				//Mostra as texturas que foram selecionadas 
				GUI.Box(new Rect(46,191,387,387),times[timeCasaAtual].GetComponent<TimeData>().miniKitsLeft[numKitCasaAtual]);
				GUI.Box(new Rect(594,191,387,387),times[timeForaAtual].GetComponent<TimeData>().miniKitsRight[numKitForaAtual]);
			}
			
			if (Network.peerType == NetworkPeerType.Disconnected || Network.isServer) {
				//Cria as setas para trocar de time ou uniforme
				if(GUI.Button(new Rect(182,590,19,38),chosserLeftButton,guiSkin.GetStyle("chosseTeam"))){
					if(isSelectTime){
						//Se estiver selecionando time essa linha vai trazer o time anterior
						trocaTimeCasa(-1);
						if(Network.peerType != NetworkPeerType.Disconnected){
							networkView.RPC("trocaTimeCasa",RPCMode.OthersBuffered,-1);
						}
					}else{
						//Se NÃO estiver selecionando time essa linha vai trazer a textura anterior
						trocaKitTimeCasa(-1);
						if(Network.peerType != NetworkPeerType.Disconnected){
							networkView.RPC("trocaKitTimeCasa",RPCMode.OthersBuffered,-1);
						}
					}
				}
				if(GUI.Button(new Rect(278,590,19,38),chosserRightButton,guiSkin.GetStyle("chosseTeam"))){
					if(isSelectTime){
						trocaTimeCasa(1);
						if(Network.peerType != NetworkPeerType.Disconnected){
							networkView.RPC("trocaTimeCasa",RPCMode.OthersBuffered,1);
						}
					}else{
						trocaKitTimeCasa(1);
						if(Network.peerType != NetworkPeerType.Disconnected){
							networkView.RPC("trocaKitTimeCasa",RPCMode.OthersBuffered,1);
						}
					}
				}
			}
			
			if (Network.peerType == NetworkPeerType.Disconnected || Network.isClient) {
				if(GUI.Button(new Rect(727,590,19,38),chosserLeftButton,guiSkin.GetStyle("chosseTeam"))){
					if(isSelectTime){
						trocaTimeFora(-1);
						if(Network.peerType != NetworkPeerType.Disconnected){
							networkView.RPC("trocaTimeFora",RPCMode.OthersBuffered,-1);
						}
					}else{
						trocaKitTimeFora(-1);
						if(Network.peerType != NetworkPeerType.Disconnected){
							networkView.RPC("trocaKitTimeFora",RPCMode.OthersBuffered,-1);
						}
					}
				}
				if(GUI.Button(new Rect(823,590,19,38),chosserRightButton,guiSkin.GetStyle("chosseTeam"))){
					if(isSelectTime){
						trocaTimeFora(1);
						if(Network.peerType != NetworkPeerType.Disconnected){
							networkView.RPC("trocaTimeFora",RPCMode.OthersBuffered,1);
						}
					}else{
						trocaKitTimeFora(1);
						if(Network.peerType != NetworkPeerType.Disconnected){
							networkView.RPC("trocaKitTimeFora",RPCMode.OthersBuffered,1);
						}
					}
				}
			}
			
			if(GUI.Button(new Rect(182,685,303,62),"",guiSkin.GetStyle("buttonVoltar"))){
				if(isSelectTime && Network.peerType != NetworkPeerType.Disconnected)
					networkView.RPC("voltar",RPCMode.Others);
				
				voltar();
			}
			if(GUI.Button(new Rect(539,685,303,62),"",guiSkin.GetStyle("buttonAvancar"))){
				avancar();
			}
		}
	}
	
	[RPC]
	void trocaTimeCasa(int soma){
		timeCasaAtual += soma;
		if(timeCasaAtual < 0){
			timeCasaAtual = times.Length-1;
		}
		if(timeCasaAtual == times.Length){
			timeCasaAtual = 0;
		}
		//Joga no objeto data as propriedades do time da casa
		data.nomeTimeCasa = times[timeCasaAtual].GetComponent<TimeData>().name;
		data.kitTimeCasa = times[timeCasaAtual].GetComponent<TimeData>().kits[0];
		data.goleiroTimeCasa = times[timeCasaAtual].GetComponent<TimeData>().goalieKit;
		data.logoTimeCasa = times[timeCasaAtual].GetComponent<TimeData>().scoreboardLogo;
		data.pausaLogoTimeCasa = times[timeCasaAtual].GetComponent<TimeData>().guiLogoLeft;
		data.gol = times[timeCasaAtual].GetComponent<TimeData>().gol;
		data.ball = times[timeCasaAtual].GetComponent<TimeData>().ball;
		data.field = times[timeCasaAtual].GetComponent<TimeData>().field;
		data.nomeEstadio = times[timeCasaAtual].GetComponent<TimeData>().nomeEstadio;
		numKitCasaAtual = 0;
	}
	
	[RPC]
	void trocaTimeFora(int soma){
		timeForaAtual += soma;
		if(timeForaAtual < 0){
			timeForaAtual = times.Length-1;
		}
		if(timeForaAtual == times.Length){
			timeForaAtual = 0;
		}
		//Joga no objeto data as propriedades do time de fora
		data.nomeTimeFora = times[timeForaAtual].GetComponent<TimeData>().name;
		data.kitTimeFora = times[timeForaAtual].GetComponent<TimeData>().kits[0];
		data.logoTimeFora = times[timeForaAtual].GetComponent<TimeData>().scoreboardLogo;
		data.pausaLogoTimeFora = times[timeForaAtual].GetComponent<TimeData>().guiLogoRight;
		data.goleiroTimeFora = times[timeForaAtual].GetComponent<TimeData>().goalieKit;
		numKitForaAtual = 0;
	}
	
	[RPC]
	void trocaKitTimeCasa(int soma){
		Material[] kits = times[timeCasaAtual].GetComponent<TimeData>().kits;
		numKitCasaAtual += soma;
		if(numKitCasaAtual < 0){
			numKitCasaAtual = kits.Length-1;
		}
		if(numKitCasaAtual == kits.Length){
			numKitCasaAtual = 0;
		}
		data.kitTimeCasa = kits[numKitCasaAtual];
	}
	
	[RPC]
	void trocaKitTimeFora(int soma){
		Material[] kits = times[timeForaAtual].GetComponent<TimeData>().kits;
		numKitForaAtual += soma;
		if(numKitForaAtual < 0){
			numKitForaAtual = kits.Length-1;
		}
		if(numKitForaAtual == kits.Length){
			numKitForaAtual = 0;
		}
		data.kitTimeFora = kits[numKitForaAtual];
	}
	
	[RPC]
	void voltar(){
		if(isSelectTime){
			GameObject.Destroy(data);
			if(Network.peerType != NetworkPeerType.Disconnected){
				Network.Disconnect();
			}
			Application.LoadLevel("startscreen");
		}else{
			if(Network.peerType == NetworkPeerType.Disconnected) isSelectTime = true;
		}
	}
	
	[RPC]
	void avancar(){
		if(Network.peerType == NetworkPeerType.Disconnected || (servidorPronto && clientePronto)){
			if(isSelectTime){
					isSelectTime = false;
					servidorPronto = false;
					clientePronto = false;
			}else{
				Application.LoadLevel("field");
			}
		}else if(Network.isServer){
			networkView.RPC("serverReady",RPCMode.OthersBuffered);
			serverReady();
		}else{
			networkView.RPC("clientReady",RPCMode.OthersBuffered);
			clientReady();
		}
	}
	
	[RPC]
	void serverReady(){
		servidorPronto = true;
	}
	
	[RPC]
	void clientReady(){
		clientePronto = true;
	}
	
	[RPC]
	void ajustaSharedData(int tempoJogo, int numeroMaximoToques){
		data.tempoJogo = tempoJogo;
		data.numeroToqueMaximo = numeroMaximoToques;
	}
}