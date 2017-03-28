using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour {

	public float SecondsTillDeath;

	void Start () {
		StartCoroutine ("death");
	}
	IEnumerator death(){
		yield return new WaitForSeconds (SecondsTillDeath);
		Destroy (gameObject);
	}
}
