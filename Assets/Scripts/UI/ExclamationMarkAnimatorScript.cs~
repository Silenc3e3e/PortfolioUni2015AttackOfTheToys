using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class ExclamationMarkAnimatorScript : MonoBehaviour {

	public float SecondsTillDeath;
	public bool Bounce=true;
	public float IsDead=10f;
	private float PrevTime;
	private bool TestForSpawn=false;
	private Animator ThisAnim;
	private List<GameObject> ObjectsToNotify;
	private bool Lerp = false;
	private Vector3 LerpTarget;
	public GameObject NotifiedEnemyPrefab;
	private float LerpMultiplier;

	void Start () {
		PrevTime = Time.time;
		ThisAnim = GetComponent<Animator> ();
		ThisAnim.SetBool ("Bounce", Bounce);
		StartCoroutine ("PingAnimation");
	}
	IEnumerator PingAnimation(){
		yield return new WaitForSeconds (SecondsTillDeath);
		ThisAnim.SetBool ("Die", true);
//		Destroy (gameObject);
	}
	void Update(){
		IsDead -= Time.time-PrevTime;
		if (IsDead<=0f) {
			Destroy (gameObject);
		}
	
		if (TestForSpawn && !Bounce) {
			//Debug.LogError ("Passed");
			TestForSpawn=false;
			//Debug.LogError("Number of allies to notify is "+ObjectsToNotify.Count.ToString());
			foreach(GameObject current in ObjectsToNotify){
				if(current!=gameObject){
					//error. currently spawns object in the center of the map. Instead of at it's own position
					GameObject spawnedPrefab = (GameObject)Instantiate(NotifiedEnemyPrefab,transform.parent.transform.position,transform.parent.transform.rotation);
					spawnedPrefab.GetComponentInChildren<ExclamationMarkAnimatorScript>().BeginLerp(current.transform.position);
					spawnedPrefab.transform.Rotate(270,0,0);
				}
			}
		}
		if (Lerp) {
			transform.parent.transform.position=Vector3.Lerp(transform.parent.transform.position,LerpTarget,LerpMultiplier);
			LerpMultiplier+=Time.time-PrevTime;
		}
		PrevTime = Time.time;
	}
	public void AlliesToNotify(List<GameObject> ObjsToNotify){
		ObjectsToNotify = ObjsToNotify;
		TestForSpawn=true;
	}
	public void BeginLerp(Vector3 Target){
		LerpTarget = Target;
		LerpMultiplier = 1f;
		Bounce = false;
		if (ThisAnim == null)
			ThisAnim = GetComponent<Animator> ();
		if (ThisAnim == null)
			Debug.LogError ("Missing Anim");
		ThisAnim.SetBool ("Bounce", false);
		Lerp = true;
	}
}
