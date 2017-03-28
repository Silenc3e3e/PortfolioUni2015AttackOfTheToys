using UnityEngine;
using System.Collections;

public class PlayerArrowScript : MonoBehaviour {

	[Range(1f,50f)]public float moveSpeed=10f;
	[Range(1f,50f)]public float maxDistance=10f;
	private float timer;
	private float previousTime;
	
	private float myDamage = 1f;
	private float myKnockBackDuration=0f;
	private float myknockBackDistance=0f;

	private Vector3 myKnockbackAwayFromPoint;
	
	void Start () {
		//transform.rotation=Quaternion.LookRotation (GameObject.FindGameObjectWithTag ("Player").transform.position - transform.position);
		timer=maxDistance/moveSpeed;
		previousTime = Time.time;
		myKnockbackAwayFromPoint=transform.position;
	}
	
	void Update () {
		transform.position += transform.forward*Time.deltaTime*moveSpeed;
		timer -= Time.time - previousTime;
		if (timer <= 0)
			Destroy (gameObject);
		previousTime = Time.time;
	}
	void OnTriggerEnter(Collider other) {
		if (other.tag == "StaticEnvironment")
			Destroy (gameObject);
		else if (other.tag == "Enemy") {
			Enemy otherMHBE = other.GetComponent<Enemy> ();
			//Debug.LogError("otherMHBE "+ other.gameObject.name);
			if(otherMHBE!=null){
				otherMHBE.Damage(myDamage);
				otherMHBE.knockback(myKnockBackDuration,myknockBackDistance,myKnockbackAwayFromPoint);
			}
			else{
				Boss otherBossScript = other.GetComponent<Boss> ();
				if(otherBossScript!=null){
					otherBossScript.Damage(myDamage);
				}else{
					Debug.LogWarning("No Enemy script found!");
				}
			}
			Destroy (gameObject);
		}else if (other.tag == "Lever")
        {
            other.GetComponent<Lever>().Click(true);
            Destroy(gameObject);
        }
	}
	public void ResetFlight(float speed, float distance, Quaternion Direction, Vector3 KnockbackToMoveAwayFrom){
		moveSpeed=speed;
		maxDistance=distance;
		timer=distance/speed;
		transform.rotation=Direction;
		myKnockbackAwayFromPoint=KnockbackToMoveAwayFrom;
	}

	public void SetDamageAndKnockback(float damage, float knockBackDuration, float knockBackDistance){
		myDamage = damage;
		myknockBackDistance = knockBackDistance;
		myKnockBackDuration = knockBackDuration;
	}
}
