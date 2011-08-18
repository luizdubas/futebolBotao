using UnityEngine;
using System.Collections;

public class TimeControl : MonoBehaviour
{
	private GameStatus game;
	public string tagJogadores;
	
	void Awake(){
		game = GameObject.Find("Game").GetComponent<GameStatus>();
	}
	
	// Use this for initialization
	void Start ()
	{
		if(tagJogadores.Equals("jogador time a")){
			mudaUniforme(game.data.kitTimeCasa, game.data.goleiroTimeCasa);
		}else{
			mudaUniforme(game.data.kitTimeFora, game.data.goleiroTimeFora);
		}
	}
	
	private void mudaUniforme(Material uniforme, Material uniformeGoleiro){
		MeshRenderer[] meshRenders = GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer render in meshRenders){
			if(render.gameObject.name == "jogador"){
				render.material = uniforme;
			}
			if(render.gameObject.name == "goleiro"){
				render.material = uniformeGoleiro;
			}
		}
	}
}

