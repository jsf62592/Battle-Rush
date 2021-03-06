﻿using UnityEngine;
using System.Collections;
using System;

//ATTACH THIS SCRIPT TO CAMERA 
public class Interface : MonoBehaviour {
	
	public GameObject selected; //the player character that takes input
	public GameObject target; //the targeted enemy
	public Boolean targeted; //is there a taget
	public Boolean targeting; //are we looking for a target
	CharacterState state; //the state of the selected player character
	public Boolean drawButtons = false; //draw the ability input buttons	
	
	public static Interface instance;
	
	//declare these somewhere based on ability button prefabs
	public Texture ability1Texture;
	public Texture ability2Texture;
	public Texture ability3Texture;
	
	public Texture victoryScreen;
	public Texture defeatScreen;
	public Texture startScreen;
	public Texture startButton;
	public Texture offColor;
	
	public int touchX; //current input touch position
	public int touchY; 
	public int button = 0; //button that was hit
	
	public Boolean touch; //are we taking touch input instead of mouse
	public Boolean turn; //is it the player's turn
	public Vector3 retouch; //touch position fixed for scene positions
	
	public Boolean end;
	public Boolean start;
	public Boolean dead;
	public Boolean noTarget;
	public int size = 100;
	public int endCount;
	public int okayEndCount = 100;
	public int maxCooldown = 5;
	bool once;
	
	
	Ability ba;
	Ability ab;
	Ability heal;
	
	
	// Use this for initialization
	void Start () {
		once = false;
		instance = this;
		selected = GameObject.Find("P1");
		state = selected.GetComponent<CharacterState> ();
		target = null;
		targeted = false; 
		turn = false;
		end = false;
		dead = false;
		endCount = 0;
		ability1Texture = Resources.Load("Saw") as Texture;
		ability2Texture = Resources.Load("Bottle") as Texture;
		ability3Texture = Resources.Load("Bible") as Texture;
		
		ba = selected.GetComponent<Ability_Basic_Attack>();
		ab = selected.GetComponent<Ability_Alch_Bomb>();
		heal = selected.GetComponent<Ability_Heal>();
		
		victoryScreen = Resources.Load("victoryScreen") as Texture;
		defeatScreen = Resources.Load("defeatScreen") as Texture;
		startScreen = Resources.Load("startScreen") as Texture;
		startButton = Resources.Load("startButton") as Texture;
		offColor = Resources.Load("offColor") as Texture;
		
		
		noTarget = false;
		
		if(checkRestart() == 0){ start = true; }else{ start = false; }
		if(Application.loadedLevelName == "BossScene"){
			start = false;
		}
	}
	
	//find out what platform is running the code
	RuntimePlatform platform = Application.platform;
	
	void Update(){
		if(start){ 
			GameManager.instance.FreezeOtherCharacters(selected);
			state.setInactive();
		}
		//if(start){ Time.timeScale = 0; }
		if (end) { endCount++; }
		
		//Pop player input UI
		if (!state.on_cooldown_huh () && state.getActive () && !state.isDead()) {
			retouch = camera.WorldToScreenPoint (selected.transform.position);
			GameManager.instance.FreezeOtherCharacters (selected);
			turn = true;
		}
		if (state.on_cooldown_huh () || !state.getActive ()) {
			targeting = false;
			turn = false;
		}
		//If on a touch platform, detect touch instead
		if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.WindowsPlayer) {
			touch = true;
		} else {
			touch = false;
		}
		
		//on click
		if ((touch && Input.touchCount > 0) || (!touch && Input.GetMouseButtonDown (0))) {
			StartCoroutine(decideEnding());
			
			RaycastHit2D hit;
			
			//raycast based on mouse position
			if (touch) {
				hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero); 
			} else {
				hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10)), Vector2.zero);
			}
			
			//if an object is clicked
			if (hit != null && hit.collider != null && hit.collider.tag == "Enemy" && targeting) {
				CharacterState enemyState = hit.collider.GetComponent<CharacterState>();
				if(!enemyState.isDead()){
					drawButtons = false;
					target = hit.collider.gameObject;
					targeted = true;
					turn = false;
					state.setInactive();
				}
			}
			
			if(button == 3){
				drawButtons = false;
				targeted = true;
				turn = false;
				state.setInactive();
			}
		}
		
		
		//if we have a target enemy
		if(targeted && !drawButtons){ 
			drawButtons = false;
			//Which ability
			if(button == 1){ 
				Debug.Log ("ABLITY1 USED"); 
				BasicAttack();
				targeted = false;
			}else if(button == 2){ 
				Debug.Log ("ABLITY2 USED");
				Alch_Bomb();
				targeted = false;
			}else if(button == 3){ 
				Debug.Log ("ABLITY3 USED"); 
				Heal();
				targeted = false;
			}
			ResetInput();
		}
		
		if (!state.getActive () || state.on_cooldown_huh ()) {
			drawButtons = false;
		}
		
		if (button != 0) { } //Once an ability is selected, next input should be a target enemy
		//if (targeted || targeting || !turn) { drawButtons = false; }
		
	}
	
	
	public void OnGUI(){

		if (!targeting && !targeted && turn) {
			drawButtons = true;
		} else {
			drawButtons = false;
		}

		if (drawButtons && !end) {
			GUI.Box(new Rect(0, Screen.height - size,size,size), offColor);
			GUI.Box(new Rect(0 + size, Screen.height - size,size,size),offColor);
			GUI.Box (new Rect(0 + size*2, Screen.height - size,size,size),offColor);
		}

		
		//Draw the buttons for abilities id the player is selected
		if (!end) {

			Rect button1 = new Rect(0, Screen.height - size,size,size);
			Rect button2 = new Rect(0 + size, Screen.height - size,size,size);
			Rect button3 = new Rect(0 + size*2, Screen.height - size,size,size);
			
			
			GUI.DrawTexture(button1,  ability1Texture);
			GUI.DrawTexture(button2,  ability2Texture);
			GUI.DrawTexture(button3,  ability3Texture);
			Event e = Event.current;
			if (e.type == EventType.MouseDown) {
				if (button1.Contains(e.mousePosition)) {
					Debug.Log ("Button1 hit");
					button = 1;
					drawButtons = false;
					targeting = true;
				}
				
				if (button2.Contains(e.mousePosition)) {
					Debug.Log ("Button2 hit");
					button = 2;
					drawButtons = false;
					targeting = true;
				}
				
				if (button3.Contains(e.mousePosition)) {
					Debug.Log ("Button3 hit");
					button = 3;
					drawButtons = false;
					targeting = true;
				}
			}
		}
		float cool = determineCool();
		if (!end) {
			GUI.Box(new Rect(0, Screen.height - size*cool,size,size), "");
			GUI.Box(new Rect(0 + size, Screen.height - size*cool,size,size),"");
			GUI.Box (new Rect(0 + size*2, Screen.height - size*cool,size,size),"");
		}
		
		if (!drawButtons && !end) {
			GUI.Box(new Rect(0, Screen.height - size,size,size), "");
			GUI.Box(new Rect(0 + size, Screen.height - size,size,size),"");
			GUI.Box (new Rect(0 + size*2, Screen.height - size,size,size),"");
		}
		
		
		if (end) {
			Rect endScreen = new Rect(0, 0,Screen.width,Screen.height);
			if(dead && !once){ 
				StartCoroutine(loseFade());
			}else if(!once){
				StartCoroutine(winFade());
			}
			
		}

		if (start) {
			Rect endScreen = new Rect(0, 0,Screen.width,Screen.height);
			GUI.DrawTexture(endScreen,  startScreen);
		}
		
		if (end && endCount > okayEndCount) {
			Rect endScreen = new Rect(0, 0,Screen.width,Screen.height);
			GUI.DrawTexture(endScreen,  startButton);
		}

	}

	IEnumerator winFade(){
		once = true;
		FadeScript.instance.BeginWinFade(1);
		yield return new WaitForSeconds(.8f);
		FadeScript.instance.BeginWinFade(-1);
	}

	IEnumerator loseFade(){
		once = true;	
		FadeScript.instance.BeginLoseFade(1);
		yield return new WaitForSeconds(.8f);
		FadeScript.instance.BeginLoseFade(-1);
	}
	
	//Reset after player attack
	public void ResetInput(){
		selected.GetComponent<PlayerAction>().DeSelect();
		GameManager.instance.UnFreezeCharacters();
		button = 0;
		drawButtons = false;
		//targeting = false;
		targeted = false;
		target = null;
		turn = false;
		Debug.Log("input reset");
	}
	
	//Fireball ability
	public void BasicAttack(){
		ba.add_to_queue(target);
	}
	
	//Fireball ability
	public void Alch_Bomb(){
		ab.add_to_queue(target);
	}
	
	//Fireball ability
	public void Heal(){
		heal.add_to_queue(null);
	}
	
	public void GameOver(){
		end = true;
		if(Application.loadedLevelName == "BossScene"){
			BackgroundBossMusic.instance.basicStinger();
		} else {
			BackgroundMusic.instance.basicStinger ();
		}
	}
	
	public void Dead(){
		dead = true;
		saveRestart();
	}
	
	public float determineCool(){
		
		float cool = state.cooldown / maxCooldown;
		
		if (cool > 1.0f) {	
			return 1.0f;
		} else {
			return cool;
		}
	}
	
	public IEnumerator decideEnding(){
		if (end && endCount > okayEndCount) { 
			if (dead) {
				saveNoRestart();
				Application.LoadLevel (Application.loadedLevel); 
				BackgroundMusic.instance.StartBackground();
			} else {
				FadeScript.instance.BeginBlackFade(1);
				yield return new WaitForSeconds(.8f);
				Application.LoadLevel ("BossScene"); 
				GameManager.instance.UnFreezeCharacters();
				start = false;
			}
		}
		if(start){
			TitleAudio.aud.Transition();
			FadeScript.instance.BeginBlackFade(1);
			yield return new WaitForSeconds(.8f);
			FadeScript.instance.BeginBlackFade(-1);
			GameManager.instance.UnFreezeCharacters();
			start = false;
			Time.timeScale = 1; 
		}
	}
	
	//save player prefs to restart scene with title screen (defeat) or not
	
	void saveRestart() {
		PlayerPrefs.SetInt("restart", 0);
	}
	
	void saveNoRestart() {
		int level = checkRestart();
		level++;
		PlayerPrefs.SetInt("restart", level);
	}
	
	int checkRestart() {
		int level = PlayerPrefs.GetInt ("restart", 0);
		Debug.Log ("level checked: " + level);
		return level;
	}
	
	void OnApplicationQuit() {
		saveRestart ();
	}
	
	
}
