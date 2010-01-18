using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	private float forca = 1;
	private Vector3 playerPosition;
	private Vector3 playerFinalPosition;
	private GameStatus game;
	private SmoothFollow smoothFollow;
	
	void Awake(){
		smoothFollow = (Camera.main.GetComponent(typeof(SmoothFollow)) as SmoothFollow);
		game = GameObject.Find("Game").GetComponent<GameStatus>();
	}
	
	void FixedUpdate(){
		if(!game.gameStarted || (game.chute && game.existeJogadorSelecionado))
			return;
			
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitInfo = new RaycastHit();
		if (Physics.Raycast (ray,out hitInfo,Mathf.Infinity)) {
			if(Input.GetMouseButton(0)){
				if(!game.existeJogadorSelecionado){
					Collider body = hitInfo.collider;
					if(body != null && body.tag != "campo"){
						PlayerControl playerControl = body.GetComponent<PlayerControl>();
						if(playerControl != null){
							if((game.turnoTimeA && body.tag == "jogador time a") || (!game.turnoTimeA && body.tag == "jogador time b")){
								playerControl.ativarJogador();
								forca = 1;
								if(game.chute){
									smoothFollow.height = 2;
									smoothFollow.distance = 10;
									if(game.turnoTimeA){
										smoothFollow.angleY = 90;
									}else{
										smoothFollow.angleY = 270;
									}
									smoothFollow.angleX = 0;
								}
							}
						}
					}
				}else{
					if(forca < 10){
						forca += 0.05f;
					}
					playerPosition = game.jogadorSelecionado.GetComponent<Transform>().position;
					playerFinalPosition = -(hitInfo.point-playerPosition)*forca;
					game.highlightSelecionado.transform.LookAt(new Vector3(playerFinalPosition.x,0,playerFinalPosition.z));
				}
			}else{
				moverJogador();
			}
		}
	}
	
	void moverJogador(){
		if(game.existeJogadorSelecionado){
			PlayerControl playerControl = game.jogadorSelecionado.GetComponent<PlayerControl>();
			playerControl.posicaoFinal = playerFinalPosition;
			game.deselecionaJogador();
			game.turnoTimeA = !game.turnoTimeA;
		}
	}
}
