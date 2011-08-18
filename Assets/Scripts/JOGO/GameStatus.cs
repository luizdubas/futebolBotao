using UnityEngine;
using System.Collections;

public class GameStatus : MonoBehaviour {

	public enum TipoToque { normal, chute, falta, escanteio, tiroDeMeta, lateral };
	public enum PauseState { none, pause, halfTime, fullTime, replaying }
	
	public PauseState pauseState;
	public TipoToque tipoToque;
	public TipoToque showMessage;
	public SharedData data;
	private ChuteScript chuteScript;
	
	public int golTimeA;
	public int golTimeB;
	public int tempoDecorrido;
	public int tempoJogo;
	public int numeroToqueTime;
	
	public Material nightSky;
	
	public GameObject jogadorSelecionado;
	public GameObject activeBall;
	public GameObject line;
	private GameObject goleiroTimeA;
	private GameObject goleiroTimeB;
	
	public bool existeJogadorSelecionado;
	public bool gameStarted;
	public bool turnoTimeA;
	public bool chute;
	public bool goleiroPronto;
	public bool tocouBola;
	public bool botaoEmMovimento;
	public bool bolaParada;
	public bool timeAUltimoTocar; //variavel só para ver quem foi o ultimo time a tocar na bola
	public bool segundoTempo;
	public bool turnoDoJogador;
	
	private Vector3 posicaoInicialBola;
	private Vector3[] posicaoInicialTimeA;
	private Vector3[] posicaoInicialTimeB;
	
	public float forca;
	
	public AudioClip taLaTimeA;
	public AudioClip taLaTimeB;
	public AudioClip gritoGolTimeA;
	public AudioClip gritoGolTimeB;
	
	public GameObject prefabTimeA;
	public GameObject prefabTimeB;
	
	// Awake é chamado antes do start
	void Awake(){
		StartCoroutine("instanciaObjetos");
	}
	
	void Start () {
		data = GameObject.FindWithTag ("data").GetComponent<SharedData>();
		chuteScript = GetComponent<ChuteScript>();
		
		GameObject[] timeA = GameObject.FindGameObjectsWithTag("jogador time a");
		GameObject[] timeB = GameObject.FindGameObjectsWithTag("jogador time b");
		posicaoInicialTimeA = new Vector3[11];
		posicaoInicialTimeB = new Vector3[11];
		salvaPosicaoTime(timeA,true);
		salvaPosicaoTime(timeB,false);
		botaoEmMovimento = false;
		
		mudaTexturas();
		
		goleiroTimeA = GameObject.Find("/time a(Clone)/goleiro");
		goleiroTimeB = GameObject.Find("/time b(Clone)/goleiro");
		
        if (System.DateTime.Now.TimeOfDay.Hours > 6 && System.DateTime.Now.TimeOfDay.Hours < 18){
            Application.LoadLevelAdditive("estadio-"+data.nomeEstadio);
        }else{
			Camera.main.GetComponent<Skybox>().material = nightSky;
            Application.LoadLevelAdditive("estadio-"+data.nomeEstadio+"-noite");
        }
		forca = 0;
		chute = false;
		bolaParada = false;
		pauseState = PauseState.none;
		
		if(Network.peerType != NetworkPeerType.Disconnected) data.mostraReplay = false;
		
		activeBall = GameObject.Find("ball1");
		posicaoInicialBola = new Vector3(0,0.3f,0);
		reposicionarBolaSemWait();
	}
	
	IEnumerator instanciaObjetos(){
		if(Network.peerType != NetworkPeerType.Disconnected){
			if(Network.isServer){
				Network.Instantiate(prefabTimeA,Vector3.zero,Quaternion.identity,0);
			}else{
				Network.Instantiate(prefabTimeB,Vector3.zero,Quaternion.identity,0);
			}
		}else{
			Instantiate(prefabTimeA);
			Instantiate(prefabTimeB);
		}
		yield return new WaitForSeconds(5f);
	}
	
	// Update is called once per frame
	void Update () {
		if(Network.peerType == NetworkPeerType.Disconnected && Input.GetKeyDown(KeyCode.Escape)){
			pauseState = PauseState.pause;
			Time.timeScale = 0; //pausa o jogo
		}
		
		if(turnoDoJogador && Network.peerType != NetworkPeerType.Disconnected)
			networkView.RPC("sincronizaGameStatus",RPCMode.Others,existeJogadorSelecionado.ToString(),turnoTimeA.ToString(),timeAUltimoTocar.ToString(),segundoTempo.ToString(),numeroToqueTime);
		
		turnoDoJogador = (Network.peerType == NetworkPeerType.Disconnected || (Network.isServer && turnoTimeA) || (Network.isClient && !turnoTimeA));
		
		if(pauseState == PauseState.halfTime && !segundoTempo){
			segundoTempo = true;
			jogadorSelecionado = null;
			existeJogadorSelecionado = false;
			turnoTimeA = false;
			chute = false;
			goleiroPronto = false;
			tipoToque = TipoToque.normal;
			reposicionarBolaSemWait();
		}
		
		if(pauseState != PauseState.none)
			return;
		
		if(turnoDoJogador){
			if(Input.GetButtonDown("Chute") && !chute){
				if(Network.peerType == NetworkPeerType.Disconnected){
					avisoChute();
				}else{
					tipoToque = GameStatus.TipoToque.chute;
					networkView.RPC("avisoChute",RPCMode.All);
				}
			}
		}
	}
	
	[RPC]
	void sincronizaGameStatus(string syncEJS,string syncTTA,string syncTAUT, string syncSEGT, int syncNUMTOQUE){
		existeJogadorSelecionado = bool.Parse(syncEJS);
		turnoTimeA = bool.Parse(syncTTA);
		timeAUltimoTocar = bool.Parse(syncTAUT);
		segundoTempo = bool.Parse(syncSEGT);
		numeroToqueTime = syncNUMTOQUE;
	}
	
	public void selecionaJogador(GameObject jogador){
		existeJogadorSelecionado = true;
		jogadorSelecionado = jogador;
	}
	
	public void deselecionaJogador(){
		existeJogadorSelecionado = false;
	}
	
	public void gol(bool timeA){
		Debug.Log("its goal fucking looser");
		if(timeA){
			turnoTimeA = false;
		}else{
			turnoTimeA = true;
		}
		numeroToqueTime = 0;
		SendMessage("show","gol");
		if(Network.peerType == NetworkPeerType.Disconnected)
			StartCoroutine(reposicionarBola());
		else
			networkView.RPC("reposicionarBolaSemWait",RPCMode.All);
	}
	
	public void foraDeJogo(){
		if(Network.peerType == NetworkPeerType.Disconnected)
        	StartCoroutine(reposicionarBola());
		else
			networkView.RPC("reposicionarBolaSemWait",RPCMode.All);
	}
	
	public void verificaFalta(string tagJogadorInfrator,string tagName, Vector3 posicao){
		Debug.Log("verificaFalta()");
		GameObject[] time;
		GameObject jogadorMaisPerto;
		float batedorX,
			batedorZ,
			cateto,
			hipotenusa,
			novaHipotenusa,
			novoCatetoAux,
			posicaoGoleiroX;
		Vector3 posicaoBatedor;
		
		/** Verificar se foi penalti  x(26)  z(15)*/
		if (posicao.z >= -15 && posicao.z <=15){
			Debug.Log("tagname = "+tagJogadorInfrator+"  x? = "+(posicao.x <= -26)+"  ?  "+(posicao.x >= -41));
				if(tagJogadorInfrator == "jogador time b" && posicao.x >= 26 && posicao.x <= 41){
					Debug.Log("Penalti do time B");
					turnoTimeA = true;
					this.bolaParada = true;
					penalti(posicao,true);
					return;
				}else if(tagJogadorInfrator == "jogador time a" && posicao.x <= -26 && posicao.x >= -41){
					Debug.Log("Penalti do time A");
					turnoTimeA = false;
					this.bolaParada = true;	
					penalti(posicao,false);
					return;
				}
		}
		
		novoCatetoAux = 1;
		posicaoGoleiroX= 42;
		
		//Se for turno do time B e ele tocar um Alguem do time A
		if(tagJogadorInfrator == "jogador time b" && tagName == "jogador time a"  ){
			Debug.Log("Falta do time B em A x = "+posicao.x+" posicao z = "+posicao.z);
			time = GameObject.FindGameObjectsWithTag("jogador time a");	
			
			batedorX = -1.5f;
			novoCatetoAux = posicao.z > 0 ? -1.0f : 1.0f;
			turnoTimeA = true;
			this.bolaParada = true;
		}
		//Se for turno do time A e ele tocar um Alguem do time B
		else if(tagJogadorInfrator == "jogador time a"  && tagName =="jogador time b" ){
			Debug.Log("Falta do time A em B x = "+posicao.x+" posicao z = "+posicao.z);
			time = GameObject.FindGameObjectsWithTag("jogador time b");	
			
			posicaoGoleiroX *= -1;
			batedorX = 1.5f;
			novoCatetoAux = posicao.z > 0 ? 1.0f : -1.0f;
			turnoTimeA = false;
			this.bolaParada = true;
		}else{
			return;
		}
		
		this.changeBall();
			numeroToqueTime = 0;
		afastarJogadores(posicao, 7.5f);
		jogadorMaisPerto = getJogadorMaisPerto(posicao, time);

		this.activeBall.transform.position = new Vector3(posicao.x, 0.5f, posicao.z);
		botaoEmMovimento = false;
		SendMessage("show","falta");
		
		//Debug.Log("posicao goleiro x = "+posicaoGoleiroX+" batedorX = "+batedorX); 
		
		printGOPosition(jogadorMaisPerto);
		/** Calculando para jogador ficar virado para o gol */
		
		//Calcula a Distancia (Hipotenusa) do centro gol adversario até o jogador
		hipotenusa = Mathf.Sqrt( Mathf.Pow(posicaoGoleiroX - posicao.x, 2) + Mathf.Pow(posicao.z, 2) );
		//Debug.Log("Hipotenusa = "+hipotenusa);
		//Cacula a distancia em X horizontal(cateto) do gol até o jogador
		cateto = Mathf.Sqrt( Mathf.Pow(posicaoGoleiroX - posicao.x, 2)  );
		//Debug.Log("cateto = "+cateto);
		//Distancia da Bola até onde o jogador irá ficar para cobrar a falta
		novaHipotenusa = (hipotenusa * batedorX) / cateto;
		novaHipotenusa = posicao.z > 0 ? novaHipotenusa: novaHipotenusa * -1;
		//Debug.Log("Nova Hipotenusa = "+novaHipotenusa);
		//Distancia em Z Vertical da bola até o jogador
		batedorZ = Mathf.Sqrt( Mathf.Pow(novaHipotenusa, 2) - Mathf.Pow(batedorX, 2) );
		
		batedorZ = posicao.z > 0 ? batedorZ : batedorZ * -1;
		//Debug.Log("Novo Cateto = "+novoCateto);
		jogadorMaisPerto.rigidbody.velocity = new Vector3(0,0,0);
		
		posicaoBatedor = new Vector3(posicao.x + batedorX, 0.3f, posicao.z + batedorZ);
		
		jogadorMaisPerto.transform.position = posicaoBatedor;
		SendMessage("show","falta");
		//printGOPosition(jogadorMaisPerto);
	} 
	
	public void penalti(Vector3 posicao, bool penaltiTimeA){
		float penaltiX,
			dist;
		GameObject jogadorMaisPerto;
		GameObject[] time;
		
		afastarJogadores(posicao, 7.5f);
		
		dist = 2.0f;
		penaltiX = 31.5f;
		
		if(posicao.x < 0){
			penaltiX *= -1;
			dist *= - 1;
		}
		
		numeroToqueTime = 0;
		if(penaltiTimeA){
			time = GameObject.FindGameObjectsWithTag("jogador time a");	
		}else{
			time = GameObject.FindGameObjectsWithTag("jogador time b");			
		}
		
		jogadorMaisPerto = getJogadorMaisPerto(this.activeBall.transform.position, time);
		
		this.activeBall.transform.position = new Vector3(penaltiX, 0.3f, 0);
		
		jogadorMaisPerto.transform.position = new Vector3(penaltiX - dist, 0.3f, 0);
		botaoEmMovimento = false;
		
	}
	
	public void tiroDeMetaEscanteio(){
		/**Verificando se é escanteio ou tiro de meta */
		if((timeAUltimoTocar && this.activeBall.transform.position.x > 0) || (!timeAUltimoTocar && this.activeBall.transform.position.x < 0)){
			tiroDeMeta(true);
		}else{
			escanteio();
		}
	}
	
	public void tiroDeMeta(bool mudaTurno){
		Debug.Log("tiroDeMeta()");
		float tiroDeMetaX;
		GameObject[] time;
		GameObject jogadorMaisPerto;
		Vector3 posicaoTiroDeMeta;
	
		this.bolaParada = true;
		posicaoTiroDeMeta = new Vector3( 36.5f , 0.3f, 7.4f);
		tiroDeMetaX = posicaoTiroDeMeta.x + 2.0f;
		if(this.activeBall.transform.position.x < 0){
			posicaoTiroDeMeta.x *= -1;
			tiroDeMetaX *= -1;
		}
		
		if(this.activeBall.transform.position.z < 0){
			posicaoTiroDeMeta.z *= -1;
		}
		
		numeroToqueTime = 0;
		this.changeBall();
		if(mudaTurno) turnoTimeA = !timeAUltimoTocar;
		
		if(turnoTimeA){
			time = GameObject.FindGameObjectsWithTag("jogador time a");	
		}else{
			time = GameObject.FindGameObjectsWithTag("jogador time b");			
		}
		
		jogadorMaisPerto = getJogadorMaisPerto(posicaoTiroDeMeta, time);
		
		afastarJogadores(posicaoTiroDeMeta, 7.5f);
		
		jogadorMaisPerto.transform.position = new Vector3(tiroDeMetaX, posicaoTiroDeMeta.y, posicaoTiroDeMeta.z);
		tipoToque = TipoToque.tiroDeMeta;
		this.activeBall.transform.position = posicaoTiroDeMeta;
		botaoEmMovimento = false;
		SendMessage("show","tiroDeMeta");
	}
	
	private void escanteio(){
		Debug.Log("escanteio()");	
		float escanteioZ,
			escanteioX;
		GameObject[] time;
		GameObject jogadorMaisPerto;
		Vector3 posicaoEscanteio;
	
		this.bolaParada = true;
		posicaoEscanteio = new Vector3( 41.0f , 0.3f, 25.0f);
		escanteioZ = posicaoEscanteio.z + 1.5f;
		escanteioX = posicaoEscanteio.x + 1.5f;

		if(this.activeBall.transform.position.x < 0){
			Debug.Log("x negativo");
			posicaoEscanteio.x *= -1;
			escanteioX *= -1;
		}
		
		if(this.activeBall.transform.position.z < 0){
			Debug.Log("z negativo");
			posicaoEscanteio.z *= -1;
			escanteioZ *= -1;
		}		
		
		this.changeBall();
		numeroToqueTime = 0;
		
		turnoTimeA = !timeAUltimoTocar;
		
		if(turnoTimeA){
			time = GameObject.FindGameObjectsWithTag("jogador time a");	
		}else{
			time = GameObject.FindGameObjectsWithTag("jogador time b");			
		}
	
		jogadorMaisPerto = getJogadorMaisPerto(posicaoEscanteio, time);
		afastarJogadores(posicaoEscanteio, 7.5f);
		
		jogadorMaisPerto.transform.position = new Vector3(escanteioX, posicaoEscanteio.y, escanteioZ);
		
		this.activeBall.transform.position = posicaoEscanteio;
		botaoEmMovimento = false;
		SendMessage("show","escanteio");
	}
	
	public void lateral(){
		Debug.Log("Lateral()");
		GameObject[] time;
		GameObject ball,
			jogadorMaisPerto;
		float lateralPosition;
		Vector3 posicaoLateral;		
		
		ball = this.activeBall;
		posicaoLateral = ball.transform.position;
				
		this.changeBall();
		this.bolaParada = true;
		
		lateralPosition = posicaoLateral.z > 0 ? +26 : -26;
		
		posicaoLateral.z = lateralPosition;
		
		this.activeBall.transform.position = new Vector3(posicaoLateral.x, 0.3f, posicaoLateral.z) ;
		
		turnoTimeA = !timeAUltimoTocar;
		
		numeroToqueTime = 0;
		
		if(turnoTimeA){
			time = GameObject.FindGameObjectsWithTag("jogador time a");
		}else{
			time = GameObject.FindGameObjectsWithTag("jogador time b");			
		}
			
		jogadorMaisPerto = getJogadorMaisPerto(posicaoLateral, time);
		afastarJogadores(posicaoLateral, 7.5f);
		
		/** Posicionando-o */
		lateralPosition = lateralPosition > 0 ? 1.75f: -1.75f;
		jogadorMaisPerto.rigidbody.velocity = new Vector3(0,0,0);
		jogadorMaisPerto.transform.position = new Vector3(posicaoLateral.x, 0.3f, posicaoLateral.z + lateralPosition);
		botaoEmMovimento = false;
		SendMessage("show","lateral");
	}
	
	public void afastarJogadores(Vector3 posicao, float distancia){
		float distanciaAtual,
			diferenca,
			novoX,
			novoZ;
		GameObject[] timeA = GameObject.FindGameObjectsWithTag("jogador time a");
		GameObject[] timeB = GameObject.FindGameObjectsWithTag("jogador time b");
		
		/** Arrumar isso, usar somente um array */
		foreach(GameObject botao in timeA){
			if(botao.name == "goleiro") continue;
			distanciaAtual = Mathf.Sqrt( Mathf.Pow( posicao.x - botao.transform.position.x, 2) + 
				Mathf.Pow( posicao.z - botao.transform.position.z, 2) );
			
			if(Mathf.Abs(distanciaAtual) < distancia){
				diferenca = distancia - distanciaAtual;
				
				//Debug.Log("1 = diferenca = "+diferenca+ " distancia Do botao = "+distanciaAtual);
				
				novoX = botao.transform.position.x + (botao.transform.position.x > posicao.x ? diferenca : (diferenca * -1));
				novoZ = botao.transform.position.z + (botao.transform.position.z > posicao.z ? diferenca : (diferenca * -1));
				
				/*
				Debug.Log("novoX = "+novoX+" novoZ = "+novoZ);
				distanciaAtual = Mathf.Sqrt( Mathf.Pow( posicao.x - botao.transform.position.x, 2) + Mathf.Pow( posicao.z - botao.transform.position.z, 2) );
				Debug.Log("diferenca agora = "+distanciaAtual);
				*/
				botao.transform.position = new Vector3( novoX, 0.3f, novoZ);
				botao.rigidbody.velocity = new Vector3(0,0,0);
			}
			
		}
		
		foreach(GameObject botao in timeB){
			if(botao.name == "goleiro") continue;
			distanciaAtual = Mathf.Sqrt( Mathf.Pow( posicao.x - botao.transform.position.x, 2) + 
				Mathf.Pow( posicao.z - botao.transform.position.z, 2) );
			
			if(Mathf.Abs(distanciaAtual) < distancia){
				diferenca = distancia - distanciaAtual;
				
				//Debug.Log("2 = diferenca = "+diferenca+ " distancia Do botao = "+distanciaAtual);
				
				novoX = botao.transform.position.x + (botao.transform.position.x > posicao.x ? diferenca : (diferenca * -1));
				novoZ = botao.transform.position.z + (botao.transform.position.z > posicao.z ? diferenca : (diferenca * -1));
				
				/*
				Debug.Log("novoX = "+novoX+" novoZ = "+novoZ);
				distanciaAtual = Mathf.Sqrt( Mathf.Pow( posicao.x - botao.transform.position.x, 2) + Mathf.Pow( posicao.z - botao.transform.position.z, 2) );
				Debug.Log("diferenca agora = "+distanciaAtual);
				*/
				
				botao.transform.position = new Vector3( novoX, 0.3f, novoZ);
				botao.rigidbody.velocity = new Vector3(0,0,0);
			}	
		}
	}
	
	public GameObject getJogadorMaisPerto(Vector3 posicao, GameObject[] time){
		GameObject retorno;
		int posicaoMenor,
			posicaoAtual;
		float menorDistancia,
			distanciaAtual;
		
		menorDistancia = 999;
		posicaoAtual = 0;
		posicaoMenor = 0;
		
		/** Calcuando qual o jogador mais perto da bola); */
		foreach(GameObject botao in time){
			Debug.Log("Nome = "+botao.name);
			if(botao.name == "goleiro") continue;
			distanciaAtual = Mathf.Sqrt( Mathf.Pow( posicao.x - botao.transform.position.x, 2) + 
				Mathf.Pow( posicao.z - botao.transform.position.z, 2) );
			
			if(distanciaAtual <= menorDistancia){
				menorDistancia = distanciaAtual;
				posicaoMenor = posicaoAtual;
			}
			botao.rigidbody.velocity = new Vector3(0,0,0);
			posicaoAtual++;
		}
		
		if(time[posicaoMenor].name == "goleiro"){
			if(posicaoMenor < time.Length-1) posicaoMenor++;
			else if(posicaoMenor > 0) posicaoMenor--;
		}
		
		retorno = time[posicaoMenor];
		
		return retorno;
	}
	
    IEnumerator reposicionarBola(){
        yield return new WaitForSeconds(2f);
		this.changeBall();
		this.activeBall.transform.position = new Vector3(0, 5, 0);
		GameObject[] timeA = GameObject.FindGameObjectsWithTag("jogador time a");
		GameObject[] timeB = GameObject.FindGameObjectsWithTag("jogador time b");
		GameObject ball = this.activeBall;
		BallControl ballControl = ball.GetComponent<BallControl>();
		ballControl.disparaTrigger = false;
		recuperaPosicaoTime(timeA,true);
		recuperaPosicaoTime(timeB,false);
	}
	
	[RPC]
	public void reposicionarBolaSemWait(){
		this.changeBall();
		this.activeBall.transform.position = new Vector3(0, 5, 0);
		GameObject[] timeA = GameObject.FindGameObjectsWithTag("jogador time a");
		GameObject[] timeB = GameObject.FindGameObjectsWithTag("jogador time b");
		GameObject ball = this.activeBall;
		BallControl ballControl = ball.GetComponent<BallControl>();
		ballControl.disparaTrigger = false;
		recuperaPosicaoTime(timeA,true);
		recuperaPosicaoTime(timeB,false);
	}
	
	private void mudaTexturas(){
		GameObject[] golList = GameObject.FindGameObjectsWithTag("gol");
		GameObject[] bolaList = GameObject.FindGameObjectsWithTag("ball");
		GameObject[] campoList = GameObject.FindGameObjectsWithTag("campo");
		
		foreach(GameObject gol in golList){
			MeshRenderer[] meshRenders =  gol.GetComponentsInChildren<MeshRenderer>();
			foreach(MeshRenderer render in meshRenders){
				render.material = data.gol;
			}
		}
		
		foreach(GameObject bola in bolaList){
			MeshRenderer[] meshRenders =  bola.GetComponentsInChildren<MeshRenderer>();
			foreach(MeshRenderer render in meshRenders){
				render.material = data.ball;
			}
		}
		
		foreach(GameObject campo in campoList){
			MeshRenderer[] meshRenders =  campo.GetComponentsInChildren<MeshRenderer>();
			foreach(MeshRenderer render in meshRenders){
				render.material = data.field;
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
	
	public void recuperaPosicaoTime(GameObject[] time, bool timeA){
		int numJogador = 0;
		foreach(GameObject jogador in time){
			if(timeA){
				jogador.transform.position = posicaoInicialTimeA[numJogador] ;
				if(turnoTimeA){
					if(jogador.name == "jogador10A") jogador.transform.position = new Vector3(-0.2309722f,0.4f,1.911556f);
					else if(jogador.name == "jogador11A") jogador.transform.position = new Vector3(-1.338067f,0.4f,-1.100141f);
				}
			}else{
				jogador.transform.position = posicaoInicialTimeB[numJogador];
				if(!turnoTimeA){
					if(jogador.name == "jogador10B") jogador.transform.position = new Vector3(0.2309722f,0.4f,1.911556f);
					else if(jogador.name == "jogador11B") jogador.transform.position = new Vector3(1.338067f,0.4f,-1.100141f);
				}
			}
			numJogador++;
		}
	}
	
	public void zeraToquesTime(bool timeA){
		string nomeTime = "jogador time a";
		if(!timeA) nomeTime = "jogador time b";
		GameObject[] time = GameObject.FindGameObjectsWithTag(nomeTime);
		int numJogador = 0;
		foreach(GameObject jogador in time){
			jogador.GetComponent<PlayerControl>().numeroToqueJogador = 0;
			numJogador++;
		}
	}
	
	public void changeBall(){
		activeBall.GetComponent<BallControl>().disparaTrigger = false;
		activeBall.rigidbody.velocity = Vector3.zero;
	}
	
	public void printGOPosition(GameObject obj){
		Debug.Log("Name = "+obj.name+" X = "+obj.transform.position.x+" Y = "+obj.transform.position.y+" Z = "+obj.transform.position.z);
	}
	
	[RPC]
	void avisoChute(){
		SendMessage("show","chute");
		chute = true;
		botaoEmMovimento = false;
		AudioClip clipUtilizado = taLaTimeA;
		if(!turnoTimeA) clipUtilizado = taLaTimeB;
		GetComponent<AudioSource>().clip = clipUtilizado;
		GetComponent<AudioSource>().Play();
		GameObject.Find("Game").GetComponent<ReplayScript>().startRecordReplay();
		tipoToque = TipoToque.chute;
		if(turnoTimeA){
			chuteScript.goleiroUtilizado = goleiroTimeB;
			chuteScript.nomeCollider = "golColliderTimeB";
		}else{
			chuteScript.goleiroUtilizado = goleiroTimeA;
			chuteScript.nomeCollider = "golColliderTimeA";
		}
	}
}