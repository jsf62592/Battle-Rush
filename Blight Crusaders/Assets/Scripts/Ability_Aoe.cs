﻿//citation of artwork --- Aoe  
//original author : Max@wordpress http://orangemushroom.net/author/highonmushrooms/
//original artwork page: http://orangemushroom.net/2013/05/28/kmst-ver-1-2-478-adventurer-warrior-and-magician-reorganizations/



using UnityEngine;
using System.Collections;

public class Ability_Aoe : Ability {
	
	public Transform prefab;
	// Use this for initialization
	void Start () {
		setup (5, false, "Prefabs/aoe");

	}
	
	protected override void attachEffects(GameObject given_target){
		//will be given some other attributes
	//	given_target.AddComponent<SE_Strike> ();
	//	prefab = (Transform) Instantiate(prefab, given_target.transform.position + new Vector3(0.0f,0.0f,0.0f), transform.rotation);// as Transform;

	}
}