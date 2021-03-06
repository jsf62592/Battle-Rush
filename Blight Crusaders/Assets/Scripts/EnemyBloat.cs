﻿using UnityEngine;
using System.Collections;

public class EnemyBloat : Enemy {


	Ability mt;
	Ability fb;
	Ability hl;
	Ability lc;

	double target_health;

	CharacterState state;

	// Use this for initialization
	void Start () {
		fb = GetComponent<Ability_BossBolt> ();
		mt = GetComponent<Ability_Meteor> ();
		hl = GetComponent<Ability_EnemyHeal> ();
		lc = GetComponent<Ability_Leech> ();

		state = GetComponent<CharacterState> ();
	}
	
	public override void decision(GameObject target){
		target_health = target.GetComponent<CharacterState> ().get_health ();
		if (state.get_health () < 150) {
			lc.add_to_queue(target);
		} else if (state.get_health () < 75) {
			hl.add_to_queue(null);
		} else if(target_health > 320){
			fb.add_to_queue(target);
		} else {
			mt.add_to_queue(target);
		}
	}
}
