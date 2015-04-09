﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ability_Alch_Bomb : Ability {
	void Start(){
		setup (7, false, "Prefabs/Alch_Bomb");
	}
	protected override void attachEffects(GameObject given_target){
		given_target.AddComponent<SE_Alch_Bomb> ();
	}

}


