using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	private float forca = 0;
	private Vector3 playerPosition;
	private Vector3 playerFinalPosition;
	private GameStatus game;
	private ReplayScript replay;
	  
	void Awake(){
		game = GameObject.Find("Game").GetComponent<GameStatus>();
		replay = GameObject.Find("Game").GetComponent<ReplayScript>();
	}
	
	void FixedUpdate(){
		if(!game.gameStarted || (game.pauseState != GameStatus.PauseState.none && replay.estado == ReplayScript.ReplayState.none) || game.botaoEmMovimento || (game.chute && !game.goleiroPronto))
			return;
		if(game.turnoDoJogador){
			if(game.data.estiloControles == 0){
				estiloControleArrasta();
			}else{
				estiloControleToque();
			}
		}
	}
	
	void estiloControleArrasta(){
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
								game.tipoToque = GameStatus.TipoToque.normal;
								if(game.chute) game.tipoToque = GameStatus.TipoToque.chute;
								playerControl.ativarJogador();
								forca = 2f;
								game.forca = forca;
								if(game.tipoToque == GameStatus.TipoToque.chute){ forca = 5;}
							}
						}
					}
				}else{
					aplicaForca(hitInfo.point);
				}
			}else{
				retiraLinha();
				moverJogador();
			}
		}
	}
	
	[RPC]
	void aplicaForca(Vector3 point){
		if(forca < 25){
			forca += (4f*Time.deltaTime);
		}
		game.jogadorSelecionado.GetComponent<PlayerControl>().forca = forca;
		game.forca = forca;
		playerPosition = game.jogadorSelecionado.transform.position;
		playerFinalPosition = -((point-playerPosition));
		playerFinalPosition = new Vector3(playerFinalPosition.x,0.3f,playerFinalPosition.z);
		mostraLinha(playerPosition,playerFinalPosition);
	}
	
	[RPC]
	void ativarJogador(string nome){
		PlayerControl playerControl = GameObject.Find(nome).GetComponent<PlayerControl>();
		playerControl.ativarJogador();
	}
	
	void moverJogador(){
		if(game.existeJogadorSelecionado){
			PlayerControl playerControl = game.jogadorSelecionado.GetComponent<PlayerControl>();
			playerControl.posicaoFinal = playerFinalPosition;
			playerControl.numeroToqueJogador++;
			playerControl.wait = 0;
			game.numeroToqueTime++;
			game.chute = false;
			game.forca = 0;
			game.tocouBola = false;
			game.bolaParada = false;
			game.deselecionaJogador();
		}
	}
	
	[RPC]
	void moverJogador(Vector3 point){
		if(game.existeJogadorSelecionado){
			PlayerControl playerControl = game.jogadorSelecionado.GetComponent<PlayerControl>();
			playerControl.posicaoFinal = point;
			playerControl.numeroToqueJogador++;
			playerControl.wait = 0;
			game.numeroToqueTime++;
			game.chute = false;
			game.forca = 0;
			game.tocouBola = false;
			game.bolaParada = false;
			game.deselecionaJogador();
		}
	}

	void estiloControleToque(){
		if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetMouseButtonDown(1))
        {
            if (forca < 50) forca += 2.5f;
            Debug.Log("forca = " + forca);
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetMouseButtonDown(2))
        {
            if (forca > 0) forca -= 2.5f;
            Debug.Log("forca = " + forca);
        }


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            Collider body = hitInfo.collider;
            if (body != null && body.tag != "campo")
            {
                PlayerControl playerControl = body.GetComponent<PlayerControl>();
                if (playerControl != null)
                {
                    if ((game.turnoTimeA && body.tag == "jogador time a") || (!game.turnoTimeA && body.tag == "jogador time b")){
                        if (Input.GetMouseButton(0)){
                            playerControl.ativarJogador();
							game.tocouBola = false;
							playerControl.forca = forca;
                            playerPosition = game.jogadorSelecionado.GetComponent<Transform>().position;
                            playerFinalPosition = -(hitInfo.point - playerPosition);
                            playerFinalPosition = new Vector3(playerFinalPosition.x * forca, 0, playerFinalPosition.z * forca);
                            playerControl.posicaoFinal = playerFinalPosition;
                            game.deselecionaJogador();
                            forca = 0;
                        }
                        else{
                            playerPosition = body.gameObject.transform.position;
                            playerFinalPosition = -(hitInfo.point - playerPosition);
                            playerFinalPosition = new Vector3(playerFinalPosition.x * forca, 0, playerFinalPosition.z * forca);
							mostraLinha(playerPosition,playerFinalPosition);
                        }
                    }
                }else{
					retiraLinha();
				}
            }else{
				retiraLinha();
			}
        }
	}
	
	public void mostraLinha(Vector3 playerPosition, Vector3 playerFinalPosition){
		game.line.transform.position = playerPosition;
		game.line.GetComponent<LineRenderer>().SetPosition(1,playerFinalPosition);
	}
	
	public void retiraLinha(){
		game.line.transform.position = new Vector3(0,0,0);
		game.line.GetComponent<LineRenderer>().SetPosition(1,new Vector3(0,0,0));
	}
}
