using UnityEngine;
using System.Collections;

public class ChuteScript : MonoBehaviour {
	
	private float forca = 0.01f;
	private Vector3 playerPosition;
	private Vector3 playerFinalPosition;
	private Vector3 ballPosition;
	private Vector3 ballFinalPosition;
	private GameStatus game;
	private GameObject ball;
	public GameObject goleiroUtilizado;
	public GameObject jogadorUtilizado;
	private SmoothFollow smoothFollow;
	public string nomeCollider;
	private bool chutePronto;
	private bool chutePreparado;
	private Vector3 goleiroPosition;
	private Quaternion rotacaoOriginal;
	
	void Awake(){
		smoothFollow = (Camera.main.GetComponent(typeof(SmoothFollow)) as SmoothFollow);
		game = GameObject.Find("Game").GetComponent<GameStatus>();
		ball = GameObject.FindWithTag("ball");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(!game.gameStarted || !game.chute || !game.existeJogadorSelecionado)
			return;
		
		if (Input.GetMouseButton(1)){
			Debug.Log("rot "+(Input.GetAxis("Mouse Y")*Mathf.Rad2Deg+90));
			goleiroUtilizado.transform.RotateAround(goleiroUtilizado.transform.position,Vector3.up,(Input.GetAxis("Mouse Y")*Mathf.Rad2Deg)*0.2f);
		}
		if(Input.GetMouseButton(0)){
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hitInfo = new RaycastHit();
			if (Physics.Raycast (ray,out hitInfo,Mathf.Infinity)) {
				ballPosition = ball.transform.position;
				if(hitInfo.collider.tag == nomeCollider){
					if(!chutePronto){
						if(forca < 3){
							forca += 0.02f;
						}
						playerFinalPosition = new Vector3(ballPosition.x*10,0,ballPosition.z*10);
						ballFinalPosition = (hitInfo.point-ballPosition) * forca;
						chutePreparado= true;
					}
				}else if(hitInfo.collider.tag == "campo" && chutePronto && chutePreparado){
					goleiroUtilizado.transform.position = new Vector3(hitInfo.point.x,goleiroUtilizado.transform.position.y,hitInfo.point.z);
					goleiroUtilizado.transform.eulerAngles = new Vector3(270,goleiroUtilizado.transform.eulerAngles.y,0);
					Debug.DrawRay(Vector3.zero,goleiroPosition);
				}
			}
		}else{
			if(chutePronto && game.goleiroPronto){
				chutar();
			}else if(chutePreparado){
				chutePronto = true;
				smoothFollow.height = 25;
				smoothFollow.distance = 0;
				smoothFollow.angleY = 0;
				smoothFollow.angleX = 90;
			}
			if(chutePronto && chutePreparado && Input.GetKey(KeyCode.R)){
				game.goleiroPronto = true;
			}
		}
	}
	
	void chutar(){
		smoothFollow.height = 25;
		smoothFollow.distance = 0;
		smoothFollow.angleY = 0;
		smoothFollow.angleX = 90;
		StartCoroutine(chute());
	}
	
	IEnumerator moverJogador(){
		yield return new WaitForSeconds (0.6f);
		ball.rigidbody.AddForce(ballFinalPosition,ForceMode.VelocityChange);
		ballFinalPosition = Vector3.zero;
		chutePronto = false;
		game.chute = false;
		game.bolaChutada = true;
		game.goleiroPronto = false;
		chutePreparado = false;
		game.turnoTimeA = !game.turnoTimeA;
		if(game.jogadorSelecionado != null){
			game.deselecionaJogador();
		}
	}
	
	IEnumerator chute(){
		ballFinalPosition = new Vector3(Random.Range(ballFinalPosition.x-(forca*0.5f),ballFinalPosition.x+(forca*0.5f)),Random.Range(ballFinalPosition.y-(forca*0.5f),ballFinalPosition.y+(forca*0.5f)),Random.Range(ballFinalPosition.z-(forca*0.5f),ballFinalPosition.z+(forca*0.5f)));
		Debug.Log("Final = "+ballFinalPosition);
		PlayerControl playerControl = jogadorUtilizado.GetComponent<PlayerControl>();
		playerControl.posicaoFinal = playerFinalPosition;
		yield return new WaitForSeconds (0.4f);
		StartCoroutine(moverJogador());
	}
}
