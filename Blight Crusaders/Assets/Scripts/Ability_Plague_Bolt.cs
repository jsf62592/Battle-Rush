﻿using UnityEngine;
using System.Collections;

public class Ability_Plague_Bolt : Ability {
	void Start(){
		setup (5, Visual_Types.ranged_projectile, "Prefabs/Alch_Bomb");
	}
	protected override void attachEffects(GameObject given_target){
		given_target.AddComponent<SE_Alch_Bomb> ();
	}

}
