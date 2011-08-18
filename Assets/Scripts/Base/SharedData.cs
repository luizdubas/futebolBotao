using UnityEngine;
using System.Collections;

public class SharedData : MonoBehaviour {
		
	public string nomeTimeCasa = "Selecione uma equipe";
	public string nomeTimeFora = "Selecione uma equipe";
    public string nomeEstadio = "";
	public Material kitTimeCasa;
	public Material goleiroTimeCasa;
	public Material kitTimeFora;
	public Material goleiroTimeFora;
	public Texture2D logoTimeCasa;
	public Texture2D pausaLogoTimeCasa;
	public Texture2D logoTimeFora;
	public Texture2D pausaLogoTimeFora;
	public Material ball;
	public Material gol;
	public Material field;
	public int estiloControles;
	public int tempoJogo;
	public int numeroToqueMaximo;
	public int numeroToqueJogadorSeguido;
	public int linguagem;
	public bool mostraReplay;
	public bool isNetworkGame;
	
	void Awake () {
		DontDestroyOnLoad (this);
	}
	
	void Start() {
		tempoJogo = 1;
		numeroToqueMaximo = 12;
		numeroToqueJogadorSeguido = 3;
		mostraReplay = true;
		linguagem = 0;
	}
}
