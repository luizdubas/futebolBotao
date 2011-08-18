using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ReplayScript : MonoBehaviour {

	public enum ReplayState { none, recording, playing,  };
	
	private float actualTime;
	private float recordFrameTime;
	private float timeToRecord;
	private int actualReplay;
	private int playingPosition;
	private int numberOfReplayedObjects = 24;
	private bool playAllReplays;
	
	private List<List<Vector3>> replay;
	private List<List<List<Vector3>>> bestMoments;
	private List<Vector3> auxList;
	
	private GameStatus.PauseState estadoAntesDeAcionarMelhoresMomentos;
	private float tempoAntesMelhoresMomentos;
	
	GameStatus game;
	GUIPlacar placar;
	GameObject[] timeA;
	GameObject[] timeB;
	
	public ReplayState estado;
	private Collider callbackCollider;
	
	List<Vector3> posList;
	
	// Use this for initialization 
	void Start () {
		actualTime = 0;
		recordFrameTime = 0.05f;
		estado = ReplayState.none;
		timeToRecord = 15;
		bestMoments = new List<List<List<Vector3>>>();
		
		game = GameObject.Find("Game").GetComponent<GameStatus>();
		placar= GameObject.Find("Game").GetComponent<GUIPlacar>();
		
		timeA = GameObject.FindGameObjectsWithTag("jogador time a");
		timeB = GameObject.FindGameObjectsWithTag("jogador time b");
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if(game.pauseState != GameStatus.PauseState.none)
			return;
		*/
		if(Network.peerType == NetworkPeerType.Disconnected)
			switch(estado){
				case ReplayState.recording:
					actualTime += Time.deltaTime;
	
					if(actualTime > recordFrameTime){
	//					Debug.Log("Gravando...");	
						actualTime -= recordFrameTime;
	
						gravaPosicoes();
						
						if(replay.Count >= timeToRecord / recordFrameTime){
							replay.RemoveAt(0);
						}
					}
				break;
				case ReplayState.playing:
					actualTime += Time.deltaTime;
					if(actualTime > recordFrameTime){
						//Debug.Log("Reproduzindo... "+playingPosition+" - "+replay.Count);
						actualTime -= recordFrameTime;
						
						if(playingPosition < replay.Count){
							reproduzPosioes();
							playingPosition++;
						}else{
							if(playAllReplays){
								nextReplay();
							}else{
								stopReplay();
							}
						}
					}
				break;
			}
	}

	public void startRecordReplay(){
		//Debug.Log("startRecordReplay()");
		replay = new List<List<Vector3>>((int)(timeToRecord / recordFrameTime));
		estado = ReplayState.recording;
	}
	
	public void startReplay(Collider other){
		//Debug.Log("startReplay()");
		if(estado == ReplayState.recording){
			stopRecordReplay();
		}
		if(estado == ReplayState.playing){
			return;
		}
		if(replay.Count == 0){
			estado = ReplayState.none;
			return;
		}
		Time.timeScale = 0.7f;
		SendMessage("show","replay");
		gravaPosicoes();
		
		playAllReplays = false;
		estado = ReplayState.playing;
		actualTime = 0;
		playingPosition = 0;
		callbackCollider = other;
	}
	
	public IEnumerator startReplayWithWait(Collider other){
        //Debug.Log("startReplay()");
		yield return new WaitForSeconds(0.2f);
		if(estado == ReplayState.recording){
			stopRecordReplay();
		}
		if(estado == ReplayState.playing){
			yield break;
		}
		if(replay.Count == 0){
			estado = ReplayState.none;
			yield break;
		}
		SendMessage("show","replay");
		
		gravaPosicoes();
		
		playAllReplays = false;
		estado = ReplayState.playing;
		actualTime = 0;
		playingPosition = 0;
		callbackCollider = other;
	}
	
	public void startAllReplay(GameStatus.PauseState estadoGame, float tempoGravar){
		//Debug.Log("startAllReplay() "+bestMoments.Count);
		if(bestMoments.Count == 0){
			estado = ReplayState.none;
			return;
		}
		SendMessage("show","replay");
		estadoAntesDeAcionarMelhoresMomentos = estadoGame;
		tempoAntesMelhoresMomentos = tempoGravar;
		playAllReplays = true;
		estado = ReplayState.playing;
		actualTime = 0;
		playingPosition = 0;
		actualReplay = 0;
		replay = bestMoments[actualReplay];
	}

	public void stopRecordReplay(){
		//Debug.Log("stopRecordReplay()");
		estado = ReplayState.none;
		bestMoments.Add(replay);
	}
	
	public void stopReplay(){
		estado = ReplayState.none;
		
		replay = new List<List<Vector3>>();
		BallControl ball = GameObject.FindWithTag("ball").GetComponent<BallControl>();
		
		ball.callOnTriggerEnter(callbackCollider);
		Time.timeScale = 1;
		SendMessage("hide");
	}
	
	public void nextReplay(){
		actualReplay++;
		actualTime = 0;
		playingPosition = 0;
		if(actualReplay >= bestMoments.Count){
			Time.timeScale = 0;
			estado = ReplayState.none;
			game.pauseState = estadoAntesDeAcionarMelhoresMomentos;			
			GameObject.Find("Game").GetComponent<GUIPlacar>().tempo = tempoAntesMelhoresMomentos;
			game.reposicionarBolaSemWait();
			SendMessage("hide");
			return;
		}
		
		replay = bestMoments[actualReplay];
	}
	
	public void gravaPosicoes(){
		auxList = new List<Vector3>(numberOfReplayedObjects);
		
		foreach(GameObject botao in timeA){
			auxList.Add(botao.transform.position);
		}
					
		foreach(GameObject botao in timeB){
			auxList.Add(botao.transform.position);			
		}
		
		auxList.Add(new Vector3(placar.tempo,0 ,0));
		
		auxList.Add(game.activeBall.transform.position);			
		
		auxList.Add(game.activeBall.rigidbody.velocity);			
		auxList.Add(game.activeBall.rigidbody.angularVelocity);			
		
		replay.Add(auxList);
	}
	
	public void reproduzPosioes(){
		posList = replay[playingPosition];	
		
		for( int i = 0; i < 11;  i++){
			timeA[i].transform.position = posList[i];
			timeA[i].rigidbody.velocity = new Vector3(0,0,0);
		}
		
		for( int i = 0; i < 11;  i++){
			timeB[i].transform.position  = posList[i + 11];
			timeB[i].rigidbody.velocity = new Vector3(0,0,0);
		}
		
		placar.tempo = posList[22].x - Time.deltaTime;
		
		if(posList[23].y < 0.225f){
			game.activeBall.transform.position = new Vector3(posList[23].x, 0.225f, posList[23].z);	
			game.activeBall.rigidbody.velocity = new Vector3(0,0,0);
		}else{
			game.activeBall.transform.position = posList[23];
			game.activeBall.rigidbody.velocity = new Vector3(0,0,0);
		}
		
	}
	
	public bool isPlayingReplay(){
		return estado == ReplayState.playing;
	}
}