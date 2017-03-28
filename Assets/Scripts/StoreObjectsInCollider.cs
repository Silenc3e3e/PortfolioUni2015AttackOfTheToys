﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class StoreObjectsInCollider : MonoBehaviour {

	//public int id;
	
	private List<GameObject> objectsInMyCollider;
	public string tagToLookFor;

	//raycasting between the player and the enemy
	private int EnvironmentLayerMask;
	private Transform ThePlayer;

	//number of units in this collider
	public int NoInCollider;

	void Update(){
		NoInCollider = objectsInMyCollider.Count;
	}

	void Start(){
		objectsInMyCollider= new List<GameObject>();
		ThePlayer = GameObject.FindGameObjectWithTag("Player").transform;
		EnvironmentLayerMask=1<<LayerMask.NameToLayer("Environment");
	}

	public void OnTriggerEnter(Collider other) {

		//Debug.LogError("Tag check "+other.tag+" : " + other.name + " "+". looking for "+tagToLookFor+". Caller is "+name);
		if (other.tag == tagToLookFor) {
			objectsInMyCollider.Add (other.gameObject);

			//Activate the enemie's flashing to show targetable
			/*Enemy currentEnemyScript =other.GetComponent<Enemy>();
			if(currentEnemyScript!=null && !Physics.Linecast(ThePlayer.position,other.transform.position,EnvironmentLayerMask)){
				currentEnemyScript.Flashing(true);
			}*/
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.tag == tagToLookFor) {
			//bool Passed=false;
			//Debug.DrawLine(ThePlayer.position,other.transform.position);
			/*if(!Physics.Linecast(ThePlayer.position,other.transform.position,EnvironmentLayerMask)){
				Enemy currentEnemyScript =other.GetComponent<Enemy>();
				if(currentEnemyScript!=null)
					currentEnemyScript.Flashing(true);
			}*/
		}
	}

	public void OnTriggerExit(Collider other) {
		foreach (GameObject currentObject in objectsInMyCollider) {
			if(currentObject){
				if(currentObject==other.gameObject){
					//Deactivate the enemie's flashing to show no longer targetable
					/*Enemy currentEnemyScript =currentObject.GetComponent<Enemy>();
					if(currentEnemyScript!=null)
						currentEnemyScript.Flashing(false);*/

					objectsInMyCollider.Remove(currentObject);
					break;
				}
			}
		}
//		if(!Passed)
//			objectsInMyCollider= new List<GameObject>();
	}

	public List<GameObject> ReturnObjectList(){
		return objectsInMyCollider;
	}
}