using UnityEngine;
using System.Collections;

public class SharedData : MonoBehaviour {
	
	public string nomeTimeCasa = "Selecione uma equipe";
	public string nomeTimeFora = "Selecione uma equipe";
	public string apelidoTimeCasa = "";
	public string apelidoTimeFora = "";
	public Material kitTimeCasa;
	public Material kitTimeFora;
	public Texture2D logoTimeCasa;
	public Texture2D logoTimeFora;
	
	void Awake () {
		DontDestroyOnLoad (this);
	}
	
	void Start() {
		nomeTimeCasa = "Selecione uma equipe";
		nomeTimeFora = "Selecione uma equipe";
		apelidoTimeCasa = "";
		apelidoTimeFora = "";
		kitTimeCasa = null;
		kitTimeFora = null;
	}
}
