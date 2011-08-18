using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct CameraSetting{ 
	private int distance, height, angleX, angleY;
	
	public CameraSetting(int distance, int height, int angleX, int angleY){
		this.distance = distance;
		this.height = height;
		this.angleX = angleX;
		this.angleY = angleY;
	}
	
	public int Distance{
		get { return distance; }
		set { distance = value; }
	}
	
	public int Height{
		get { return height; }
		set { height = value; }
	}
	
	public int AngleX{
		get { return angleX; }
	}
	
	public int AngleY{
		get { return angleY; }
	}
}

public class CameraControl : MonoBehaviour {

	private string cameraSettings;
	private Dictionary<string, CameraSetting> camera;
	
	void Start(){
		camera = createCameraSettings();
		cameraSettings = "5";
	}
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) cameraSettings = "1";
		else if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) cameraSettings = "2";
		else if(Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) cameraSettings = "3";
		else if(Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) cameraSettings = "4";
		else if(Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) cameraSettings = "5";
		else if(Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) cameraSettings = "6";
		else if(Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) cameraSettings = "7";
		else if(Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) cameraSettings = "8";
		else if(Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) cameraSettings = "9";
		
		if(camera == null) camera = createCameraSettings();
		
		CameraSetting setting = camera[cameraSettings];
		if(Input.GetButtonDown("Diminui Distancia")) setting.Distance-=5;
		if(Input.GetButtonDown("Aumenta Distancia")) setting.Distance+=5;
		if(Input.GetButtonDown("Diminui Altura")) setting.Height-=5;
		if(Input.GetButtonDown("Aumenta Altura")) setting.Height+=5;
		camera[cameraSettings] = setting;
		(GetComponent(typeof(SmoothFollow)) as SmoothFollow).height = setting.Height;
		(GetComponent(typeof(SmoothFollow)) as SmoothFollow).distance = setting.Distance;
		(GetComponent(typeof(SmoothFollow)) as SmoothFollow).angleX = setting.AngleX;
		(GetComponent(typeof(SmoothFollow)) as SmoothFollow).angleY = setting.AngleY;
	}
	
	private Dictionary<string, CameraSetting> createCameraSettings(){
		string settingName = ""; CameraSetting setting;
		Dictionary<string, CameraSetting> dictionary = new Dictionary<string,CameraSetting>();
		
		settingName = "1"; setting = new CameraSetting(50,20,30,45);
		dictionary.Add(settingName, setting);
		
		settingName = "2"; setting = new CameraSetting(30,40,50,0);
		dictionary.Add(settingName, setting);
		
		settingName = "3"; setting = new CameraSetting(50,20,30,-45);
		dictionary.Add(settingName, setting);
		
		settingName = "4"; setting = new CameraSetting(40,30,25,90);
		dictionary.Add(settingName, setting);
		
		settingName = "5"; setting = new CameraSetting(0,70,90,0);
		dictionary.Add(settingName, setting);
		
		settingName = "6"; setting = new CameraSetting(40,30,25,270);
		dictionary.Add(settingName, setting);
		
		settingName = "7"; setting = new CameraSetting(50,20,30,135);
		dictionary.Add(settingName, setting);
		
		settingName = "8"; setting = new CameraSetting(30,40,50,180);
		dictionary.Add(settingName, setting);
		
		settingName = "9"; setting = new CameraSetting(50,20,30,225);
		dictionary.Add(settingName, setting);
		return dictionary;
	}
}
