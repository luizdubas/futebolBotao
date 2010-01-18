using UnityEngine;
using System.Collections;

public class GUISelect : MonoBehaviour {
	
	public GUISkin guiSkin;
	public Texture2D[] logoTimes;
	public string[] nomeTimes;
	public string[] apelidoTimes;
	public string[] nomeTimesTextura;
	public Material[] kitsMaterial;
	public Texture2D[] kitsTexturas;
	public Texture2D chooserBackground;
	public Texture2D chooserHeaderLeft;
	public Texture2D chooserHeaderRight;
	public float areaWidth;
	public float areaHeight;
	private bool isSelectTime;
	private SharedData data;
	
	void Awake(){
		isSelectTime = true;
		data = GameObject.FindWithTag ("data").GetComponent<SharedData>();
	}
	
	void OnGUI(){
		GUI.skin = guiSkin;
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (Screen.height / 768f, Screen.height / 768f, 1)); 
		if(data != null){
			GUI.Box(new Rect(101f,170f,chooserBackground.width,chooserBackground.height),chooserBackground,guiSkin.GetStyle("chooserBox"));
			GUI.Box(new Rect(65f,135f,chooserHeaderLeft.width,chooserHeaderLeft.height),chooserHeaderLeft,guiSkin.GetStyle("chooserBox"));
			GUI.Box(new Rect(606f,170f,chooserBackground.width,chooserBackground.height),chooserBackground,guiSkin.GetStyle("chooserBox"));
			GUI.Box(new Rect(579f,135f,chooserHeaderRight.width,chooserHeaderRight.height),chooserHeaderRight,guiSkin.GetStyle("chooserBox"));
			GUI.Label(new Rect(125f,135f,chooserHeaderLeft.width,chooserHeaderLeft.height),data.nomeTimeCasa,guiSkin.GetStyle("labelRight"));
			GUI.Label(new Rect(515f,135f,chooserHeaderLeft.width,chooserHeaderLeft.height),data.nomeTimeFora,guiSkin.GetStyle("labelLeft"));
			if(isSelectTime){
				montaLogos(106f,185f,50,50,logoTimes,true);
				montaLogos(611f,185f,50,50,logoTimes,false);
				if(GUI.Button (new Rect(355f,676f,315f,30f),"Pronto",guiSkin.GetStyle("chooserButton"))){
					if(data.nomeTimeCasa != "Selecione uma equipe" && data.nomeTimeFora != "Selecione uma equipe"){
						isSelectTime = false;
					}
				}
			}else{
				montaLogos(106f,185f,50,50,kitsTexturas,data.nomeTimeCasa,true);
				montaLogos(611f,185f,50,50,kitsTexturas,data.nomeTimeFora,false);
				if(GUI.Button (new Rect(355f,676f,315f,30f),"Pronto",guiSkin.GetStyle("chooserButton"))){
					if(data.kitTimeCasa != null && data.kitTimeFora != null){
						Application.LoadLevel("field");
					}
				}
			}
		}
	}
	
	private void montaLogos(float screenX, float screenY, float width, float height, Texture2D[] images,bool timeCasa){
		int time = 0;
		foreach(Texture2D logo in images){
			if(GUI.Button (new Rect(screenX,screenY,width,height),logo,guiSkin.GetStyle("kitLogoButton")))
			{
				if(timeCasa){
					data.nomeTimeCasa= nomeTimes[time];
					data.apelidoTimeCasa = apelidoTimes[time];
					data.logoTimeCasa = logo;
				}else{
					data.nomeTimeFora= nomeTimes[time];
					data.apelidoTimeFora = apelidoTimes[time];
					data.logoTimeFora = logo;
				}
			}
			time++;
			screenX += 55f;
		}
	}
	
	private void montaLogos(float screenX, float screenY, float width, float height, Texture2D[] images,string nomeTime,bool timeCasa){
		int time = 0;
		foreach(Texture2D logo in images){
			if(nomeTime== nomeTimesTextura[time]){
				if(GUI.Button (new Rect(screenX,screenY,width,height),logo,guiSkin.GetStyle("kitLogoButton")))
				{
					if(timeCasa){
						data.kitTimeCasa = kitsMaterial[time];
					}else{
						data.kitTimeFora = kitsMaterial[time];
					}
				}
				screenX += 55f;
			}
			time++;
		}
	}
}